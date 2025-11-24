using AutoMapper;
using FluentValidation;
using MailKit.Net.Smtp; // MailKit
using Microsoft.EntityFrameworkCore;
using MimeKit; // MailKit
using Service.DAL;
using Service.Domain.Helpers;
using Service.Domain.ModelsDb;
using Service.Domain.Response;
using Service.Domain.ViewModels.LoginAndRegistration;
using Service.Services.Interfaces;
using System.Security.Claims;

namespace Service.Services.Realizations
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IValidator<RegisterViewModel> _registerValidator;
        private readonly IValidator<LoginViewModel> _loginValidator;

        public AccountService(
            ApplicationDbContext context,
            IMapper mapper,
            IValidator<RegisterViewModel> registerValidator,
            IValidator<LoginViewModel> loginValidator)
        {
            _context = context;
            _mapper = mapper;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        public async Task<BaseResponse<ConfirmEmailViewModel>> Register(RegisterViewModel model)
        {
            try
            {
                // 1. Валидация
                var validationResult = await _registerValidator.ValidateAsync(model);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return new BaseResponse<ConfirmEmailViewModel>()
                    {
                        Description = errors,
                        StatusCode = RoleStatusCode.BadRequest
                    };
                }

                // 2. Проверка существования
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == model.Email || x.Login == model.Username);
                if (user != null)
                {
                    return new BaseResponse<ConfirmEmailViewModel>()
                    {
                        Description = "Пользователь с таким логином или почтой уже есть",
                        StatusCode = RoleStatusCode.BadRequest
                    };
                }

                // 3. Генерация кода и отправка почты
                Random random = new Random();
                string confirmationCode = random.Next(100000, 999999).ToString();

                await SendEmail(model.Email, confirmationCode);

                // 4. Подготовка модели для подтверждения
                var confirmModel = _mapper.Map<ConfirmEmailViewModel>(model);
                confirmModel.GeneratedCode = confirmationCode;

                return new BaseResponse<ConfirmEmailViewModel>()
                {
                    Data = confirmModel,
                    Description = "Код отправлен на почту",
                    StatusCode = RoleStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ConfirmEmailViewModel>()
                {
                    Description = ex.Message,
                    StatusCode = RoleStatusCode.InternalServerError
                };
                // Если произойдет ошибка, она выбросится наверх и мы увидим её в браузере (alert) или в консоли
                throw new Exception("Ошибка при отправке письма: " + ex.Message);
            }
        }

        public async Task<BaseResponse<ClaimsIdentity>> ConfirmEmail(ConfirmEmailViewModel model)
        {
            try
            {
                // Проверка кода (в реальном проекте лучше хешировать код или хранить в кэше)
                if (model.Code != model.GeneratedCode)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Неверный код подтверждения",
                        StatusCode = RoleStatusCode.BadRequest
                    };
                }

                // Создание пользователя ПОСЛЕ подтверждения
                var newUser = new UserDb()
                {
                    Id = Guid.NewGuid(),
                    Login = model.UserName,
                    Email = model.UserEmail,
                    Role = (int)Service.Domain.Enums.UserRole.User,
                    Password = HashPasswordHelper.HashPassword(model.UserPassword),
                    CreatedAt = DateTime.UtcNow,
                };

                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();

                var result = Authenticate(newUser);

                return new BaseResponse<ClaimsIdentity>()
                {
                    Data = result,
                    Description = "Пользователь успешно зарегистрирован",
                    StatusCode = RoleStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = ex.Message,
                    StatusCode = RoleStatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<ClaimsIdentity>> Login(LoginViewModel model)
        {
            try
            {
                var validationResult = await _loginValidator.ValidateAsync(model);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = errors,
                        StatusCode = RoleStatusCode.BadRequest
                    };
                }

                var user = await _context.Users.FirstOrDefaultAsync(x => x.Login == model.Login || x.Email == model.Login);
                if (user == null)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Пользователь не найден",
                        StatusCode = RoleStatusCode.NotFound
                    };
                }

                if (user.Password != HashPasswordHelper.HashPassword(model.Password))
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Неверный пароль или логин",
                        StatusCode = RoleStatusCode.BadRequest
                    };
                }

                var result = Authenticate(user);

                return new BaseResponse<ClaimsIdentity>()
                {
                    Data = result,
                    Description = "Успешный вход",
                    StatusCode = RoleStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = ex.Message,
                    StatusCode = RoleStatusCode.InternalServerError
                };
            }
        }

        private ClaimsIdentity Authenticate(UserDb user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
            };
            return new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }

        private async Task SendEmail(string email, string code)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Inventory System", "nik.okhlopovskiy.31@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = "Подтверждение регистрации";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = "<html>" +
                       "<head>" +
                       "<style>" +
                       "body { font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px; }" +
                       ".container { max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; border-radius: 10px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); }" +
                       ".header { text-align: center; margin-bottom: 20px; }" +
                       ".code { font-size: 24px; font-weight: bold; text-align: center; color: #333; margin: 20px 0; }" +
                       ".footer { text-align: center; color: #777; font-size: 12px; margin-top: 20px; }" +
                       "</style>" +
                       "</head>" +
                       "<body>" +
                       "<div class='container'>" +
                       "<div class='header'><h2>Добро пожаловать!</h2></div>" +
                       "<p>Спасибо за регистрацию в Inventory System. Ваш код подтверждения:</p>" +
                       $"<div class='code'>{code}</div>" +
                       "<p>Введите этот код на сайте для завершения регистрации.</p>" +
                       "<div class='footer'>Если вы не регистрировались, просто проигнорируйте это письмо.</div>" +
                       "</div>" +
                       "</body>" +
                       "</html>"
            };

            using (var client = new SmtpClient())
            {
                // Используем smtp.gmail.com для Google
                await client.ConnectAsync("smtp.gmail.com", 465, true); // 465 для SSL
                // Вставляем свои данные!
                await client.AuthenticateAsync("nik.okhlopovskiy.31@gmail.com", "pimn dldp uvzz qfwh");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
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

        public async Task<BaseResponse<ClaimsIdentity>> Register(RegisterViewModel model)
        {
            try
            {
                // 1. Проверка валидности данных с помощью FluentValidation
                var validationResult = await _registerValidator.ValidateAsync(model);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = errors,
                        StatusCode = RoleStatusCode.BadRequest
                    };
                }

                // 2. Стандартная проверка на существование пользователя
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == model.Email || x.Login == model.Username);
                if (user != null)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Пользователь с таким логином или почтой уже есть",
                        StatusCode = RoleStatusCode.BadRequest
                    };
                }

                var newUser = new UserDb()
                {
                    Id = Guid.NewGuid(),
                    Login = model.Username,
                    Email = model.Email,
                    Role = (int)Service.Domain.Enums.UserRole.User,
                    Password = HashPasswordHelper.HashPassword(model.Password),
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
                // 1. Проверка валидности данных
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
    }
}
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Service.DAL;
using Service.Domain.Helpers; // Подключаем хешер
using Service.Domain.ModelsDb;
using Service.Domain.Response;
using Service.Services.Interfaces;
using System.Security.Claims; // Подключаем Claims

namespace Service.Services.Realizations
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AccountService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BaseResponse<ClaimsIdentity>> Register(string login, string password, string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email || x.Login == login);
                if (user != null)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Пользователь с таким логином или почтой уже есть",
                    };
                }

                var newUser = new UserDb()
                {
                    Id = Guid.NewGuid(),
                    Login = login,
                    Email = email,
                    // ИСПРАВЛЕНИЕ: Добавлено явное приведение (int)
                    Role = (int)Service.Domain.Enums.UserRole.User, // Роль по умолчанию
                    // Хешируем пароль перед сохранением
                    Password = HashPasswordHelper.HashPassword(password),
                    CreatedAt = DateTime.UtcNow,
                };

                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();

                // Формируем ClaimsIdentity для автоматического входа после регистрации
                var result = Authenticate(newUser);

                return new BaseResponse<ClaimsIdentity>()
                {
                    Data = result,
                    Description = "Объект добавился",
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

        public async Task<BaseResponse<ClaimsIdentity>> Login(string login, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Login == login || x.Email == login);

                if (user == null)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Пользователь не найден"
                    };
                }

                // Сравниваем хеш введенного пароля с хешем в БД
                if (user.Password != HashPasswordHelper.HashPassword(password))
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Неверный пароль или логин"
                    };
                }

                var result = Authenticate(user);

                return new BaseResponse<ClaimsIdentity>()
                {
                    Data = result,
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

        // Вспомогательный метод для создания ClaimsIdentity
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
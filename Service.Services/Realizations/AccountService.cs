using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Service.DAL;
using Service.Domain.ModelsDb;
using Service.Domain.Response;
using Service.Services.Interfaces;

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

        public async Task<BaseResponse<string>> Login(string login, string password)
        {
            try
            {
                // Ищем пользователя по логину или email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Login == login || u.Email == login);

                if (user == null)
                {
                    return new BaseResponse<string>
                    {
                        Description = "Пользователь не найден",
                        StatusCode = RoleStatusCode.NotFound,
                        Data = null
                    };
                }

                // Проверяем пароль
                if (user.Password != password)
                {
                    return new BaseResponse<string>
                    {
                        Description = "Неверный пароль",
                        StatusCode = RoleStatusCode.BadRequest,
                        Data = null
                    };
                }

                // Генерируем токен
                var token = GenerateToken(user);

                return new BaseResponse<string>
                {
                    Description = "Успешный вход",
                    StatusCode = RoleStatusCode.OK,
                    Data = token
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<string>
                {
                    Description = $"Ошибка: {ex.Message}",
                    StatusCode = RoleStatusCode.InternalServerError,
                    Data = null
                };
            }
        }

        public async Task<BaseResponse<bool>> Register(string login, string password, string email)
        {
            try
            {
                // Проверяем, существует ли пользователь
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Login == login || u.Email == email);

                if (existingUser != null)
                {
                    return new BaseResponse<bool>
                    {
                        Description = "Пользователь с таким логином или email уже существует",
                        StatusCode = RoleStatusCode.BadRequest,
                        Data = false
                    };
                }

                // Создаем нового пользователя
                var newUser = new UserDb
                {
                    Id = Guid.NewGuid(),
                    Login = login,
                    Password = password,
                    Email = email,
                    Role = 0,
                    CreatedAt = DateTime.UtcNow
                };

                // Добавляем в базу данных
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();

                return new BaseResponse<bool>
                {
                    Description = "Пользователь успешно зарегистрирован",
                    StatusCode = RoleStatusCode.OK,
                    Data = true
                };
            }
            catch (DbUpdateException dbEx)
            {
                return new BaseResponse<bool>
                {
                    Description = $"Ошибка базы данных: {dbEx.InnerException?.Message ?? dbEx.Message}",
                    StatusCode = RoleStatusCode.InternalServerError,
                    Data = false
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>
                {
                    Description = $"Ошибка регистрации: {ex.Message}",
                    StatusCode = RoleStatusCode.InternalServerError,
                    Data = false
                };
            }
        }

        private string GenerateToken(UserDb user)
        {
            return $"token-{user.Id}-{DateTime.UtcNow.Ticks}";
        }
    }
}
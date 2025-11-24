using System.Security.Claims;
using Service.Domain.Response;

namespace Service.Services.Interfaces
{
    public interface IAccountService
    {
        // Возвращаем ClaimsIdentity для создания куки авторизации
        Task<BaseResponse<ClaimsIdentity>> Register(string login, string password, string email);
        Task<BaseResponse<ClaimsIdentity>> Login(string login, string password);
    }
}
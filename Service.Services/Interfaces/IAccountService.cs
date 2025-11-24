using Service.Domain.Response;
using Service.Domain.ViewModels.LoginAndRegistration;
using System.Security.Claims;

namespace Service.Services.Interfaces
{
    public interface IAccountService
    {
        // Методы теперь принимают ViewModels для удобной валидации
        Task<BaseResponse<ClaimsIdentity>> Register(RegisterViewModel model);
        Task<BaseResponse<ClaimsIdentity>> Login(LoginViewModel model);
    }
}
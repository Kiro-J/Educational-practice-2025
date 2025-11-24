using System.Security.Claims;
using Service.Domain.Response;
using Service.Domain.ViewModels.LoginAndRegistration;

namespace Service.Services.Interfaces
{
    public interface IAccountService
    {
        // Register теперь просто отправляет письмо и возвращает данные для подтверждения
        Task<BaseResponse<ConfirmEmailViewModel>> Register(RegisterViewModel model);

        // ConfirmEmail проверяет код и создает пользователя
        Task<BaseResponse<ClaimsIdentity>> ConfirmEmail(ConfirmEmailViewModel model);

        Task<BaseResponse<ClaimsIdentity>> Login(LoginViewModel model);
    }
}
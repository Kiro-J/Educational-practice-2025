using Service.Domain.Response;

namespace Service.Services.Interfaces
{
    public interface IAccountService
    {
        Task<BaseResponse<string>> Login(string login, string password);
        Task<BaseResponse<bool>> Register(string login, string password, string email);
    }
}
using ApiApplicationCore.Dtos;

namespace ApiApplicationCore.Services.Contract
{
    public interface IAuthService
    {
        ServiceResponse<string> RegisterUserService(RegisterDto register);

        ServiceResponse<string> LoginUserService(LoginDto login);
    }
}

using MVCApplicationCore.ViewModels;

namespace MVCApplicationCore.Services.Contract
{
    public interface IAuthService
    {
        string RegisterUserService(RegisterViewModel register);

        string LoginUserService(LoginViewModel login);
    }
}

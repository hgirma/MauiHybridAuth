using MauiHybridAuth.Shared.Models;

namespace MauiHybridAuth.Shared.Services
{
    public interface IRegisterService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest registerModel);
    }
}

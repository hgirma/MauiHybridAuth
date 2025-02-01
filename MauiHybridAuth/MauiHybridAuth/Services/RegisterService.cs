using System.Net.Http.Json;
using MauiHybridAuth.Shared.Models;
using MauiHybridAuth.Shared.Services;

namespace MauiHybridAuth.Services
{
    internal class RegisterService(HttpClient httpClient) : IRegisterService
    {
        private readonly HttpClient httpClient = httpClient;

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            // simplified for brevity
            try
            {
                var httpResponse = await httpClient.PostAsJsonAsync("register2", registerRequest);

                return await httpResponse.Content.ReadFromJsonAsync<RegisterResponse>();
            }
            catch (Exception ex)
            {
                return new RegisterResponse { ErrorMessage = ex.Message };
            }
        }
    }
}

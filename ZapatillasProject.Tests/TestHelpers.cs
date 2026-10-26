using System.Net.Http.Headers;
using System.Net.Http.Json;
using ZapatillasProject.DTOs;

namespace ZapatillasProject.Tests
{
    public static class TestHelpers
    {
        public const string AdminEmail = "admin@zapatillas.cl";
        public const string AdminPassword = "Admin123!";

        public static async Task<string> LoginAsync(HttpClient client, string email, string password)
        {
            var resp = await client.PostAsJsonAsync("/api/v1/User/login", new LoginDto { Email = email, Password = password });
            resp.EnsureSuccessStatusCode();
            var token = await resp.Content.ReadFromJsonAsync<TokenDto>();
            return token!.Token;
        }

        public static Task<string> LoginAdminAsync(HttpClient client)
        {
            return LoginAsync(client, AdminEmail, AdminPassword);
        }

        // registra un cliente nuevo (email unico) y devuelve su token
        public static async Task<string> CrearClienteYLoginAsync(HttpClient client)
        {
            var email = $"cliente_{Guid.NewGuid():N}@test.cl";
            var resp = await client.PostAsJsonAsync("/api/v1/User/registro",
                new RegistroDto { Nombre = "Cliente Test", Email = email, Password = "Cliente123!" });
            resp.EnsureSuccessStatusCode();
            return await LoginAsync(client, email, "Cliente123!");
        }

        public static HttpClient ConToken(this HttpClient client, string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public static ZapatillaCreateDto ZapatillaValida(string modelo = "Air Max 90")
        {
            return new ZapatillaCreateDto
            {
                Modelo = modelo,
                Precio = 119990,
                IdCategoria = 2,
                IdMarca = 1,
                Descripcion = "Zapatilla de prueba"
            };
        }
    }
}

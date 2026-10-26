using System.Net;
using System.Net.Http.Json;
using ZapatillasProject.DTOs;

namespace ZapatillasProject.Tests
{
    public class AuthTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public AuthTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Login_ConCredencialesCorrectas_DevuelveToken()
        {
            var client = _factory.CreateClient();

            var resp = await client.PostAsJsonAsync("/api/v1/User/login",
                new LoginDto { Email = TestHelpers.AdminEmail, Password = TestHelpers.AdminPassword });

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var token = await resp.Content.ReadFromJsonAsync<TokenDto>();
            Assert.False(string.IsNullOrWhiteSpace(token!.Token));
            Assert.Equal("Admin", token.Rol);
        }

        [Fact]
        public async Task Login_ConPasswordIncorrecta_Devuelve401()
        {
            var client = _factory.CreateClient();

            var resp = await client.PostAsJsonAsync("/api/v1/User/login",
                new LoginDto { Email = TestHelpers.AdminEmail, Password = "incorrecta" });

            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task Registro_CreaUsuarioConRolCliente()
        {
            var client = _factory.CreateClient();
            var email = $"nuevo_{Guid.NewGuid():N}@test.cl";

            var resp = await client.PostAsJsonAsync("/api/v1/User/registro",
                new RegistroDto { Nombre = "Nuevo", Email = email, Password = "Secreta123" });

            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
            var usuario = await resp.Content.ReadFromJsonAsync<UsuarioDto>();
            Assert.Equal("Cliente", usuario!.Rol);
        }

        [Fact]
        public async Task Registro_ConEmailRepetido_Devuelve409()
        {
            var client = _factory.CreateClient();
            var dto = new RegistroDto { Nombre = "Repetido", Email = $"rep_{Guid.NewGuid():N}@test.cl", Password = "Secreta123" };

            await client.PostAsJsonAsync("/api/v1/User/registro", dto);
            var resp = await client.PostAsJsonAsync("/api/v1/User/registro", dto);

            Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
        }

        [Fact]
        public async Task Registro_ConEmailInvalido_Devuelve400()
        {
            var client = _factory.CreateClient();

            var resp = await client.PostAsJsonAsync("/api/v1/User/registro",
                new RegistroDto { Nombre = "X", Email = "no-es-email", Password = "123" });

            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task ListarUsuarios_ComoCliente_Devuelve403()
        {
            var client = _factory.CreateClient();
            var token = await TestHelpers.CrearClienteYLoginAsync(client);

            var resp = await client.ConToken(token).GetAsync("/api/v1/User");

            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }
    }
}

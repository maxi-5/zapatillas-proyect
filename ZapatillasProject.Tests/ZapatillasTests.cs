using System.Net;
using System.Net.Http.Json;
using ZapatillasProject.DTOs;

namespace ZapatillasProject.Tests
{
    public class ZapatillasTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public ZapatillasTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetZapatillas_EsPublico_Devuelve200()
        {
            var client = _factory.CreateClient();

            var resp = await client.GetAsync("/api/v1/Zapatillas");

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }

        [Fact]
        public async Task PostZapatilla_SinToken_Devuelve401()
        {
            var client = _factory.CreateClient();

            var resp = await client.PostAsJsonAsync("/api/v1/Zapatillas", TestHelpers.ZapatillaValida());

            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task PostZapatilla_ComoCliente_Devuelve403()
        {
            var client = _factory.CreateClient();
            var token = await TestHelpers.CrearClienteYLoginAsync(client);

            var resp = await client.ConToken(token).PostAsJsonAsync("/api/v1/Zapatillas", TestHelpers.ZapatillaValida());

            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task PostZapatilla_DatosInvalidos_Devuelve400()
        {
            var client = _factory.CreateClient();
            client.ConToken(await TestHelpers.LoginAdminAsync(client));
            var dto = TestHelpers.ZapatillaValida();
            dto.Modelo = "";
            dto.Precio = -5;

            var resp = await client.PostAsJsonAsync("/api/v1/Zapatillas", dto);

            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task PostZapatilla_MarcaInexistente_Devuelve400()
        {
            var client = _factory.CreateClient();
            client.ConToken(await TestHelpers.LoginAdminAsync(client));
            var dto = TestHelpers.ZapatillaValida();
            dto.IdMarca = 9999;

            var resp = await client.PostAsJsonAsync("/api/v1/Zapatillas", dto);

            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task GetZapatilla_Inexistente_Devuelve404()
        {
            var client = _factory.CreateClient();

            var resp = await client.GetAsync("/api/v1/Zapatillas/99999");

            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task CrudCompleto_ComoAdmin_FuncionaConCodigosCorrectos()
        {
            var client = _factory.CreateClient();
            client.ConToken(await TestHelpers.LoginAdminAsync(client));

            // CREATE -> 201
            var crear = await client.PostAsJsonAsync("/api/v1/Zapatillas", TestHelpers.ZapatillaValida("Ultraboost 22"));
            Assert.Equal(HttpStatusCode.Created, crear.StatusCode);
            Assert.NotNull(crear.Headers.Location);
            var creada = await crear.Content.ReadFromJsonAsync<ZapatillaDto>();
            Assert.Equal("Ultraboost 22", creada!.Modelo);
            Assert.Equal("Nike", creada.Marca);

            // READ -> 200
            var leer = await client.GetAsync($"/api/v1/Zapatillas/{creada.IdZapatilla}");
            Assert.Equal(HttpStatusCode.OK, leer.StatusCode);

            // UPDATE -> 204
            var cambios = TestHelpers.ZapatillaValida("Ultraboost 23");
            cambios.Precio = 99990;
            var editar = await client.PutAsJsonAsync($"/api/v1/Zapatillas/{creada.IdZapatilla}", cambios);
            Assert.Equal(HttpStatusCode.NoContent, editar.StatusCode);

            var editada = await client.GetFromJsonAsync<ZapatillaDto>($"/api/v1/Zapatillas/{creada.IdZapatilla}");
            Assert.Equal("Ultraboost 23", editada!.Modelo);
            Assert.Equal(99990, editada.Precio);

            // DELETE -> 204 y luego 404
            var borrar = await client.DeleteAsync($"/api/v1/Zapatillas/{creada.IdZapatilla}");
            Assert.Equal(HttpStatusCode.NoContent, borrar.StatusCode);

            var despues = await client.GetAsync($"/api/v1/Zapatillas/{creada.IdZapatilla}");
            Assert.Equal(HttpStatusCode.NotFound, despues.StatusCode);
        }

        [Fact]
        public async Task GetZapatillas_FiltroPorMarca_SoloDevuelveEsaMarca()
        {
            var client = _factory.CreateClient();
            client.ConToken(await TestHelpers.LoginAdminAsync(client));
            var adidas = TestHelpers.ZapatillaValida("Samba OG");
            adidas.IdMarca = 2;
            await client.PostAsJsonAsync("/api/v1/Zapatillas", adidas);

            var lista = await client.GetFromJsonAsync<List<ZapatillaDto>>("/api/v1/Zapatillas?idMarca=2");

            Assert.NotEmpty(lista!);
            Assert.All(lista!, z => Assert.Equal(2, z.IdMarca));
        }
    }
}

using System.Net;
using System.Net.Http.Json;
using ZapatillasProject.DTOs;

namespace ZapatillasProject.Tests
{
    public class VentasTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public VentasTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        // crea (como admin) una zapatilla con stock en una talla y devuelve su id
        private async Task<int> CrearZapatillaConStock(decimal talla, int cantidad, int precio)
        {
            var admin = _factory.CreateClient();
            admin.ConToken(await TestHelpers.LoginAdminAsync(admin));

            var dto = TestHelpers.ZapatillaValida("Modelo venta " + Guid.NewGuid().ToString("N")[..6]);
            dto.Precio = precio;
            var resp = await admin.PostAsJsonAsync("/api/v1/Zapatillas", dto);
            var zapatilla = await resp.Content.ReadFromJsonAsync<ZapatillaDto>();

            var stock = await admin.PostAsJsonAsync("/api/v1/StockTallas",
                new StockTallaCreateDto { IdZapatilla = zapatilla!.IdZapatilla, Talla = talla, Cantidad = cantidad });
            Assert.Equal(HttpStatusCode.Created, stock.StatusCode);

            return zapatilla.IdZapatilla;
        }

        [Fact]
        public async Task PostVenta_CalculaMontoYDescuentaStock()
        {
            var idZapatilla = await CrearZapatillaConStock(42m, 2, 50000);
            var client = _factory.CreateClient();
            client.ConToken(await TestHelpers.CrearClienteYLoginAsync(client));

            var venta = new VentaCreateDto
            {
                TipoPago = "Debito",
                Detalles = new List<DetalleVentaCreateDto>
                {
                    new() { IdZapatilla = idZapatilla, Talla = 42m },
                    new() { IdZapatilla = idZapatilla, Talla = 42m }
                }
            };
            var resp = await client.PostAsJsonAsync("/api/v1/Ventas", venta);

            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
            var creada = await resp.Content.ReadFromJsonAsync<VentaDto>();
            Assert.Equal(100000, creada!.Monto);
            Assert.Equal(2, creada.Detalles.Count);

            var stock = await client.GetFromJsonAsync<List<StockTallaDto>>($"/api/v1/StockTallas/zapatilla/{idZapatilla}");
            Assert.Equal(0, stock!.Single().Cantidad);
        }

        [Fact]
        public async Task PostVenta_SinStock_Devuelve409()
        {
            var idZapatilla = await CrearZapatillaConStock(40m, 0, 30000);
            var client = _factory.CreateClient();
            client.ConToken(await TestHelpers.CrearClienteYLoginAsync(client));

            var resp = await client.PostAsJsonAsync("/api/v1/Ventas", new VentaCreateDto
            {
                TipoPago = "Efectivo",
                Detalles = new List<DetalleVentaCreateDto> { new() { IdZapatilla = idZapatilla, Talla = 40m } }
            });

            Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
        }

        [Fact]
        public async Task PostVenta_TipoPagoInvalido_Devuelve400()
        {
            var idZapatilla = await CrearZapatillaConStock(41m, 5, 30000);
            var client = _factory.CreateClient();
            client.ConToken(await TestHelpers.CrearClienteYLoginAsync(client));

            var resp = await client.PostAsJsonAsync("/api/v1/Ventas", new VentaCreateDto
            {
                TipoPago = "Bitcoin",
                Detalles = new List<DetalleVentaCreateDto> { new() { IdZapatilla = idZapatilla, Talla = 41m } }
            });

            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task GetVentas_ComoCliente_Devuelve403()
        {
            var client = _factory.CreateClient();
            client.ConToken(await TestHelpers.CrearClienteYLoginAsync(client));

            var resp = await client.GetAsync("/api/v1/Ventas");

            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task PostResena_ValoracionFueraDeRango_Devuelve400()
        {
            var idZapatilla = await CrearZapatillaConStock(43m, 1, 20000);
            var client = _factory.CreateClient();
            client.ConToken(await TestHelpers.CrearClienteYLoginAsync(client));

            var resp = await client.PostAsJsonAsync("/api/v1/Resenas",
                new ResenaCreateDto { IdZapatilla = idZapatilla, Resena = "Muy buenas", Valoracion = 9 });

            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }
    }
}

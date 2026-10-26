using Microsoft.EntityFrameworkCore;
using ZapatillasProject.Models;

namespace ZapatillasProject.Services
{
    // Crea el usuario Admin inicial si no existe (la contraseña se guarda con BCrypt)
    public static class DbSeeder
    {
        public static async Task CrearAdminAsync(IServiceProvider services, IConfiguration config, ILogger logger)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ZapatillasDbContext>();

            try
            {
                // en las pruebas (base en memoria) se crea el esquema con los datos iniciales
                if (context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
                {
                    await context.Database.EnsureCreatedAsync();
                }

                var email = config["Admin:Email"] ?? "admin@zapatillas.cl";
                var password = config["Admin:Password"] ?? "Admin123!";

                if (!await context.Usuarios.AnyAsync(u => u.Email == email))
                {
                    context.Usuarios.Add(new Usuario
                    {
                        Nombre = "Administrador",
                        Email = email,
                        Password = BCrypt.Net.BCrypt.HashPassword(password),
                        IdRol = 1
                    });
                    await context.SaveChangesAsync();
                    logger.LogInformation("Usuario admin creado: {Email}", email);
                }
            }
            catch (Exception ex)
            {
                // si la base aun no tiene las migraciones aplicadas, la API igual arranca
                logger.LogWarning("No se pudo crear el admin inicial (¿aplicaste Update-Database?): {Mensaje}", ex.Message);
            }
        }
    }
}

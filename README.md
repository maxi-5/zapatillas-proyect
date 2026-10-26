# API Tienda de Zapatillas — Trabajo 2 Construcción de Software

API RESTful en **ASP.NET Core Web API (.NET 10, con Controllers)** + **Entity Framework Core** + **SQL Server** para gestionar el catálogo, el stock por talla, las ventas y las reseñas de una tienda de zapatillas.

---

## 1. Caso de negocio

Una tienda de zapatillas necesita digitalizar su catálogo y sus ventas:

- El **Administrador** gestiona el catálogo (zapatillas, marcas, categorías), el **stock por talla** y los usuarios, y revisa todas las ventas.
- El **Cliente** se registra, consulta el catálogo, **compra** (el sistema calcula el monto y descuenta el stock de la talla) y deja **reseñas** con valoración de 1 a 5.

**Recurso principal:** `Zapatilla` (CRUD completo).

## 2. Base de datos (ZapatillasDB)

```
Rol 1───* Usuario 1───* Venta 1───* DetalleVenta *───1 Zapatilla
                 └──────* Resena *──────────────────────┘ │
Categoria 1───* Zapatilla *───1 Marca                      │
                Zapatilla 1───* StockTalla ────────────────┘
```

| Tabla | Para qué sirve |
|---|---|
| Rol | Admin (1) y Cliente (2) |
| Usuario | Login (email único + contraseña hasheada con BCrypt) |
| Marca / Categoria | Clasificación del catálogo |
| Zapatilla | **Recurso principal**: modelo, precio, foto, descripción |
| StockTalla | Cantidad disponible de cada zapatilla por talla (ej. 42.5) |
| Venta / DetalleVenta | Compra y sus ítems (1 detalle = 1 par, con el precio del momento) |
| Resena | Opinión y valoración (1–5) de un usuario sobre una zapatilla |

Las tablas, columnas y llaves foráneas son **las mismas del script validado**. Además se agregó un índice **UNIQUE en `Usuario.email`** (porque el login se hace con el email) y **datos iniciales** (roles, 4 categorías y 4 marcas).

## 3. Arquitectura

```
ZapatillasProject/
├── Controllers/     → endpoints REST (versionados /api/v1/...)
├── DTOs/            → lo que entra y sale de la API, con validaciones [Required], [Range]...
├── Models/          → entidades + ZapatillasDbContext (mapeo a las tablas)
├── Repositories/    → patrón Repository (los controladores NO usan el DbContext directo)
├── Services/        → TokenService (JWT) y DbSeeder (crea el admin inicial)
├── Middleware/      → LoggingMiddleware y ErrorHandlerMiddleware (propios)
├── Migrations/      → migración Inicial (creada con Add-Migration)
└── Program.cs       → inyección de dependencias, JWT, versionamiento, Swagger, pipeline
ZapatillasProject.Tests/ → pruebas de integración (xUnit + WebApplicationFactory)
```

**Flujo de una petición:**
`Cliente HTTP → ErrorHandlerMiddleware → LoggingMiddleware → Autenticación JWT → Autorización por rol → Controller → Repository → DbContext (EF Core) → SQL Server`

### Inyección de dependencias y scopes

| Servicio | Scope | Por qué |
|---|---|---|
| `ZapatillasDbContext` | Scoped | Un contexto por petición HTTP (no es thread-safe) |
| `IRepository<T>`, `IZapatillaRepository`, etc. | Scoped | Usan el DbContext, así que viven lo mismo que él |
| `ITokenService` | Singleton | No guarda estado: solo lee la configuración y firma tokens |
| Middlewares | Una instancia por aplicación | Reciben `ILogger` en el constructor y usan `HttpContext` en cada llamada |

### Middleware propio

- **LoggingMiddleware**: registra el método, la ruta, el código de respuesta, el tiempo en ms y el usuario de cada petición.
- **ErrorHandlerMiddleware**: captura cualquier excepción y responde un JSON `ProblemDetails` (500, o 409 si falla la BD) sin mostrar el stack trace.

## 4. Endpoints (v1)

| Método | Ruta | Acceso | Respuestas |
|---|---|---|---|
| POST | `/api/v1/User/registro` | Público | 201, 400, 409 |
| POST | `/api/v1/User/login` | Público | 200 (token), 401 |
| GET | `/api/v1/User/perfil` | Logueado | 200, 401 |
| GET | `/api/v1/User` · `/{id}` | Admin | 200, 403, 404 |
| PUT | `/api/v1/User/{id}/rol` | Admin | 204, 400, 404 |
| DELETE | `/api/v1/User/{id}` | Admin | 204, 404 |
| **GET** | **`/api/v1/Zapatillas?idMarca=&idCategoria=`** | Público | 200 |
| **GET** | **`/api/v1/Zapatillas/{id}`** | Público | 200, 404 |
| **POST** | **`/api/v1/Zapatillas`** | Admin | 201, 400, 401, 403 |
| **PUT** | **`/api/v1/Zapatillas/{id}`** | Admin | 204, 400, 404 |
| **DELETE** | **`/api/v1/Zapatillas/{id}`** | Admin | 204, 404, 409 (tiene ventas) |
| GET/POST/PUT/DELETE | `/api/v1/Marcas`, `/api/v1/Categorias` | GET logueado · escritura Admin | 200, 201, 204, 400, 404, 409 |
| GET | `/api/v1/StockTallas/zapatilla/{id}` | Público | 200, 404 |
| POST/PUT/DELETE | `/api/v1/StockTallas` | Admin | 201, 204, 400, 404, 409 |
| GET | `/api/v1/Resenas/zapatilla/{id}` | Público | 200, 404 |
| POST | `/api/v1/Resenas` | Cliente o Admin | 201, 400, 401 |
| DELETE | `/api/v1/Resenas/{id}` | Admin | 204, 404 |
| POST | `/api/v1/Ventas` | Cliente o Admin | 201, 400, 409 (sin stock) |
| GET | `/api/v1/Ventas/mis-compras` | Logueado | 200 |
| GET | `/api/v1/Ventas` | Admin | 200, 403 |
| GET | `/api/v1/Ventas/{id}` | Admin o dueño de la venta | 200, 403, 404 |
| DELETE | `/api/v1/Ventas/{id}` | Admin | 204, 404 |

**Ejemplo de compra** (`POST /api/v1/Ventas`):
```json
{ "tipoPago": "Debito", "detalles": [ { "idZapatilla": 1, "talla": 42 }, { "idZapatilla": 3, "talla": 40.5 } ] }
```
El servidor toma el precio desde la BD, calcula `monto`, genera `numero_venta`, toma el usuario desde el token y descuenta 1 unidad de stock por cada par, todo en una sola transacción.

## 5. Cómo ejecutarlo

### Requisitos
- Visual Studio Community 2026 (o 2022 17.14+) con la carga "Desarrollo de ASP.NET y web" y el **SDK de .NET 10**
- SQL Server Express (`.\SQLEXPRESS`) + SQL Server Management Studio

### Paso 1 — Secretos en variables de entorno (una vez)
Doble clic en **`configurar-variables.bat`**. Guarda en Windows:
- `ConnectionStrings__MiConexion` → la cadena de conexión
- `Jwt__Key` → una llave aleatoria de 64 caracteres

Si tu SQL Server usa usuario y contraseña (como en clases: `User Id=sa;Password=...`), edita la línea `CONEXION` del `.bat` antes de ejecutarlo. **Después cierra y vuelve a abrir Visual Studio.**

> Los secretos nunca quedan en `appsettings.json` ni en el repositorio. ASP.NET Core lee automáticamente las variables de entorno (`__` equivale a `:`, así que `Jwt__Key` se lee como `Jwt:Key`).

### Paso 2 — Crear la base con migraciones
> ⚠️ **No ejecutes el script SQL a mano.** La migración crea la misma base de datos. Si ya la habías creado con el script, bórrala antes en SSMS (`DROP DATABASE ZapatillasDB`).

La migración `Inicial` ya está en la carpeta `Migrations`, solo hay que aplicarla. En Visual Studio: Herramientas → Administrador de paquetes NuGet → **Consola del Administrador de paquetes** (proyecto predeterminado: ZapatillasProject):
```powershell
Update-Database
```
O desde la terminal: `dotnet ef database update --project ZapatillasProject`

### Paso 3 — Ejecutar y probar en Swagger
1. Presiona **F5** (perfil https). Se abre `https://localhost:7274/swagger`.
2. `POST /api/v1/User/login` con:
   ```json
   { "email": "admin@zapatillas.cl", "password": "Admin123!" }
   ```
3. Copia el `token`, presiona **Authorize** 🔒 y pégalo (solo el token, sin "Bearer").
4. Ya puedes probar los endpoints de Admin. Para probar como Cliente, registra un usuario en `/User/registro` y haz login con él.

### Paso 4 — Pruebas de integración
Prueba → **Explorador de pruebas** → Ejecutar todo (o `dotnet test`). Las pruebas levantan la API completa en memoria (JWT, roles, middleware, repositorios) con una base InMemory, así que no tocan tu base real.

**Qué se prueba (19 pruebas):** login correcto y con contraseña incorrecta (401), registro (201), email repetido (409), datos inválidos (400), permisos por rol (401 sin token, 403 con rol Cliente), el CRUD completo de Zapatillas con sus códigos (201, 200, 204, 404), la validación de la marca, los filtros, una venta que calcula el monto y descuenta el stock, una venta sin stock (409), un tipo de pago inválido (400) y una valoración fuera de rango (400).

## 6. Entregables

| Entregable | Dónde |
|---|---|
| Swagger | `https://localhost:7274/swagger` y exportado en `evidencias/swagger.json` |
| Evidencia de las pruebas | `evidencias/resultados-pruebas.html` (+ captura del Explorador de pruebas) |
| Migraciones | carpeta `ZapatillasProject/Migrations` + tabla `__EFMigrationsHistory` en SSMS |
| Repositorio Git | ver la sección 7 |

## 7. Git

```powershell
git init
git add .
git commit -m "Estructura base: modelos, DbContext y migracion inicial"
```
Sube los cambios por partes, con mensajes claros (por ejemplo: repositorios, CRUD de zapatillas, JWT y roles, versionamiento y Swagger, middleware, pruebas). Revisa que **`bin/`, `obj/` y `.vs/` no se suban** (ya están en `.gitignore`) y que ningún archivo tenga la cadena de conexión ni la llave.

## 8. Guion corto para la exposición

1. **Negocio (1 min):** problema de la tienda, los dos roles y qué puede hacer cada uno.
2. **Base de datos (2 min):** el diagrama de la sección 2; por qué `StockTalla` es una tabla aparte (una zapatilla tiene muchas tallas), por qué `DetalleVenta` guarda el precio (si el precio cambia mañana, la venta histórica no cambia) y por qué el email es UNIQUE.
3. **Arquitectura (2 min):** Controller → Repository → DbContext; DTOs para no exponer las entidades (por ejemplo, el password nunca sale en un JSON); los scopes de DI.
4. **Seguridad (2 min):** el login con BCrypt, el JWT con sus claims (`sub`, `id_user`, `nombre`, `role`), `[Authorize(Roles="Admin")]`; demo de 401 sin token, 403 con Cliente y 201 con Admin; los secretos en variables de entorno.
5. **Documentación y versionamiento (1 min):** `/api/v1/` con Asp.Versioning y Swagger con el botón Authorize.
6. **Pruebas (1 min):** mostrar el Explorador de pruebas en verde.

**Preguntas probables:**
- *¿Por qué Scoped y no Singleton para el DbContext?* Porque guarda estado (el change tracker) y no es thread-safe. Uno por petición evita mezclar datos entre usuarios.
- *¿Dónde se valida?* En los DTOs con DataAnnotations: `[ApiController]` responde 400 automáticamente. Las reglas de negocio (si la marca existe, si hay stock) se validan en el controlador.
- *¿Qué pasa si hay una excepción?* La captura `ErrorHandlerMiddleware` y responde ProblemDetails.
- *¿Por qué el cliente no envía el precio ni el monto?* Para que no pueda manipularlos: el servidor los calcula desde la BD.
- *Limitación conocida:* el esquema validado no guarda la talla en `DetalleVenta`, así que la talla se usa para descontar el stock pero no queda registrada en el detalle. Una mejora futura sería agregar las columnas `talla` y `cantidad`.

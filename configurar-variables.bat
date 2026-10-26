@echo off
REM ============================================================
REM  Guarda los SECRETOS como variables de entorno de Windows
REM  (no se suben al repositorio). Ejecutar UNA sola vez.
REM  Si tu SQL Server usa usuario/clave, edita la linea CONEXION:
REM  Server=.\SQLEXPRESS;Database=ZapatillasDB;User Id=sa;Password=TU_CLAVE;TrustServerCertificate=True;
REM ============================================================
set "CONEXION=Server=.\SQLEXPRESS;Database=ZapatillasDB;Trusted_Connection=True;TrustServerCertificate=True;"

for /f %%k in ('powershell -NoProfile -Command "[guid]::NewGuid().ToString('N') + [guid]::NewGuid().ToString('N')"') do set "LLAVE=%%k"

setx ConnectionStrings__MiConexion "%CONEXION%" >nul
setx Jwt__Key "%LLAVE%" >nul

echo.
echo Variables de entorno guardadas:
echo   ConnectionStrings__MiConexion = %CONEXION%
echo   Jwt__Key = (llave aleatoria de 64 caracteres)
echo.
echo IMPORTANTE: cierra y vuelve a abrir Visual Studio para que las lea.
pause

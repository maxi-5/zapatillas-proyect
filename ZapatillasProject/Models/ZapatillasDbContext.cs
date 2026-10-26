using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ZapatillasProject.Models;

public partial class ZapatillasDbContext : DbContext
{
    public ZapatillasDbContext()
    {
    }

    public ZapatillasDbContext(DbContextOptions<ZapatillasDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categorium> Categoria { get; set; }

    public virtual DbSet<DetalleVentum> DetalleVenta { get; set; }

    public virtual DbSet<Marca> Marcas { get; set; }

    public virtual DbSet<Resena> Resenas { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<StockTalla> StockTallas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Ventum> Venta { get; set; }

    public virtual DbSet<Zapatilla> Zapatillas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categorium>(entity =>
        {
            entity.HasKey(e => e.IdCategoria);

            entity.ToTable("Categoria");

            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<DetalleVentum>(entity =>
        {
            entity.HasKey(e => e.IdDetalle);

            entity.ToTable("DetalleVenta");

            entity.Property(e => e.IdDetalle).HasColumnName("id_detalle");
            entity.Property(e => e.IdVenta).HasColumnName("id_venta");
            entity.Property(e => e.IdZapatilla).HasColumnName("id_zapatilla");
            entity.Property(e => e.Precio).HasColumnName("precio");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.IdVenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleVenta_Venta");

            entity.HasOne(d => d.IdZapatillaNavigation).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.IdZapatilla)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleVenta_Zapatilla");
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.HasKey(e => e.IdMarca);

            entity.ToTable("Marca");

            entity.Property(e => e.IdMarca).HasColumnName("id_marca");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Pais)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("pais");
        });

        modelBuilder.Entity<Resena>(entity =>
        {
            entity.HasKey(e => e.IdResena);

            entity.ToTable("Resena");

            entity.Property(e => e.IdResena).HasColumnName("id_resena");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.IdZapatilla).HasColumnName("id_zapatilla");
            entity.Property(e => e.Resena1)
                .IsUnicode(false)
                .HasColumnName("resena");
            entity.Property(e => e.Valoracion).HasColumnName("valoracion");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Resenas)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK_Resena_Usuario");

            entity.HasOne(d => d.IdZapatillaNavigation).WithMany(p => p.Resenas)
                .HasForeignKey(d => d.IdZapatilla)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Resena_Zapatilla");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol);

            entity.ToTable("Rol");

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<StockTalla>(entity =>
        {
            entity.HasKey(e => e.IdStock);

            entity.ToTable("StockTalla");

            entity.Property(e => e.IdStock).HasColumnName("id_stock");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.IdZapatilla).HasColumnName("id_zapatilla");
            entity.Property(e => e.Talla)
                .HasColumnType("numeric(4, 1)")
                .HasColumnName("talla");

            entity.HasOne(d => d.IdZapatillaNavigation).WithMany(p => p.StockTallas)
                .HasForeignKey(d => d.IdZapatilla)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockTalla_Zapatilla");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUser);

            entity.ToTable("Usuario");

            // el email se usa para el login, no se puede repetir
            entity.HasIndex(e => e.Email, "UQ_Usuario_Email").IsUnique();

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Password)
                .IsUnicode(false)
                .HasColumnName("password");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Rol");
        });

        modelBuilder.Entity<Ventum>(entity =>
        {
            entity.HasKey(e => e.IdVenta);

            entity.ToTable("Venta");

            entity.Property(e => e.IdVenta).HasColumnName("id_venta");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Monto).HasColumnName("monto");
            entity.Property(e => e.NumeroVenta).HasColumnName("numero_venta");
            entity.Property(e => e.TipoPago)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipo_pago");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK_Venta_Usuario");
        });

        modelBuilder.Entity<Zapatilla>(entity =>
        {
            entity.HasKey(e => e.IdZapatilla);

            entity.ToTable("Zapatilla");

            entity.Property(e => e.IdZapatilla).HasColumnName("id_zapatilla");
            entity.Property(e => e.Descripcion)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Foto)
                .IsUnicode(false)
                .HasColumnName("foto");
            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.IdMarca).HasColumnName("id_marca");
            entity.Property(e => e.Modelo)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("modelo");
            entity.Property(e => e.Precio).HasColumnName("precio");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Zapatillas)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Zapatilla_Categoria");

            entity.HasOne(d => d.IdMarcaNavigation).WithMany(p => p.Zapatillas)
                .HasForeignKey(d => d.IdMarca)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Zapatilla_Marca");
        });

        // datos iniciales: roles, categorias y marcas
        modelBuilder.Entity<Rol>().HasData(
            new Rol { IdRol = 1, Nombre = "Admin" },
            new Rol { IdRol = 2, Nombre = "Cliente" });

        modelBuilder.Entity<Categorium>().HasData(
            new Categorium { IdCategoria = 1, Nombre = "Running" },
            new Categorium { IdCategoria = 2, Nombre = "Urbana" },
            new Categorium { IdCategoria = 3, Nombre = "Basketball" },
            new Categorium { IdCategoria = 4, Nombre = "Skate" });

        modelBuilder.Entity<Marca>().HasData(
            new Marca { IdMarca = 1, Nombre = "Nike", Pais = "Estados Unidos" },
            new Marca { IdMarca = 2, Nombre = "Adidas", Pais = "Alemania" },
            new Marca { IdMarca = 3, Nombre = "Puma", Pais = "Alemania" },
            new Marca { IdMarca = 4, Nombre = "Vans", Pais = "Estados Unidos" });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

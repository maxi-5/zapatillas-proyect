-- Script original validado por el docente (solo como REFERENCIA del diseno).
-- La base se crea con las migraciones de EF Core (Add-Migration / Update-Database),
-- que generan exactamente estas tablas, columnas y llaves foraneas.
USE [master]
GO
CREATE DATABASE [ZapatillasDB]
GO
USE [ZapatillasDB]
GO
CREATE TABLE [dbo].[Rol]([id_rol] [int] IDENTITY(1,1) NOT NULL,[nombre] [varchar](50) NOT NULL, CONSTRAINT [PK_Rol] PRIMARY KEY CLUSTERED ([id_rol] ASC))
GO
CREATE TABLE [dbo].[Categoria]([id_categoria] [int] IDENTITY(1,1) NOT NULL,[nombre] [varchar](50) NOT NULL, CONSTRAINT [PK_Categoria] PRIMARY KEY CLUSTERED ([id_categoria] ASC))
GO
CREATE TABLE [dbo].[Marca]([id_marca] [int] IDENTITY(1,1) NOT NULL,[nombre] [varchar](150) NOT NULL,[pais] [varchar](50) NOT NULL, CONSTRAINT [PK_Marca] PRIMARY KEY CLUSTERED ([id_marca] ASC))
GO
CREATE TABLE [dbo].[Usuario]([id_user] [int] IDENTITY(1,1) NOT NULL,[nombre] [varchar](150) NOT NULL,[email] [varchar](150) NOT NULL,[password] [varchar](max) NOT NULL,[id_rol] [int] NOT NULL, CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED ([id_user] ASC))
GO
CREATE TABLE [dbo].[Zapatilla]([id_zapatilla] [int] IDENTITY(1,1) NOT NULL,[modelo] [varchar](150) NOT NULL,[precio] [int] NOT NULL,[foto] [varchar](max) NULL,[id_categoria] [int] NOT NULL,[id_marca] [int] NOT NULL,[descripcion] [varchar](max) NULL, CONSTRAINT [PK_Zapatilla] PRIMARY KEY CLUSTERED ([id_zapatilla] ASC))
GO
CREATE TABLE [dbo].[Venta]([id_venta] [int] IDENTITY(1,1) NOT NULL,[numero_venta] [int] NOT NULL,[tipo_pago] [varchar](50) NOT NULL,[monto] [int] NOT NULL,[fecha] [date] NOT NULL,[id_user] [int] NULL, CONSTRAINT [PK_Venta] PRIMARY KEY CLUSTERED ([id_venta] ASC))
GO
CREATE TABLE [dbo].[StockTalla]([id_stock] [int] IDENTITY(1,1) NOT NULL,[id_zapatilla] [int] NOT NULL,[talla] [numeric](4,1) NOT NULL,[cantidad] [int] NOT NULL, CONSTRAINT [PK_StockTalla] PRIMARY KEY CLUSTERED ([id_stock] ASC))
GO
CREATE TABLE [dbo].[DetalleVenta]([id_detalle] [int] IDENTITY(1,1) NOT NULL,[id_zapatilla] [int] NOT NULL,[precio] [int] NOT NULL,[id_venta] [int] NOT NULL, CONSTRAINT [PK_DetalleVenta] PRIMARY KEY CLUSTERED ([id_detalle] ASC))
GO
CREATE TABLE [dbo].[Resena]([id_resena] [int] IDENTITY(1,1) NOT NULL,[resena] [varchar](max) NOT NULL,[valoracion] [int] NOT NULL,[id_zapatilla] [int] NOT NULL,[id_user] [int] NULL, CONSTRAINT [PK_Resena] PRIMARY KEY CLUSTERED ([id_resena] ASC))
GO
ALTER TABLE [dbo].[Usuario] WITH CHECK ADD CONSTRAINT [FK_Usuario_Rol] FOREIGN KEY([id_rol]) REFERENCES [dbo].[Rol] ([id_rol])
GO
ALTER TABLE [dbo].[Zapatilla] WITH CHECK ADD CONSTRAINT [FK_Zapatilla_Categoria] FOREIGN KEY([id_categoria]) REFERENCES [dbo].[Categoria] ([id_categoria])
GO
ALTER TABLE [dbo].[Zapatilla] WITH CHECK ADD CONSTRAINT [FK_Zapatilla_Marca] FOREIGN KEY([id_marca]) REFERENCES [dbo].[Marca] ([id_marca])
GO
ALTER TABLE [dbo].[Venta] WITH CHECK ADD CONSTRAINT [FK_Venta_Usuario] FOREIGN KEY([id_user]) REFERENCES [dbo].[Usuario] ([id_user])
GO
ALTER TABLE [dbo].[StockTalla] WITH CHECK ADD CONSTRAINT [FK_StockTalla_Zapatilla] FOREIGN KEY([id_zapatilla]) REFERENCES [dbo].[Zapatilla] ([id_zapatilla])
GO
ALTER TABLE [dbo].[DetalleVenta] WITH CHECK ADD CONSTRAINT [FK_DetalleVenta_Zapatilla] FOREIGN KEY([id_zapatilla]) REFERENCES [dbo].[Zapatilla] ([id_zapatilla])
GO
ALTER TABLE [dbo].[DetalleVenta] WITH CHECK ADD CONSTRAINT [FK_DetalleVenta_Venta] FOREIGN KEY([id_venta]) REFERENCES [dbo].[Venta] ([id_venta])
GO
ALTER TABLE [dbo].[Resena] WITH CHECK ADD CONSTRAINT [FK_Resena_Zapatilla] FOREIGN KEY([id_zapatilla]) REFERENCES [dbo].[Zapatilla] ([id_zapatilla])
GO
ALTER TABLE [dbo].[Resena] WITH CHECK ADD CONSTRAINT [FK_Resena_Usuario] FOREIGN KEY([id_user]) REFERENCES [dbo].[Usuario] ([id_user])
GO

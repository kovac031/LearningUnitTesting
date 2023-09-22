USE [master]
GO
/****** Object:  Database [JustStudents]    Script Date: 22/09/2023 14:17:25 ******/
CREATE DATABASE [JustStudents]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'JustStudents', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\JustStudents.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'JustStudents_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\JustStudents_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [JustStudents] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [JustStudents].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [JustStudents] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [JustStudents] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [JustStudents] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [JustStudents] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [JustStudents] SET ARITHABORT OFF 
GO
ALTER DATABASE [JustStudents] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [JustStudents] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [JustStudents] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [JustStudents] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [JustStudents] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [JustStudents] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [JustStudents] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [JustStudents] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [JustStudents] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [JustStudents] SET  ENABLE_BROKER 
GO
ALTER DATABASE [JustStudents] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [JustStudents] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [JustStudents] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [JustStudents] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [JustStudents] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [JustStudents] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [JustStudents] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [JustStudents] SET RECOVERY FULL 
GO
ALTER DATABASE [JustStudents] SET  MULTI_USER 
GO
ALTER DATABASE [JustStudents] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [JustStudents] SET DB_CHAINING OFF 
GO
ALTER DATABASE [JustStudents] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [JustStudents] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [JustStudents] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [JustStudents] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'JustStudents', N'ON'
GO
ALTER DATABASE [JustStudents] SET QUERY_STORE = ON
GO
ALTER DATABASE [JustStudents] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [JustStudents]
GO
/****** Object:  Table [dbo].[Student]    Script Date: 22/09/2023 14:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Student](
	[Id] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[DateOfBirth] [datetime] NOT NULL,
	[EmailAddress] [varchar](255) NOT NULL,
	[RegisteredOn] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Student] ([Id], [FirstName], [LastName], [DateOfBirth], [EmailAddress], [RegisteredOn]) VALUES (N'35c85f72-13bf-4047-907c-1cc28eede96c', N'Kwame', N'Mensah', CAST(N'2001-04-15T00:00:00.000' AS DateTime), N'kwame.mensah@mail.com', CAST(N'2022-08-15T14:31:58.000' AS DateTime))
GO
INSERT [dbo].[Student] ([Id], [FirstName], [LastName], [DateOfBirth], [EmailAddress], [RegisteredOn]) VALUES (N'fe80338c-88fa-4e6c-b60e-23c636892cd9', N'İsmail', N'Yılmaz', CAST(N'2001-06-10T00:00:00.000' AS DateTime), N'ismail.yilmaz@turkpost.com.tr', CAST(N'2021-10-08T06:13:58.000' AS DateTime))
GO
INSERT [dbo].[Student] ([Id], [FirstName], [LastName], [DateOfBirth], [EmailAddress], [RegisteredOn]) VALUES (N'5425ef56-9b52-43a2-8dde-2579680b067b', N'Kai', N'Yamamoto', CAST(N'1987-01-21T00:00:00.000' AS DateTime), N'kai.y@icloud.com', CAST(N'2020-09-01T14:34:11.000' AS DateTime))
GO
INSERT [dbo].[Student] ([Id], [FirstName], [LastName], [DateOfBirth], [EmailAddress], [RegisteredOn]) VALUES (N'aaf0c978-16e6-43b4-943a-326c6c3aa958', N'Sergei', N'Petrov', CAST(N'1999-01-04T00:00:00.000' AS DateTime), N'sergei.p@yandex.com', CAST(N'2020-10-23T06:05:30.000' AS DateTime))
GO
INSERT [dbo].[Student] ([Id], [FirstName], [LastName], [DateOfBirth], [EmailAddress], [RegisteredOn]) VALUES (N'6f5211b5-d541-4f88-8210-33e0b709902d', N'Chioma', N'Okafor', CAST(N'2001-11-06T00:00:00.000' AS DateTime), N'chioma.okafor@aol.com', CAST(N'2020-01-23T14:21:12.000' AS DateTime))
GO
INSERT [dbo].[Student] ([Id], [FirstName], [LastName], [DateOfBirth], [EmailAddress], [RegisteredOn]) VALUES (N'faa6a496-85e2-4799-b327-3cb634d3cfec', N'John', N'Doe', CAST(N'2001-07-22T00:00:00.000' AS DateTime), N'jdoe420@gmail.com', CAST(N'2023-06-15T05:53:15.000' AS DateTime))
GO
INSERT [dbo].[Student] ([Id], [FirstName], [LastName], [DateOfBirth], [EmailAddress], [RegisteredOn]) VALUES (N'17bdb6bf-da54-4626-b183-5269a0e84915', N'Chen', N'Wang', CAST(N'2004-12-04T00:00:00.000' AS DateTime), N'chen.wang@protonmail.com', CAST(N'2023-05-15T16:49:50.000' AS DateTime))
GO
INSERT [dbo].[Student] ([Id], [FirstName], [LastName], [DateOfBirth], [EmailAddress], [RegisteredOn]) VALUES (N'10bd8ba2-7132-441d-906d-53e103189802', N'Mårten', N'Åberg', CAST(N'2000-12-13T00:00:00.000' AS DateTime), N'marten.aberg@swedishmail.se', CAST(N'2022-09-08T20:45:15.000' AS DateTime))
GO
INSERT [dbo].[Student] ([Id], [FirstName], [LastName], [DateOfBirth], [EmailAddress], [RegisteredOn]) VALUES (N'7e4c3c78-c8f0-45ac-bc64-6796fc9d27e9', N'Emilia', N'Rodriguez', CAST(N'1999-12-26T00:00:00.000' AS DateTime), N'emilia.rodriguez@yahoo.com', CAST(N'2023-07-10T22:37:37.000' AS DateTime))
GO
INSERT [dbo].[Student] ([Id], [FirstName], [LastName], [DateOfBirth], [EmailAddress], [RegisteredOn]) VALUES (N'df38d6ac-6d8e-4577-8c54-6ce5d15df3d4', N'Sanjay', N'Patel', CAST(N'1984-04-29T00:00:00.000' AS DateTime), N'sanjay.p@hotmail.com', CAST(N'2019-05-23T01:12:43.000' AS DateTime))
GO
INSERT [dbo].[Student] ([Id], [FirstName], [LastName], [DateOfBirth], [EmailAddress], [RegisteredOn]) VALUES (N'24a15d25-34d2-455e-a9e4-730d7f93455f', N'Mohamed', N'Saleh', CAST(N'1990-01-01T00:00:00.000' AS DateTime), N'mohamed.s@zoho.com', CAST(N'2021-07-09T01:27:09.000' AS DateTime))
GO
INSERT [dbo].[Student] ([Id], [FirstName], [LastName], [DateOfBirth], [EmailAddress], [RegisteredOn]) VALUES (N'37390480-561a-41ab-9e88-86ce1b2c1ce7', N'Lúcia', N'Fernández', CAST(N'2004-05-19T00:00:00.000' AS DateTime), N'lucia.fernandez@correo.es', CAST(N'2022-12-10T22:22:25.000' AS DateTime))
GO
INSERT [dbo].[Student] ([Id], [FirstName], [LastName], [DateOfBirth], [EmailAddress], [RegisteredOn]) VALUES (N'ae44b9b1-c8ab-401a-9203-99d970087c5b', N'Yasmin', N'Al-Salem', CAST(N'2005-03-20T00:00:00.000' AS DateTime), N'yasmin.alsalem@outlook.com', CAST(N'2022-04-24T16:33:20.000' AS DateTime))
GO
INSERT [dbo].[Student] ([Id], [FirstName], [LastName], [DateOfBirth], [EmailAddress], [RegisteredOn]) VALUES (N'474aa50a-bb5e-40f7-8cd5-9cf9d5a735f1', N'Ivan', N'Kovač', CAST(N'1992-03-24T00:00:00.000' AS DateTime), N'ivan.kovac.email@gmail.com', CAST(N'2023-07-20T01:35:19.000' AS DateTime))
GO
INSERT [dbo].[Student] ([Id], [FirstName], [LastName], [DateOfBirth], [EmailAddress], [RegisteredOn]) VALUES (N'aad374a0-247c-44e7-b635-9ff6925039fa', N'Eléonore', N'Dûmont', CAST(N'2004-08-01T00:00:00.000' AS DateTime), N'eleonore.dumont@orange.fr', CAST(N'2023-02-28T09:33:50.000' AS DateTime))
GO
INSERT [dbo].[Student] ([Id], [FirstName], [LastName], [DateOfBirth], [EmailAddress], [RegisteredOn]) VALUES (N'366dbd4b-f354-4436-acd9-f4779a42a603', N'Maya', N'Johnson', CAST(N'1998-02-10T00:00:00.000' AS DateTime), N'maya.j@msn.com', CAST(N'2019-05-10T00:38:00.000' AS DateTime))
GO
USE [master]
GO
ALTER DATABASE [JustStudents] SET  READ_WRITE 
GO

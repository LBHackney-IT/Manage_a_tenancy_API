USE [master]
GO
/****** Object:  Database [ManageATenancy]    Script Date: 20/06/2019 10:39:54 ******/
CREATE DATABASE [ManageATenancy]

USE [ManageATenancy]
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ManageATenancy].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

/****** Object:  Table [db_accessadmin].[__EFMigrationsHistory]    Script Date: 20/06/2019 12:17:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [db_accessadmin].[NewTenancyLastRun]    Script Date: 20/06/2019 12:17:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NewTenancyLastRun](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LastRun] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_NewTenancyLastRun] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190514151843_InitialCreate', N'2.1.11-servicing-32099')
GO



CREATE TABLE [dbo].[HousingArea](
	[AreaId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](150) NOT NULL,
 CONSTRAINT [PK_HousingArea] PRIMARY KEY CLUSTERED 
(
	[AreaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TRA]    Script Date: 20/06/2019 10:39:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TRA](
	[TRAId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](150) NOT NULL,
	[AreaId] [int] NOT NULL,
	[Email] [varchar](50) NULL,
	[PatchId] [uniqueidentifier] NULL,
	[Notes] [varchar](max) NULL,
 CONSTRAINT [PK_TRA] PRIMARY KEY CLUSTERED 
(
	[TRAId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TRAEstate]    Script Date: 20/06/2019 10:39:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TRAEstate](
	[TRAId] [int] NOT NULL,
	[EstateUHRef] [varchar](50) NOT NULL,
	[EstateName] [varchar](100) NULL,
 CONSTRAINT [PK_TRAEstates_1] PRIMARY KEY CLUSTERED 
(
	[TRAId] ASC,
	[EstateUHRef] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TRAPatchAssociation]    Script Date: 20/06/2019 10:39:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TRAPatchAssociation](
	[TRAId] [int] NOT NULL,
	[PatchCRMId] [varchar](150) NOT NULL,
 CONSTRAINT [PK_TRAPatchAssociation] PRIMARY KEY CLUSTERED 
(
	[TRAId] ASC,
	[PatchCRMId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TRARole]    Script Date: 20/06/2019 10:39:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TRARole](
	[Role] [varchar](150) NOT NULL,
	[Name] [varchar](150) NOT NULL,
 CONSTRAINT [PK_TRARoles] PRIMARY KEY CLUSTERED 
(
	[Role] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TRARoleAssignment]    Script Date: 20/06/2019 10:39:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TRARoleAssignment](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TRAId] [int] NOT NULL,
	[Role] [varchar](150) NOT NULL,
	[PersonName] [varchar](100) NULL,
 CONSTRAINT [PK_TRARoleAssignments] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[HousingArea] ON 
GO
INSERT [dbo].[HousingArea] ([AreaId], [Name]) VALUES (1, N'Central')
GO
INSERT [dbo].[HousingArea] ([AreaId], [Name]) VALUES (2, N'Clapton')
GO
INSERT [dbo].[HousingArea] ([AreaId], [Name]) VALUES (3, N'Homerton 1')
GO
INSERT [dbo].[HousingArea] ([AreaId], [Name]) VALUES (4, N'Homerton 2')
GO
INSERT [dbo].[HousingArea] ([AreaId], [Name]) VALUES (5, N'Shoreditch')
GO
INSERT [dbo].[HousingArea] ([AreaId], [Name]) VALUES (6, N'Stamford Hill')
GO
INSERT [dbo].[HousingArea] ([AreaId], [Name]) VALUES (7, N'Stoke Newington')
GO
SET IDENTITY_INSERT [dbo].[HousingArea] OFF
GO
SET IDENTITY_INSERT [dbo].[TRA] ON 
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (1, N'Alden and Broadway TRA', 1, N'test@test.com', NULL, N'Test')
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (2, N'Blackstone Estate TRA', 1, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (3, N'De Beauvoir TRA', 1, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (4, N'Lockner Estate TRA', 1, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (5, N'Middleton Road TRA', 1, N'testemail@middletonroad.com', NULL, N'test notes for middleton road estate')
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (6, N'Morland, Blanchard & Gayhurst TRA', 1, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (7, N'Regents Estate TRA', 1, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (8, N'Shrubland Estate TRA', 1, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (9, N'Warburton & Darcy TRA', 1, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (10, N'Welshpool Estate TRA', 1, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (11, N'Beecholme & Casimir Community Association', 2, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (12, N'Jack Watts TRA', 2, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (13, N'Keir Hardie Estate TRA', 2, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (14, N'Landfields Estate TRA', 2, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (15, N'Lea View House TRA', 2, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (16, N'Nightingale Partnership Residents Association', 2, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (17, N'Radley Square & Southwold TRA', 2, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (18, N'The Beckers TRA', 2, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (19, N'Tower Gardens TRA', 2, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (20, N'Wrens Park TRA', 2, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (21, N'Aspland and Marcon TRA', 3, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (22, N'Boscobel House TRA', 3, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (23, N'Frampton Park TRA', 3, NULL, NULL, N'Remember to take the keys. Room is in the Block  next door')
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (24, N'Mountford Estate and Sigdon Road Community TRA', 3, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (25, N'New Banbury Estate TRA', 3, NULL, NULL, N'new note 2')
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (26, N'Parkside TRA', 3, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (27, N'The Sylvester House Association', 3, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (28, N'Trelawney Estate TRA', 3, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (29, N'Valette House TRA', 3, NULL, NULL, N'HO notes about TRA')
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (30, N'Wayman Court TRA', 3, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (31, N'Wilton Estate TRA', 3, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (32, N'Banister House & Priory Court TRA', 4, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (33, N'Gascoyne 2 TRA', 4, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (34, N'Herbert Butler TRA', 4, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (35, N'Jack Dunning Estate TRA', 4, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (36, N'Linzell TRA', 4, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (37, N'Nisbet House TRA', 4, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (38, N'Sherrys Wharf TRA', 4, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (39, N'Trowbridge Resident Association', 4, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (40, N'Colville Estate TRA', 5, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (41, N'Hobbs Place Estate TRA', 5, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (42, N'Provost TRA', 5, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (43, N'Shepherds Market TRA', 5, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (44, N'St John Estate TRA', 5, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (45, N'St Marys Estate TRA', 5, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (46, N'Holmleigh Road Estate TRA', 6, NULL, NULL, N'dhdhd')
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (47, N'Joseph Court TRA', 6, NULL, NULL, N'22222')
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (48, N'Lincoln Court TRA', 6, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (49, N'Manor TRA', 6, N'string', NULL, N'notes added')
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (50, N'Webb Estate TRA', 6, NULL, NULL, N'new note 1')
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (51, N'Burma, Arakan and Clissord TRA', 7, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (52, N'Forest, Acer & Holly Street', 7, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (53, N'Hawksley Court TRA', 7, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (54, N'Milton Gardens TRA', 7, NULL, NULL, N'kjhlkjh')
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (55, N'North & South Defoe TRA', 7, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (56, N'Rhodes Estate TRA', 7, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (57, N'Shellgrove Estate TRA', 7, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (58, N'Smalley Road TRA', 7, NULL, NULL, N'Type your personal notes here')
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (59, N'Somerford and Shacklewell TRA', 7, NULL, NULL, NULL)
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (60, N'Yorkshire Grove TRA', 7, NULL, NULL, N'Remember to take the keys')
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (62, N'Nadd4b793e8 Estate TRA', 1, N'nadd4b793e8.com', N'f18b2363-8453-e811-8126-70106faaf8c1', N'Notes for Nad{id} Estate TRA')
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (63, N'Nad48fda8d6 Estate TRA', 1, N'nad48fda8d6.com', N'f18b2363-8453-e811-8126-70106faaf8c1', N'Notes for Nad{id} Estate TRA')
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (64, N'Nad07b5d3e6 Estate TRA', 1, N'nad07b5d3e6.com', N'f18b2363-8453-e811-8126-70106faaf8c1', N'Notes for Nad{id} Estate TRA')
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (65, N'Nadc90d0ab8 Estate TRA', 1, N'nadc90d0ab8.com', N'f18b2363-8453-e811-8126-70106faaf8c1', N'Notes for Nad{id} Estate TRA')
GO
INSERT [dbo].[TRA] ([TRAId], [Name], [AreaId], [Email], [PatchId], [Notes]) VALUES (66, N'Nad452de375 Estate TRA', 1, N'nad452de375.com', N'f18b2363-8453-e811-8126-70106faaf8c1', N'Notes for Nad{id} Estate TRA')
GO
SET IDENTITY_INSERT [dbo].[TRA] OFF
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (1, N'00078352', N'Green Lanes  Amwell Court Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (1, N'00078569', N'Broadway Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (2, N'00078568', N'Blackstone Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (3, N'00078614', N'De Beauvoir Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (4, N'00078619', N'Lockner Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (5, N'00078629', N'Middleton House Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (5, N'00078691', N'Middleton Road Properties')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (6, N'00078602', N'Morland Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (7, N'00078630', N'Regents Court Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (7, N'00078633', N'Regent Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (8, N'00078632', N'Shrubland Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (9, N'00078570', N'Warburton Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (10, N'00078572', N'Welshpool Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (11, N'00078500', N'Beecholme Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (12, N'00078495', N'Jack Watts Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (13, N'00078451', N'Keir Hardie Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (14, N'00078477', N'Landfield Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (15, N'00078457', N'Lea View Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (16, N'00078483', N'Nightingale Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (17, N'00078525', N'Radley Square Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (18, N'00078472', N'The Beckers Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (19, N'00078461', N'Tower Gardens Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (20, N'00078467', N'Wrens Park Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (21, N'00078519', N'Aspland Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (21, N'00078711', N'Marcon Court Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (22, N'00078586', N'Boscobel House Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (23, N'00078556', N'Frampton Park Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (24, N'00078583', N'Mountford Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (24, N'00078590', N'Sigdon Road Properties')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (25, N'00078543', N'Banbury Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (26, N'00078542', N'Parkside Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (27, N'00078567', N'Sylvester House Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (28, N'00078557', N'Trelawney Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (29, N'00078559', N'Valette House Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (30, N'00078603', N'Wayman Court Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (31, N'00078595', N'Wilton Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (32, N'00078516', N'Banister House Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (32, N'00078540', N'Priory Court Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (33, N'00078661', N'Gascoyne Estate New')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (34, N'00078550', N'Herbert Butler Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (35, N'00078512', N'Jack Dunning Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (36, N'00078531', N'Linzell Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (37, N'00078541', N'Nisbet House Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (38, N'00078536', N'Sherrys Wharf Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (39, N'00078551', N'Trowbridge Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (40, N'00078616', N'Colville Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (41, N'00078617', N'Hobbs Place Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (42, N'00078656', N'Provost Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (43, N'00078647', N'Shepherd Market Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (44, N'00078654', N'St Johns Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (45, N'00078642', N'St Marys Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (45, N'00078672', N'Dunloe Court Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (46, N'00078400', N'Holmleigh Road Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (47, N'00078369', N'Joseph Court Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (48, N'00078359', N'Lincoln Court Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (49, N'00078352', N'Amwell Court Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (49, N'00078358', N'Hill Court Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (50, N'00078446', N'Webb Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (51, N'00078387', N'Burma Court Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (52, N'00078581', N'Holly Street Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (53, N'00078379', N'Hawksley Court Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (54, N'00078577', N'Milton Gardens Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (55, N'00078383', N'Defoe Road Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (55, N'00089163', N'Beavis House Defoe Road plus Edward Friend House B')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (56, N'00078593', N'Rhodes Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (57, N'00078580', N'Shellgrove Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (58, N'00078404', N'Smalley Road Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (59, N'00078413', N'Shacklewell House Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (59, N'00078414', N'Somerford Grove Estate')
GO
INSERT [dbo].[TRAEstate] ([TRAId], [EstateUHRef], [EstateName]) VALUES (60, N'00078374', N'Yorkshire Grove Estate')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (1, N'4c37485a-8453-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (2, N'7d8f2e6c-8453-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (3, N'7d8f2e6c-8453-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (4, N'f18b2363-8453-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (5, N'f18b2363-8453-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (6, N'7d8f2e6c-8453-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (7, N'f18b2363-8453-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (8, N'748f2e6c-8453-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (9, N'c0d67dd5-8653-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (10, N'7d8f2e6c-8453-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (11, N'186142d3-8453-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (12, N'3f6142d3-8453-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (13, N'186142d3-8453-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (14, N'2bbdfbd9-8453-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (15, N'3dbdfbd9-8453-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (16, N'7cdab1c9-8453-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (17, N'3f6142d3-8453-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (18, N'2bbdfbd9-8453-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (19, N'3dbdfbd9-8453-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (20, N'3f6142d3-8453-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (21, N'eb3927b9-8553-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (22, N'2f4297c9-8553-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (23, N'3b5eb2d0-8553-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (24, N'eb3927b9-8553-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (25, N'a739aa44-8553-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (26, N'a739aa44-8553-e811-8126-70106faaf8c1"')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (27, N'eb3927b9-8553-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (28, N'2f4297c9-8553-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (29, N'3b5eb2d0-8553-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (30, N'2f4297c9-8553-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (31, N'2f4297c9-8553-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (32, N'ee3927b9-8553-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (33, N'5d4108a9-8553-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (34, N'4bea75b1-8553-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (35, N'ee3927b9-8553-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (36, N'f2ccbfc1-8553-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (37, N'f2ccbfc1-8553-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (38, N'ee3927b9-8553-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (39, N'4bea75b1-8553-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (40, N'99dd7677-7154-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (41, N'a521e922-7154-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (42, N'a521e922-7154-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (43, N'4401789b-7154-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (44, N'ec130b35-7154-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (45, N'bb967e89-7154-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (46, N'52008449-8653-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (47, N'b8388243-8653-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (48, N'b8388243-8653-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (49, N'8e958a37-8653-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (50, N'74d3823d-8653-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (51, N'95ef187e-8653-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (52, N'd3a22684-8653-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (53, N'95ef187e-8653-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (54, N'573c0b78-8653-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (55, N'baef187e-8653-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (56, N'd3a22684-8653-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (57, N'0a5dcc70-8653-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (58, N'f1a22684-8653-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (59, N'0a5dcc70-8653-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRAPatchAssociation] ([TRAId], [PatchCRMId]) VALUES (60, N'f1a22684-8653-e811-8126-70106faaf8c1')
GO
INSERT [dbo].[TRARole] ([Role], [Name]) VALUES (N'chair', N'Chair')
GO
INSERT [dbo].[TRARole] ([Role], [Name]) VALUES (N'secretary', N'Secretary')
GO
INSERT [dbo].[TRARole] ([Role], [Name]) VALUES (N'treasurer', N'Treasurer')
GO
INSERT [dbo].[TRARole] ([Role], [Name]) VALUES (N'vice_chair', N'Vice chair')
GO
ALTER TABLE [dbo].[TRA]  WITH CHECK ADD  CONSTRAINT [FK_TRA_Area] FOREIGN KEY([AreaId])
REFERENCES [dbo].[HousingArea] ([AreaId])
GO
ALTER TABLE [dbo].[TRA] CHECK CONSTRAINT [FK_TRA_Area]
GO
ALTER TABLE [dbo].[TRAEstate]  WITH CHECK ADD  CONSTRAINT [FK_TRAEstates_TRA] FOREIGN KEY([TRAId])
REFERENCES [dbo].[TRA] ([TRAId])
GO
ALTER TABLE [dbo].[TRAEstate] CHECK CONSTRAINT [FK_TRAEstates_TRA]
GO
ALTER TABLE [dbo].[TRAPatchAssociation]  WITH CHECK ADD  CONSTRAINT [FK_TRAPatchAssosiation_TRA] FOREIGN KEY([TRAId])
REFERENCES [dbo].[TRA] ([TRAId])
GO
ALTER TABLE [dbo].[TRAPatchAssociation] CHECK CONSTRAINT [FK_TRAPatchAssosiation_TRA]
GO
ALTER TABLE [dbo].[TRARoleAssignment]  WITH CHECK ADD  CONSTRAINT [FK_TRARoleAssignments_Roles] FOREIGN KEY([Role])
REFERENCES [dbo].[TRARole] ([Role])
GO
ALTER TABLE [dbo].[TRARoleAssignment] CHECK CONSTRAINT [FK_TRARoleAssignments_Roles]
GO
ALTER TABLE [dbo].[TRARoleAssignment]  WITH CHECK ADD  CONSTRAINT [FK_TRARoleAssignments_TRA] FOREIGN KEY([TRAId])
REFERENCES [dbo].[TRA] ([TRAId])
GO
ALTER TABLE [dbo].[TRARoleAssignment] CHECK CONSTRAINT [FK_TRARoleAssignments_TRA]
GO
/****** Object:  StoredProcedure [dbo].[get_tra_for_patch]    Script Date: 20/06/2019 10:39:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Returns a list of TRAs for a patch ID
-- =============================================
CREATE PROCEDURE [dbo].[get_tra_for_patch]
	@PatchCRMId varchar(100)
AS
BEGIN
   	SELECT * from TRA where TRAId in (select TRAId from TRAPatchAssociation where PatchCRMId=@PatchCRMId)
END
GO
/****** Object:  StoredProcedure [dbo].[get_tra_information]    Script Date: 20/06/2019 10:39:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Returns TRA information based on TRA ID
-- =============================================
CREATE PROCEDURE [dbo].[get_tra_information]
	-- Add the parameters for the stored procedure here
@TRAId int
AS
BEGIN

select * from TRA 
FULL OUTER JOIN TRARoleAssignment ON (TRARoleAssignment.TRAId = TRA.TRAId)
FULL OUTER JOIN TRAPatchAssociation ON (TRAPatchAssociation.TRAId = TRA.TRAId)
FULL OUTER JOIN TRAEstate ON (TRAEstate.TRAId = TRA.TRAId)	
FULL OUTER JOIN TRARole ON (TRARole.[Role]=TRARoleAssignment.[Role])
where TRA.TRAId=@TRAId


END
GO
USE [master]
GO
ALTER DATABASE [ManageATenancy] SET  READ_WRITE 
GO

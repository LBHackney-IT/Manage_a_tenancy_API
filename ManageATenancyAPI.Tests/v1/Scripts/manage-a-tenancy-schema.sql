/****** Object:  Table [dbo].[HousingArea]    Script Date: 13/12/2018 09:06:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
/****** Object:  Table [dbo].[TRA]    Script Date: 13/12/2018 09:06:57 ******/
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
/****** Object:  Table [dbo].[TRAEstate]    Script Date: 13/12/2018 09:06:57 ******/
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
/****** Object:  Table [dbo].[TRAPatchAssociation]    Script Date: 13/12/2018 09:06:57 ******/
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
/****** Object:  Table [dbo].[TRARole]    Script Date: 13/12/2018 09:06:57 ******/
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
/****** Object:  Table [dbo].[TRARoleAssignment]    Script Date: 13/12/2018 09:06:57 ******/
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
/****** Object:  StoredProcedure [dbo].[get_tra_for_patch]    Script Date: 13/12/2018 09:06:57 ******/
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
   	SELECT * from TRA where TRAId in (select TRAId from TRAPatchAssosiation where PatchCRMId=@PatchCRMId)
END
GO
/****** Object:  StoredProcedure [dbo].[get_tra_information]    Script Date: 13/12/2018 09:06:57 ******/
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
FULL OUTER JOIN TRARoleAssignments ON (TRARoleAssignments.TRAId = TRA.TRAId)
FULL OUTER JOIN TRAPatchAssociation ON (TRAPatchAssociation.TRAId = TRA.TRAId)
FULL OUTER JOIN TRAEstates ON (TRAEstates.TRAId = TRA.TRAId)	
FULL OUTER JOIN TRARoles ON (TRARoles.RoleId=TRARoleAssignments.RoleId)
where TRA.TRAId=@TRAId


END
GO

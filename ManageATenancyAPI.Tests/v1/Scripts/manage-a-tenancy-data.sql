﻿-- TraEstateRepositoryTests.AddEstateToTraThenRemove_EstateAddedEstateRemoved
INSERT INTO [dbo].[HousingArea] ([Name]) VALUES ('Area51')
INSERT INTO [dbo].[TRA]([Name],[AreaId],[Email],[PatchId],[Notes])
     VALUES ('TestTRA1',1,'test@test.com','85d2da23-93b2-4296-9acb-1aa9b3b035d8','Test note')
INSERT INTO [dbo].[TRA]([Name],[AreaId],[Email],[PatchId],[Notes])
     VALUES ('AddRemoveRole TRA',5,'test@test.com','85d2da23-93b2-4296-9acb-1aa9b3b035d8','AddRemoveRole TRA')
--TraEstateRepositoryTests.GetAllUsedEstateRefsTest_ReturnsList_OnlyUsed

INSERT INTO [dbo].[TRA]([Name],[AreaId],[Email],[PatchId],[Notes])
     VALUES ('TestTRA1',1,'test@test.com','85d2da23-93b2-4296-9acb-1aa9b3b035d9','Test note')
INSERT INTO [dbo].[TRAEstate]([TRAId],[EstateUHRef],[EstateName])
     VALUES (2,'EstateUHRef02','EstateName002');

INSERT INTO [dbo].[TRAEstate]([TRAId],[EstateUHRef],[EstateName])
     VALUES (2,'EstateUHRef03','EstateName003');

--TraEstateRepositoryTests.GetEstatesByTraId_ListEstates
INSERT INTO [dbo].[TRA]([Name],[AreaId],[Email],[PatchId],[Notes])
     VALUES ('TestTRA3',1,'test@test.com','85d2da23-93b2-4296-9acb-1aa9b3b035d9','Test note')
INSERT INTO [dbo].[TRAEstate]([TRAId],[EstateUHRef],[EstateName])
     VALUES (3,'EstateUHRef04','EstateName004');

--TraRoleAssignmentTests.TraRoleAssignmentTests_AddRoleAssignment
INSERT INTO TRARole (Role,Name) VALUES ('chair','Chair');
INSERT INTO TRARole (Role,Name) VALUES ('vice_chair','Vice chair');
INSERT INTO TRARole (Role,Name) VALUES ('secretary','Secretary');
INSERT INTO TRARole (Role,Name) VALUES ('treasurer','Treasurer');
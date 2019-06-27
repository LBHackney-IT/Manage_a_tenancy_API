 --BlockRepositoryTests.GetBlocksByEstateId
INSERT INTO [dbo].[property](prop_ref,major_ref, level_code,managed_property,ownership,letable,lounge,laundry,visitor_bed,store,warden_flat,sheltered,no_single_beds,no_double_beds,core_shared,shower,rtb,dtstamp,asbestos,online_repairs,repairable) 
VALUES ('BLOCKID0001','ESTATEID0001',3,'','','',0,0,0,0,0,0,0,0,0,0,0,0,0,0,0);

INSERT INTO [dbo].[property](prop_ref,major_ref, level_code,managed_property,ownership,letable,lounge,laundry,visitor_bed,store,warden_flat,sheltered,no_single_beds,no_double_beds,core_shared,shower,rtb,dtstamp,asbestos,online_repairs,repairable) 
VALUES ('BLOCKID0002','ESTATEID0001',3,'','','',0,0,0,0,0,0,0,0,0,0,0,0,0,0,0);

INSERT INTO [dbo].[property](prop_ref,major_ref, level_code,managed_property,ownership,letable,lounge,laundry,visitor_bed,store,warden_flat,sheltered,no_single_beds,no_double_beds,core_shared,shower,rtb,dtstamp,asbestos,online_repairs,repairable) 
VALUES ('BLOCKID0003','ESTATEID0001',3,'','','',0,0,0,0,0,0,0,0,0,0,0,0,0,0,0);


--EstateRepositoryTests.GetEstates_OneResult_Populated
INSERT INTO [dbo].[property](prop_ref,major_ref, level_code,managed_property,ownership,letable,lounge,laundry,visitor_bed,store,warden_flat,sheltered,no_single_beds,no_double_beds,core_shared,shower,rtb,dtstamp,asbestos,online_repairs,repairable,short_address) 
VALUES ('ESTATEID0011','',2,'','','',0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,'EstateName001');

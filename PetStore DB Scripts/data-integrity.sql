/* 
Purpose: Apply data integrity in PetStoreCrm database
Script Date: 09-04-2021
Developed by: Emma
*/

/*
REMINDER:
	1. Primary Key
	2. Foreign Key
	3. Default
	4. Unique (apply to all non-key columns)
	5. Check
*/

/*
CONSTRAINT TYPES
	1. Primary Key (pk_)
		alter table schema_name.table_name
			add constraint pk_table_name primary key clustered (column_name asc)

	2.Foreign Key (fk_)
		alter table schema_name.table_name
			add constraint fk_first_table_name_second_table_name foreign key (fk_column_name)
				references second_table_schema.table_name (pk_column_name)

	3.Default (df_)
		alter table schema_name.table_name
			add constraint df_column_name_table_name default (value) for column_name

	4.Check (ck_)
		alter table schema_name.table_name
			add constraint ck_column_name_table_name check (condition)

	5.Unique (uq_)
		alter table schema_name.table_name
			add constraint uq_column_name_table_name unique (column_name)
*/

/* ****** Foreign key constraints ****** */

-- 1)Between Animals table and Breeds table
ALTER TABLE Animals	
DROP CONSTRAINT IF EXISTS fk_Animals_Breeds
;
go

alter table Animals
	add constraint fk_Animals_Breeds FOREIGN KEY (BreedID)
		REFERENCES Breeds (BreedID)
;
go

/*
-- 2)Between Animals table and [Animal Services] table
ALTER TABLE Animals	
DROP CONSTRAINT IF EXISTS fk_Animals_Animal_Services
;
go

alter table Animals
	add constraint fk_Animals_Animal_Services FOREIGN KEY (ServiceID)
		REFERENCES [Animal Services] (ServiceID)
;
go
*/

-- 3)Between Animals table and Owners table
ALTER TABLE Animals	
DROP CONSTRAINT IF EXISTS fk_Animals_Owners
;
go

alter table Animals
	add constraint fk_Animals_Owners FOREIGN KEY (OwnerID)
		REFERENCES Owners (OwnerID)
;
go

-- 4)Between Animals table and Crates table
ALTER TABLE Animals	
DROP CONSTRAINT IF EXISTS fk_Animals_Crates
;
go

alter table Animals
	add constraint fk_Animals_Crates FOREIGN KEY (CrateID)
		REFERENCES Crates (CrateID)
;
go

-- 5a) Between [Animal Services] table and Animals table
ALTER TABLE [Animal Services]	
DROP CONSTRAINT IF EXISTS fk_Animal_Services_Animals
;
go

alter table [Animal Services]
	add constraint fk_Animal_Services_Animals FOREIGN KEY (AnimalID)
		REFERENCES Animals (AnimalID)
;
go

-- 5b)Between [Animal Services] table and Employees table
ALTER TABLE [Animal Services]	
DROP CONSTRAINT IF EXISTS fk_Animal_Services_Employees
;
go

alter table [Animal Services]
	add constraint fk_Animal_Services_Employees FOREIGN KEY (EmployeeID)
		REFERENCES Employees (EmployeeID)
;
go

/* ****** Default constraints ****** */

-- Owners table
ALTER TABLE Owners	
DROP CONSTRAINT IF EXISTS df_Country_Owners
;
go

ALTER TABLE Owners
	add constraint df_Country_Owners default ('Canada') for Country
;
go

-- Employees table
ALTER TABLE Employees	
DROP CONSTRAINT IF EXISTS df_Country_Employees
;
go

ALTER TABLE Employees
	add constraint df_Country_Employees default ('Canada') for Country
;
go

/* ****** Check constraints ****** */

-- 1)Check that employee StartDate < EndDate
ALTER TABLE Employees	
DROP CONSTRAINT IF EXISTS ck_StartDate_Employees
;
go

alter table Employees
	add constraint ck_StartDate_Employees check (StartDate <= EndDate)
;
go

-- 2)Check that Animal DateArrived < DateAdopted
ALTER TABLE Animals	
DROP CONSTRAINT IF EXISTS ck_dateArrived_Animals
;
go

alter table Animals
	add constraint ck_dateArrived_Animals check (DateArrived <= DateAdopted)
;
go

-- 3) Animal Services Date <= today's date
ALTER TABLE [Animal Services]	
DROP CONSTRAINT IF EXISTS ck_ServiceDate_Animal_Services
;
go

alter table [Animal Services]
	add constraint ck_ServiceDate_Animal_Services check (ServiceDate <= SYSDATETIME())
;
go

/* ??? Animal Services date are for given services ??? */

/* ****** Unique constraints ****** */

-- 1) Employees table
ALTER TABLE Employees	
DROP CONSTRAINT IF EXISTS uq_email_employees
;
go

alter table Employees
	add constraint uq_email_employees unique (Email)
;
go

ALTER TABLE Employees	
DROP CONSTRAINT IF EXISTS uq_sin_employees
;
go

alter table Employees
	add constraint uq_sin_employees unique (SIN)
;
go
/*
Purpose: Create table objects in PetStoreCrm database
Script date: 09-04-2021
Developed by: Emma - Elina - Raisa
*/

/* *** Table No.1 - Animals *** */
if OBJECT_ID('dbo.Animals', 'U') is not null
	drop table dbo.Animals
;
go

CREATE TABLE Animals (
	AnimalID int IDENTITY(1,1) not NULL, -- auto generate number assigned to new Animal
	Species int not NULL, -- ENUM
	Gender int not NULL, -- ENUM
	BreedID int not NULL, -- FK ---> Breeds Table
	Weight decimal(5,2) not NULL,
	Photo image NULL,
	DOB date NULL,
	Name nvarchar(50) NULL,
	DateArrived date not NULL, -- CHECK
	DateAdopted date NULL, 
	Microchipped bit not NULL, 
	Warmed bit not NULL,
	Neutured bit not NULL,
	OwnerID int NULL, -- Fk ---> Owners Table
	CrateID int NULL, -- Fk ---> Owners Tables
	Description nvarchar(200) NULL, -- user-defined data type?
	CONSTRAINT pk_Animals PRIMARY KEY CLUSTERED (AnimalID ASC)
	);
go

EXEC sp_rename 'Animals.Warmed', 'Wormed', 'Column'
;
go

/* *** Table No.2 - Breeds *** */
if OBJECT_ID('dbo.Breeds', 'U') is not null
	drop table dbo.Breeds
;
go

CREATE TABLE Breeds (
	BreedID int IDENTITY(1,1) not NULL, -- auto generate number assigned to new Breed
	BreedName nvarchar(50) not NULL,
	SpeciesCode int not NULL,
	Color int not NULL, -- ENUM
	Size int not NULL, -- ENUM
	FurType int not NULL, -- ENUM
	CONSTRAINT pk_Breeds PRIMARY KEY CLUSTERED (BreedID ASC)
	);
go

/* *** Table No.3 - Animal Services *** */
if OBJECT_ID('dbo.[Animal Services]', 'U') is not null
	drop table dbo.[Animal Services]
;
go

CREATE TABLE [Animal Services] (
	AnimalID int not NULL, -- C.PK ---> Animals table
	EmployeeID int not NULL, -- C.PK --> Employees table
	ServiceType int not NULL, -- ENUM
	ServiceDate datetime not NULL, 
	Description nvarchar(200) NULL,
	CONSTRAINT pk_Animal_Services PRIMARY KEY CLUSTERED (
		AnimalID ASC,
		EmployeeID ASC,
		ServiceDate ASC)
	); -- composite PK for lookup junction table
go

/* *** Table No.4 - Employees *** */
if OBJECT_ID('dbo.Employees', 'U') is not null
	drop table dbo.Employees
;
go

CREATE TABLE Employees (
	EmployeeID int IDENTITY(1,1) not NULL, -- auto generate number assigned to new Employee
	Name nvarchar(50) not null,
	MiddleName nvarchar(50) NULL,
	LastName nvarchar(50) not NULL,
	Position int not NULL, -- ENUM
	SIN nchar(11) not NULL, -- REGEX UNIQUE
	StartDate date not NULL, -- CHECK (should've preferably been on EndDate)
	EndDate date NULL, 
	PrimaryPhone nchar(13) not NULL, -- REGEX (515)111-11111
	SecondaryPhone nchar(13) NULL, -- REGEX (515)111-11111
	Email nvarchar(30) not NULL, -- UNIQUE ??? REGEX work email check end of string @ourshelter.com ???
	CrmPassword nvarchar(50) not NULL, -- UNIQUE  ??? REGEX ???
	Note nvarchar(200) NULL,
	Address nvarchar(50) not NULL, 
	PostalCode nchar(7) not NULL, -- REGEX ???? and/or API validation with CanadaPost API?
	City nvarchar(50) not NULL,
	Province int not NULL, -- ENUM ?
	Country nchar(6) not NULL, -- DEFAULT constraint
	CONSTRAINT pk_Employees PRIMARY KEY CLUSTERED (EmployeeID ASC) 
	);
go

/* *** Table No.4 - Owners *** */
if OBJECT_ID('dbo.Owners', 'U') is not null
	drop table dbo.Owners
;
go

CREATE TABLE Owners (
	OwnerID int IDENTITY(1,1) not NULL, -- auto generate number assigned to new Owner
	Name nvarchar(50) not NULL,
	MiddleName nvarchar(50) NULL,
	LastName nvarchar(50) not NULL,
	PrimaryPhone nchar(13) not NULL, -- REGEX (515)111-11111
	SecondaryPhone nchar(13) NULL, -- REGEX (515)111-11111
	Address nvarchar(50) not NULL, 
	PostalCode nchar(7) not NULL, -- REGEX ???? and/or API validation with CanadaPost API?
	City nvarchar(50) not NULL,
	Province int not NULL, -- ENUM
	Country nchar(6) not NULL, -- DEFAULT constraint
	Email nvarchar(30) NULL, -- UNIQUE
	Note nvarchar(200) NULL,
	Fostering bit not NULL,
	CONSTRAINT pk_Owners PRIMARY KEY CLUSTERED (OwnerID ASC)
	);
go

/* *** Table No.5 - Crates *** */
if OBJECT_ID('dbo.Crates', 'U') is not null
	drop table dbo.Crates
;
go

CREATE TABLE Crates (
	CrateID int IDENTITY(1,1) not NULL, -- auto generate number assigned to new Crate
	Status bit not NULL, 
	Size int not NULL, -- ENUM (x-small, small, medium, large, x-large)
	CONSTRAINT pk_Crates PRIMARY KEY CLUSTERED (CrateID ASC)
	);
go

-- RecommendedAnimal nvarchar(15) not NULL, -- Rodents, Cats, Dogs, Large Dogs


/*
Purpose: Create table objects in PetStoreCrm database
Script date: 09-04-2021
Developed by: Emma - Elina - Raisa
*/

bulk insert [Animal Services]
from 'C:\Users\eltia\Desktop\John Abbott\IPD-24\.net programming\group project\PetStoreCrmGroupProject\CSV\ServiceTableDummy.csv'
with
(
	FirstRow = 2,
	RowTerminator = '\n',
	FieldTerminator = ','
)
;
go

bulk insert Breeds
from 'C:\Users\eltia\Desktop\John Abbott\IPD-24\.net programming\group project\PetStoreCrmGroupProject\CSV\BreedTableDummy.csv'
with
(
	FirstRow = 2,
	RowTerminator = '\n',
	FieldTerminator = ','
)
;
go

bulk insert Crates
from 'C:\Users\eltia\Desktop\John Abbott\IPD-24\.net programming\group project\PetStoreCrmGroupProject\CSV\CrateTableDummy.csv'
with
(
	FirstRow = 2,
	RowTerminator = '\n',
	FieldTerminator = ','
)
;
go

bulk insert Owners
from 'C:\Users\eltia\Desktop\John Abbott\IPD-24\.net programming\group project\PetStoreCrmGroupProject\CSV\OwnerTableDummy.csv'
with
(
	FirstRow = 2,
	RowTerminator = '\n',
	FieldTerminator = ','
)
;
go

bulk insert Employees -- problem of insertin in row 4 col 11
from 'C:\Users\eltia\Desktop\John Abbott\IPD-24\.net programming\group project\PetStoreCrmGroupProject\CSV\EmployeeTableDummy.csv'
with
(
	FirstRow = 2,
	RowTerminator = '\n',
	FieldTerminator = ','
)
;
go


bulk insert Animals -- didn't work either
from 'C:\Users\eltia\Desktop\John Abbott\IPD-24\.net programming\group project\PetStoreCrmGroupProject\CSV\AnimalTableDummy.csv'
with
(
	FirstRow = 2,
	RowTerminator = '\n',
	FieldTerminator = ','
)
;
go
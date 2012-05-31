alter table "Bookmark" 
    add extravalue NUMBER(10,0) not null
	
	create table "TestSomething" (
    Id NUMBER(10,0) not null,
    TypeName NVARCHAR2(255),
    primary key (Id)
)
    
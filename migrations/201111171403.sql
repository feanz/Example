create table "Role" (
    Id NUMBER(10,0) not null,
    RoleName NVARCHAR2(255),
    primary key (Id)
)
create table UserRoles (
    Role_id NUMBER(10,0) not null,
    User_id NUMBER(10,0) not null
)
create table UserTable (
    Id NUMBER(10,0) not null,
    UserName NVARCHAR2(255),
    FirstName NVARCHAR2(255),
    LastName NVARCHAR2(255),
    Email NVARCHAR2(255),
    CreatedOn TIMESTAMP(4),
    LastLogin TIMESTAMP(4),
    primary key (Id)
)  
alter table UserRoles 
    add constraint FK2A0A9B1F580565D8 
    foreign key (User_id) 
    references UserTable
alter table UserRoles 
    add constraint FK2A0A9B1F56D55AA9 
    foreign key (Role_id) 
    references "Role"
create sequence hibernate_sequence;
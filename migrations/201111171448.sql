create table "Bookmark" (
    Id NUMBER(10,0) not null,
    Title NVARCHAR2(255),
    Url NVARCHAR2(255),
    Description NVARCHAR2(255),
    DateStarted TIMESTAMP(4),
    DateClosed TIMESTAMP(4),
    Tags NVARCHAR2(255),
    Type_id NUMBER(10,0),
    primary key (Id)
)
create table "BookmarkType" (
    Id NUMBER(10,0) not null,
    TypeName NVARCHAR2(255),
    primary key (Id)
)
alter table "Bookmark" 
    add constraint FK35AEC9255A0D4E4 
    foreign key (Type_id) 
    references "BookmarkType"




DECLARE @CompanyId INT=16,
@tableName  varchar(500),
 @columnName varchar(500),
 @Id INT

declare curTag cursor for SELECT Id,TableName,ColumnName
FROM dbo.BaseTemplateTags

open curTag

fetch next from curTag INTO @Id, @tableName,@columnName

WHILE   @@FETCH_STATUS = 0   
begin

IF NOT EXISTS(SELECT TOP(1) Id FROM dbo.TemplateTags WHERE LOWER(ColumnName)=LOWER(@columnName) AND CompanyId=@CompanyId) BEGIN

INSERT INTO dbo.TemplateTags
(
    ColumnName,
    TableName,
    Tag,
    Lable,
    TagType,
    IsDeleted,
    CompanyId,
    CreatedBy,
    CreatedAt,
    ModifiedBy,
    ModifiedAt
)
SELECT 
       ColumnName,
       TableName,
       Tag,
       Lable,
       TagType,
       IsDeleted,
       @CompanyId,
       CreatedBy,
       CreatedAt,
       ModifiedBy,
       ModifiedAt FROM dbo.BaseTemplateTags WHERE Id=@Id


END	

fetch next from curTag INTO @Id, @tableName,@columnName

end

close curTag

deallocate curTag
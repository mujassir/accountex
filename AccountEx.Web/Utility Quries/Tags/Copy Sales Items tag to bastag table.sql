


declare @tableName  varchar(500)
declare @columnName varchar(500)

declare curSaleTag cursor for SELECT TABLE_NAME,COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = N'vw_SaleItems'

open curSaleTag

fetch next from curSaleTag into @tableName,@columnName

WHILE   @@FETCH_STATUS = 0   
begin

IF NOT EXISTS(SELECT TOP(1) Id FROM dbo.BaseTemplateTags WHERE LOWER(ColumnName)=LOWER(@columnName)) BEGIN

INSERT INTO dbo.BaseTemplateTags
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
VALUES
(   @columnName,        -- ColumnName - varchar(500)
    @tableName,        -- TableName - varchar(500)
    '{{ SaleItem.'+@columnName+' }}',        -- Tag - varchar(500)
    @columnName,        -- Lable - varchar(500)
    2,         -- TagType - tinyint
    0,      -- IsDeleted - bit
    0,         -- CompanyId - int
    0,         -- CreatedBy - int
    GETDATE(), -- CreatedAt - datetime
    NULL,         -- ModifiedBy - int
   NULL  -- ModifiedAt - datetime
    )

END	

fetch next from curSaleTag into @tableName,@columnName

end

close curSaleTag

deallocate curSaleTag
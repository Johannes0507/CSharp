CREATE TABLE dbo.notes(
  id BIGINT IDENTITY(1,1),  -- `id` 是一個 BIGINT 類型的列，設置為自增欄位。起始值為 1，每次增加 1。
  [description] NVARCHAR(MAX)  -- `description` 是一個 NVARCHAR(MAX) 類型的列，可以存儲大量的 Unicode 文本資料。
)
select * from dbo.notes
insert into dbo.notes values ('Hello, World!!')
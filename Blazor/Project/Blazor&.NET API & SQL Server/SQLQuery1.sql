CREATE TABLE dbo.notes(
  id BIGINT IDENTITY(1,1),  -- `id` �O�@�� BIGINT �������C�A�]�m���ۼW���C�_�l�Ȭ� 1�A�C���W�[ 1�C
  [description] NVARCHAR(MAX)  -- `description` �O�@�� NVARCHAR(MAX) �������C�A�i�H�s�x�j�q�� Unicode �奻��ơC
)
select * from dbo.notes
insert into dbo.notes values ('Hello, World!!')
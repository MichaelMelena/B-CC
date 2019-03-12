CREATE TABLE [dbo].[bank]
(
	[id] INT NOT NULL PRIMARY KEY,
	[name] VARCHAR(50) NOT NULL,
	[short_Name] VARCHAR(20) NULL,
	[description] TEXT NULL,
	[updated] TIMESTAMP NULL,
	[created] TIMESTAMP NOT NULL,
)
GO

CREATE TRIGGER [dbo].[trigger_bank]
ON [dbo].[bank]
FOR  INSERT, UPDATE
AS
	DECLARE @id int

    if (select count(*) from inserted) <> 0 and (select count(*) from deleted) = 0 --insert
	begin
		SELECT @id = [id] FROM inserted;		
		UPDATE [bank] SET [created] = CURRENT_TIMESTAMP, [updated] = CURRENT_TIMESTAMP WHERE [id] = @id;
	end
	
	if (select count(*) from inserted) <> 0 and (select count(*) from deleted) <> 0 --update
	begin		
		SELECT @id = [id] FROM inserted;		
		UPDATE [bank] SET [updated]= CURRENT_TIMESTAMP WHERE [id] = @id;		
	end
GO

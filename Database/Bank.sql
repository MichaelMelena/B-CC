CREATE TABLE [dbo].[bank]
(
	[short_name] VARCHAR(10) NOT NULL PRIMARY KEY,
	[name]  VARCHAR(50) NOT NULL UNIQUE,
	[description] TEXT NULL,
	[updated] DATETIME NULL,
	[created] DATETIME NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE TRIGGER [dbo].[trigger_bank]
ON [dbo].[bank]
FOR  INSERT, UPDATE
AS
	DECLARE @short_name VARCHAR(10)

    if (select count(*) from inserted) <> 0 and (select count(*) from deleted) = 0 --insert
	begin
		SELECT @short_name = [short_name] FROM inserted;		
		UPDATE [bank] SET [created] = GETUTCDATE(), [updated] = GETUTCDATE() WHERE [short_name] = @short_name;
	end
	
	if (select count(*) from inserted) <> 0 and (select count(*) from deleted) <> 0 --update
	begin		
		SELECT @short_name = [short_name] FROM inserted;		
		UPDATE [bank] SET [updated]= GETUTCDATE() WHERE [short_name] = @short_name;		
	end
GO

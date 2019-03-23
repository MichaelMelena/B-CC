CREATE TABLE [dbo].[bank_connector]
(
	[id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[name] VARCHAR(50) NOT NULL UNIQUE,
	[bank_short_name] VARCHAR(10) NOT NULL,
	[dll_name] VARCHAR(50) NOT NULL,
	[enabled] BIT NOT NULL DEFAULT 1,
	[updated] DATETIME NULL,
	[created] DATETIME NOT NULL DEFAULT GETUTCDATE(),
	CONSTRAINT [fk_bank_connector_bank] FOREIGN KEY([bank_short_name])
		REFERENCES [bank]([short_name])
		ON DELETE CASCADE
		ON UPDATE CASCADE,
)
GO

CREATE TRIGGER [dbo].[trigger_bank_connector]
ON [dbo].[bank_connector]
FOR  INSERT, UPDATE
AS
	DECLARE @id int

    if (select count(*) from inserted) <> 0 and (select count(*) from deleted) = 0 --insert
	begin
		SELECT @id = [id] FROM inserted;		
		UPDATE [bank_connector] SET [created] = GETUTCDATE(), [updated] = GETUTCDATE() WHERE [id] = @id;
	end
	
	if (select count(*) from inserted) <> 0 and (select count(*) from deleted) <> 0 --update
	begin		
		SELECT @id = [id] FROM inserted;		
		UPDATE [bank_connector] SET [updated]= GETUTCDATE() WHERE [id] = @id;		
	end
GO

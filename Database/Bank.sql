CREATE TABLE [dbo].[bank]
(
	[id] INT NOT NULL IDENTITY(1,1) CONSTRAINT pk_bank PRIMARY KEY,
	[name] VARCHAR(50) NOT NULL,
	[short_Name] VARCHAR(20) NULL,
	[description] TEXT NULL,
	[updated] DATETIME NULL,
	[created] DATETIME NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE TRIGGER [dbo].[trigger_bank]
ON [dbo].[bank]
FOR  INSERT, UPDATE
AS
	DECLARE @id int

    if (select count(*) from inserted) <> 0 and (select count(*) from deleted) = 0 --insert
	begin
		SELECT @id = [id] FROM inserted;		
		UPDATE [bank] SET [created] = GETUTCDATE(), [updated] = GETUTCDATE() WHERE [id] = @id;
	end
	
	if (select count(*) from inserted) <> 0 and (select count(*) from deleted) <> 0 --update
	begin		
		SELECT @id = [id] FROM inserted;		
		UPDATE [bank] SET [updated]= GETUTCDATE() WHERE [id] = @id;		
	end
GO

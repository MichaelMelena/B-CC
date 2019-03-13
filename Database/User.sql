CREATE TABLE [dbo].[user]
(
	[id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [username] VARCHAR(20) NOT NULL,
	[email] VARCHAR(50) NOT NULL,
	[password] VARCHAR(64) NOT NULL,
	[updated] DATETIME NULL,
	[created] DATETIME NOT NULL DEFAULT GETUTCDATE(),
	[last_login] DATETIME NULL
)
GO

CREATE TRIGGER [dbo].[trigger_user]
ON [dbo].[user]
FOR  INSERT, UPDATE
AS
	DECLARE @id int

    if (select count(*) from inserted) <> 0 and (select count(*) from deleted) = 0 --insert
	begin
		SELECT @id = [id] FROM inserted;		
		UPDATE [user] SET [created] = GETUTCDATE(), [updated] = GETUTCDATE() WHERE [id]= @id;
	end
	
	if (select count(*) from inserted) <> 0 and (select count(*) from deleted) <> 0 --update
	begin		
		SELECT @id = [id] FROM inserted;		
		UPDATE [user] SET [updated] = GETUTCDATE() WHERE [id] = @id;		
	end
GO
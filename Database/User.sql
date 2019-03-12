CREATE TABLE [dbo].[user]
(
	[id] INT NOT NULL PRIMARY KEY, 
    [username] VARCHAR(20) NOT NULL,
	[email] VARCHAR(50) NOT NULL,
	[password] VARCHAR(256) NOT NULL,
	[updated] TIMESTAMP NULL,
	[created] TIMESTAMP NOT NULL
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
		UPDATE [user] SET [created] = CURRENT_TIMESTAMP, [updated] = CURRENT_TIMESTAMP WHERE [id]= @id;
	end
	
	if (select count(*) from inserted) <> 0 and (select count(*) from deleted) <> 0 --update
	begin		
		SELECT @id = [id] FROM inserted;		
		UPDATE [user] SET [updated] = CURRENT_TIMESTAMP WHERE [id] = @id;		
	end
GO
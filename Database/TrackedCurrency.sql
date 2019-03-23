CREATE TABLE [dbo].[tracked_currency]
(
	[user_id] INT NOT NULL,
	[iso_name] CHAR(3) NOT NULL,
	[created] DATETIME NOT NULL DEFAULT GETUTCDATE(),
	CONSTRAINT [pk_tracked_currency] PRIMARY KEY CLUSTERED(
		[user_id],[iso_name]
	),
	CONSTRAINT [fk_tracked_currency_user_id] FOREIGN KEY ([user_id])
		REFERENCES [user]([id])
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	CONSTRAINT [fk_tracked_currency_iso_name] FOREIGN KEY ([iso_name])
		REFERENCES [currency_metadata]([iso_name])
		ON DELETE CASCADE
		ON UPDATE CASCADE
)
GO 

CREATE TRIGGER [dbo].[trigger_tracked_currency]
ON [dbo].[tracked_currency]
FOR  INSERT
AS
	DECLARE @iso_name CHAR(3)
	DECLARE @user_id INT 
	BEGIN
		SELECT @iso_name = [iso_name], @user_id = [user_id] FROM inserted;	
		UPDATE [tracked_currency] SET [created] = GETUTCDATE() WHERE [iso_name] = @iso_name AND [user_id] = @user_id;
	END
GO
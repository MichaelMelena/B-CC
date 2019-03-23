CREATE TABLE [dbo].[currency_metadata]
(
	[iso_name] CHAR(3) NOT NULL CONSTRAINT pk_currency_metadata PRIMARY KEY,
	[country] VARCHAR(30) NULL,
	[name] VARCHAR(30) NOT NULL,
	[quantity] INT DEFAULT 1 NOT NULL,
	[description] Text NULL DEFAULT NULL,
	[updated] DATETIME NULL,
	[created] DATETIME NOT NULL DEFAULT GETUTCDATE(),
	CONSTRAINT [Check_positive_quantity] CHECK ([quantity] >= 1)
);
GO

CREATE TRIGGER [dbo].[trigger_currency_metadata]
ON [dbo].[currency_metadata]
FOR  INSERT, UPDATE
AS
	DECLARE @id CHAR(3)

    if (select count(*) from inserted) <> 0 and (select count(*) from deleted) = 0 --insert
	begin
		SELECT @id = [iso_name] FROM inserted;		
		UPDATE [currency_metadata] SET [created] = GETUTCDATE(), [updated] = GETUTCDATE() WHERE [iso_name] = @id;
	end
	
	if (select count(*) from inserted) <> 0 and (select count(*) from deleted) <> 0 --update
	begin		
		SELECT @id = [iso_name] FROM inserted;		
		UPDATE [currency_metadata] SET [updated] = GETUTCDATE() WHERE [iso_name] = @id;		
	end
GO
	
	
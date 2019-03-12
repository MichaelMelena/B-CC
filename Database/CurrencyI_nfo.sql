CREATE TABLE [dbo].[currency_info]
(
	[iso_name] CHAR(3) NOT NULL PRIMARY KEY,
	[country] VARCHAR(30) NULL,
	[name] VARCHAR(30) NOT NULL,
	[quantity] INT DEFAULT 1 NOT NULL,
	[description] Text NULL DEFAULT NULL,
	[updated] TIMESTAMP NULL,
	[created] TIMESTAMP NOT NULL,
);
GO

CREATE TRIGGER [dbo].[trigger_currency_info]
ON [dbo].[currency_info]
FOR  INSERT, UPDATE
AS
	DECLARE @id int

    if (select count(*) from inserted) <> 0 and (select count(*) from deleted) = 0 --insert
	begin
		SELECT @id = [iso_name] FROM inserted;		
		UPDATE [currency_info] SET [created] = CURRENT_TIMESTAMP, [updated] = CURRENT_TIMESTAMP WHERE [iso_name] = @id;
	end
	
	if (select count(*) from inserted) <> 0 and (select count(*) from deleted) <> 0 --update
	begin		
		SELECT @id = [iso_name] FROM inserted;		
		UPDATE [currency_info] SET [updated] = CURRENT_TIMESTAMP WHERE [iso_name] = @id;		
	end
GO
	
	
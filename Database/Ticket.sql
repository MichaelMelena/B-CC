CREATE TABLE [dbo].[ticket]
(
	[id] INT IDENTITY (1,1) CONSTRAINT pk_ticket PRIMARY KEY,
	[bank_id] INT NOT NULL,
	[date] DATETIME NOT NULL,
	[updated] DATETIME NULL,
	[created] DATETIME NOT NULL DEFAULT GETUTCDATE(),
	CONSTRAINT [unique_bank_and_date] UNIQUE NONCLUSTERED
    (
        [bank_id], [date]
    ),
	CONSTRAINT [fk_ticket_bank_id] FOREIGN KEY([bank_id])
		REFERENCES [bank]([id])
		ON DELETE CASCADE
		ON UPDATE CASCADE
)
GO

CREATE TRIGGER [dbo].[trigger_ticket]
ON [dbo].[ticket]
FOR  INSERT, UPDATE
AS
	DECLARE @id int

    if (select count(*) from inserted) <> 0 and (select count(*) from deleted) = 0 --insert
	begin
		SELECT @id = [id] FROM inserted;		
		UPDATE [ticket] SET [created] = GETUTCDATE(), [updated] = GETUTCDATE() WHERE [id]= @id;
	end
	
	if (select count(*) from inserted) <> 0 and (select count(*) from deleted) <> 0 --update
	begin		
		SELECT @id = [id] FROM inserted;		
		UPDATE [ticket] SET [updated] = GETUTCDATE() WHERE [id] = @id;		
	end
	GO

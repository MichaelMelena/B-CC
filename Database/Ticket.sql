CREATE TABLE [dbo].[ticket]
(
	[id] INT CONSTRAINT pk PRIMARY KEY,
	[bank_id] INT NOT NULL,
	[date] DATETIME NOT NULL,
	[updated] TIMESTAMP NULL,
	[created] TIMESTAMP,
	CONSTRAINT [unique_bank_and_date] UNIQUE NONCLUSTERED
    (
        [bank_id], [date]
    ),
	CONSTRAINT [fk_ticket_bank_id] FOREIGN KEY([bank_id])
		REFERENCES [bank]([id])
		ON DELETE SET NULL 
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
		UPDATE [ticket] SET [created] = CURRENT_TIMESTAMP, [updated] = CURRENT_TIMESTAMP WHERE [id]= @id;
	end
	
	if (select count(*) from inserted) <> 0 and (select count(*) from deleted) <> 0 --update
	begin		
		SELECT @id = [id] FROM inserted;		
		UPDATE [ticket] SET [updated] = CURRENT_TIMESTAMP WHERE [id] = @id;		
	end
	GO

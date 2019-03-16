CREATE TABLE [dbo].[currency]
(
	[iso_name] CHAR(3) NOT NULL,
	[ticket_id] INT NOT NULL ,
	[buy] REAL NOT NULL,
	[sell] REAL NULL,
	CONSTRAINT [pk_currency] PRIMARY KEY CLUSTERED(
		[iso_name],[ticket_id]
	),
	CONSTRAINT [fk_currency_to_iso_name] FOREIGN KEY([iso_name])
		REFERENCES [currency_metadata]([iso_name])
		ON DELETE CASCADE
		ON UPDATE CASCADE
)
GO

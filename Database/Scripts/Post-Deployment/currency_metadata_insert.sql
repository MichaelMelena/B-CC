-- Insert scripts for [currency_metadata] table

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'USD' )BEGIN
	INSERT [dbo].[currency_metadata]([iso_name], [name], [country], [quantity], [description] ) VALUES(
		'USD',
		'dolar',
		N'Spojené Státy Americké',
		1,
		NULL
	)
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'EUR' )BEGIN
	INSERT [dbo].[currency_metadata]([iso_name], [name], [country], [quantity], [description] ) VALUES(
		'EUR',
		'euro',
		N'EMU',
		1,
		NULL
	)
END
GO


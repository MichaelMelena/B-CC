-- Insert scripts for [bank] table

IF NOT EXISTS(SELECT * FROM [dbo].[bank_connector] WHERE [name] = N'Česká Národní banka') BEGIN
	INSERT [dbo].[bank_connector]([name], [bank_short_name], [dll_name], [enabled]) VALUES(
		N'CNB konektor',
		N'ČNB',
		'BCC.Core.CNB.CNBank',
		1
	)
END
GO
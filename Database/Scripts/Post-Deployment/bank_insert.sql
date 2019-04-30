-- Insert scripts for [bank] table

IF NOT EXISTS(SELECT * FROM [dbo].[bank] WHERE [name] = N'Česká Národní banka') BEGIN
	INSERT [dbo].[bank]([name], [short_Name], [description]) VALUES(
		N'Česká Národní banka',
		N'CNB',
		N'Centrální banka České republiky, která nastavuje kurz měny'
	)
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[bank] WHERE [name] = N'Československá obchodní banka') BEGIN
	INSERT [dbo].[bank]([name], [short_Name], [description]) VALUES(
		N'Československá obchodní banka',
		N'CSOB',
		null
	)
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[bank] WHERE [name] = N'Komerční banka') BEGIN
	INSERT [dbo].[bank]([name], [short_Name], [description]) VALUES(
		N'Komerční banka',
		N'KB',
		null
	)
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[bank] WHERE [name] = N'Raiffeisen Bank') BEGIN
	INSERT [dbo].[bank]([name], [short_Name], [description]) VALUES(
		N'Raiffeisen Bank',
		N'RB',
		null
	)
END
GO


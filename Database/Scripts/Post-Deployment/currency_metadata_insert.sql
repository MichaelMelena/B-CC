-- Insert scripts for [currency_metadata] table

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'AUD' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'AUD', NULL, N'Austrálie', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'BGN' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'BGN', NULL, N'Bulharsko', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'BRL' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'BRL', NULL, N'Brazílie', 1, NULL, CAST(N'2019-03-16T17:06:19.260' AS DateTime), CAST(N'2019-03-16T17:06:19.260' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'CAD' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'CAD', NULL, N'Kanada', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'CHF' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'CHF', NULL, N'Švýcarsko', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'CNY' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'CNY', NULL, N'Cína', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'DKK' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'DKK', NULL, N'Dánsko', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'EUR' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'EUR', NULL, N'EMU', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'GBP' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'GBP', NULL, N'Velká Británie', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'HKD' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'HKD', NULL, N'Hongkong', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'HRK' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'HRK', NULL, N'Chorvatsko', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'HUF' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'HUF', NULL, N'Madarsko', 100, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'IDR' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'IDR', NULL, N'Indonesie', 1000, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'ILS' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'ILS', NULL, N'Izrael', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'INR' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'INR', NULL, N'Indie', 100, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'ISK' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'ISK', NULL, N'Island', 100, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'JPY' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'JPY', NULL, N'Japonsko', 100, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'KRW' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'KRW', NULL, N'Korejská republika', 100, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'MXN' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'MXN', NULL, N'Mexiko', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'MYR' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'MYR', NULL, N'Malajsie', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'NOK' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'NOK', NULL, N'Norsko', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'NZD' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'NZD', NULL, N'Nový Zéland', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'PHP' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'PHP', NULL, N'Filipíny', 100, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'PLP' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'PLN', NULL, N'Polsko', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'RON' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'RON', NULL, N'Rumunsko', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'RUB' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'RUB', NULL, N'Rusko', 100, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'SEK' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'SEK', NULL, N'Švédsko', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'SGD' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'SGD', NULL, N'Singapur', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'THB' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'THB', NULL, N'Thajsko', 100, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'TRY' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'TRY', NULL, N'Turecko', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'USD' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'USD', NULL, N'USA', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'XDR' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'XDR', NULL, N'MMF', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[currency_metadata] WHERE [iso_name] = 'ZAR' )BEGIN
INSERT [dbo].[currency_metadata] ([iso_name], [country], [name], [quantity], [description], [updated], [created]) VALUES (N'ZAR', NULL, N'Jižní Afrika', 1, NULL, NULL, CAST(N'2019-03-16T17:06:19.257' AS DateTime))
END
GO
﻿-- Insert scripts for [bank] table

IF NOT EXISTS(SELECT * FROM [dbo].[bank_connector] WHERE [bank_short_name] = 'CNB') BEGIN
	INSERT [dbo].[bank_connector]([name], [bank_short_name], [dll_name], [enabled]) VALUES(
		N'CNB connector',
		N'CNB',
		'BCC.Core.CNB.CNBank',
		1
	)
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[bank_connector] WHERE [bank_short_name]  = 'KB') BEGIN
	INSERT [dbo].[bank_connector]([name], [bank_short_name], [dll_name], [enabled]) VALUES(
		N'KB connector',
		N'KB',
		'BCC.Core.KB.KBank',
		1
	)
END
GO

IF NOT EXISTS(SELECT * FROM [dbo].[bank_connector] WHERE [bank_short_name] = 'CSOB') BEGIN
	INSERT [dbo].[bank_connector]([name], [bank_short_name], [dll_name], [enabled]) VALUES(
		N'CSOB konektor',
		N'CSOB',
		'BCC.Core.CNB.CSOBank',
		1
	)
END
GO
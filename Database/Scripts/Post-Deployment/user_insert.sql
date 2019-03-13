-- Insert script for [user] table

IF NOT EXISTS(SELECT * FROM [dbo].[user] WHERE [username] = 'admin') BEGIN
	INSERT [dbo].[user]([username], [email], [password]) VALUES(
		'admin',
		'admin@exmaple.com',
		'8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918' -- SHA256 of: admin
	)
END
GO
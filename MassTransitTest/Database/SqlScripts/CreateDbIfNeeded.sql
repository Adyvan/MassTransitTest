IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'MassTest')
BEGIN
	CREATE DATABASE [MassTest]
END
SET QUOTED_IDENTIFIER, ANSI_NULLS ON
GO

--Stored procedure to add new Executable files to table
CREATE PROCEDURE [dbo].[usp_AddNewExecutableFile]
(
	@executableFileName NVARCHAR(100),
	@executableFileSize INT, 
	@executableFilePath NVARCHAR(100),
	@gameID INT
)
AS
BEGIN
INSERT INTO ExecutableFiles (executableFileName,executableFileSize,executableFilePath,gameID)
VALUES (@executableFileName,@executableFileSize,@executableFilePath,@gameID)
END
GO
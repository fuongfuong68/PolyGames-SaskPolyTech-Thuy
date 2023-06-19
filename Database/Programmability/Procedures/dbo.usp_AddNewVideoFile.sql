SET QUOTED_IDENTIFIER, ANSI_NULLS ON
GO

--Stored procedure to add new Video files to table
CREATE PROCEDURE [dbo].[usp_AddNewVideoFile]
(
	@videoFileName NVARCHAR(100),
	@videoFileSize INT, 
	@videoFilePath NVARCHAR(100),
	@gameID INT
)
AS
BEGIN
INSERT INTO VideoFiles (videoFileName,videoFileSize,videoFilePath,gameID)
VALUES (@videoFileName,@videoFileSize,@videoFilePath,@gameID)
END
GO
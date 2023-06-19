SET QUOTED_IDENTIFIER, ANSI_NULLS ON
GO

--Stored procedure to add new Pictures files to table
CREATE PROCEDURE [dbo].[usp_AddNewPictureFile]
(
	@pictureFileName NVARCHAR(100),
	@pictureFileSize INT, 
	@pictureFilePath NVARCHAR(100),
	@gameID INT
)
AS
BEGIN
INSERT INTO PictureFiles(pictureFileName,pictureFileSize,pictureFilePath,gameID)
VALUES (@pictureFileName,@pictureFileSize,@pictureFilePath,@gameID)
END;
GO
SET QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetAllGamesByYear]
	@GameYear int = 2021
AS
BEGIN
SELECT Games.GameId as GameId,Games.GameName as GameName,Games.GameDescription as GameDescription,
PictureFiles.pictureFilePath as pictureFilePath
FROM Games INNER JOIN
     (select PictureFiles.*, row_number() over (partition by GameId order by GameId desc) as seqnum
      from PictureFiles
     ) PictureFiles
    ON Games.GameId = PictureFiles.GameId and seqnum = 1
WHERE Games.Year = @GameYear ORDER BY Games.GameName;
END
GO
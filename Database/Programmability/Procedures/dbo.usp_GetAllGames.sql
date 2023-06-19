SET QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetAllGames]
AS
BEGIN
	SELECT Games.GameId as GameId,Games.GameName as GameName,Games.GameDescription as GameDescription,
PictureFiles.pictureFilePath as pictureFilePath
FROM Games INNER JOIN
     (select PictureFiles.*--, row_number() over (partition by GameId order by GameId desc) as seqnum
      from PictureFiles
     ) PictureFiles
    ON Games.GameId = PictureFiles.GameId --and seqnum = 1
ORDER BY Games.GameName;
END
GO
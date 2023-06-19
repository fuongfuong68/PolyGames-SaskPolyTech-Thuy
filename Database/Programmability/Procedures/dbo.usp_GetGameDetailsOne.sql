SET QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetGameDetailsOne]
	@GameId int = 1
AS
BEGIN
SELECT Games.GameId,Games.GameName,Games.GameDescription,Games.GroupId,Games.Year, Games.HtmlVersionLink, PolyGamesGroups.GroupName, 
VideoFiles.videoId, VideoFiles.videoFilePath, ExecutableFiles.executableId, ExecutableFiles.executableFilePath
  FROM Games INNER JOIN PolyGamesGroups ON Games.GroupId = PolyGamesGroups.GroupId INNER JOIN VideoFiles ON
  Games.GameId = VideoFiles.gameID INNER JOIN ExecutableFiles ON Games.GameId = ExecutableFiles.gameID
  WHERE Games.GameId = @GameId;
END
GO
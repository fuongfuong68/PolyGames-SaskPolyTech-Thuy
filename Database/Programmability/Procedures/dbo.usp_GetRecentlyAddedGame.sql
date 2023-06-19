SET QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetRecentlyAddedGame]
AS
BEGIN
	SELECT Games.GameId,Games.GameName,Games.GameDescription,Games.Year,VideoFiles.videoFilePath
	FROM Games INNER JOIN VideoFiles ON VideoFiles.GameId=Games.GameId WHERE Games.GameId= 
	(SELECT MAX(Games.GameId) FROM Games);
END
GO
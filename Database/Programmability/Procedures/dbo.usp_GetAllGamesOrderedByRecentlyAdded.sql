SET QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetAllGamesOrderedByRecentlyAdded]
AS
BEGIN
	SELECT Games.GameId as GameId,Games.GameName as GameName,Games.GameDescription as GameDescription
	FROM Games
ORDER BY Games.GameId desc;
END

GO
SET QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_DeleteGame]
	@GameId int, @GroupId int
AS
BEGIN
	delete from ExecutableFiles where gameID = @GameId;

	delete from PictureFiles where gameID = @GameId;

	delete from VideoFiles where gameID = @GameId;

	delete from games where GameId = @GameId;
END
GO
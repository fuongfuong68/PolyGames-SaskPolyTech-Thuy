SET QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[usp_GetGroupNameByMemberId]
	@MemberId int
AS
	
	SELECT PolyGamesGroups.GroupId, GroupName FROM PolyGamesGroups 
	INNER JOIN MemberToGroup
	ON PolyGamesGroups.GroupId = MemberToGroup.GroupId
	WHERE MemberToGroup.MemberId = @MemberId;
GO
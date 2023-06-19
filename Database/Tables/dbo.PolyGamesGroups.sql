CREATE TABLE [dbo].[PolyGamesGroups] (
  [GroupId] [int] IDENTITY,
  [GroupName] [varchar](225) NULL,
  CONSTRAINT [PK_PolyGamesGroups] PRIMARY KEY CLUSTERED ([GroupId])
)
ON [PRIMARY]
GO
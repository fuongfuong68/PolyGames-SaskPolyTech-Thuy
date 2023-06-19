CREATE TABLE [dbo].[Games] (
  [GameId] [int] IDENTITY,
  [Year] [int] NULL,
  [GameName] [varchar](255) NULL,
  [GameDescription] [varchar](255) NULL,
  [GroupId] [int] NULL CONSTRAINT [nextGroupId] DEFAULT (NEXT VALUE FOR [seq02_Games]),
  [HtmlVersionLink] [varchar](255) NULL,
  PRIMARY KEY CLUSTERED ([GameId])
)
ON [PRIMARY]
GO

ALTER TABLE [dbo].[Games]
  ADD CONSTRAINT [FK_Games_PolyGamesGroups] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[PolyGamesGroups] ([GroupId])
GO
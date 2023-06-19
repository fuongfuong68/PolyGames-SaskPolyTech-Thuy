CREATE TABLE [dbo].[MemberToGroup] (
  [MemberId] [int] NOT NULL,
  [GroupId] [int] NOT NULL,
  [StudentRole] [nchar](100) NULL,
  [IsHidden] [bit] NULL,
  CONSTRAINT [PK_MemberToGroup] PRIMARY KEY CLUSTERED ([MemberId], [GroupId])
)
ON [PRIMARY]
GO

ALTER TABLE [dbo].[MemberToGroup]
  ADD CONSTRAINT [FK_MemberToGroup_PolyGamesGroups] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[PolyGamesGroups] ([GroupId])
GO

ALTER TABLE [dbo].[MemberToGroup]
  ADD CONSTRAINT [FK_MemberToGroup_UserLogin] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[UserLogin] ([MemberId])
GO
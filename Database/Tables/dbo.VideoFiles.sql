CREATE TABLE [dbo].[VideoFiles] (
  [videoId] [int] IDENTITY,
  [videoFileName] [nvarchar](100) NULL,
  [videoFileSize] [int] NULL,
  [videoFilePath] [nvarchar](100) NULL,
  [gameID] [int] NOT NULL,
  CONSTRAINT [PK_VideoFiles] PRIMARY KEY CLUSTERED ([videoId])
)
ON [PRIMARY]
GO

ALTER TABLE [dbo].[VideoFiles]
  ADD CONSTRAINT [FK_VideoFiles_Games] FOREIGN KEY ([gameID]) REFERENCES [dbo].[Games] ([GameId])
GO
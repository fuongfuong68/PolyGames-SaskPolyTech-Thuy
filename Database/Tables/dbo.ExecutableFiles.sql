CREATE TABLE [dbo].[ExecutableFiles] (
  [executableId] [int] IDENTITY,
  [executableFileName] [nvarchar](100) NULL,
  [executableFileSize] [int] NULL,
  [executableFilePath] [nvarchar](100) NULL,
  [gameID] [int] NOT NULL,
  CONSTRAINT [PK_ExecutableFiles] PRIMARY KEY CLUSTERED ([executableId])
)
ON [PRIMARY]
GO

ALTER TABLE [dbo].[ExecutableFiles]
  ADD CONSTRAINT [FK_ExecutableFiles_Games] FOREIGN KEY ([gameID]) REFERENCES [dbo].[Games] ([GameId])
GO
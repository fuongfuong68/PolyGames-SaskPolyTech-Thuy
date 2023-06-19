CREATE TABLE [dbo].[PictureFiles] (
  [pictureID] [int] IDENTITY,
  [pictureFileName] [nvarchar](100) NULL,
  [pictureFileSize] [int] NULL,
  [pictureFilePath] [nvarchar](100) NULL,
  [gameID] [int] NOT NULL,
  CONSTRAINT [PK_PictureFiles] PRIMARY KEY CLUSTERED ([pictureID])
)
ON [PRIMARY]
GO

ALTER TABLE [dbo].[PictureFiles]
  ADD CONSTRAINT [FK_PictureFiles_Games] FOREIGN KEY ([gameID]) REFERENCES [dbo].[Games] ([GameId])
GO
CREATE TABLE [dbo].[AccessCodes] (
  [Code] [varchar](100) NOT NULL,
  [ExpirationDate] [datetime] NOT NULL,
  PRIMARY KEY CLUSTERED ([Code])
)
ON [PRIMARY]
GO
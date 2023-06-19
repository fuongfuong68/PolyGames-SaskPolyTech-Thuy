CREATE TABLE [dbo].[UserLogin] (
  [MemberId] [int] IDENTITY,
  [Email] [varchar](50) NOT NULL,
  [Password] [varchar](200) NOT NULL,
  [IsActive] [bit] NOT NULL,
  [IsAdmin] [bit] NOT NULL,
  [Name] [nchar](40) NOT NULL,
  [RegistrationDate] [datetime] NOT NULL,
  [PasswordResetRequest] [bit] NOT NULL,
  CONSTRAINT [PK__UserLogi__A9D105351C5FC7B4] PRIMARY KEY CLUSTERED ([MemberId])
)
ON [PRIMARY]
GO
CREATE TABLE [dbo].[Policyholders]
(
	[Id]			BIGINT IDENTITY(1,1)	NOT NULL,
	[FirstName]		VARCHAR(100)			NOT NULL,
	[LastName]		VARCHAR(100)			NOT NULL,
	[DateOfBirth]	DATETIME2				NOT NULL,
	[CreatedDate]	DATETIME2				NOT NULL,
	[PolicyId]		UNIQUEIDENTIFIER		NOT NULL,
	CONSTRAINT [PK_dbo.Policyholders] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.Policyholders_dbo.Policies_PolicyId] FOREIGN KEY ([PolicyId]) REFERENCES [dbo].[Policies]([Id]) ON DELETE CASCADE
);

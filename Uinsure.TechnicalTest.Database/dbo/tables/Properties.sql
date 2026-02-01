CREATE TABLE [dbo].[Properties]
(
	[Id]			BIGINT IDENTITY(1,1)	NOT NULL,
	[AddressLine1]	VARCHAR(MAX)			NOT NULL,
	[AddressLine2]	VARCHAR(MAX)			NOT NULL,
	[AddressLine3]	VARCHAR(MAX)			NOT NULL,
	[Postcode]		VARCHAR(8)				NOT NULL,
	[CreatedDate]	DATETIMEOFFSET(7)		NOT NULL,
	[PolicyId]		UNIQUEIDENTIFIER		NOT NULL,
	CONSTRAINT [PK_dbo.Properties] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.Properties_dbo.Policies_PolicyId] FOREIGN KEY ([PolicyId]) REFERENCES [dbo].[Policies]([Id]) ON DELETE CASCADE
);

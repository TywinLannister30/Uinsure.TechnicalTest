CREATE TABLE [dbo].[Payments]
(
	[Id]				BIGINT IDENTITY(1,1)	NOT NULL,
	[PaymentReference]	VARCHAR(50)				NOT NULL,
	[Type]				INT						NOT NULL,
	[Amount]			DECIMAL(18,2)			NOT NULL,
	[TransactionType]	INT						NOT NULL,
	[CreatedDate]		DATETIMEOFFSET(7)		NOT NULL,
	[PolicyId]			UNIQUEIDENTIFIER		NOT NULL,
	CONSTRAINT [PK_dbo.Payments] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.Payments_dbo.Policies_PolicyId] FOREIGN KEY ([PolicyId]) REFERENCES [dbo].[Policies]([Id]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[Policies]
(
	[Id]			UNIQUEIDENTIFIER	NOT NULL PRIMARY KEY,
	[InsuranceType] INT					NOT NULL,
	[StartDate]		DATETIME2			NOT NULL,
	[EndDate]		DATETIME2			NOT NULL,
	[Amount]		DECIMAL(18,2)		NOT NULL,
	[HasClaims]		BIT					NOT NULL,
	[AutoRenew]		BIT					NOT NULL,
	[CreatedDate]	DATETIME2			NOT NULL
);

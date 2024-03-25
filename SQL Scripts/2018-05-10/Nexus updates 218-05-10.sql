
SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL Serializable
GO
BEGIN TRANSACTION
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[Nexus_Vw_Cases]'
GO






CREATE VIEW [dbo].[Nexus_Vw_Cases]
AS
    SELECT  
			NC.ID,
            NC.CaseNumber,
            NC.PatientID,
			NP.FirstName+' '+ISNULL(NP.LastName,'') AS PatientName,
            NC.RegistrationDate,
            NC.ReportingDate,
            NC.ReferenceID,
            NC.ReferenceName,
            NC.ConsultantID,
            NC.ConsultantName,
            NC.RegistrationLocation,
            NC.DestinationLocation,
            NC.TotalAmount,
            NC.Discount,
            NC.Less,
            NC.NetAmount,
            NC.PaidAmount,
            NC.Due,
            NC.Completed,
            NC.Comments,
            NC.CreatedDate,
            NC.ModifiedBy,
            NC.ModifiedDate,
            NC.CreatedBy,
            NC.Status,
            NC.BankDueReceived,
            NC.BankPaid,
            NC.DueReceived,
            NC.AlertSent
           
    FROM    dbo.Nexus_Case AS NC
			LEFT JOIN dbo.Nexus_Patient NP
			ON NC.PatientID=NP.ID







GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[Nexus_PostedCasesItems]'
GO
CREATE TABLE [dbo].[Nexus_PostedCasesItems]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[PostedCaseId] [int] NOT NULL,
[TestId] [int] NOT NULL,
[TestName] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Price] [decimal] (18, 4) NOT NULL CONSTRAINT [DF_Nexus_PostedCasesItems_Price] DEFAULT ((0)),
[IsDeleted] [bit] NOT NULL,
[CompanyId] [int] NULL,
[OldId] [int] NULL,
[CreatedBy] [int] NOT NULL CONSTRAINT [DF_Nexus_PostedCasesItems_CreatedBy] DEFAULT ((0)),
[CreatedAt] [datetime] NOT NULL CONSTRAINT [DF_Nexus_PostedCasesItems_CreatedAt] DEFAULT (getdate()),
[ModifiedBy] [int] NULL,
[ModifiedAt] [datetime] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_Nexus_PostedCasesItems] on [dbo].[Nexus_PostedCasesItems]'
GO
ALTER TABLE [dbo].[Nexus_PostedCasesItems] ADD CONSTRAINT [PK_Nexus_PostedCasesItems] PRIMARY KEY CLUSTERED  ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Altering [dbo].[DepartmentRateListItems]'
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
ALTER TABLE [dbo].[DepartmentRateListItems] ADD
[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_DepartmentRateListItems_IsDeleted] DEFAULT ((0))
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Altering [dbo].[DepartmentRateLists]'
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
ALTER TABLE [dbo].[DepartmentRateLists] ADD
[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_DepartmentRateLists_IsDeleted] DEFAULT ((0))
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Altering [dbo].[Tests]'
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
ALTER TABLE [dbo].[Tests] ADD
[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_Tests_IsDeleted] DEFAULT ((0))
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[Nexus_PostedCases]'
GO
CREATE TABLE [dbo].[Nexus_PostedCases]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[VoucherNumber] [int] NOT NULL,
[Date] [date] NOT NULL CONSTRAINT [DF__Nexus_Post__Date__5DD4DB0C] DEFAULT (getdate()),
[CaseId] [bigint] NOT NULL,
[EmployeeId] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Relationship] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DepartmentId] [int] NULL,
[TotalAmount] [decimal] (18, 4) NOT NULL CONSTRAINT [DF_Nexus_PostedCases_TotalAmount] DEFAULT ((0)),
[Discount] [decimal] (18, 4) NOT NULL CONSTRAINT [DF_Nexus_PostedCases_TotalAmount4] DEFAULT ((0)),
[Less] [decimal] (18, 4) NOT NULL CONSTRAINT [DF_Nexus_PostedCases_TotalAmount3] DEFAULT ((0)),
[NetAmount] [decimal] (18, 4) NOT NULL CONSTRAINT [DF_Nexus_PostedCases_TotalAmount2] DEFAULT ((0)),
[PaidAmount] [decimal] (18, 4) NOT NULL CONSTRAINT [DF_Nexus_PostedCases_TotalAmount1] DEFAULT ((0)),
[Due] [decimal] (18, 4) NOT NULL CONSTRAINT [DF_Nexus_PostedCases_PaidAmount1] DEFAULT ((0)),
[FiscalId] [int] NULL,
[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_Nexus_PostedCases_IsDeleted] DEFAULT ((0)),
[CompanyId] [int] NULL,
[OldId] [int] NULL,
[CreatedBy] [int] NOT NULL CONSTRAINT [DF_Nexus_PostedCases_CreatedBy] DEFAULT ((0)),
[CreatedAt] [datetime] NOT NULL CONSTRAINT [DF_Nexus_PostedCases_CreatedAt] DEFAULT (getdate()),
[ModifiedBy] [int] NULL,
[ModifiedAt] [datetime] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_Nexus_PostedCases] on [dbo].[Nexus_PostedCases]'
GO
ALTER TABLE [dbo].[Nexus_PostedCases] ADD CONSTRAINT [PK_Nexus_PostedCases] PRIMARY KEY CLUSTERED  ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding constraints to [dbo].[DepartmentRateListItems]'
GO
ALTER TABLE [dbo].[DepartmentRateListItems] ADD CONSTRAINT [DF_DepartmentRateListItems_CompanyId] DEFAULT ((0)) FOR [CompanyId]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding constraints to [dbo].[DepartmentRateLists]'
GO
ALTER TABLE [dbo].[DepartmentRateLists] ADD CONSTRAINT [DF_DepartmentRateLists_CompanyId] DEFAULT ((0)) FOR [CompanyId]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding constraints to [dbo].[Tests]'
GO
ALTER TABLE [dbo].[Tests] ADD CONSTRAINT [DF_Tests_CompanyId] DEFAULT ((0)) FOR [CompanyId]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding foreign keys to [dbo].[Nexus_PostedCasesItems]'
GO
ALTER TABLE [dbo].[Nexus_PostedCasesItems] ADD CONSTRAINT [FK_Nexus_PostedCasesItems_Nexus_PostedCases] FOREIGN KEY ([PostedCaseId]) REFERENCES [dbo].[Nexus_PostedCases] ([Id]) ON DELETE CASCADE
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding foreign keys to [dbo].[Nexus_PostedCases]'
GO
ALTER TABLE [dbo].[Nexus_PostedCases] ADD CONSTRAINT [FK_Nexus_PostedCases_Nexus_PostedCases] FOREIGN KEY ([Id]) REFERENCES [dbo].[Nexus_PostedCases] ([Id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
COMMIT TRANSACTION
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
-- This statement writes to the SQL Server Log so SQL Monitor can show this deployment.
IF HAS_PERMS_BY_NAME(N'sys.xp_logevent', N'OBJECT', N'EXECUTE') = 1
BEGIN
    DECLARE @databaseName AS nvarchar(2048), @eventMessage AS nvarchar(2048)
    SET @databaseName = REPLACE(REPLACE(DB_NAME(), N'\', N'\\'), N'"', N'\"')
    SET @eventMessage = N'Redgate SQL Compare: { "deployment": { "description": "Redgate SQL Compare deployed to ' + @databaseName + N'", "database": "' + @databaseName + N'" }}'
    EXECUTE sys.xp_logevent 55000, @eventMessage
END
GO
DECLARE @Success AS BIT
SET @Success = 1
SET NOEXEC OFF
IF (@Success = 1) PRINT 'The database update succeeded'
ELSE BEGIN
	IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
	PRINT 'The database update failed'
END
GO


/****** Object:  Table [dbo].[Nexus_Case]    Script Date: 06/05/2018 1:16:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Nexus_Case](
	[ID] [bigint] NOT NULL,
	[CaseNumber] [varchar](25) NOT NULL,
	[PatientID] [bigint] NOT NULL,
	[RegistrationDate] [smalldatetime] NOT NULL,
	[ReportingDate] [smalldatetime] NOT NULL,
	[ReferenceID] [int] NULL,
	[ReferenceName] [varchar](100) NOT NULL,
	[ConsultantID] [int] NULL,
	[ConsultantName] [varchar](64) NOT NULL,
	[RegistrationLocation] [int] NOT NULL,
	[DestinationLocation] [int] NOT NULL,
	[TotalAmount] [money] NOT NULL,
	[Discount] [tinyint] NOT NULL,
	[Less] [money] NOT NULL,
	[NetAmount] [money] NOT NULL,
	[PaidAmount] [money] NULL,
	[Due] [money] NOT NULL,
	[Completed] [bit] NOT NULL,
	[Comments] [varchar](500) NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[ModifiedBy] [varchar](30) NOT NULL,
	[ModifiedDate] [smalldatetime] NOT NULL,
	[CreatedBy] [varchar](30) NOT NULL,
	[Status] [bit] NOT NULL,
	[BankDueReceived] [money] NULL,
	[BankPaid] [money] NULL,
	[DueReceived] [money] NULL,
	[AlertSent] [bit] NOT NULL,
 CONSTRAINT [PK_Case] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Nexus_CaseDetail]    Script Date: 06/05/2018 1:16:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Nexus_CaseDetail](
	[ID] [bigint] NOT NULL,
	[CaseID] [bigint] NOT NULL,
	[TestID] [int] NOT NULL,
	[TestName] [varchar](200) NOT NULL,
	[Rate] [decimal](15, 2) NOT NULL CONSTRAINT [DF__tmp_CaseDe__Rate__569F9A3F]  DEFAULT ((0)),
	[TestStatus] [smallint] NOT NULL,
	[ConductedAt] [int] NOT NULL,
	[ReportingDate] [smalldatetime] NOT NULL,
	[CreatedBy] [varchar](30) NOT NULL CONSTRAINT [DF__tmp_CaseD__Creat__5793BE78]  DEFAULT ('Admin'),
	[CreatedDate] [smalldatetime] NOT NULL CONSTRAINT [DF__tmp_CaseD__Creat__5887E2B1]  DEFAULT (getdate()),
	[ModifiedBy] [varchar](30) NOT NULL CONSTRAINT [DF__tmp_CaseD__Modif__597C06EA]  DEFAULT ('Admin'),
	[ModifiedDate] [smalldatetime] NOT NULL CONSTRAINT [DF__tmp_CaseD__Modif__5A702B23]  DEFAULT (getdate()),
	[Status] [bit] NOT NULL CONSTRAINT [DF__tmp_CaseD__Statu__5B644F5C]  DEFAULT ((0)),
	[TemplateID] [smallint] NOT NULL CONSTRAINT [DF__tmp_CaseD__Templ__5C587395]  DEFAULT ((1)),
	[Comments] [varchar](1000) NULL,
	[IsDelayed] [bit] NOT NULL CONSTRAINT [DF_CaseDetail_IsDelayed]  DEFAULT ((0)),
	[ConductedBy] [varchar](50) NULL,
	[ApprovedBy] [varchar](50) NULL,
 CONSTRAINT [PK_CaseDetail] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Nexus_City]    Script Date: 06/05/2018 1:16:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Nexus_City](
	[CityCode] [int] NOT NULL,
	[CityName] [varchar](100) NOT NULL,
	[CountryCode] [int] NOT NULL,
	[CreatedBy] [varchar](30) NOT NULL CONSTRAINT [DF_City_CreatedBy]  DEFAULT ('Admin'),
	[CreatedDate] [smalldatetime] NOT NULL CONSTRAINT [DF_City_CreatedDate]  DEFAULT (getdate()),
	[ModifiedBy] [varchar](30) NOT NULL CONSTRAINT [DF_City_ModifiedBy]  DEFAULT ('Admin'),
	[ModifiedDate] [smalldatetime] NOT NULL CONSTRAINT [DF_City_ModifiedDate]  DEFAULT (getdate()),
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED 
(
	[CityCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Nexus_Consultant]    Script Date: 06/05/2018 1:16:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Nexus_Consultant](
	[ID] [int] NOT NULL,
	[Code] [varchar](5) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Company] [varchar](100) NOT NULL,
	[Address] [varchar](200) NULL,
	[City] [int] NULL CONSTRAINT [DF_Consultant_City]  DEFAULT ((1)),
	[Country] [int] NULL CONSTRAINT [DF_Consultant_Country]  DEFAULT ((164)),
	[Mobile] [varchar](64) NULL,
	[Phone] [varchar](64) NULL,
	[Fax] [varchar](64) NULL,
	[Email] [varchar](100) NULL,
	[Description] [varchar](500) NULL,
	[CreatedBy] [varchar](30) NOT NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[ModifiedBy] [varchar](30) NOT NULL,
	[ModifiedDate] [smalldatetime] NOT NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_Consultant_Status]  DEFAULT ((0)),
 CONSTRAINT [PK_Consultant] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [unique_Code] UNIQUE NONCLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Nexus_Country]    Script Date: 06/05/2018 1:16:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Nexus_Country](
	[CountryCode] [int] NOT NULL,
	[CountryName] [varchar](100) NOT NULL,
	[CreatedBy] [varchar](30) NOT NULL,
	[CreatedDate] [smalldatetime] NOT NULL CONSTRAINT [DF_Country_CreatedDate]  DEFAULT (getdate()),
	[ModifiedBy] [varchar](30) NOT NULL,
	[ModifiedDate] [smalldatetime] NOT NULL CONSTRAINT [DF_Country_ModifiedDate]  DEFAULT (getdate()),
	[Status] [bit] NOT NULL CONSTRAINT [DF_Country_Status]  DEFAULT ((0)),
 CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED 
(
	[CountryCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Nexus_Patient]    Script Date: 06/05/2018 1:16:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Nexus_Patient](
	[ID] [bigint] NOT NULL,
	[FirstName] [varchar](64) NOT NULL,
	[LastName] [varchar](64) NOT NULL,
	[FHName] [varchar](50) NOT NULL CONSTRAINT [DF_Patient_FHName]  DEFAULT ('Father/Husband Name'),
	[Sex] [tinyint] NOT NULL,
	[DateOfBirth] [datetime] NOT NULL,
	[MaritalStatus] [tinyint] NOT NULL,
	[BloodGroup] [varchar](10) NOT NULL,
	[NIC] [varchar](16) NULL,
	[Phone] [varchar](64) NULL,
	[Mobile] [varchar](64) NULL,
	[Fax] [varchar](64) NULL,
	[Email] [varchar](100) NULL,
	[Address] [varchar](200) NULL,
	[City] [varchar](100) NOT NULL,
	[Country] [varchar](100) NOT NULL,
	[DateRegistered] [smalldatetime] NOT NULL CONSTRAINT [DF_Patient_DateRegistered]  DEFAULT (getdate()),
	[PatientNumber] [varchar](25) NOT NULL CONSTRAINT [DF_Patient_PatientNumber]  DEFAULT ('(See Table Description)'),
	[CreatedBy] [varchar](30) NOT NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[ModifiedBy] [varchar](30) NOT NULL,
	[ModifiedDate] [smalldatetime] NOT NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_Patient_Status]  DEFAULT ((0)),
	[Location] [int] NOT NULL CONSTRAINT [DF_Patient_Location]  DEFAULT ((1)),
	[MedicalRecordNo] [varchar](50) NULL,
	[CABGNo] [varchar](50) NULL,
 CONSTRAINT [PK_Patient] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Nexus_RateType]    Script Date: 06/05/2018 1:16:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Nexus_RateType](
	[ID] [smallint] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Description] [varchar](1000) NULL,
	[Createdby] [varchar](30) NOT NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[ModifiedBy] [varchar](30) NOT NULL,
	[ModifiedDate] [smalldatetime] NOT NULL,
	[Status] [bit] NOT NULL,
	[IsFixPrice] [bit] NOT NULL,
	[Special] [decimal](15, 4) NOT NULL,
	[Routine] [decimal](15, 4) NOT NULL,
	[Biopsy] [decimal](15, 4) NOT NULL,
	[PCR] [decimal](15, 4) NOT NULL,
	[Rebate] [decimal](15, 5) NOT NULL,
	[Radiology] [decimal](15, 4) NOT NULL,
	[Other] [decimal](15, 4) NOT NULL,
	[Outside] [decimal](15, 4) NOT NULL,
 CONSTRAINT [PK_RateType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Nexus_Reference]    Script Date: 06/05/2018 1:16:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Nexus_Reference](
	[ID] [int] NOT NULL,
	[Code] [varchar](5) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Address] [varchar](200) NULL,
	[City] [int] NULL,
	[Country] [int] NULL,
	[Phone] [varchar](64) NULL,
	[Fax] [varchar](64) NULL,
	[Email] [varchar](100) NULL,
	[RateTypeID] [smallint] NOT NULL,
	[PaymentMode] [tinyint] NOT NULL,
	[CreditLimit] [decimal](15, 5) NOT NULL,
	[CreditDays] [smallint] NOT NULL,
	[CurrentBalance] [decimal](15, 5) NOT NULL,
	[DefaultDiscount] [decimal](5, 2) NOT NULL,
	[MaxDiscount] [decimal](5, 2) NOT NULL,
	[Description] [varchar](500) NULL,
	[ContactPerson] [varchar](100) NULL,
	[ContactPhone] [varchar](64) NULL,
	[ContactMobile] [varchar](64) NULL,
	[ContactEmail] [varchar](100) NULL,
	[CreatedBy] [varchar](30) NOT NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[ModifiedBy] [varchar](30) NOT NULL,
	[ModifiedDate] [smalldatetime] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_Reference] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Nexus_Test]    Script Date: 06/05/2018 1:16:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Nexus_Test](
	[ID] [int] NOT NULL,
	[Code] [varchar](10) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[ReportName] [varchar](200) NOT NULL,
	[TestHeading] [varchar](150) NULL,
	[ReportGroup] [varchar](150) NULL,
	[Synonyms] [varchar](500) NULL,
	[GroupID] [smallint] NOT NULL,
	[Type] [tinyint] NOT NULL,
	[IsSpecial] [bit] NOT NULL,
	[TestDays] [tinyint] NULL,
	[ReportDays] [tinyint] NULL,
	[SpecimenID] [smallint] NULL,
	[SpecimenReqQuantity] [varchar](100) NULL,
	[Comments] [varchar](5000) NULL,
	[SortOrder] [int] NULL,
	[Rate] [decimal](15, 5) NOT NULL,
	[Unit] [varchar](100) NULL,
	[TemplateID] [smallint] NULL,
	[Format] [varchar](50) NULL,
	[CriticalValueLowerBound] [decimal](15, 5) NULL,
	[CriticalValueUpperBound] [decimal](15, 5) NULL,
	[Createdby] [varchar](30) NOT NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[ModifiedBy] [varchar](30) NOT NULL,
	[ModifiedDate] [smalldatetime] NOT NULL,
	[Status] [bit] NOT NULL,
	[TestType] [smallint] NOT NULL,
	[StabilityFrozen] [varchar](50) NULL,
	[StabilityRefrigerated] [varchar](50) NULL,
	[StabilityRoom] [varchar](50) NULL,
 CONSTRAINT [PK_Test] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Nexus_TestDepartment]    Script Date: 06/05/2018 1:16:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Nexus_TestDepartment](
	[ID] [smallint] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Description] [varchar](1000) NULL,
	[CreatedBy] [varchar](30) NOT NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[ModifiedBy] [varchar](30) NOT NULL,
	[ModifiedDate] [smalldatetime] NOT NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_TestDepartment_Status]  DEFAULT ((0)),
 CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Nexus_TestDoctor]    Script Date: 06/05/2018 1:16:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Nexus_TestDoctor](
	[ID] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Description] [varchar](500) NULL,
	[Signature] [image] NULL,
	[UserID] [int] NOT NULL,
	[CreatedBy] [varchar](30) NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [smalldatetime] NULL,
	[Status] [bit] NOT NULL,
	[IsDoctor] [bit] NOT NULL,
	[CommissionPercent] [decimal](18, 2) NULL,
 CONSTRAINT [PK_Doctor] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Nexus_TestGroup]    Script Date: 06/05/2018 1:16:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Nexus_TestGroup](
	[ID] [smallint] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[ReportName] [varchar](100) NOT NULL,
	[DepartmentID] [smallint] NOT NULL,
	[SortOrder] [smallint] NULL,
	[CreatedBy] [varchar](30) NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [varchar](30) NULL,
	[ModifiedDate] [smalldatetime] NULL,
	[Status] [bit] NULL,
 CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Nexus_TestNormalValues]    Script Date: 06/05/2018 1:16:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Nexus_TestNormalValues](
	[ID] [int] NOT NULL,
	[TestID] [int] NOT NULL,
	[Gender] [tinyint] NOT NULL,
	[FromAge] [tinyint] NULL,
	[ToAge] [tinyint] NULL,
	[FromValue] [decimal](15, 5) NULL,
	[ToValue] [decimal](15, 5) NULL,
	[TextValue] [varchar](100) NULL,
	[Remarks] [varchar](3000) NULL,
	[CreatedBy] [varchar](30) NOT NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[ModifiedBy] [varchar](30) NOT NULL,
	[ModifiedDate] [smalldatetime] NOT NULL,
	[Status] [bit] NOT NULL,
	[AgeType] [varchar](50) NULL,
 CONSTRAINT [PK_TestNormalValues] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Nexus_TestSpecimen]    Script Date: 06/05/2018 1:16:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Nexus_TestSpecimen](
	[ID] [smallint] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Description] [varchar](1000) NULL,
	[CreatedBy] [varchar](30) NOT NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[ModifiedBy] [varchar](30) NOT NULL,
	[ModifiedDate] [smalldatetime] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_Specimen] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[Nexus_Case] ADD  DEFAULT ((0)) FOR [TotalAmount]
GO
ALTER TABLE [dbo].[Nexus_Case] ADD  DEFAULT ((0)) FOR [Discount]
GO
ALTER TABLE [dbo].[Nexus_Case] ADD  DEFAULT ((0)) FOR [Less]
GO
ALTER TABLE [dbo].[Nexus_Case] ADD  DEFAULT ((0)) FOR [NetAmount]
GO
ALTER TABLE [dbo].[Nexus_Case] ADD  DEFAULT ((0)) FOR [PaidAmount]
GO
ALTER TABLE [dbo].[Nexus_Case] ADD  DEFAULT ((0)) FOR [Due]
GO
ALTER TABLE [dbo].[Nexus_Case] ADD  CONSTRAINT [DF_Case_Completed]  DEFAULT ((0)) FOR [Completed]
GO
ALTER TABLE [dbo].[Nexus_Case] ADD  CONSTRAINT [DF_Case_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Nexus_Case] ADD  CONSTRAINT [DF_Case_ModifiedBy]  DEFAULT ('Admin') FOR [ModifiedBy]
GO
ALTER TABLE [dbo].[Nexus_Case] ADD  CONSTRAINT [DF_Case_ModifiedDate]  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[Nexus_Case] ADD  CONSTRAINT [DF_Case_CreatedBy]  DEFAULT ('Admin') FOR [CreatedBy]
GO
ALTER TABLE [dbo].[Nexus_Case] ADD  DEFAULT ((0)) FOR [BankDueReceived]
GO
ALTER TABLE [dbo].[Nexus_Case] ADD  DEFAULT ((0)) FOR [BankPaid]
GO
ALTER TABLE [dbo].[Nexus_Case] ADD  DEFAULT ((0)) FOR [DueReceived]
GO
ALTER TABLE [dbo].[Nexus_Case] ADD  CONSTRAINT [DF_Case_AlertSent]  DEFAULT ((0)) FOR [AlertSent]
GO
ALTER TABLE [dbo].[Nexus_RateType] ADD  DEFAULT (' ') FOR [Createdby]
GO
ALTER TABLE [dbo].[Nexus_RateType] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Nexus_RateType] ADD  DEFAULT (' ') FOR [ModifiedBy]
GO
ALTER TABLE [dbo].[Nexus_RateType] ADD  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[Nexus_RateType] ADD  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Nexus_RateType] ADD  DEFAULT ((0)) FOR [IsFixPrice]
GO
ALTER TABLE [dbo].[Nexus_RateType] ADD  CONSTRAINT [DF_RateType_Special]  DEFAULT ((0)) FOR [Special]
GO
ALTER TABLE [dbo].[Nexus_RateType] ADD  CONSTRAINT [DF_RateType_Routine]  DEFAULT ((0)) FOR [Routine]
GO
ALTER TABLE [dbo].[Nexus_RateType] ADD  CONSTRAINT [DF_RateType_Biopsy]  DEFAULT ((0)) FOR [Biopsy]
GO
ALTER TABLE [dbo].[Nexus_RateType] ADD  CONSTRAINT [DF_RateType_PCR]  DEFAULT ((0)) FOR [PCR]
GO
ALTER TABLE [dbo].[Nexus_RateType] ADD  CONSTRAINT [DF_RateType_Rebate]  DEFAULT ((0)) FOR [Rebate]
GO
ALTER TABLE [dbo].[Nexus_RateType] ADD  CONSTRAINT [DF_RateType_Radiology]  DEFAULT ((0)) FOR [Radiology]
GO
ALTER TABLE [dbo].[Nexus_RateType] ADD  CONSTRAINT [DF_RateType_Other]  DEFAULT ((0)) FOR [Other]
GO
ALTER TABLE [dbo].[Nexus_RateType] ADD  CONSTRAINT [DF_RateType_Outside]  DEFAULT ((0)) FOR [Outside]
GO
ALTER TABLE [dbo].[Nexus_Reference] ADD  CONSTRAINT [DF_Reference_AllowCredit]  DEFAULT ((0)) FOR [PaymentMode]
GO
ALTER TABLE [dbo].[Nexus_Reference] ADD  CONSTRAINT [DF_Reference_CreditLimit]  DEFAULT ((0)) FOR [CreditLimit]
GO
ALTER TABLE [dbo].[Nexus_Reference] ADD  CONSTRAINT [DF_Reference_CurrentBalance]  DEFAULT ((0)) FOR [CurrentBalance]
GO
ALTER TABLE [dbo].[Nexus_Reference] ADD  CONSTRAINT [DF_Reference_DefaultDiscount]  DEFAULT ((0)) FOR [DefaultDiscount]
GO
ALTER TABLE [dbo].[Nexus_Reference] ADD  CONSTRAINT [DF_Reference_MaxDiscount]  DEFAULT ((0)) FOR [MaxDiscount]
GO
ALTER TABLE [dbo].[Nexus_Reference] ADD  CONSTRAINT [DF_Reference_CreatedBy]  DEFAULT ('Admin') FOR [CreatedBy]
GO
ALTER TABLE [dbo].[Nexus_Reference] ADD  CONSTRAINT [DF_Reference_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Nexus_Reference] ADD  CONSTRAINT [DF_Reference_ModifiedBy]  DEFAULT ('Admin') FOR [ModifiedBy]
GO
ALTER TABLE [dbo].[Nexus_Reference] ADD  CONSTRAINT [DF_Reference_ModifiedDate]  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[Nexus_Reference] ADD  CONSTRAINT [DF_Reference_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Nexus_Test] ADD  CONSTRAINT [DF_Test_Type]  DEFAULT ((0)) FOR [Type]
GO
ALTER TABLE [dbo].[Nexus_Test] ADD  CONSTRAINT [DF_Test_IsSpecial]  DEFAULT ((0)) FOR [IsSpecial]
GO
ALTER TABLE [dbo].[Nexus_Test] ADD  CONSTRAINT [DF_Test_ReportDays]  DEFAULT ((0)) FOR [ReportDays]
GO
ALTER TABLE [dbo].[Nexus_Test] ADD  CONSTRAINT [DF_Test_Rate]  DEFAULT ((0)) FOR [Rate]
GO
ALTER TABLE [dbo].[Nexus_Test] ADD  CONSTRAINT [DF_Test_TemplateID]  DEFAULT ((0)) FOR [TemplateID]
GO
ALTER TABLE [dbo].[Nexus_Test] ADD  CONSTRAINT [DF_Test_Format]  DEFAULT ((0)) FOR [Format]
GO
ALTER TABLE [dbo].[Nexus_Test] ADD  CONSTRAINT [DF_Test_Createdby]  DEFAULT (' ') FOR [Createdby]
GO
ALTER TABLE [dbo].[Nexus_Test] ADD  CONSTRAINT [DF_Test_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Nexus_Test] ADD  CONSTRAINT [DF_Test_ModifiedBy]  DEFAULT (' ') FOR [ModifiedBy]
GO
ALTER TABLE [dbo].[Nexus_Test] ADD  CONSTRAINT [DF_Test_ModifiedDate]  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[Nexus_Test] ADD  CONSTRAINT [DF_Test_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Nexus_Test] ADD  CONSTRAINT [DF_Test_TestType]  DEFAULT ((1)) FOR [TestType]
GO
ALTER TABLE [dbo].[Nexus_TestDoctor] ADD  CONSTRAINT [DF__tmp_TestD__Creat__24341603]  DEFAULT ('Admin') FOR [CreatedBy]
GO
ALTER TABLE [dbo].[Nexus_TestDoctor] ADD  CONSTRAINT [DF__tmp_TestD__Creat__25283A3C]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Nexus_TestDoctor] ADD  CONSTRAINT [DF__tmp_TestD__Modif__261C5E75]  DEFAULT ('Admin') FOR [ModifiedBy]
GO
ALTER TABLE [dbo].[Nexus_TestDoctor] ADD  CONSTRAINT [DF__tmp_TestD__Modif__271082AE]  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[Nexus_TestDoctor] ADD  CONSTRAINT [DF__tmp_TestD__Statu__2804A6E7]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Nexus_TestDoctor] ADD  CONSTRAINT [DF_TestDoctor_IsDoctor]  DEFAULT ((0)) FOR [IsDoctor]
GO
ALTER TABLE [dbo].[Nexus_TestGroup] ADD  CONSTRAINT [DF_TestGroup_CreatedBy]  DEFAULT ('Admin') FOR [CreatedBy]
GO
ALTER TABLE [dbo].[Nexus_TestGroup] ADD  CONSTRAINT [DF_TestGroup_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Nexus_TestGroup] ADD  CONSTRAINT [DF_TestGroup_ModifiedBy]  DEFAULT ('Admin') FOR [ModifiedBy]
GO
ALTER TABLE [dbo].[Nexus_TestGroup] ADD  CONSTRAINT [DF_TestGroup_ModifiedDate]  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[Nexus_TestGroup] ADD  CONSTRAINT [DF_TestGroup_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Nexus_TestNormalValues] ADD  CONSTRAINT [DF_TestNormalValues_Gender]  DEFAULT ((0)) FOR [Gender]
GO
ALTER TABLE [dbo].[Nexus_TestNormalValues] ADD  CONSTRAINT [DF_TestNormalValues_CreatedBy]  DEFAULT ('Admin') FOR [CreatedBy]
GO
ALTER TABLE [dbo].[Nexus_TestNormalValues] ADD  CONSTRAINT [DF_TestNormalValues_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Nexus_TestNormalValues] ADD  CONSTRAINT [DF_TestNormalValues_ModifiedBy]  DEFAULT ('Admin') FOR [ModifiedBy]
GO
ALTER TABLE [dbo].[Nexus_TestNormalValues] ADD  CONSTRAINT [DF_TestNormalValues_ModifiedDate]  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[Nexus_TestNormalValues] ADD  CONSTRAINT [DF_TestNormalValues_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Nexus_TestSpecimen] ADD  CONSTRAINT [DF_TestSpecimen_CreatedBy]  DEFAULT ('Admin') FOR [CreatedBy]
GO
ALTER TABLE [dbo].[Nexus_TestSpecimen] ADD  CONSTRAINT [DF_TestSpecimen_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Nexus_TestSpecimen] ADD  CONSTRAINT [DF_TestSpecimen_ModifiedBy]  DEFAULT ('Admin') FOR [ModifiedBy]
GO
ALTER TABLE [dbo].[Nexus_TestSpecimen] ADD  CONSTRAINT [DF_TestSpecimen_ModifiedDate]  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[Nexus_TestSpecimen] ADD  CONSTRAINT [DF_TestSpecimen_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Nexus_Case]  WITH NOCHECK ADD  CONSTRAINT [FK_Case_Consultant] FOREIGN KEY([ConsultantID])
REFERENCES [dbo].[Nexus_Consultant] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nexus_Case] CHECK CONSTRAINT [FK_Case_Consultant]
GO
ALTER TABLE [dbo].[Nexus_Case]  WITH NOCHECK ADD  CONSTRAINT [FK_Case_Patient] FOREIGN KEY([PatientID])
REFERENCES [dbo].[Nexus_Patient] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nexus_Case] CHECK CONSTRAINT [FK_Case_Patient]
GO
ALTER TABLE [dbo].[Nexus_Case]  WITH NOCHECK ADD  CONSTRAINT [FK_Case_Reference] FOREIGN KEY([ReferenceID])
REFERENCES [dbo].[Nexus_Reference] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nexus_Case] CHECK CONSTRAINT [FK_Case_Reference]
GO
ALTER TABLE [dbo].[Nexus_CaseDetail]  WITH NOCHECK ADD  CONSTRAINT [FK_CaseDetail_Case] FOREIGN KEY([CaseID])
REFERENCES [dbo].[Nexus_Case] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nexus_CaseDetail] NOCHECK CONSTRAINT [FK_CaseDetail_Case]
GO
ALTER TABLE [dbo].[Nexus_Reference]  WITH NOCHECK ADD  CONSTRAINT [FK_Reference_RateType] FOREIGN KEY([RateTypeID])
REFERENCES [dbo].[Nexus_RateType] ([ID])
GO
ALTER TABLE [dbo].[Nexus_Reference] CHECK CONSTRAINT [FK_Reference_RateType]
GO
ALTER TABLE [dbo].[Nexus_Test]  WITH NOCHECK ADD  CONSTRAINT [FK_Test_Group] FOREIGN KEY([GroupID])
REFERENCES [dbo].[Nexus_TestGroup] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nexus_Test] CHECK CONSTRAINT [FK_Test_Group]
GO
ALTER TABLE [dbo].[Nexus_Test]  WITH NOCHECK ADD  CONSTRAINT [FK_Test_TestSpecimen] FOREIGN KEY([SpecimenID])
REFERENCES [dbo].[Nexus_TestSpecimen] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nexus_Test] CHECK CONSTRAINT [FK_Test_TestSpecimen]
GO
ALTER TABLE [dbo].[Nexus_TestGroup]  WITH NOCHECK ADD  CONSTRAINT [FK_Group_Department] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[Nexus_TestDepartment] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Nexus_TestGroup] CHECK CONSTRAINT [FK_Group_Department]
GO
ALTER TABLE [dbo].[Nexus_TestNormalValues]  WITH NOCHECK ADD  CONSTRAINT [FK_TestNormalValues_Test] FOREIGN KEY([TestID])
REFERENCES [dbo].[Nexus_Test] ([ID])
GO
ALTER TABLE [dbo].[Nexus_TestNormalValues] CHECK CONSTRAINT [FK_TestNormalValues_Test]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'When the patient can collect Reports default value will be collected from Application Settings' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Case', @level2type=N'COLUMN',@level2name=N'ReportingDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Who is reffering the patient to Lab. Default value will come from Patient else Default Reference ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Case', @level2type=N'COLUMN',@level2name=N'ReferenceID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Name of Reference will also stored. ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Case', @level2type=N'COLUMN',@level2name=N'ReferenceName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'which doctor has been reffered to patient ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Case', @level2type=N'COLUMN',@level2name=N'ConsultantID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'In which center case was registered and value will come from Application Settings' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Case', @level2type=N'COLUMN',@level2name=N'RegistrationLocation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Center from which patient want to collect reports. Default value is RegistrationLocation but user can override it. ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Case', @level2type=N'COLUMN',@level2name=N'DestinationLocation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Total Bill Amount Sum of Total Test Amount + Other Charges (Read Only on Front End)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Case', @level2type=N'COLUMN',@level2name=N'TotalAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Percentage of Discount. Default value come from Patient if refernce has mentioned then come from Reference' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Case', @level2type=N'COLUMN',@level2name=N'Discount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Lessed Amount (i.e) value should be rounded to multiple of 10 or 5 which will be stored in Appliation Settings but use can override it.  ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Case', @level2type=N'COLUMN',@level2name=N'Less'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Total - Discounted Amount  - Less (Read Only on Front End)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Case', @level2type=N'COLUMN',@level2name=N'NetAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Default Value is Total - Discounted Amount  - Less. But user can override it  ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Case', @level2type=N'COLUMN',@level2name=N'PaidAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'NetAmount - PaidAmount (Read Only on Front End)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Case', @level2type=N'COLUMN',@level2name=N'Due'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'When all the tests of the patient will be approved then it will be changed to 1 using trigger on Detail Table  ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Case', @level2type=N'COLUMN',@level2name=N'Completed'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Case', @level2type=N'COLUMN',@level2name=N'CreatedBy'
GO
EXEC sys.sp_addextendedproperty @name=N'Ms_Description', @value=N'It will be used to store information of the patient when he comes to register some test(s).
Amounts are aggregated fields and should be updated when user receive dues or modify some tests' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Case'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Current Status of the Of ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_CaseDetail', @level2type=N'COLUMN',@level2name=N'TestStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Location where test will be conducted default value will come from Test' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_CaseDetail', @level2type=N'COLUMN',@level2name=N'ConductedAt'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'When Test will be delivered. Default Value of the Reporting date will be Current date + reporting days of the test' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_CaseDetail', @level2type=N'COLUMN',@level2name=N'ReportingDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_CaseDetail', @level2type=N'COLUMN',@level2name=N'CreatedBy'
GO
EXEC sys.sp_addextendedproperty @name=N'Ms_Description', @value=N'It stores the information of the test(s) registered for a case' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_CaseDetail'
GO
EXEC sys.sp_addextendedproperty @name=N'Ms_Description', @value=N'Consultant is  doctor who have asked the patient for the Test.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Consultant'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'For Internal Use' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Patient', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'caption', @value=N'Test Decription' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Patient'
GO
EXEC sys.sp_addextendedproperty @name=N'description', @value=N'Test Decription' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Patient'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'This entity deals with the patient who is coming first time. It contains one record per patient. Each patient will be assigned Unique Number which is called as Patient Number which will be a barcode. Patient Number generation will be written separately so that it can be changed per requirements. 
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Patient'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_RateType', @level2type=N'COLUMN',@level2name=N'IsFixPrice'
GO
EXEC sys.sp_addextendedproperty @name=N'Ms_Description', @value=N'Rates of Test will be defined separately in this entity. Row wilth ID 0 will be used to store standard rates.  ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_RateType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0 For No Credit, 1 For Bill to Company ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Reference', @level2type=N'COLUMN',@level2name=N'PaymentMode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Maximum Amount which can be credit' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Reference', @level2type=N'COLUMN',@level2name=N'CreditLimit'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Calculated Using trigger on ReferencePayments' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Reference', @level2type=N'COLUMN',@level2name=N'CurrentBalance'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Default Discount in Percentage which will be given to patients when they come through reference' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Reference', @level2type=N'COLUMN',@level2name=N'DefaultDiscount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Max Discount in Percentage which can be  given to patients when they come through reference' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Reference', @level2type=N'COLUMN',@level2name=N'MaxDiscount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Reference', @level2type=N'COLUMN',@level2name=N'CreatedBy'
GO
EXEC sys.sp_addextendedproperty @name=N'Ms_Description', @value=N'Reference is the company who will be paying us per month or lumsum or the patients coming  with the reference will be discounted as dealed with company.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Reference'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0 For Normal , 1 for Profile and 2 for Package' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Test', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'use to save testtype (0 THEN ''Routine'' WHEN 1 THEN ''Special'' WHEN 2 THEN ''PCR'' WHEN 3 THEN ''BIOPSY'' )' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_Test', @level2type=N'COLUMN',@level2name=N'TestType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_TestGroup', @level2type=N'COLUMN',@level2name=N'CreatedBy'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0 for Male , 1 for Female and 2 for Both' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_TestNormalValues', @level2type=N'COLUMN',@level2name=N'Gender'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_TestNormalValues', @level2type=N'COLUMN',@level2name=N'CreatedBy'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Nexus_TestSpecimen', @level2type=N'COLUMN',@level2name=N'CreatedBy'
GO

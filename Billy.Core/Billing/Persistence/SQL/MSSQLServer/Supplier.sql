CREATE TABLE [dbo].[Supplier](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Notes] [varchar](180) NULL,
	[Email] [varchar](50) NULL,
	[Website] [varchar](100) NULL,
	[Phone] [varchar](20) NULL,
	[Fax] [varchar](20) NULL,
	-- Agent
	[Agent_Name] [varchar](50) NULL,
	[Agent_Surname] [varchar](50) NULL,
	[Agent_Phone] [varchar](50) NULL,
	-- Address
	[Address_Country] [varchar](50) NULL,
	[Address_Province] [varchar](50) NULL,
	[Address_City] [varchar](50) NULL,
	[Address_Zip] [varchar](20) NULL,
	[Address_Street] [varchar](50) NULL,
	[Address_Number] [varchar](20) NULL,
 CONSTRAINT [PK_Supplier] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Supplier_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

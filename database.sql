CREATE DATABASE [LIUSHUIZHANG2022]
GO
USE [LIUSHUIZHANG2022]
GO
/****** Object:  Table [dbo].[BIZHONG]    Script Date: 11/7/2022 10:50:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BIZHONG](
	[BIZHONGID] [bigint] IDENTITY(1,1) NOT NULL,
	[BIZHONG] [nvarchar](50) NULL,
	[LIANG] [decimal](18, 2) NULL,
	[PINGJUNJIA] [decimal](18, 2) NULL,
	[YIGONG] [decimal](18, 2) NULL,
	[LEI] [int] NULL,
	[TINGYONG] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[BIZHONGID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LIUSHUI]    Script Date: 11/7/2022 10:50:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LIUSHUI](
	[LIUSHUIID] [bigint] IDENTITY(1,1) NOT NULL,
	[RIZI] [date] NULL,
	[QIANE] [decimal](18, 0) NULL,
	[XIANE] [decimal](18, 0) NULL,
	[XIANGCHA] [decimal](18, 0) NULL,
	[DIANSUANJIEGUO] [decimal](18, 0) NULL,
	[_500000] [int] NULL,
	[_200000] [int] NULL,
	[_100000] [int] NULL,
	[_50000] [int] NULL,
	[_20000] [int] NULL,
	[_10000] [int] NULL,
	[_5000] [int] NULL,
	[_2000] [int] NULL,
	[_1000] [int] NULL,
 CONSTRAINT [PK_LIUSHUI] PRIMARY KEY CLUSTERED 
(
	[LIUSHUIID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RENYUAN]    Script Date: 11/7/2022 10:50:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RENYUAN](
	[RENYUANID] [bigint] IDENTITY(1,1) NOT NULL,
	[YONGHU] [varchar](50) NULL,
	[MIMA] [varchar](50) NULL,
	[QUAN] [int] NULL,
	[TINGYONG] [bit] NULL,
 CONSTRAINT [PK_RENYUAN] PRIMARY KEY CLUSTERED 
(
	[RENYUANID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

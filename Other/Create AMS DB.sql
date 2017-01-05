USE master
DECLARE @dbname varchar(20)
SET @dbname=N'AMSDashboardTest'

IF (EXISTS (SELECT name 
	FROM master.dbo.sysdatabases 
	WHERE ('[' + name + ']' = @dbname 
	OR name = @dbname)))
BEGIN
	Print 'Dropping previous db version'
	--This process makes system changes so an existing database can be safely deleted
	--EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = @dbname--N'IT560Final'
	
	--USE [master]
	
	--ALTER DATABASE [AMSDashboard] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
	
	/****** Object:  Database [AMSDashboard]    Script Date: 4/22/2016 ******/
	DROP DATABASE [AMSDashboardTest]
	--GO
END
Print 'Creating new Database'
/****** Object:  Database [AMSDashboard]    Script Date: 4/22/2016 ******/
CREATE DATABASE [AMSDashboardTest]
Print 'New Database Created'
GO


USE AMSDashboardTest

/****** Object:  Table [dbo].[MediaServicesAccount]    Script Date: 4/19/2016 12:06:14 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
Print 'Creating Tables'
Print 'Creating Table: MediaServicesAccount'
GO

CREATE TABLE [MediaServicesAccount](
	[AccountId] [int] IDENTITY(1,1) NOT NULL,
	[AccountName] [nvarchar](50) NOT NULL,
	[SubscriptionName] [nvarchar](75) NULL,
	[DataCenter] [nvarchar](75) NULL,
	[Location] [nvarchar](75) NULL,
	[AccountCreated] [date] NOT NULL,
 CONSTRAINT [PK_AccountSubscription] PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
Create NonClustered Index idx_AccountName on [MediaServicesAccount](AccountName Asc)
Print 'Creating Table: Origin'  
GO


/****** Object:  Table [dbo].[Origin]    Script Date: 4/19/2016 12:11:30 PM ******/
CREATE TABLE [Origin](
	[OriginId] [nvarchar](60) Unique NOT NULL,
	[AccountId] [int] NOT NULL,
	[OriginName] [nvarchar](max) NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[HealthIncluded] [bit] default 1,
 CONSTRAINT [PK_Origin] PRIMARY KEY CLUSTERED 
(
	[OriginId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

Create NonClustered Index idx_OriginAccount on [Origin](AccountId)

GO

ALTER TABLE [dbo].[Origin]  WITH CHECK ADD  CONSTRAINT [FK_Origin_Account] FOREIGN KEY([AccountId])
REFERENCES [dbo].[MediaServicesAccount] ([AccountId])
GO

ALTER TABLE [dbo].[Origin] CHECK CONSTRAINT [FK_Origin_Account]
Print 'Creating Table: Channel'
GO

/****** Object:  Table [dbo].[Channel]    Script Date: 4/19/2016 12:14:06 PM ******/
CREATE TABLE [dbo].[Channel](
	[ChannelId] [nvarchar](60) Unique NOT NULL,
	[AccountId] [int] NOT NULL,
	[OriginId] [nvarchar](60) NULL,
	[ChannelName] [nvarchar](50) NOT NULL,
	[Created] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Channel] PRIMARY KEY CLUSTERED 
(
	[ChannelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
Create NonClustered Index idx_ChannelAccount on [Channel](AccountId)
Create NonClustered Index idx_ChannelName on [Channel](ChannelName)
Create NonClustered Index idx_ChannelOrigin on [Channel](OriginID)
GO

ALTER TABLE [dbo].[Channel]  WITH CHECK ADD  CONSTRAINT [FK_Channel_Account] FOREIGN KEY([AccountId])
REFERENCES [dbo].[MediaServicesAccount] ([AccountId])
GO

ALTER TABLE [dbo].[Channel] CHECK CONSTRAINT [FK_Channel_Account]
Print 'Creating Table: Program'
GO


/****** Object:  Table [dbo].[Program]    Script Date: 4/19/2016 12:43:00 PM ******/
CREATE TABLE [dbo].[Program](
	[ProgramID] [nvarchar](60) Unique NOT NULL,
	[ChannelID] [nvarchar](60) NOT NULL,
	[ProgramName] [nvarchar](max) NOT NULL,
	[Created] [datetime2](7) NULL,
 CONSTRAINT [PK_Program] PRIMARY KEY CLUSTERED 
(
	[ProgramID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
Create NonClustered Index idx_ProgramChannel on [Program](ChannelID)
GO

ALTER TABLE [dbo].[Program]  WITH NOCHECK ADD CONSTRAINT [FK_Program_Channel] FOREIGN KEY([ChannelID])
REFERENCES [dbo].[Channel] ([ChannelID])
GO

ALTER TABLE [dbo].[Program] NOCHECK CONSTRAINT [FK_Program_Channel]
Print 'Creating Table: AlertType'
GO



/****** Object:  Table [dbo].[AlertType]    Script Date: 4/19/2016 12:29:54 PM ******/

SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[AlertType](
	[AlertTypeID] [int] IDENTITY(1,1) NOT NULL,
	[AlertTypeName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_AlertType] PRIMARY KEY CLUSTERED 
(
	[AlertTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

INSERT INTO AlertType(AlertTypeName) Values ('Global')
INSERT INTO AlertType(AlertTypeName) Values ('Channel')
INSERT INTO AlertType(AlertTypeName) Values ('Origin')
INSERT INTO AlertType(AlertTypeName) Values ('Archive')

/****** Object:  Table [dbo].[Alert]    Script Date: 4/19/2016 12:26:56 PM ******/


SET ANSI_PADDING ON
Print 'Creating Table: Alert'
GO

CREATE TABLE [dbo].[Alert](
	[AlertID] [int] IDENTITY(1,1) NOT NULL,
	[AlertTypeID] [int] NOT NULL,
	[AccountId] [int] NOT NULL,
	[OriginID] [nvarchar](60) NULL,
	[ChannelID] [nvarchar](60) NULL,
	[ProgramID] [nvarchar](60) NULL,
	[TrackID] [nvarchar](75) NULL,
	[StreamID] [nvarchar](75) NULL,
	[MetricType] [nvarchar](10) NOT NULL,
	[MetricName] [nvarchar](75) NOT NULL,
	[AlertValue] [decimal](18,3) NULL,
	[ErrorLevel] [varchar](10) NOT NULL,
	[AlertDate] [date] NOT NULL,
	[AlertTime] [time] NOT NULL,

	[Details] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Alert] PRIMARY KEY CLUSTERED 
(
	[AlertID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

Create NonClustered Index idx_AlertDate on [Alert](AlertDate)
Create NonClustered Index idx_AlertChannel on [Alert](ChannelID, AlertDate)
Create NonClustered Index idx_AlertOrigin on [Alert](OriginID, AlertDate)
Create NonClustered Index idx_AlertProgram on [Alert](ProgramID, AlertDate)
Create NonClustered Index idx_AlertAccount on [Alert](AlertDate Asc, AccountID)
Create NonClustered Index idx_AlertAccountChannel on [Alert](AlertDate DESC, AlertTime DESC,AccountID, ChannelID)
Create NonClustered Index idx_AlertAccountOrigin on [Alert](AlertDate DESC, AlertTime DESC,AccountID, OriginID)
Create NonClustered Index idx_AlertAccountProgram on [Alert](AlertDate DESC, AlertTime DESC,AccountID, ProgramID)
GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Alert]  WITH CHECK ADD  CONSTRAINT [FK_Alert_AlertType] FOREIGN KEY([AlertTypeID])
REFERENCES [dbo].[AlertType] ([AlertTypeID])
GO

ALTER TABLE [dbo].[Alert] CHECK CONSTRAINT [FK_Alert_AlertType]
GO

ALTER TABLE [dbo].[Alert]  WITH NOCHECK ADD  CONSTRAINT [FK_Alert_Account] FOREIGN KEY([AccountId])
REFERENCES [dbo].[MediaServicesAccount] ([AccountId])
GO

ALTER TABLE [dbo].[Alert] NOCHECK CONSTRAINT [FK_Alert_Account]
GO

ALTER TABLE [dbo].[Alert]  WITH NOCHECK ADD  CONSTRAINT [FK_Alert_Channel] FOREIGN KEY([ChannelID])
REFERENCES [dbo].[Channel] ([ChannelId])
GO

ALTER TABLE [dbo].[Alert] NOCHECK CONSTRAINT [FK_Alert_Channel]
GO

ALTER TABLE [dbo].[Alert]  WITH NOCHECK ADD  CONSTRAINT [FK_Alert_Origin] FOREIGN KEY([OriginID])
REFERENCES [dbo].[Origin] ([OriginID])
GO

ALTER TABLE [dbo].[Alert] NOCHECK CONSTRAINT [FK_Alert_Origin]
GO

ALTER TABLE [dbo].[Alert]  WITH NOCHECK ADD  CONSTRAINT [FK_Alert_Program] FOREIGN KEY([ProgramID])
REFERENCES [dbo].[Program] ([ProgramID])
GO

ALTER TABLE [dbo].[Alert] NOCHECK CONSTRAINT [FK_Alert_Program]
GO

Print 'Creating View: ChannelAlert'
GO

/** This View Actually pulls only the most recent 30 seconds of data **/
Create View ChannelAlert as (
SELECT
		ISNULL(alt.AlertID, -1) as AlertID,        
		acct.SubscriptionName,
		acct.AccountName,
		acct.DataCenter,
		at.AlertTypeName, 
		ch.AccountId,
		alt.ChannelId, 
		ch.ChannelName,
		ch.OriginID,
		org.OriginName, 
		alt.AlertDate, 
		alt.AlertTime, 
		alt.TrackID,
		alt.StreamID,
		alt.MetricType,
		alt.MetricName, 
		alt.ErrorLevel,
		alt.AlertValue,
		alt.Details
	FROM dbo.Alert alt
		JOIN dbo.MediaServicesAccount acct ON alt.AccountId = acct.AccountId 
		JOIN dbo.Channel ch ON alt.ChannelID = ch.ChannelId AND acct.AccountId = ch.AccountId
		JOIN dbo.Origin org on org.AccountId = acct.AccountId and ch.OriginId = org.OriginId
		JOIN dbo.AlertType as at ON alt.AlertTypeID = at.AlertTypeID
	WHERE at.AlertTypeID=2 
)
GO
Print 'Creating View: OriginAlert'
GO

Create View OriginAlert as (
	SELECT
		ISNULL(alt.AlertID, -1) as AlertID,        
		acct.SubscriptionName,
		acct.AccountName,
		at.AlertTypeName, 
		alt.AccountId,
		alt.OriginID, 
		org.OriginName, 
		alt.AlertDate, 
		alt.AlertTime, 
		alt.MetricType,
		alt.MetricName, 
		alt.ErrorLevel,
		alt.AlertValue,
		alt.Details
	FROM dbo.Alert alt
		JOIN dbo.MediaServicesAccount acct ON alt.AccountId = acct.AccountId 
		JOIN dbo.Origin org ON alt.OriginID = org.OriginId AND acct.AccountId = org.AccountId
		JOIN dbo.AlertType at ON alt.AlertTypeID = at.AlertTypeID
	WHERE at.AlertTypeID=3 
)
GO

Print 'Creating View: ArchiveAlert'
GO
Create View ArchiveAlert as (
	SELECT
		ISNULL(alt.AlertID, -1) as AlertID,        
		acct.SubscriptionName,
		acct.AccountName,
		at.AlertTypeName, 
		prg.ChannelID,
		prg.ProgramID, 
		prg.ProgramName, 
		alt.AlertDate, 
		alt.AlertTime,
		alt.TrackID,
		alt.StreamID,
		alt.MetricType,
		alt.MetricName, 
		alt.ErrorLevel,
		alt.AlertValue,
		alt.Details
	FROM dbo.Alert alt
		JOIN dbo.MediaServicesAccount acct ON alt.AccountId = acct.AccountId 
		JOIN dbo.Program prg ON alt.ProgramID = prg.ProgramID AND alt.AccountId = acct.AccountId
		JOIN dbo.AlertType at ON alt.AlertTypeID = at.AlertTypeID
	WHERE at.AlertTypeID=4 
)

GO

Print 'Creating View: GlobalAlert'
GO
Create View GlobalAlert as (
	SELECT 
		ISNULL(alt.AlertID, -1) as AlertID,       
		acct.SubscriptionName,
		acct.AccountName,
		at.AlertTypeName, 
		alt.AlertDate, 
		alt.AlertTime, 
		alt.MetricType,
		alt.MetricName, 
		alt.ErrorLevel,
		alt.AlertValue,
		alt.Details
	FROM dbo.Alert alt
		JOIN dbo.MediaServicesAccount acct ON alt.AccountId = acct.AccountId 
		JOIN dbo.AlertType at ON alt.AlertTypeID = at.AlertTypeID
	WHERE at.AlertTypeID=1 
)
GO

Print 'Database creation, including relationships and necessary indexes, now complete'

/*** THE FOLLOWING SECTION IS FOR USE ONLY ON LOCAL MACHINE  ***/
Print 'Updating user login privileges'
GO
--Create Login [dashboard] for master 

CREATE USER [dashboard] FOR LOGIN [dashboard]
GO

ALTER ROLE [db_owner] ADD MEMBER [dashboard]
Print 'User Login privileges complete'
GO

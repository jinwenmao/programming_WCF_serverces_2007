IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'WCFLogbook')
	DROP DATABASE [WCFLogbook]
GO

CREATE DATABASE [WCFLogbook]  ON (NAME = N'WCFLogbook', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL\data\WCFLogbook.mdf' , SIZE = 2, FILEGROWTH = 10%) LOG ON (NAME = N'WCFLogbook_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL\data\WCFLogbook_log.ldf' , SIZE = 2, FILEGROWTH = 10%)
 COLLATE SQL_Latin1_General_CP1_CI_AS
GO

exec sp_dboption N'WCFLogbook', N'autoclose', N'false'
GO

exec sp_dboption N'WCFLogbook', N'bulkcopy', N'false'
GO

exec sp_dboption N'WCFLogbook', N'trunc. log', N'false'
GO

exec sp_dboption N'WCFLogbook', N'torn page detection', N'true'
GO

exec sp_dboption N'WCFLogbook', N'read only', N'false'
GO

exec sp_dboption N'WCFLogbook', N'dbo use', N'false'
GO

exec sp_dboption N'WCFLogbook', N'single', N'false'
GO

exec sp_dboption N'WCFLogbook', N'autoshrink', N'false'
GO

exec sp_dboption N'WCFLogbook', N'ANSI null default', N'false'
GO

exec sp_dboption N'WCFLogbook', N'recursive triggers', N'false'
GO

exec sp_dboption N'WCFLogbook', N'ANSI nulls', N'false'
GO

exec sp_dboption N'WCFLogbook', N'concat null yields null', N'false'
GO

exec sp_dboption N'WCFLogbook', N'cursor close on commit', N'false'
GO

exec sp_dboption N'WCFLogbook', N'default to local cursor', N'false'
GO

exec sp_dboption N'WCFLogbook', N'quoted identifier', N'false'
GO

exec sp_dboption N'WCFLogbook', N'ANSI warnings', N'false'
GO

exec sp_dboption N'WCFLogbook', N'auto create statistics', N'true'
GO

exec sp_dboption N'WCFLogbook', N'auto update statistics', N'true'
GO

if( (@@microsoftversion / power(2, 24) = 8) and (@@microsoftversion & 0xffff >= 724) )
	exec sp_dboption N'WCFLogbook', N'db chaining', N'false'
GO

use [WCFLogbook]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ClearAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[ClearAll]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Entries]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Entries]
GO

CREATE TABLE [dbo].[Entries] (
	[MachineName] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[HostName] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[AssemblyName] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[FileName] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[LineNumber] [int] NULL ,
	[TypeName] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[MemberAccessed] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[EntryDate] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[EntryTime] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[ExceptionName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[ExceptionMessage] [varchar] (300) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[ProvidedFault] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[ProvidedMessage] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Event] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[EntryNumber] [int] IDENTITY (1, 1) NOT NULL 
) ON [PRIMARY]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO



CREATE PROCEDURE dbo.ClearAll
AS
	SET NOCOUNT OFF;
DELETE FROM Entries


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


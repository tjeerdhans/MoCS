USE [master]
GO
--IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'MOCS')
--BEGIN
--CREATE DATABASE [MOCS] ON  PRIMARY 
--( NAME = N'MOCS', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\MOCS.mdf' , SIZE = 2048KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
-- LOG ON 
--( NAME = N'MOCS_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\MOCS_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
--END

--GO
EXEC dbo.sp_dbcmptlevel @dbname=N'MOCS', @new_cmptlevel=90
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MOCS].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MOCS] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MOCS] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MOCS] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MOCS] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MOCS] SET ARITHABORT OFF 
GO
ALTER DATABASE [MOCS] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [MOCS] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [MOCS] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MOCS] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MOCS] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MOCS] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MOCS] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MOCS] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MOCS] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MOCS] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MOCS] SET  ENABLE_BROKER 
GO
ALTER DATABASE [MOCS] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MOCS] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MOCS] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [MOCS] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [MOCS] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MOCS] SET  READ_WRITE 
GO
ALTER DATABASE [MOCS] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [MOCS] SET  MULTI_USER 
GO
ALTER DATABASE [MOCS] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [MOCS] SET DB_CHAINING OFF 
USE [MOCS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tournament]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Tournament](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TournamentName] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_Tournament] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Assignment]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Assignment](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AssignmentName] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_Assignment] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [IX_Assignment] UNIQUE NONCLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeamSubmit_Finished]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[TeamSubmit_Finished] 
@SubmitID			int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

UPDATE TeamSubmit
Set IsFinished = 1
WHERE ID = @SubmitID

END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Team]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Team](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TeamName] [nvarchar](50) NOT NULL,
	[TeamMembers] [nvarchar](50) NULL,
	[Password] [nvarchar](50) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[IsAdmin] [bit] NULL,
 CONSTRAINT [PK_Team] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeamSubmit]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TeamSubmit](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TeamID] [int] NOT NULL,
	[AssignmentID] [int] NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[FileStream] [varbinary](max) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[CurrentStatusID] [int] NULL,
	[IsFinished] [bit] NOT NULL,
 CONSTRAINT [PK_TeamSubmit] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TournamentAssignment]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TournamentAssignment](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TournamentID] [int] NOT NULL,
	[AssignmentID] [int] NOT NULL,
	[AssignmentOrder] [int] NOT NULL,
	[Points] [int] NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_TournamentAssignment] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [IX_TournamentAssignment] UNIQUE NONCLUSTERED 
(
	[TournamentID] ASC,
	[AssignmentID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeamScore]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TeamScore](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TeamID] [int] NOT NULL,
	[AssignmentID] [int] NOT NULL,
	[Points] [int] NOT NULL,
 CONSTRAINT [PK_TeamScore] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeamAssignment]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TeamAssignment](
	[TeamID] [int] NOT NULL,
	[AssignmentID] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
 CONSTRAINT [PK_TeamAssignment] PRIMARY KEY CLUSTERED 
(
	[TeamID] ASC,
	[AssignmentID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeamSubmitStatus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TeamSubmitStatus](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TeamID] [int] NOT NULL,
	[TeamSubmitID] [int] NOT NULL,
	[StatusCode] [int] NOT NULL,
	[Details] [nvarchar](1000) NULL,
	[CreationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_TeamSumbitStatus] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeamSubmit_Select]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[TeamSubmit_Select]
	@TeamID	int
	,@AssignmentID	nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--get the monitor statsu 
	select ts.id, ts.CreationDate as submitDate, tss.StatusCode as CurrentStatus, tss.CreationDate as statusDate, tss.Details, ts.IsFinished
	 from teamsubmit ts, teamsubmitstatus tss
	where ts.currentstatusid = tss.id
	and ts.teamid = @TeamID
	and ts.assignmentID = @AssignmentID
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Team_Delete]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Team_Delete]
	@ID	int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

DELETE FROM TEAMSUBMITSTATUS WHERE TEAMID = @ID

DELETE FROM TEAMSUBMIT WHERE TEAMID = @ID

DELETE FROM TEAMASSIGNMENT WHERE TEAMID = @ID

DELETE FROM TEAM WHERE ID = @ID


    
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeamSubmit_SelectForAdmin]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[TeamSubmit_SelectForAdmin]
	@AssignmentId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	select ts.ID, t.TeamName, t.TeamMembers, ts.CreationDate as SubmitDate, ts.FileName, ts.FileStream, ts.CurrentStatusID, tss.StatusCode, tss.Details, tss.CreationDate as StatusDate, ts.AssignmentID as AssignmentName, ts.IsFinished
	 from teamsubmit ts 
	JOIN Team t ON ts.TeamID = t.ID
	JOIN TeamSubmitStatus tss ON ts.CurrentStatusID = tss.ID
	WHERE ts.AssignmentID = @AssignmentId
	ORDER BY ts.CreationDate
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeamSubmit_Delete]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[TeamSubmit_Delete]
	@SubmitID	int
AS
BEGIN

DELETE FROM TeamSubmitStatus 
WHERE TeamSubmitID = @SubmitID



DELETE FROM TeamSubmit
WHERE ID = @SubmitID


END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeamSubmit_MonitorByAssignment]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[TeamSubmit_MonitorByAssignment] 
	@AssignmentID	int
AS
BEGIN

Select tm.TeamName, tm.TeamMembers, Result.* FROM TEAM tm
LEFT OUTER JOIN (SELECT s.TeamID, s.CreationDate as SubmitDate, s.IsFinished,tss.StatusCode, tss.CreationDate as StatusDate
 FROM TeamSubmit s 
JOIN (select teamid as t, max(creationdate) as MaxDate from teamsubmit
group by teamid) as x ON s.TeamID = x.t AND x.MaxDate = s.CreationDate
JOIN TeamSubmitStatus tss ON s.CurrentStatusID = tss.ID
WHERE s.AssignmentID = @AssignmentID) AS Result ON tm.ID = Result.TEamID
WHERE (tm.IsAdmin is null OR tm.IsAdmin = 0)

END



' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeamSubmit_SelectUnprocessed]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[TeamSubmit_SelectUnprocessed] 
AS
BEGIN

select ts.ID, t.id as TeamID, t.teamname as TeamName, a.id as AssignmentID, a.AssignmentName, ts.FileName, ts.CreationDate as SubmitDate, ts.FileStream, ts.CurrentStatusID, tss.StatusCode 
from teamSubmit ts inner join teamSubmitStatus tss ON ts.CurrentStatusID = tss.ID
inner join team t on ts.teamid = t.id
inner join assignment a ON ts.AssignmentID = a.ID
where ts.IsFinished = 0
and tss.StatusCode = 0
order by ts.CreationDate asc

END



' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeamSubmitStatus_Insert]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[TeamSubmitStatus_Insert]
	@TeamID			int
	,@TeamSubmitID	int
	,@StatusCode	int
	,@Details		nvarchar(1000) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO TeamSubmitStatus (TeamID, TeamSubmitID, StatusCode, Details, CreationDate)
	VALUES(@TeamID, @TeamSubmitID, @StatusCode, @Details, GetDate())

	declare @newTeamSubmitStatusID int
	set @newTeamSubmitStatusID = @@identity

	UPDATE TeamSubmit
	Set CurrentStatusID = @newTeamSubmitStatusID
	WHERE ID = @TeamSubmitID

END


' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TournamentAssignments]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[TournamentAssignments]
AS
BEGIN

SELECT ta.AssignmentID, ta.Points, a.AssignmentName, ta.Active
FROM Tournament t 
INNER JOIN TournamentAssignment ta ON t.ID = ta.TournamentID
INNER JOIN Assignment a ON ta.AssignmentID = a.ID
WHERE t.ID = 1
ORDER BY ta.AssignmentOrder


END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Assignment_SelectAll]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Assignment_SelectAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ID, AssignmentName from Assignment
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TournamentAssignment_GetActive]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[TournamentAssignment_GetActive] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    select TOP 1 a.ID, AssignmentName, Points
	from TournamentAssignment ta 
	JOIN Assignment a on a.id = ta.assignmentid
	where ta.active = 1
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TournamentAssignments_GetActive]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[TournamentAssignments_GetActive]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

select TOP 1 a.ID, AssignmentName, Points from TournamentAssignment ta 
JOIN Assignment a on a.id = ta.assignmentid
where ta.active = 1
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeamAssignment_GetByID]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[TeamAssignment_GetByID]
	@TeamId int,
	@AssignmentId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	select a.ID, a.AssignmentName, ta.Points, ta.Active, tma.StartDate 
	from assignment a
	inner join tournamentAssignment ta on a.id = ta.assignmentid
	inner join teamAssignment tma on a.id = tma.AssignmentID
	WHERE tma.TeamID = @TeamId
	AND tma.AssignmentID = @AssignmentID
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeamAssignment_SelectAll]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[TeamAssignment_SelectAll] 
	@TeamId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	select a.ID, a.AssignmentName, ta.Points, ta.Active, tma.StartDate 
	from assignment a
	inner join tournamentAssignment ta on a.id = ta.assignmentid
	inner join teamAssignment tma on a.id = tma.AssignmentID
	WHERE tma.TeamID = @TeamId
END


' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TournamentAssignment_Activate]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[TournamentAssignment_Activate]
	-- Add the parameters for the stored procedure here
	@ID	INT,
	@Active BIT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE TournamentAssignment
	SET Active = @Active
	WHERE ID = @ID

END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Team_Authenticate]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[Team_Authenticate]
	@TeamName	nvarchar(50),
	@Password	nvarchar(50)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT ID, TeamName, TeamMembers, Password, IsAdmin, CreationDate
	FROM TEAM
	WHERE TeamName = @TeamName
	AND Password = @Password 

END



' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Team_SelectAll]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Team_SelectAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT t.ID, t.TeamName, t.TeamMembers, t.Password, t.CreationDate, t.IsAdmin, x.TotalPoints
	FROM Team t 
	LEFT JOIN (select TeamID, SUM(Points) as TotalPoints
				from teamscore
				group by teamid) as X ON t.ID = x.TeamID
	ORDER BY t.TeamName

END




' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Team_Update]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Team_Update] 
	-- Add the parameters for the stored procedure here
@ID	int,
@TeamName nvarchar(50),
@TeamMembers nvarchar(100),
@IsAdmin	bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF(@ID<>0)
		BEGIN
			UPDATE Team
			SET TeamName = @TeamName,
			TeamMembers = @TeamMembers,
			IsAdmin = @IsAdmin
			WHERE ID = @ID
		END
	ELSE
		BEGIN
			INSERT INTO TEAM(TeamName, Password, CreationDate, IsAdmin, TeamMembers)
			VALUES (@TeamName, @TeamName, GetDate(), @IsAdmin, @TeamMembers)
	
			Select @@Identity
		END
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Team_Insert]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Team_Insert]
	@TeamName	nvarchar(50),
	@Password	nvarchar(50),
	@TeamMembers nvarchar(100),
	@IsAdmin bit

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO TEAM(TeamName, Password,CreationDate, IsAdmin, TeamMembers)
	VALUES (@TeamName, @Password, GetDate(), @IsAdmin, @TeamMembers)
	
    Select @@Identity
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Team_GetById]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Team_GetById] 
	-- Add the parameters for the stored procedure here
	@ID	int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ID, TeamName, TeamMembers, Password, CreationDate, IsAdmin FROM Team where ID=@ID
END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Team_GetByTeamName]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Team_GetByTeamName] 
	-- Add the parameters for the stored procedure here
	@TeamName	nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ID, TeamName, TeamMembers, Password, CreationDate, IsAdmin FROM Team where Teamname=@TeamName
END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeamAssignment_Insert]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[TeamAssignment_Insert]
	@TeamID	int,	
	@AssignmentID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO TeamAssignment (TeamID, AssignmentID, StartDate)
	Values(@TeamID, @AssignmentID, GetDate())

END
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TeamSubmit_Insert]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[TeamSubmit_Insert] 
	@TeamID			int
	,@AssignmentId	int
	,@FileName		nvarchar(255)
	,@FileStream	varbinary(max)
AS
BEGIN

	SET NOCOUNT ON;

	INSERT INTO TeamSubmit(TeamID, AssignmentID, FileName, FileStream, CreationDate, isFinished) 
	VALUES(@TeamID, @AssignmentID, @FileName, @FileStream, GetDate(), 0)

	declare @newTeamSubmitID int 
	set @newTeamSubmitID = @@identity


	exec TeamSubmitStatus_Insert @TeamID, @newTeamSubmitID, 0, null




END





' 
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TeamSubmit_Assignment]') AND parent_object_id = OBJECT_ID(N'[dbo].[TeamSubmit]'))
ALTER TABLE [dbo].[TeamSubmit]  WITH CHECK ADD  CONSTRAINT [FK_TeamSubmit_Assignment] FOREIGN KEY([AssignmentID])
REFERENCES [dbo].[Assignment] ([ID])
GO
ALTER TABLE [dbo].[TeamSubmit] CHECK CONSTRAINT [FK_TeamSubmit_Assignment]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TeamSubmit_Team]') AND parent_object_id = OBJECT_ID(N'[dbo].[TeamSubmit]'))
ALTER TABLE [dbo].[TeamSubmit]  WITH CHECK ADD  CONSTRAINT [FK_TeamSubmit_Team] FOREIGN KEY([TeamID])
REFERENCES [dbo].[Team] ([ID])
GO
ALTER TABLE [dbo].[TeamSubmit] CHECK CONSTRAINT [FK_TeamSubmit_Team]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TeamSubmit_TeamSubmitStatus]') AND parent_object_id = OBJECT_ID(N'[dbo].[TeamSubmit]'))
ALTER TABLE [dbo].[TeamSubmit]  WITH NOCHECK ADD  CONSTRAINT [FK_TeamSubmit_TeamSubmitStatus] FOREIGN KEY([CurrentStatusID])
REFERENCES [dbo].[TeamSubmitStatus] ([ID])
GO
ALTER TABLE [dbo].[TeamSubmit] NOCHECK CONSTRAINT [FK_TeamSubmit_TeamSubmitStatus]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TournamentAssignment_Assignment]') AND parent_object_id = OBJECT_ID(N'[dbo].[TournamentAssignment]'))
ALTER TABLE [dbo].[TournamentAssignment]  WITH CHECK ADD  CONSTRAINT [FK_TournamentAssignment_Assignment] FOREIGN KEY([AssignmentID])
REFERENCES [dbo].[Assignment] ([ID])
GO
ALTER TABLE [dbo].[TournamentAssignment] CHECK CONSTRAINT [FK_TournamentAssignment_Assignment]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TournamentAssignment_Tournament]') AND parent_object_id = OBJECT_ID(N'[dbo].[TournamentAssignment]'))
ALTER TABLE [dbo].[TournamentAssignment]  WITH CHECK ADD  CONSTRAINT [FK_TournamentAssignment_Tournament] FOREIGN KEY([TournamentID])
REFERENCES [dbo].[Tournament] ([ID])
GO
ALTER TABLE [dbo].[TournamentAssignment] CHECK CONSTRAINT [FK_TournamentAssignment_Tournament]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TeamScore_Assignment]') AND parent_object_id = OBJECT_ID(N'[dbo].[TeamScore]'))
ALTER TABLE [dbo].[TeamScore]  WITH CHECK ADD  CONSTRAINT [FK_TeamScore_Assignment] FOREIGN KEY([AssignmentID])
REFERENCES [dbo].[Assignment] ([ID])
GO
ALTER TABLE [dbo].[TeamScore] CHECK CONSTRAINT [FK_TeamScore_Assignment]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TeamScore_Team]') AND parent_object_id = OBJECT_ID(N'[dbo].[TeamScore]'))
ALTER TABLE [dbo].[TeamScore]  WITH CHECK ADD  CONSTRAINT [FK_TeamScore_Team] FOREIGN KEY([TeamID])
REFERENCES [dbo].[Team] ([ID])
GO
ALTER TABLE [dbo].[TeamScore] CHECK CONSTRAINT [FK_TeamScore_Team]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TeamAssignment_Assignment]') AND parent_object_id = OBJECT_ID(N'[dbo].[TeamAssignment]'))
ALTER TABLE [dbo].[TeamAssignment]  WITH CHECK ADD  CONSTRAINT [FK_TeamAssignment_Assignment] FOREIGN KEY([AssignmentID])
REFERENCES [dbo].[Assignment] ([ID])
GO
ALTER TABLE [dbo].[TeamAssignment] CHECK CONSTRAINT [FK_TeamAssignment_Assignment]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TeamAssignment_Team]') AND parent_object_id = OBJECT_ID(N'[dbo].[TeamAssignment]'))
ALTER TABLE [dbo].[TeamAssignment]  WITH CHECK ADD  CONSTRAINT [FK_TeamAssignment_Team] FOREIGN KEY([TeamID])
REFERENCES [dbo].[Team] ([ID])
GO
ALTER TABLE [dbo].[TeamAssignment] CHECK CONSTRAINT [FK_TeamAssignment_Team]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TeamSubmitStatus_Team]') AND parent_object_id = OBJECT_ID(N'[dbo].[TeamSubmitStatus]'))
ALTER TABLE [dbo].[TeamSubmitStatus]  WITH CHECK ADD  CONSTRAINT [FK_TeamSubmitStatus_Team] FOREIGN KEY([TeamID])
REFERENCES [dbo].[Team] ([ID])
GO
ALTER TABLE [dbo].[TeamSubmitStatus] CHECK CONSTRAINT [FK_TeamSubmitStatus_Team]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TeamSubmitStatus_TeamSubmit]') AND parent_object_id = OBJECT_ID(N'[dbo].[TeamSubmitStatus]'))
ALTER TABLE [dbo].[TeamSubmitStatus]  WITH CHECK ADD  CONSTRAINT [FK_TeamSubmitStatus_TeamSubmit] FOREIGN KEY([TeamSubmitID])
REFERENCES [dbo].[TeamSubmit] ([ID])
GO
ALTER TABLE [dbo].[TeamSubmitStatus] CHECK CONSTRAINT [FK_TeamSubmitStatus_TeamSubmit]

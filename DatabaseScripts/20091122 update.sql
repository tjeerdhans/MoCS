set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[Tournament_Submits_SelectAll]
	@TournamentId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT ts.ID, tta.teamid, ta.assignmentid, tta.tournamentassignmentId, tta.startdate, ts.CreationDate as SubmitDate, ts.IsFinished, tss.StatusCode, tm.TeamName, tm.TeamMembers
FROM TeamSubmit ts 
INNER JOIN (select teamid as t, teamtournamentassignmentid as tid, max(creationdate) as MaxDate from teamsubmit
	group by teamid, teamtournamentassignmentid) as ls ON ts.teamtournamentassignmentid = ls.tid and ts.creationdate = ls.maxdate
INNER JOIN TeamTournamentAssignment tta ON tta.id = ts.teamtournamentassignmentid
INNER JOIN TournamentAssignment ta ON ta.id = tta.tournamentAssignmentID
INNER JOIN TeamSubmitStatus tss ON tss.ID = ts.CurrentStatusID
INNER JOIN Team tm ON ls.t = tm.id
AND ta.tournamentid = @TournamentID
--ALL ASSIGNMENTS THAT WERE STAREN BUT HAVE NO SUBMITS
UNION
SELECT NULL as ID, tta.teamid, ta.assignmentid, tta.tournamentAssignmentID, NULL as StartDate, NULL as SubmitDate, 0 as IsFinished, 0 as SatusCode, tm.TeamName, tm.TeamMembers
 FROM TEAM tm
JOIN teamtournamentassignment tta ON tm.Id = tta.teamid
INNER JOIN tournamentassignment ta ON ta.id = tta.tournamentassignmentid
AND ta.TournamentID = @TournamentID
AND NOT EXISTS (Select * from TeamSubmit ts where ts.TeamId = tm.ID and tta.ID = ts.teamtournamentassignmentid)


END


set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[TeamSubmit_Select]
	@ID	int = NULL
	,@TournamentAssignmentId int = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF(@ID IS NULL)
	BEGIN
		select ts.ID, ts.TeamID, ta.AssignmentID, ta.TournamentID, ts.TeamTournamentAssignmentID, ts.FileName, 
		ts.FileStream, ts.CreationDate as SubmitDate, ts.CurrentStatusID, ts.IsFinished, tss.StatusCode, tss.Details, tss.CreationDate as StatusDate, tta.tournamentassignmentId, tta.StartDate
		 from teamsubmit ts
		INNER JOIN teamsubmitstatus tss on ts.CurrentStatusID = tss.ID
		INNER JOIN teamtournamentassignment tta on tta.ID = ts.TeamTournamentAssignmentID
		INNER JOIN tournamentassignment ta ON tta.tournamentassignmentid = ta.id
		where tta.id = @TournamentAssignmentId
	END
	ELSE
		select ts.ID, ts.TeamID, ta.AssignmentID, ta.TournamentID, ts.TeamTournamentAssignmentID, ts.FileName, 
		ts.FileStream, ts.CreationDate as SubmitDate, ts.CurrentStatusID, ts.IsFinished, tss.StatusCode, tss.Details, tss.CreationDate as StatusDate, tta.tournamentassignmentId, tta.StartDate
		 from teamsubmit ts
		INNER JOIN teamsubmitstatus tss on ts.CurrentStatusID = tss.ID
		INNER JOIN teamtournamentassignment tta on tta.ID = ts.TeamTournamentAssignmentID
		INNER JOIN tournamentassignment ta ON tta.tournamentassignmentid = ta.id
		where ts.ID=@ID


END

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Tournament_Submits_SelectForReport]
	-- Add the parameters for the stored procedure here
	@TournamentId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

select ts.id as Id, ts.TeamID, ts.FileStream, 
ts.IsFinished, tss.StatusCode, tss.Details, tta.StartDate,ts.CreationDate as SubmitDate, ts.FileName, ta.ID as TournamentAssignmentId, tss.CreationDate as StatusDate, ta.AssignmentId, ta.TournamentId, tta.ID as teamtournamentassignmentId
from teamsubmit ts
inner join teamsubmitstatus tss on ts.currentstatusid = tss.id
inner join teamtournamentassignment tta on ts.teamtournamentassignmentid = tta.id
inner join tournamentassignment ta on tta.tournamentassignmentid = ta.id
Where ta.tournamentId = @TournamentId
END








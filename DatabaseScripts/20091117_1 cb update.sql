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

SELECT ts.ID, tta.teamid, ta.assignmentid, tta.startdate, ts.CreationDate as SubmitDate, ts.IsFinished, tss.StatusCode
FROM TeamSubmit ts 
INNER JOIN (select teamid as t, teamtournamentassignmentid as tid, max(creationdate) as MaxDate from teamsubmit
	group by teamid, teamtournamentassignmentid) as ls ON ts.teamtournamentassignmentid = ls.tid and ts.creationdate = ls.maxdate
INNER JOIN TeamTournamentAssignment tta ON tta.id = ts.teamtournamentassignmentid
INNER JOIN TournamentAssignment ta ON ta.id = tta.tournamentAssignmentID
INNER JOIN TeamSubmitStatus tss ON tss.ID = ts.CurrentStatusID
AND ta.tournamentid = @TournamentID
--ALL ASSIGNMENTS THAT WERE STAREN BUT HAVE NO SUBMITS
UNION
SELECT NULL as ID, tta.teamid, ta.assignmentid, NULL as StartDate, NULL as SubmitDate, 0 as IsFinished, 0 as SatusCode
 FROM TEAM tm
JOIN teamtournamentassignment tta ON tm.Id = tta.teamid
INNER JOIN tournamentassignment ta ON ta.id = tta.tournamentassignmentid
AND ta.TournamentID = @TournamentID
AND NOT EXISTS (Select * from TeamSubmit ts where ts.TeamId = tm.ID and tta.ID = ts.teamtournamentassignmentid)


END
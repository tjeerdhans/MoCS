set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Tournament_Submits_SelectAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

Select tm.ID as TeamID, ts.ID as SubmitId, ts.AssignmentID as AssignmentID, tm.TeamName, tm.TeamMembers,  ta.StartDate, x.MaxDate, ts.FileName, ts.FileStream, ts.IsFinished, tss.Details, tss.StatusCode
 FROM TEAM tm
JOIN (select teamid as t, assignmentId as aid, max(creationdate) as MaxDate from teamsubmit
group by teamid, assignmentid) as x ON tm.ID = x.t
JOIN teamassignment ta ON x.aid = ta.assignmentId AND x.t = ta.teamid
Join teamsubmit ts ON ts.creationdate = x.maxdate AND ts.teamid = x.t AND ts.assignmentID = x.aid
JOIN teamsubmitstatus tss ON ts.currentstatusid = tss.id
JOIN TournamentAssignment tas ON tas.AssignmentID = ta.AssignmentID
AND tas.TournamentID = 1
UNION
Select tm.ID as TeamID, NULL as SubmitId, ta.AssignmentID, tm.TeamName, tm.TeamMembers, ta.StartDate, null as MaxDate, NULL as FileName, Null as FileStream, 0 as IsFinished, NULL as Details, NULL as statusCode
 FROM TEAM tm
JOIN teamassignment ta ON tm.Id = ta.teamid
JOIN tournamentassignment tas ON tas.AssignmentID = ta.AssignmentID
AND tas.TournamentID = 1
AND NOT EXISTS (Select * from TeamSubmit ts where ts.TeamId = tm.ID and ta.assignmentID = ts.assignmentid)
order by tm.id


END


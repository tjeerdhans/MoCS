-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Tournament_Submits_SelectAll
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

Select tm.ID as TeamID, ts.ID as SubmitId, ts.AssignmentID as AssignmnetID, tm.TeamName, tm.TeamMembers,  ta.StartDate, x.MaxDate, ts.FileName, ts.FileStream, ts.IsFinished, tss.Details, tss.StatusCode
 FROM TEAM tm
JOIN (select teamid as t, assignmentId as aid, max(creationdate) as MaxDate from teamsubmit
group by teamid, assignmentid) as x ON tm.ID = x.t
JOIN teamassignment ta ON x.aid = ta.assignmentId AND x.t = ta.teamid
Join teamsubmit ts ON ts.creationdate = x.maxdate AND ts.teamid = x.t AND ts.assignmentID = x.aid
JOIN teamsubmitstatus tss ON ts.ID = tss.teamsubmitid
JOIN TournamentAssignment tas ON tas.AssignmentID = ta.AssignmentID
AND tas.TournamentID = 1
order by tm.id


END
GO

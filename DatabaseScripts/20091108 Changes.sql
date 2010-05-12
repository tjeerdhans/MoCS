set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[TournamentAssignments]
@TournamentID int,
@TeamID int
AS
BEGIN

SELECT ta.AssignmentID, ta.Points, a.AssignmentName, ta.Active, tma.StartDate, ts.IsFinished, tss.StatusCode, tsc.Points as PointsWon
FROM Tournament t 
INNER JOIN TournamentAssignment ta ON t.ID = ta.TournamentID
INNER JOIN Assignment a ON ta.AssignmentID = a.ID
LEFT JOIN TeamAssignment tma on tma.AssignmentID = ta.AssignmentID
LEFT JOIN TeamSubmit ts ON ts.teamid = @TeamID AND ts.AssignmentID = ta.AssignmentID
LEFT JOIN TeamSubmitStatus tss ON ts.CurrentStatusID = tss.ID
LEFT JOIN TeamScore tsc ON tsc.teamid = @TeamID AND tsc.AssignmentID = ta.AssignmentID
WHERE t.ID = @TournamentID
ORDER BY ta.AssignmentOrder


END


GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[Team_Delete]
	@ID	int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

DELETE FROM TEAMSUBMITSTATUS WHERE TEAMID = @ID

DELETE FROM TEAMSUBMIT WHERE TEAMID = @ID

DELETE FROM TEAMASSIGNMENT WHERE TEAMID = @ID

DELETE FROM TEAMSCORE WHERE TEAMID = @ID

DELETE FROM TEAM WHERE ID = @ID


    
END

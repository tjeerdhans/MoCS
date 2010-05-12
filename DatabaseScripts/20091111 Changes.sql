set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[Assignment_SelectAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT a.ID as AssignmentID, AssignmentName, ta.Points, ta.Active, ta.id as TournamentAssignmentId
	from Assignment a 
	inner join TournamentAssignment ta ON ta.assignmentid = a.id
	where ta.tournamentid = 1


END



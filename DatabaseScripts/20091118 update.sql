set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[TournamentAssignment_Save]
	@ID int,
	@TournamentID int,
	@AssignmentID int,
	@AssignmentOrder int,
	@Points1 int, 
	@Points2 int, 
	@Points3 int, 
	@Active	bit


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


	IF(@ID=-1)
		BEGIN
			
			INSERT INTO TournamentAssignment (TournamentID, AssignmentID, AssignmentOrder, Points1, Points2, Points3, Active)
			VALUES(@TournamentID, @AssignmentID, @AssignmentOrder, @Points1, @Points2, @Points3, @Active)

			SELECT @@IDENTITY
		END
	ELSE
		UPDATE TournamentAssignment 
		SET TournamentID = @TournamentID
		, AssignmentID = @AssignmentID
		, AssignmentOrder = @AssignmentOrder
		, Points1 = @Points1
		, Points2 = @Points2
		, Points3 = @Points3
		, Active = @Active

		WHERE ID = @ID





END



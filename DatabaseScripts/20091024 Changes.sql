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
CREATE PROCEDURE TeamAssignment_Insert
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
GO

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
CREATE PROCEDURE TeamAssignment_SelectAll 
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
GO


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
CREATE PROCEDURE TeamAssignment_GetByID
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

DELETE FROM TEAM WHERE ID = @ID


    
END

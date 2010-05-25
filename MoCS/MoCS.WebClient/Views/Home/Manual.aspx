<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Manual
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Manual</h2>
     <h1>
                Introduction</h1>
            <p>
                The Masters of C# tournament environment helps you setup tournaments for C# programmers.
                It will help you educate, inspire and challenge developers to become better at their
                work and communicate about the solutions they provide.
            </p>
            <h1>
                Welcome Page</h1>
            <p>
                This is the first page the application shows you.
            </p>
            <img src="<%=Url.Content("~/Content/Images/manual/01_welcome.png")%>" alt="welcome screen"
                width="800px" />
            <h1>
                Log In</h1>
            <p>
                On the login screen you can enter the credentials that were provided to you or your
                team
            </p>
            <img src="<%=Url.Content("~/Content/Images/manual/02_login.png")%>" alt="login screen"
                width="800px" />
            <h1>
                Logged In</h1>
            <p>
                After you entered the correct credentials, the tournament environment is ready for
                you.
            </p>
            <img src="<%=Url.Content("~/Content/Images/manual/03_loggedin.png")%>" alt="logged in"
                width="800px" />
            <h1>
                Select Tournament/Assignment Set</h1>
            <p>
                A tournament or assignment set consists of 1 or more assignments for you to solve.
                In the basic setup, we provided a try-out tournament to test the tournament environment
                and one real tournament.
            </p>
            <img src="<%=Url.Content("~/Content/Images/manual/04_assignmentset.png")%>" alt="select assignment set"
                width="800px" />
            <h1>
                Select Assignment</h1>
            <p>
                After selecting the tournament/assignment set, you can choose the assignment you
                want to solve. It's possible that you cannot start some assignments. They have to
                be made available by the tournament admins.
            </p>
            <img src="<%=Url.Content("~/Content/Images/manual/05_assignment.png")%>" alt="select assignment"
                width="800px" />
            <h1>
                Enrollment</h1>
            <p>
                Before you can start solving an assingment, you have to enroll first. The time you
                enroll will be stored and used to calculate your score. (time of submit of correct
                answer - time of enrollment). After enrollment, the assignment details will be made
                visible to you.
            </p>
            <img src="<%=Url.Content("~/Content/Images/manual/06_assignment_notenrolled.png")%>"
                alt="enrollment" width="800px" />
            <h1>
                Enrolled</h1>
            <p>
                After enrolling, you can see the details of the assignment, but you can also download
                a zip-file containing all files and information you need. An assignment always contains
                a textfile with the assignment an example and some hints, an interface file, some
                unit test files and a file in wich you can start coding. It's recommended that you
                read the instructions well! Some zips may contain project and solution files, some
                others may not.
            </p>
            <img src="<%=Url.Content("~/Content/Images/manual/07_assignment_enrolled.png")%>"
                alt="enrolled" width="800px" />
            <h1>
                Submit</h1>
            <p>
                Also, in the current assingment screen, you can see a submit button. After pressing
                this button, the system will show you a regular file dialog. You can only select
                a single .cs file here! You can only submit a new solution when there are no unprocessed
                submits for you or your team.
            </p>
            <img src="<%=Url.Content("~/Content/Images/manual/08_submit.png") %>" alt="submit"
                width="800px" />
            <h1>
                Submitted</h1>
            <p>
                After submitting the tournament server will check your solution. First it will try
                to compile, check if you implemented the correct interfaces and classes and then
                do some tests. These tests can be different from the ones you were provided with
                in the zip-download!
            </p>
            <img src="<%=Url.Content("~/Content/Images/manual/09_submitted.png")%>" alt="submitted"
                width="800px" />
            <h1>
                Scoreboard</h1>
            <p>
                The overall score for a tournament is displayed on the scoreboard. The team with
                the best overall score is displayed first and the rest is shown is descending order.
                Your status for each assingment in the set is displayed with a different icon.
            </p>
            <img src="<%=Url.Content("~/Content/Images/manual/10_scoreboard.png")%>" alt="scoreboard"
                width="800px" />
</asp:Content>

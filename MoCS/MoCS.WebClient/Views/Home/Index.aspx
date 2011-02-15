<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MoCS.WebClient.Models.HomeModel>" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        <%= Html.Encode(ViewData["Message"]) %></h2>
    <% if (!User.Identity.IsAuthenticated)
       { %><p>
           Please log in using the link in the upper right corner.
       </p>
    <%}
       else
       {
           Response.Write("<p>Hello, " + Html.Encode(User.Identity.Name) + ".</p>");
           if (Model.EnrollmentList.Count > 0)
           {
    %>
    <table style="border: 0px; width: 90%;" cellpadding="0" cellspacing="0">
        <tr style="border: 0px;">
            <td style="border: 0px; vertical-align: top;">
                You have the following active enrollments.<br />
                <table>
                    <thead>
                        <tr>
                            <th>
                                Tournament
                            </th>
                            <th>
                                Assignment
                            </th>
                            <th>
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <% foreach (var enrollment in Model.EnrollmentList)
                           {%>
                        <tr>
                            <td>
                                <%= Html.Encode(enrollment.TournamentName) %>
                            </td>
                            <td>
                                <%= Html.Encode(enrollment.AssignmentName) %>
                            </td>
                            <td>
                                <%= Html.ActionLink("select","SelectEnrollment",new {assignmentEnrollmentId = enrollment.AssignmentEnrollmentId}) %>
                            </td>
                        </tr>
                        <%} %></tbody></table>
            </td>
        </tr>
    </table>
    <%     } %>
    <table style="border: 0px; width: 90%;" cellpadding="0" cellspacing="0">
        <tr style="border: 0px;">
            <td style="border: 0px; vertical-align: top;">
                <%using (Html.BeginForm("UpdateTeamMembers", "Home"))
                  {%>
                <%= Html.EditorFor(m => m.MembersModel)%>
                <input type="submit" value="Update" />
                <%} %>
            </td>
        </tr>
    </table>
    <%
       }
    %>
</asp:Content>

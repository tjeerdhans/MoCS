<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MoCS.WebClient.Models.TournamentAssignmentsModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Assignments
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Assignments</h2>
    Current Assignment Set (tournament): <em>
        <%= Html.Encode(ViewData["tournamentName"]) %></em>.<br />
    <%= Html.ActionLink("Click here to select another set.", "Index") %>
    <br />
    <br />
    <table>
        <tr>
            <th>
                Id
            </th>
            <th>
                Name
            </th>
            <th>
                Difficulty
            </th>
            <th>
                Tagline
            </th>
            <th>
            </th>
        </tr>
        <% foreach (var a in Model)
           { %>
        <tr>
            <td>
                <%= Html.Encode(a.AssignmentId) %>
            </td>
            <td>
                <%= Html.Encode(a.AssignmentName)  %>
            </td>
            <td>
                <%= Html.Encode(a.Difficulty) %>
            </td>
            <td>
                <%= Html.Encode(a.Tagline) %>
            </td>
            <td>
                <%= Html.ActionLink("Select", "Select", new { tournamentAssignmentId = a.Id, assignmentName = a.AssignmentName}) %>
            </td>
        </tr>
        <% } %>
    </table>
</asp:Content>

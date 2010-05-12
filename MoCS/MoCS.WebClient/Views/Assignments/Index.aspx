<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MoCS.WebClient.Models.TournamentsModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Choose the Assignment Set (Tournament)
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h2>
    Choose the Assignment Set (Tournament)</h2>
  <table>
    <tr>
<%--      <th>
        Id
      </th>
--%>      <th>
        Name
      </th>
      <th>
      </th>
    </tr>
    <% foreach (var t in Model)
       { %>
    <tr>
<%--      <td>
        <% //= Html.Encode(t.Id) %>
      </td>
--%>      <td>
        <%= Html.Encode(t.Name) %>
      </td>
      <td>
        <%= Html.ActionLink("Select", "SelectTournament", new { tournamentId = t.Id })%>
      </td>
    </tr>
    <% } %>
  </table>
</asp:Content>

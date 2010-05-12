<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<MoCS.WebClient.Models.TournamentScoreboardModel>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Scoreboard
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Scoreboard</h2>
    <table>
        <tr>
            <th>
            </th>
        </tr>
        <% foreach (var item in Model)
           { %>
        <!-- write the header -->
        <tr>
            <td>
            </td>
            <% foreach (var assignment in item.Assignments)
               {%>
            <td>
                <%=  Html.Encode(assignment.FriendlyName)%>
            </td>
            <% }  %>
        </tr>
        <% foreach (var team in item.Teams)
           {%>
        <tr>
            <td>
                <%=  Html.Encode(team.Name)%><br />
                <%=  Html.Encode(team.Members)%>
            </td>
            <% foreach (var assignment in item.Assignments)
               {%>
            <td>
                <% 
                    MoCS.WebClient.Models.StatusInfo info = item.GetInfo(team, assignment);

                    switch (info.Status)
                    {
                        case MoCS.WebClient.Models.StatusEnum.Finished:
                            switch (info.FinishOrder)
                            {
                                case 1:
                %>
                <img width="40" height="40" src="../../Images/gold_medal.jpg" alt="finished first" />
                <%  
break;

                              case 2:
                %>
                <img width="40" height="40" src="../../Images/silver_medal.png" alt="finished second" />
                <%  
break;

                                                  case 3:
                %>
                <img width="40" height="40" src="../../Images/bronze_medal.png" alt="finished third" />
                <%  
break;

                                                                default:
                %>
                <img width="40" height="40" src="../../Images/finished_noglory.png" alt="no glory" />
                <%  
                    break;
                          }
                          break;
                      case MoCS.WebClient.Models.StatusEnum.Failed:
                %>
                <img width="40" height="40" src="../../Images/failure.jpg" alt="failed" />
                <%                               
break;
                      case MoCS.WebClient.Models.StatusEnum.Started:
                %>
                <img width="40" height="40" src="../../Images/inprogress.png" alt="in progress" />
                <% 
break;
                      default:
break;
                  }
                %>
            </td>
            <% }  %>
            <% }  %>
        </tr>
        <% } %>
    </table>
</asp:Content>

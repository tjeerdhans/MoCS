<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MoCS.WebClient.Models.TournamentScoreboardModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Scoreboard
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Scoreboard</h2>
    <table>
        <thead>
            <tr>
                <th>
                    Teams
                </th>
                <th>
                    Assignments
                </th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td style="width: 250px;">
                </td>
                <td>
                    <table style="border: 1px;">
                        <tr>
                            <% foreach (var assignment in Model.Assignments)
                               {%>
                            <td style="width: 100px; text-align: center; border: 1px;">
                                <%= Html.Encode(assignment.FriendlyName) %>
                            </td>
                            <%
                                } %>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table style="border:0px;">
                        <% foreach (var team in Model.Teams)
                           {%>
                        <tr>
                            <td style="border: 0px;">
                                <div style="height: 70px; width:250px; overflow: auto;">
                                    <strong>
                                        <%= Html.Encode( team.Name) %></strong><br />
                                    Total score time:
                                    <%=Html.Encode(team.Score) %>s<br />
                                    Members: <em>
                                        <%= Html.Encode(team.Members) %></em>
                                </div>
                            </td>
                        </tr>
                        <%} %>
                    </table>
                </td>
                <td>
                    <table style="border:1px;">
                        <%foreach (var team in Model.StatusInfosPerTeam.Keys)
                          {%>
                        <tr>
                            <% foreach (var statusInfo in ((List<MoCS.WebClient.Models.StatusInfo>)(Model.StatusInfosPerTeam[team.ToString()])))
                               {%>
                            <td style="width: 100px; height: 70px; text-align: center; border: 1px;">
                                <%switch (statusInfo.Status)
                                  {
                                      case MoCS.WebClient.Models.StatusEnum.Finished:
                                          switch (statusInfo.FinishOrder)
                                          {
                                              case 1:
                                %><img style="width: 40px; height: 40px;" src="<%=Request.ApplicationPath%>/Content/Images/gold_medal.jpg"
                                    title="Time taken: <%= Html.Encode(statusInfo.SecondsSinceEnrollment) %> seconds"
                                    alt="1st" />
                                <%
                                    break;
                                              case 2:
                                %><img style="width: 40px; height: 40px;" src="<%=Request.ApplicationPath%>/Content/Images/silver_medal.png"
                                    title="Time taken: <%= Html.Encode(statusInfo.SecondsSinceEnrollment) %> seconds"
                                    alt="2nd" />
                                <%
                                    break;
                                              case 3:
                                %><img style="width: 40px; height: 40px;" src="<%=Request.ApplicationPath%>/Content/Images/bronze_medal.png"
                                    title="Time taken: <%= Html.Encode(statusInfo.SecondsSinceEnrollment) %> seconds"
                                    alt="3rd" />
                                <%
                                    break;
                                              default:
                                %><img style="width: 40px; height: 40px;" src="<%=Request.ApplicationPath%>/Content/Images/finished_noglory.png"
                                    title="Time taken: <%= Html.Encode(statusInfo.SecondsSinceEnrollment) %> seconds"
                                    alt="No glory" />
                                <%
                                    break;
                                          }
                                          break;
                                      case MoCS.WebClient.Models.StatusEnum.Started:
                                %><img style="width: 40px; height: 40px;" src="<%=Request.ApplicationPath%>/Content/Images/inprogress.png"
                                    title="Time taken: <%= Html.Encode(statusInfo.SecondsSinceEnrollment) %> seconds"
                                    alt="In progress" />
                                <%
                                    break;
                                      case MoCS.WebClient.Models.StatusEnum.Failed:
                                %><img style="width: 40px; height: 40px;" src="<%=Request.ApplicationPath%>/Content/Images/failure.jpg"
                                    title="Time taken: <%= Html.Encode(statusInfo.SecondsSinceEnrollment) %> seconds"
                                    alt="FAIL" />
                                <%
                                    break;
                                      case MoCS.WebClient.Models.StatusEnum.NotStarted:
                                    break;
                                      default:
                                    break;
                                  } %>
                            </td>
                            <%} %>
                        </tr>
                        <%} %>
                    </table>
                </td>
            </tr>
        </tbody>
    </table>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MoCS.WebClient.Models.CurrentAssignmentModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Current assignment
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Current assignment</h2>
    <table class="currentassignment">
        <tr>
            <td class="assignmentlabel">
                <table class="assignmentlabel">
                    <tr>
                        <td colspan="2">
                            <strong>
                                <%= Model.AssignmentName %></strong> - <em>
                                    <%= Model.AssignmentTagline %></em>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Category:
                            <%= Model.AssignmentCategory %><br />
                            Difficulty:
                            <%= Model.AssignmentDifficulty %>
                        </td>
                        <td>
                            ENROLL BUTTON<br />
                            DOWNLOAD BUTTON
                        </td>
                    </tr>
                </table>
            </td>
            <td rowspan="2">
                <h2>
                    SUBMITS</h2>
                TODO: submitbutton<br />
                <table class="submitlist">
                    <thead>
                        <tr>
                            <td>
                                Time
                            </td>
                            <td>
                                Date
                            </td>
                            <td>
                                Result
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                        </tr>
                    </thead>
                    <tbody>
                        <% foreach (MoCS.WebClient.Models.SubmitModel submitModel in Model.SubmitModelList)
                           {%>
                        <tr>
                            <td>
                                <%= submitModel.SubmitDate.ToString("HH:mm:ss") %>
                            </td>
                            <td>
                                <%= submitModel.SubmitDate.ToShortDateString() %>
                            </td>
                            <td>
                                <%= submitModel.Result %>
                            </td>
                            <td>
                                <% if (!string.IsNullOrEmpty(submitModel.ResultDetailsURL))
                                   {  %>
                                <a href="<%= submitModel.ResultDetailsURL %>" target="_blank" title="View the results of the submit in a new window.">
                                    result details</a>
                                <%} %>
                            </td>
                            <td>
                                <% if (!string.IsNullOrEmpty(submitModel.FileURL))
                                   {  %>
                                <a href="<%= submitModel.FileURL %>" target="_blank" title="View the file you submitted in a new window.">
                                    View submitted file</a>
                                <%} %>
                            </td>
                        </tr>
                        <%} %>
                    </tbody>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <h3>
                    TODO: tabcontrol</h3>
                TABCONTROL WITH FILES
                
            </td>
        </tr>
    </table>
</asp:Content>

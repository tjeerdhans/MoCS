<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MoCS.WebClient.Models.CurrentAssignmentModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Current assignment
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            // Setup the tab control for the assignment files.
            $("#TabControlAssignmentFiles").tabs();

            // Setup the dialogs for the submit details
            $("div[id^='ProcessingDetails']").dialog({
                autoOpen: false,
                title: "Processing details",
                width: 600,
                modal: true,
                position: [350, 200]
            });
            $("div[id^='FileContents']").dialog({
                autoOpen: false,
                title: "Submitted file contents",
                width: 600,
                modal: true,
                position: [350, 200]
            });

        });
    </script>
    <div style="float: right;">
        <h3 style="color: #ff0000;">
            <%= TempData["SubmittedFileText"]%></h3>
    </div>
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
                            <%= Model.AssignmentDifficulty %><br />
                            <% if (Model.HasEnrolled)
                               {%>
                            You enrolled at:
                            <%= Html.Encode(Session["assignmentEnrollmentStartDate"])%>
                            <%  } %>
                        </td>
                        <td>
                            <% using (Html.BeginForm("Enroll", "CurrentAssignment"))
                               {
                            %>
                            <input type="submit" value="Enroll" <%= Model.HasEnrolled ? "disabled" : ""  %> />
                            <% 
                                } %>
                            <% using (Html.BeginForm("Download", "CurrentAssignment"))
                               { %>
                            <input type="submit" value="Download" <%= Model.HasEnrolled ? "" : "disabled"  %> />
                            <%} %>
                        </td>
                    </tr>
                </table>
            </td>
            <td rowspan="2">
                <div style="float: right;">
                    <% using (Html.BeginForm("UploadSubmit", "CurrentAssignment", FormMethod.Post, new { enctype = "multipart/form-data" }))
                       {%>
                    <%= Html.TextBox("SubmitFileName", "", new { type = "file", accept="text/plain",style="border: 1px solid gray;" })%><input
                        type="submit" id="ButtonUploadSubmit" value="Submit" <%= Model.HasEnrolled ? "" : "disabled"  %> />
                    <%} %></div>
                <h2>
                    Submits</h2>
                <table class="submitlist">
                    <thead>
                        <tr>
                            <th>
                                Submit time
                            </th>
                            <th>
                                Result
                            </th>
                            <th>
                                Time (m:s)
                            </th>
                            <th>
                            </th>
                            <th>
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <%if (Model.SubmitModelList.Count == 0)
                          {
                        %>
                        <tr>
                            <td colspan="5">
                                You haven't submitted anything yet.
                            </td>
                        </tr>
                        <%
                            }
                          else
                          {
                              foreach (MoCS.WebClient.Models.SubmitModel submitModel in Model.SubmitModelList)
                              {%>
                        <tr>
                            <td>
                                <%= submitModel.SubmitDate.ToString("dd-MM-yyyy HH:mm:ss") %>
                            </td>
                            <td>
                                <%= submitModel.Result %>
                            </td>
                            <td>
                                <%= submitModel.TimeTaken %>
                            </td>
                            <td>
                                <span id="openerProcessingDetails<%= Html.Encode(submitModel.Id) %>" class="ui-icon ui-icon-zoomin"
                                    title="View the results of the submit." onclick="$(ProcessingDetails<%= Html.Encode(submitModel.Id) %>).dialog('open');">
                                </span>
                                <div id="ProcessingDetails<%= Html.Encode(submitModel.Id) %>">
                                    <pre><%= Html.Encode(submitModel.ProcessingDetails) %></pre>
                                </div>
                            </td>
                            <td>
                                <span id="openerFileContents<%= Html.Encode(submitModel.Id) %>" class="ui-icon ui-icon-document"
                                    title="View the file you submitted." onclick="$(FileContents<%= Html.Encode(submitModel.Id) %>).dialog('open');">
                                </span>
                                <div id="FileContents<%= Html.Encode(submitModel.Id) %>">
                                    <pre><%= Html.Encode(submitModel.FileContents) %></pre>
                                </div>
                            </td>
                        </tr>
                        <%}
                          } %>
                    </tbody>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <div id="TabControlAssignmentFiles">
                    <% if (Model.HasEnrolled)
                       { %>
                    <ul>
                        <%
                            foreach (MoCS.WebClient.Models.TabContentModel tcm in Model.TabContentModelList)
                            { %>
                        <li><a href="#tab-<%= Html.Encode(tcm.Name) %>"><span>
                            <%= tcm.Name %></span></a></li>
                        <% }
                        %>
                    </ul>
                    <%
                        foreach (MoCS.WebClient.Models.TabContentModel tcm in Model.TabContentModelList)
                        {%>
                    <div id="tab-<%= Html.Encode(tcm.Name) %>" style="height: 300px; overflow: scroll;">
                        <%= tcm.Content %>
                    </div>
                    <%}
                       } %>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>

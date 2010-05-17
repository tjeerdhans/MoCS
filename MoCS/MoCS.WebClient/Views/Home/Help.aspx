<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Help
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            // Setup the tab control for the assignment files.
            $("#TabControlHelp").tabs();
        });
    </script>
    <h2>
        Help</h2>
    <div id="TabControlHelp">
        <ul>
            <li><a href="#tab-1"><span>MoCS Notification</span></a></li>
            <li><a href="#tab-2"><span>Getting started</span></a></li>
            <li><a href="#tab-3"><span>Network Configuration</span></a></li>
            <li><a href="#tab-4"><span>Supported Messages</span></a></li>
            <li><a href="#tab-5"><span>ContextMenu</span></a></li>
        </ul>
        <div id="tab-1" style="height: 300px; overflow: scroll;">
            <p>
                The MoCS Notification Client is used for receiving visual and sound notifications
                from the MoCS WebApplication and Build Server.<br />
                Notifications are send when a solution is submitted or being compiled or when the
                unit tests are executing.<br />
                A connection is established to the Notification Server using the Multicast or the
                TCP/IP protocol.
            </p>
            <p>
                <i><b>Note: The font-size can easily be adjusted by pressing the Ctrl button and scrolling
                    the Mousewheel.</b></i></p>
            <img src="<%=Request.ApplicationPath%>/Content/Images/MocsNotification.png" alt="MoCS Notification" />
        </div>
        <div id="tab-2" style="height: 300px; overflow: scroll;">           
            <img src="<%=Request.ApplicationPath%>/Content/Images/MocsNotificationTeamId.png"
                alt="MoCS Notification TeamId" />
            <p>
                You must enter a Team Name when the MoCS Notification Client is started for the
                first time.<br /> This Team Name will be given to you by the organizers of the contest.<br />
                This name will be stored in the application configuration file.
            </p>
        </div>
        <div id="tab-3" style="height: 300px; overflow: scroll;">           
            <img src="<%=Request.ApplicationPath%>/Content/Images/MocsConfiguration.png" alt="MoCS Notification Configuration" />
            <p>
                During startup the MoCS Notification Client tries to establish a connection to the
                following Multicast Address 224.0.0.40:7601.<br />
                When the application fails to establish a Multicast connection, it is possible to
                configure the application to use regular TCP/IP sockets.<br />
                Uncheck "Use Multicast" and specify IP Address and Port number. These will be provided
                by the organizers of the contest.
            </p>
        </div>
        <div id="tab-4" style="height: 300px; overflow: scroll;">          
            <p>
                The following messages can be send by the MoCS Webapplication or Build Server.</p>
            <table style="width:80%;">
                <thead>
                    <tr style="text-align: left; vertical-align: text-top">
                        <th>
                            MessageType
                        </th>
                        <th>
                            Category
                        </th>
                        <th>
                            Description
                        </th>
                        <th>
                            Is sent to all?
                        </th>
                        <th>
                            Color
                        </th>
                    </tr>
                </thead>
                <tr style="vertical-align: text-top">
                    <td>
                        Info
                    </td>
                    <td>
                        Submitted
                    </td>
                    <td>
                        This message will be send when a solution is submitted.
                    </td>
                    <td>
                        Everyone
                    </td>
                    <td style="background-color: LightBlue">
                        LightBlue
                    </td>
                </tr>
                <tr style="vertical-align: text-top">
                    <td>
                        Info
                    </td>
                    <td>
                        Processing
                    </td>
                    <td>
                        This message will be send when a solution is being compiled and the unit tests are
                        executing.
                    </td>
                    <td>
                        Team only
                    </td>
                    <td style="background-color: LightBlue">
                        LightBlue
                    </td>
                </tr>
                <tr style="vertical-align: text-top">
                    <td>
                        Info
                    </td>
                    <td>
                        Success
                    </td>
                    <td>
                        This message will be send when a solution is compiled successfully and all unit
                        tests are passed.
                    </td>
                    <td>
                        Everyone
                    </td>
                    <td style="background-color: LightGreen">
                        LightGreen
                    </td>
                </tr>
                <tr style="vertical-align: text-top">
                    <td>
                        Info
                    </td>
                    <td>
                        FirstPlace
                    </td>
                    <td>
                        This message will be send when a team is holding the first place for a given assignment.
                    </td>
                    <td>
                        Everyone
                    </td>
                    <td style="background-color: LightGreen">
                        LightGreen
                    </td>
                </tr>
                <tr style="vertical-align: text-top">
                    <td>
                        Error
                    </td>
                    <td>
                        ErrorCompilation<br />
                        ErrorTesting
                        <br />
                        ErrorValidation<br />
                        ErrorServer
                        <br />
                        ErrorUnknown<br />
                    </td>
                    <td>
                        This message will be send when a solution cannot be compiled or errors occured during
                        the execution of the unit tests.
                    </td>
                    <td>
                        Team only
                    </td>
                    <td style="background-color: Red">
                        Red
                    </td>
                </tr>
                <tr style="vertical-align: text-top">
                    <td>
                        Chat
                    </td>
                    <td>
                    </td>
                    <td>
                        This message will be send when another team sends a personal message to your team
                        or to everyone.
                    </td>
                    <td>
                        Everyone / Team only
                    </td>
                    <td style="background-color: PaleVioletRed">
                        PaleVioletRed
                    </td>
                </tr>
            </table>
        </div>
        <div id="tab-5" style="height: 300px; overflow: scroll;">          
            <p>
                The ContextMenu is shown when a user right clicks the MoCS Notification window or
                the Systray icon.</p>
            <img src="<%=Request.ApplicationPath%>/Content/Images/MocsNotificationMenu.png" alt="MoCS Notification Menu" />
            <table>
                <thead>
                    <tr style="text-align: left; vertical-align: text-top">
                        <th>
                            Menu Item
                        </th>
                        <th>
                            Description
                        </th>
                    </tr>
                </thead>
                <tr style="vertical-align: text-top">
                    <td>
                        Send a Message
                    </td>
                    <td>
                        Type a message which you would like to send to another team or to everyone.<br />
                        Leave "Team Id" blank to send it to everyone.<br />
                        <img src="<%=Request.ApplicationPath%>/Content/Images/MocsNotificationChat.png" alt="Send a Message" />
                    </td>
                </tr>
                <tr style="vertical-align: text-top">
                    <td>
                        Clear all Messages
                    </td>
                    <td>
                        All messages in the Grid View will be removed.
                    </td>
                </tr>
                <tr style="vertical-align: text-top">
                    <td>
                        Edit Team Name
                    </td>
                    <td>
                        Enables you to change your team name. See <b>Getting Started</b>.
                    </td>
                </tr>
                <tr style="vertical-align: text-top">
                    <td>
                        Edit Configuration
                    </td>
                    <td>
                        Enables you to change your network configuration. See <b>Network Configuration</b>.
                    </td>
                </tr>
                <tr style="vertical-align: text-top">
                    <td>
                        Block Personal Messages.
                    </td>
                    <td>
                        When checked all personal/chat messages will be ignored.
                    </td>
                </tr>
                <tr style="vertical-align: text-top">
                    <td>
                        Always on Top
                    </td>
                    <td>
                        When checked all the MoCS Notification windows will be positioned on top of all
                        other windows.
                    </td>
                </tr>
                <tr style="vertical-align: text-top">
                    <td>
                        Close Application
                    </td>
                    <td>
                        This will exit the MoCS Notification Client.
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>

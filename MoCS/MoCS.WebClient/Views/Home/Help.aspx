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
            <li><a href="#tab-1"><span>Tab 1</span></a></li>
            <li><a href="#tab-2"><span>Tab 2</span></a></li>
            <li><a href="#tab-3"><span>Tab 3</span></a></li>
            <li><a href="#tab-4"><span>Tab 4</span></a></li>
        </ul>
        <div id="tab-1" style="height: 300px; overflow: scroll;">
        <img src="<%=Request.ApplicationPath%>/Content/Images/MoCS_logo.png" alt="MoCS-logo" style="float:left;" />
        </div>
        <div id="tab-2" style="height: 300px; overflow: scroll;">2
        </div>
        <div id="tab-3" style="height: 300px; overflow: scroll;">3
        </div>
        <div id="tab-4" style="height: 300px; overflow: scroll;">4
        </div>
    </div>
</asp:Content>

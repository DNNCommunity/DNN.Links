<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Links.Settings" Codebehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<table cellspacing="0" cellpadding="3" border="0" summary="Edit Links Design Table">
    <tr>
        <td class="SubHead" width="150">
            <dnn:Label ID="plLinkModuleType" runat="server" ControlName="optModuleType" Suffix=":">
            </dnn:Label>
        </td>
        <td valign="bottom">
            <asp:DropDownList ID="optLinkModuleType" runat="server" CssClass="Normal" Width="100"
                AutoPostBack="true" OnSelectedIndexChanged="optLinkModuleType_SelectedIndexChanged">
                <asp:ListItem resourcekey="Link" Value="1">Link</asp:ListItem>
                <asp:ListItem resourcekey="Menu" Value="2">Menu</asp:ListItem>
                <asp:ListItem resourcekey="Folder" Value="3">Folder</asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="optTypeContentSelection" runat="server" CssClass="Normal" Width="200"
                Visible="false">
            </asp:DropDownList>
        </td>
    </tr>
    <!-- 2014 TODO: Menu -->
    <tr>
        <td class="SubHead" width="150">
            <dnn:Label ID="plMenuAllUsers" runat="server" Suffix=":" ControlName="optMenuAllUsers"/>
        </td>
        <td>
            <asp:RadioButtonList ID="optMenuAllUsers" runat="server" CssClass="NormalTextBox" RepeatDirection="Horizontal">
                 <asp:ListItem resourcekey="No" Value="No">No</asp:ListItem>
                <asp:ListItem resourcekey="Yes" Value="Yes">Yes</asp:ListItem>
            </asp:RadioButtonList>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150">
            <dnn:Label ID="plDisplayAttribute" runat="server" ControlName="optDisplayAttribute"
                Suffix=":" />
        </td>
        <td valign="bottom">
            <asp:DropDownList runat="server" ID="optDisplayAttribute" CssClass="Normal" Width="100">
                <asp:ListItem Value="1">Username</asp:ListItem>
                <asp:ListItem Value="2">DisplayName</asp:ListItem>
                <asp:ListItem Value="3">FirstName</asp:ListItem>
                <asp:ListItem Value="4">LastName</asp:ListItem>
                <asp:ListItem Value="5">FullName</asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList runat="server" ID="optDisplayOrder" CssClass="Normal" Width="100">
                <asp:ListItem Value="1">Asc</asp:ListItem>
                <asp:ListItem Value="2">Desc</asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150">
            <dnn:Label ID="plControl" runat="server" ControlName="optControl" Suffix=":"></dnn:Label>
        </td>
        <td valign="bottom">
            <asp:RadioButtonList ID="optControl" runat="server" CssClass="NormalTextBox" AutoPostBack="true"
                RepeatDirection="Horizontal" OnSelectedIndexChanged="optControl_SelectedIndexChanged" >
                <asp:ListItem resourcekey="List" Value="L">List</asp:ListItem>
                <asp:ListItem resourcekey="Dropdown" Value="D">Dropdown</asp:ListItem>
            </asp:RadioButtonList>
        </td>
    </tr>
    <tr runat="server" id="trOptView">
        <td class="SubHead" width="150">
            <dnn:Label ID="ploptView" runat="server" ControlName="optView" Suffix=":"></dnn:Label>
        </td>
        <td valign="bottom">
            <asp:RadioButtonList ID="optView" runat="server" CssClass="NormalTextBox" RepeatDirection="Horizontal">
                <asp:ListItem resourcekey="Vertical" Value="V">Vertical</asp:ListItem>
                <asp:ListItem resourcekey="Horizontal" Value="H">Horizontal</asp:ListItem>
            </asp:RadioButtonList>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150">
            <dnn:Label ID="plInfo" runat="server" ControlName="optInfo" Suffix=":"></dnn:Label>
        </td>
        <td valign="bottom">
            <asp:RadioButtonList ID="optInfo" runat="server" CssClass="NormalTextBox" RepeatDirection="Horizontal">
                <asp:ListItem resourcekey="None" Value="N">None</asp:ListItem>
                <asp:ListItem resourcekey="plUseEllipsis" Value="Y">Ellipsis</asp:ListItem>
                <asp:ListItem resourcekey="plUseTooltip" Value="JQ">Tooltip</asp:ListItem>
            </asp:RadioButtonList>
        </td>
    </tr>
    <tr runat="server" id="pnlWrap">
        <td class="SubHead" width="150">
            <dnn:Label ID="plNoWrap" runat="server" ControlName="optNoWrap" Suffix=":"></dnn:Label>
        </td>
        <td valign="bottom">
            <asp:RadioButtonList ID="optNoWrap" runat="server" CssClass="NormalTextBox" RepeatDirection="Horizontal">
                <asp:ListItem resourcekey="NoWrap" Value="NW">No Wrap</asp:ListItem>
                <asp:ListItem resourcekey="Wrap" Value="W">Wrap</asp:ListItem>
            </asp:RadioButtonList>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150">
            <dnn:Label ID="plUsePermissions" runat="server" ControlName="optUsePermissions" Suffix=":">
            </dnn:Label>
        </td>
        <td valign="bottom">
            <asp:RadioButtonList ID="optUsePermissions" runat="server" CssClass="NormalTextBox"
                RepeatDirection="Horizontal">
                <asp:ListItem resourcekey="notUsePermissions" Value="False">No</asp:ListItem>
                <asp:ListItem resourcekey="usePermissions" Value="True">Yes</asp:ListItem>
            </asp:RadioButtonList>
        </td>
    </tr>
    <tr id="pnlIcon" runat="server">
        <td class="SubHead" width="150">
            <dnn:Label ID="plIcon" runat="server" ControlName="ctlIcon" Suffix=":"></dnn:Label>
        </td>
        <td width="365">
            <Portal:URL ID="ctlIcon" runat="server" Width="250" ShowUrls="False" ShowTabs="False"
                ShowLog="False" ShowTrack="False" Required="False" />
        </td>
    </tr>
</table>

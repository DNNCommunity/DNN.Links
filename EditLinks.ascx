<%--[alex - 09/01/2009] changed CodeBehind to CodeFile (required for WPA projects)--%>
<%@ Control Language="C#" AutoEventWireup="false" Explicit="True"
    Inherits="DotNetNuke.Modules.Links.EditLinks" Codebehind="EditLinks.ascx.cs" %>
<%@ Register Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" TagPrefix="DNNWC" %>
<%@ Register TagPrefix="Portal" TagName="Tracking" Src="~/controls/URLTrackingControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<table width="100%" cellspacing="0" cellpadding="0" border="0" summary="Edit Links Design Table">
    <tr>
        <td colspan="2">
            <asp:Panel ID="pnlDynamicContent" runat="server">
                <asp:Label ID="plDynamicContent" runat="server" ResouceKey="plDynamicContent" Text="This Module is configured to show dynamic Data. Please use Modulesettings to change." />
            </asp:Panel>
            <br/>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="160">
            <dnn:Label ID="plTitle" runat="server" ControlName="txtTitle" Suffix=":"></dnn:Label>
        </td>
        <td width="365">
            <asp:TextBox ID="txtTitle" CssClass="NormalTextBox" Width="300" Columns="30" MaxLength="100"
                runat="server" />
            <br>
            <asp:RequiredFieldValidator ID="valTitle" resourcekey="valTitle.ErrorMessage" Enabled="true"
                Display="Dynamic" ErrorMessage="You Must Enter a Title For The Link" ControlToValidate="txtTitle"
                runat="server" CssClass="NormalRed" EnableClientScript="false" />
        </td>
        <br />
    </tr>
    <tr>
        <td class="SubHead" width="160">
            <dnn:Label ID="plURL" runat="server" ControlName="ctlURL" Suffix=":"></dnn:Label>
        </td>
        <td width="650">
            <Portal:URL ID="ctlURL" runat="server" Width="650" ShowNewWindow="True" ShowUsers="True" />
        </td>
    </tr>
</table>
<table id="tblGetContent" width="560" cellspacing="0" cellpadding="0" enableviewstate="false"
    visible="true" runat="server">
    <tr>
        <td class="SubHead" width="160">
            <dnn:Label ID="plGetContent" runat="server" ControlName="txtGetContent" Suffix=":"></dnn:Label>
        </td>
        <td width="365">
            <asp:LinkButton ID="lbtGetContent" runat="server" CssClass="SubHead" Visible="true"
                ResourceKey="lbtGetContent" OnClick="lbtGetContent_Click" />
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="160">&nbsp;
        </td>
        <td width="365">
            <asp:Label ID="lblContentRefresh" runat="server" CssClass="Normal" Text="automatically content refresh"></asp:Label>
            <asp:DropDownList ID="ddlGetContentInterval" runat="server" CssClass="Normal" Visible="true">
                <asp:ListItem Text="never" resourcekey="time_00.Text" Value="0">                   
                </asp:ListItem>
                <asp:ListItem Text="15 min" resourcekey="time_01.Text" Value="15">                   
                </asp:ListItem>
                <asp:ListItem Text="30 min" resourcekey="time_02.Text" Value="30">                
                </asp:ListItem>
                <asp:ListItem Text="60 min" resourcekey="time_03.Text" Value="60">                   
                </asp:ListItem>
                <asp:ListItem Text="1 day" resourcekey="time_04.Text" Value="1440">                
                </asp:ListItem>
                <asp:ListItem Text="1 week" resourcekey="time_05.Text" Value="10080">                  
                </asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr height="10">
        <td colspan="2"></td>
    </tr>
    <tr>
        <td class="SubHead" width="160">&nbsp;
        </td>
        <td width="365">
            <asp:Label ID="lblGetContentResult" runat="server" EnableViewState="false" />
        </td>
    </tr>
</table>
<table id="tblGrantRoles" width="560" cellspacing="0" cellpadding="0" visible="true">
    <tr>
        <td class="SubHead" width="160">
            <dnn:Label runat="server" ID="plGrantRoles" ControlName="txtGrantRoles" ResourceKey="lbtGrantRoles"
                Suffix=":"></dnn:Label>
        </td>
        <td width="365">
            <asp:CheckBoxList ID="cblGrantRoles" runat="server">
            </asp:CheckBoxList>
        </td>
    </tr>
</table>
<table width="560" cellspacing="0" cellpadding="0">
    <tr height="10">
        <td colspan="2"></td>
    </tr>
    <tr>
        <td class="SubHead" width="160">
            <dnn:Label ID="plDescription" runat="server" ControlName="txtDescription" Suffix=":"></dnn:Label>
        </td>
        <td width="365">
            <asp:TextBox ID="txtDescription" CssClass="NormalTextBox" Width="300" Columns="30"
                Rows="5" TextMode="MultiLine" MaxLength="2000" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="160">
            <dnn:Label ID="plViewOrder" runat="server" ControlName="txtViewOrder" Suffix=":"></dnn:Label>
        </td>
        <td width="365">
            <%--<asp:TextBox ID="txtViewOrder" CssClass="NormalTextBox" Width="300" Columns="30"
                MaxLength="3" runat="server" />--%>
            <asp:DropDownList ID="ddlViewOrder" runat="server" CssClass="Normal" Width="100">
                <asp:ListItem Text="Before" Value="B"></asp:ListItem>
                <asp:ListItem Text="After" Value="A"></asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="ddlViewOrderLinks" runat="server" CssClass="Normal" Width="200">
            </asp:DropDownList>
            <br />
            <%-- <asp:CompareValidator ID="valViewOrder" resourcekey="valViewOrder.ErrorMessage" runat="server"
                Display="Dynamic" ControlToValidate="txtViewOrder" CssClass="NormalRed" ErrorMessage="View Order must be a Number or an Empty String"
                Type="Integer" Operator="DataTypeCheck"></asp:CompareValidator>--%>
        </td>
    </tr>
</table>
<p>
    <asp:LinkButton ID="cmdUpdate" runat="server" OnClick="cmdUpdate_Click" >
        <asp:Image runat="server" ImageUrl="~/images/save.gif" />
        <asp:Label runat="server" ResourceKey="cmdUpdate" />
    </asp:LinkButton>    
    &nbsp;    
    <asp:LinkButton ID="cmdCancel" runat="server" OnClick="cmdCancel_Click" CausesValidation="false">
        <asp:Image runat="server" ImageUrl="~/images/action_export.gif" />
        <asp:Label runat="server" ResourceKey="cmdCancel" />
    </asp:LinkButton>
    &nbsp;
    <asp:LinkButton ID="cmdDelete" runat="server" OnClick="cmdDelete_Click" CausesValidation="false">
        <asp:Image runat="server" ImageUrl="~/images/delete.gif" />
        <asp:Label runat="server" ResourceKey="cmdDelete" />
    </asp:LinkButton>
</p>
<Portal:Audit ID="ctlAudit" runat="server" />
<br>
<br>
<Portal:Tracking ID="ctlTracking" runat="server" />

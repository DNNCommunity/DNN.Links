<%@ Control Language="C#" Inherits="DotNetNuke.Modules.Links.Links"
    AutoEventWireup="true" Explicit="True" Codebehind="Links.ascx.cs" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<asp:Panel ID="pnlList" runat="server" CssClass="link_module" >
    <asp:Repeater ID="lstLinks" runat="server" OnItemDataBound="lstLinks_ItemDataBound">
        <HeaderTemplate>
            <ul id="ulHeader" class="linklist <%# Horizontal %>" >
        </HeaderTemplate>
        <ItemTemplate>
            <li id="itemLi" class="linkitem <%# Horizontal %>" <%# NoWrap %>>
                <asp:HyperLink ID="editLink" Visible="<%# IsEditable %>" runat="server">
                    <asp:Image ID="editLinkImage" ImageUrl="~/images/edit.gif" AlternateText="Edit" Visible="<%# IsEditable %>"
                        runat="server" />
                </asp:HyperLink>
                <asp:Image ID="Image1" ImageUrl='<%# Eval("ImageURL") %>' Visible="<%# DisplayIcon %>"
                    runat="server" resourcekey="imgLinkIcon.Text" />
                <a runat="server" id="linkHyp" href="#"
                    class="Normal<%#PopupTrigger%>" alt='<%# DisplayToolTip(Eval("Description").ToString()) %>' >
                    <%#Eval("Title")%>
                </a><span id="spnSelect" runat="server" >
                    &nbsp;
                    <asp:Label ID="lblMoreInfo" resourcekey="MoreInfo.Text" runat="server" Text="Label" Visible="false" >...</asp:Label>
                </span>
                <asp:Panel ID="pnlDescription" CssClass="item_desc" Style="display: none" runat="server">
                    <asp:Label runat="server" CssClass="Normal" ID="lbldescrdiv" />
                </asp:Panel>
                <telerik:RadToolTip ID="radToolTip" runat="server" TargetControlID="linkHyp" RelativeTo="Element"
                    Position="BottomCenter" RenderInPageRoot="true" EnableShadow="true" Animation="Slide"
                    AnimationDuration="150" ShowDelay="200" AutoCloseDelay="0" Skin="Telerik" Width="300">
                    <%# HtmlDecode(Eval("Description").ToString()) %>
                </telerik:RadToolTip>
            </li>
        </ItemTemplate>
        <FooterTemplate>
            <div style="clear: both;" />
            </ul>
        </FooterTemplate>
    </asp:Repeater>
</asp:Panel>
<asp:Panel ID="pnlDropdown" runat="server">
    <asp:ImageButton ID="cmdEdit" runat="server" ImageUrl="~/images/edit.gif" AlternateText="Edit"
        resourcekey="Edit" OnClick="cmdEdit_Click" ></asp:ImageButton>
    <label style="display: none" for="<%=cboLinks.ClientID%>">
        Link</label>
    <asp:DropDownList ID="cboLinks" runat="server" DataTextField="Title" DataValueField="ItemID"
        CssClass="NormalTextBox">
    </asp:DropDownList>
    &nbsp;
    <asp:LinkButton ID="cmdGo" runat="server" CssClass="CommandButton" resourcekey="cmdGo" OnClick="cmdGo_Click" ></asp:LinkButton>&nbsp;
    <asp:LinkButton ID="cmdInfo" runat="server" CssClass="CommandButton" Text="..." OnClick="cmdInfo_Click" ></asp:LinkButton>
    <asp:Label ID="lblDescription" runat="server" CssClass="Normal"></asp:Label>
</asp:Panel>
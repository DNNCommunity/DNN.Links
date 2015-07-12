<%@ Control Language="vb" Inherits="DotNetNuke.Modules.Links.Links" CodeFile="Links.ascx.vb"
    AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<asp:Panel ID="pnlList" runat="server" CssClass="link_module">
    <asp:Repeater ID="lstLinks" runat="server">
        <HeaderTemplate>
            <ul class="linklist <%# Horizontal %>">
        </HeaderTemplate>
        <ItemTemplate>
            <li class="linkitem <%# Horizontal %>" <%# NoWrap %>>
                <asp:HyperLink ID="editLink" NavigateUrl='<%# EditURL("ItemID",Eval("ItemID")) %>'
                    Visible="<%# IsEditable %>" runat="server">
                    <asp:Image ID="editLinkImage" ImageUrl="~/images/edit.gif" AlternateText="Edit" Visible="<%# IsEditable %>"
                        runat="server" />
                </asp:HyperLink>
                <asp:Image ID="Image1" ImageUrl='<%# Eval("ImageURL") %>' Visible="<%# DisplayIcon() %>"
                    runat="server" resourcekey="imgLinkIcon.Text" />
                <a runat="server" id="linkHyp" href='<%# FormatURL(Eval("Url"),Eval("TrackClicks")) %>'
                    class="Normal<%#PopupTrigger%>" alt='<%# DisplayToolTip(Eval("Description")) %>'
                    target='<%# IIF(Eval("NewWindow"),"_blank","_self") %>'>
                    <%#Eval("Title")%>
                </a><span id="spnSelect" runat="server" visible='<%# DisplayInfo(Eval("Description")) %>'>
                    &nbsp;
                    <asp:Label ID="lblMoreInfo" resourcekey="MoreInfo.Text" runat="server" Text="Label"
                        Visible="<%#Not CBool(ShowPopup) %>">...</asp:Label>
                </span>
                <asp:Panel ID="pnlDescription" class="item_desc" Style="display: none" runat="server">
                    <asp:Label runat="server" CssClass="Normal" Text='<%# HtmlDecode(Eval("Description")) %>'
                        ID="lbldescrdiv" />
                </asp:Panel>
                <telerik:RadToolTip ID="radToolTip" runat="server" TargetControlID="linkHyp" RelativeTo="Element"
                    Position="BottomCenter" RenderInPageRoot="true" EnableShadow="true" Animation="Slide"
                    AnimationDuration="150" ShowDelay="200" AutoCloseDelay="0" Skin="Telerik" Width="300"
                    Visible='<%#CBool(ShowPopup) AndAlso (Cstr(Eval("Description")) <> "") %>'>
                    <%# HtmlDecode(Eval("Description")) %>
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
        resourcekey="Edit"></asp:ImageButton>
    <label style="display: none" for="<%=cboLinks.ClientID%>">
        Link</label>
    <asp:DropDownList ID="cboLinks" runat="server" DataTextField="Title" DataValueField="ItemID"
        CssClass="NormalTextBox">
    </asp:DropDownList>
    &nbsp;
    <asp:LinkButton ID="cmdGo" runat="server" CssClass="CommandButton" resourcekey="cmdGo"></asp:LinkButton>&nbsp;
    <asp:LinkButton ID="cmdInfo" runat="server" CssClass="CommandButton" Text="..."></asp:LinkButton>
    <asp:Label ID="lblDescription" runat="server" CssClass="Normal"></asp:Label>
</asp:Panel>
<asp:Panel runat="server" ID="pnlFriends" Visible="false">
    <telerik:RadListView runat="server" ID="lvFriends" ItemPlaceholderID="lvFriendsContainer"
        PageSize="6">
        <LayoutTemplate>
            <fieldset style="border: 1px solid #cccccc;">
                <legend>
                    <h3>
                        <span>My Friends</span>
                    </h3>
                </legend>
                <asp:PlaceHolder runat="server" ID="lvFriendsContainer" />
                <div style="clear: both">
                </div>
                <div style="padding: 5px;">
                    <div style="float: left; margin-left: 15%;">
                        <asp:Button runat="server" ID="btnFirst" CommandName="Page" CommandArgument="First"
                            Text="First" Enabled="<%#Container.CurrentPageIndex > 0 %>" />
                        <asp:Button runat="server" ID="btnPrev" CommandName="Page" CommandArgument="Prev"
                            Text="Prev" Enabled="<%#Container.CurrentPageIndex > 0 %>" />
                        <span style="vertical-align: middle;">Page <strong>
                            <%#Container.CurrentPageIndex + 1 %></strong> of <strong>
                                <%#Container.PageCount %></strong></span>
                        <asp:Button runat="server" ID="btnNext" CommandName="Page" CommandArgument="Next"
                            Text="Next" Enabled="<%#Container.CurrentPageIndex + 1 < Container.PageCount %>" />
                        <asp:Button runat="server" ID="btnLast" CommandName="Page" CommandArgument="Last"
                            Text="Last" Enabled="<%#Container.CurrentPageIndex + 1 < Container.PageCount %>" />
                    </div>
                    <div>
                        <span style="vertical-align: middle; font-weight: bold; padding-left: 5px;">Page Size:</span>
                        <telerik:RadComboBox runat="server" ID="cmbPageSize" OnSelectedIndexChanged="cmbPageSize_SelectedIndexChanged"
                            Width="40px" SelectedValue='<%#Container.PageSize %>' AutoPostBack="true">
                            <Items>
                                <telerik:RadComboBoxItem Text="6" Value="6" />
                                <telerik:RadComboBoxItem Text="9" Value="9" />
                                <telerik:RadComboBoxItem Text="12" Value="12" />
                            </Items>
                        </telerik:RadComboBox>
                    </div>
                </div>
            </fieldset>
        </LayoutTemplate>
        <ItemTemplate>
            <div style="float: left; margin: 5px 10px 5px 10px">
                <fieldset style="width: 300px; height: 100px; border: none;">
                    <table border="0" cellpadding="0" cellspacing="5" width="350px">
                        <tr>
                            <td>
                                <asp:Image ID="Image2" ImageUrl='<%# Eval("PhotoUrl")%>' runat="server" Height="65px"
                                    Width="65px" BorderStyle="Solid" BorderWidth="3px" BorderColor="#cccccc" ToolTip='<%# Eval("UserName", "Photo of {0}") %>'
                                    onclick='<%# RedirectUserProfile(Eval("UserID")) %>' />
                            </td>
                            <td>
                                <table border="0" cellpadding="3" cellspacing="0" width="100%" style="text-align: left;">
                                    <tr>
                                        <td style="width: 35%">
                                            <dnn:Label runat="server" ID="lblUsername" Text="User Name" Suffix=":" />
                                        </td>
                                        <td>
                                            <%# Eval("UserName")%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 35%">
                                            <dnn:Label runat="server" ID="lblDisplayname" Text="Display Name" Suffix=":" />
                                        </td>
                                        <td>
                                            <%# Eval("DisplayName")%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 35%">
                                            <dnn:Label runat="server" ID="lblStatus" Text="Status" Suffix=":" />
                                        </td>
                                        <td>
                                            <%# Eval("Status") %>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:LinkButton runat="server" Text="Remove Friend" ID="btnRemoveFriend" OnCommand="btnRemoveFriend_OnCommand"
                                                CommandName='<%# Eval("UserID") %>' Visible='<%# MakeVisible(Eval("Status")) %>'>
                                            </asp:LinkButton>
                                            <asp:LinkButton runat="server" Text="Accept Friend Request" ID="btnAcceptFriendRequest"
                                                OnCommand="btnAcceptFriendRequest_OnCommand" CommandName='<%# Eval("UserRelationshipID") %>'
                                                Visible='<%# MakeAcceptFriendRequestVisible(Eval("Status"), Eval("UserID")) %>'>
                                            </asp:LinkButton>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </div>
        </ItemTemplate>
    </telerik:RadListView>
</asp:Panel>

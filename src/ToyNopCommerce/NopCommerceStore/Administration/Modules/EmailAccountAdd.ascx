<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.EmailAccountAddControl"
    CodeBehind="EmailAccountAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="EmailAccountInfo" Src="EmailAccountInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.EmailAccountAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.EmailAccountAdd.Title")%><a href="EmailAccounts.aspx"
            title="<%=GetLocaleResourceString("Admin.EmailAccountAdd.BackTo")%>"> (<%=GetLocaleResourceString("Admin.EmailAccountAdd.BackTo")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.EmailAccountAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.EmailAccountAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:EmailAccountInfo ID="ctrlEmailAccountInfo" runat="server" />

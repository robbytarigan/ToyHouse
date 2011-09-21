<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CreditCardTypeAddControl"
    CodeBehind="CreditCardTypeAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="CreditCardTypeInfo" Src="CreditCardTypeInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.CreditCardTypeAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.CreditCardTypeAdd.Title")%><a href="CreditCardTypes.aspx"
            title="<%=GetLocaleResourceString("Admin.CreditCardTypeAdd.BackToCards")%>"> (<%=GetLocaleResourceString("Admin.CreditCardTypeAdd.BackToCards")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.CreditCardTypeAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.CreditCardTypeAdd.SaveButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CreditCardTypeAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:CreditCardTypeInfo ID="ctrlCreditCardTypeInfo" runat="server" />

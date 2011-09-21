<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.AddressDetailsControl"
    CodeBehind="AddressDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="EmailTextBox" Src="EmailTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-customers.png" alt="<%=GetLocaleResourceString("Admin.AddressDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.AddressDetails.Title")%>
        <asp:HyperLink runat="server" ID="lnkBack" ToolTip="<% $NopResources:Admin.AddressDetails.BackToCustomer %>"
            Text="<% $NopResources:Admin.AddressDetails.BackToCustomer %>" />
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.AddressDetails.SaveAddress.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.AddressDetails.SaveAddress.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.AddressDetails.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.AddressDetails.DeleteAddress.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.AddressDetails.DeleteAddress.Tooltip %>" />
    </div>
</div>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCustomerTitle" Text="<% $NopResources:Admin.AddressInfo.Customer %>"
                ToolTip="<% $NopResources:Admin.AddressInfo.Customer.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblCustomer" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblFirstNameTitle" Text="<% $NopResources:Admin.AddressInfo.FirstName %>"
                ToolTip="<% $NopResources:Admin.AddressInfo.FirstName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtFirstName" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblLastNameTitle" Text="<% $NopResources:Admin.AddressInfo.LastName %>"
                ToolTip="<% $NopResources:Admin.AddressInfo.LastName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtLastName" ErrorMessage="<% $NopResources:Admin.AddressInfo.LastName.ErrorMessage %>"
                CssClass="adminInput"></nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblPhoneNumberTitle" Text="<% $NopResources:Admin.AddressInfo.Phone %>"
                ToolTip="<% $NopResources:Admin.AddressInfo.Phone.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblEmailTitle" Text="<% $NopResources:Admin.AddressInfo.Email %>"
                ToolTip="<% $NopResources:Admin.AddressInfo.Email.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:EmailTextBox runat="server" ID="txtEmail" CssClass="adminInput"></nopCommerce:EmailTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblFaxNumberTitle" Text="<% $NopResources:Admin.AddressInfo.Fax %>"
                ToolTip="<% $NopResources:Admin.AddressInfo.Fax.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtFaxNumber" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCompanyTitle" Text="<% $NopResources:Admin.AddressInfo.Company %>"
                ToolTip="<% $NopResources:Admin.AddressInfo.Company.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtCompany" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAddress1Title" Text="<% $NopResources:Admin.AddressInfo.Address1 %>"
                ToolTip="<% $NopResources:Admin.AddressInfo.Address1.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtAddress1" ErrorMessage="<% $NopResources:Admin.AddressInfo.ErrorMessage %>"
                CssClass="adminInput"></nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAddress2Title" Text="<% $NopResources:Admin.AddressInfo.Address2 %>"
                ToolTip="<% $NopResources:Admin.AddressInfo.Address2.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtAddress2" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCityTitle" Text="<% $NopResources:Admin.AddressInfo.City %>"
                ToolTip="<% $NopResources:Admin.AddressInfo.City.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtCity" ErrorMessage="<% $NopResources:Admin.AddressInfo.City.ErrorMessage %>"
                CssClass="adminInput"></nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblStateProvinceTitle" Text="<% $NopResources:Admin.AddressInfo.State %>"
                ToolTip="<% $NopResources:Admin.AddressInfo.State.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlStateProvince" AutoPostBack="False" runat="server" CssClass="adminInput">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblZipPostalTitle" Text="<% $NopResources:Admin.AddressInfo.Zip %>"
                ToolTip="<% $NopResources:Admin.AddressInfo.Zip.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtZipPostalCode" ErrorMessage="<% $NopResources:Admin.AddressInfo.Zip.ErrorMessage %>"
                CssClass="adminInput"></nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCountry" Text="<% $NopResources:Admin.AddressInfo.Country %>"
                ToolTip="<% $NopResources:Admin.AddressInfo.Country.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlCountry" AutoPostBack="True" runat="server" CssClass="adminInput"
                OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged">
            </asp:DropDownList>
        </td>
    </tr>
</table>
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />

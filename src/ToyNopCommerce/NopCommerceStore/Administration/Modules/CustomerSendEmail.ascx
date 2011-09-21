<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CustomerSendEmailControl"
    CodeBehind="CustomerSendEmail.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="FCKeditorV2" %>

    
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblSubject" Text="<% $NopResources:Admin.CustomerSendEmail.Subject %>"
                ToolTip="<% $NopResources:Admin.CustomerSendEmail.Subject.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtSubject" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvSubject" runat="server" ControlToValidate="txtSubject"
                ErrorMessage="<% $NopResources:Admin.CustomerSendEmail.Subject.Required %>" ValidationGroup="SendEmail">*</asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblBody" Text="<% $NopResources:Admin.CustomerSendEmail.Body %>"
                ToolTip="<% $NopResources:Admin.CustomerSendEmail.Body.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <FCKeditorV2:FCKeditor ID="txtBody" runat="server" AutoDetectLanguage="false" Height="350"
                Width="800px" />
        </td>
    </tr>
    <tr>
        <td colspan="2" class="adminData">
            <asp:Button runat="server" ID="btnSend" CssClass="adminButton" OnClick="btnSend_Click"
                Text="<% $NopResources:Admin.CustomerSendEmail.SendButton %>"
                ValidationGroup="SendEmail" />
        </td>
    </tr>
</table>
﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HostedPaymentConfig.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.CyberSource.HostedPaymentConfig" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td colspan="2">
            <b>Please ensure that nopCommerce primary currency matches CyberSource currency.</b>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Merchant ID:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtMerchantId" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Public Key:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtPublicKey" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Gateway URL:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtGatewayUrl" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Additional fee [<%=this.CurrencyService.PrimaryStoreCurrency.CurrencyCode%>]:
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" ID="txtAdditionalFee" Value="0" RequiredErrorMessage="Additional fee is required"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="The value must be from 0 to 100,000,000"
                CssClass="adminInput"></nopCommerce:DecimalTextBox>
        </td>
    </tr>
     <tr>
        <td class="adminTitle">
            Serial Number:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtSerialNumber" CssClass="adminInput" />
        </td>
    </tr>
</table>

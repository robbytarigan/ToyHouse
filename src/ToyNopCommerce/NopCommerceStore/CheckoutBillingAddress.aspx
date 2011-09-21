<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.CheckoutBillingAddressPage" CodeBehind="CheckoutBillingAddress.aspx.cs"
     %>

<%@ Register Src="~/Modules/OrderProgress.ascx" TagName="OrderProgress" TagPrefix="nopCommerce" %>
<%@ Register TagPrefix="nopCommerce" TagName="CheckoutBillingAddress" Src="~/Modules/CheckoutBillingAddress.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="OrderSummary" Src="~/Modules/OrderSummary.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="checkout-page">
        <nopCommerce:OrderProgress ID="OrderProgressControl" runat="server" OrderProgressStep="Address" />
        <div class="clear">
        </div>
        <div class="page-title">
            <h1><%=GetLocaleResourceString("Checkout.BillingAddress")%></h1>
        </div>
        <div class="clear">
        </div>
        <nopCommerce:CheckoutBillingAddress ID="ctrlCheckoutBillingAddress" runat="server" />
        <div class="clear">
        </div>
        <div class="order-summary-title">
            <%=GetLocaleResourceString("Checkout.OrderSummary")%>
        </div>
        <div class="clear">
        </div>
        <div class="order-summary-body">
            <nopCommerce:OrderSummary ID="OrderSummaryControl" runat="server" IsShoppingCart="false">
            </nopCommerce:OrderSummary>
        </div>
    </div>
</asp:Content>

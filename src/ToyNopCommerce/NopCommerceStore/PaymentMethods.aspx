<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="false" CodeBehind="PaymentMethods.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.PaymentMethodsPage" %>

<%@ Register TagPrefix="nopCommerce" TagName="Topic" Src="~/Modules/Topic.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:Topic ID="topicPaymentMethod" runat="server" TopicName="PaymentMethods">
    </nopCommerce:Topic>
</asp:Content>
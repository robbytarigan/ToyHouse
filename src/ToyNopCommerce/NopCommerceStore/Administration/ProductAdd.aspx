<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ProductAdd"
    CodeBehind="ProductAdd.aspx.cs"  %>

<%@ Register TagPrefix="nopCommerce" TagName="ProductAdd" Src="Modules/ProductAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ProductAdd runat="server" ID="ctrlProductAdd" />
</asp:Content>

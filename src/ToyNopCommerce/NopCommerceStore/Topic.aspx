<%@ Page Language="C#" MasterPageFile="~/MasterPages/ThreeColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.TopicPage" CodeBehind="Topic.aspx.cs"
     %>
<%@ Register TagPrefix="nopCommerce" TagName="TopicPage" Src="~/Modules/TopicPage.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:TopicPage ID="ctrlTopicPage" runat="server" />
</asp:Content>

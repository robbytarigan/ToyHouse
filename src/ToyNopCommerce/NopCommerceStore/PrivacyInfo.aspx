<%@ Page Language="C#" MasterPageFile="~/MasterPages/ThreeColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.PrivacyInfoPage" CodeBehind="PrivacyInfo.aspx.cs"
     %>

<%@ Register TagPrefix="nopCommerce" TagName="Topic" Src="~/Modules/Topic.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:Topic ID="topicPrivacyInfo" runat="server" TopicName="PrivacyInfo">
    </nopCommerce:Topic>
</asp:Content>

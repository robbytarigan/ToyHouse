<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.MeasureDimensionDetailsControl"
    CodeBehind="MeasureDimensionDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="MeasureDimensionInfo" Src="MeasureDimensionInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.Measures.MeasureDimensionDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.Measures.MeasureDimensionDetails.Title")%>
        <a href="Measures.aspx" title="<%=GetLocaleResourceString("Admin.Measures.MeasureDimensionDetails.BackToMeasures")%>">
            (<%=GetLocaleResourceString("Admin.Measures.MeasureDimensionDetails.BackToMeasures")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.Measures.MeasureDimensionDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.Measures.MeasureDimensionDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.MeasureDimensionDetails.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.Measures.MeasureDimensionDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.Measures.MeasureDimensionDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:MeasureDimensionInfo ID="ctrlMeasureDimensionInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />

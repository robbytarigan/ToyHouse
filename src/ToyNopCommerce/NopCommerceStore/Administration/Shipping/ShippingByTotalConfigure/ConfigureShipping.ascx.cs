//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.Web.Templates.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Administration.Shipping.ShippingByTotalConfigure
{
    public partial class ConfigureShipping : BaseNopAdministrationUserControl, IConfigureShippingRateComputationMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FillDropDowns();
                BindSettings();
                BindData();
            }
        }

        private void FillDropDowns()
        {
            ddlShippingMethod.Items.Clear();
            var shippingMethodCollection = this.ShippingService.GetAllShippingMethods();
            foreach (ShippingMethod shippingMethod in shippingMethodCollection)
            {
                ListItem item = new ListItem(shippingMethod.Name, shippingMethod.ShippingMethodId.ToString());
                ddlShippingMethod.Items.Add(item);
            }
        }

        private void BindData()
        {
            var shippingByTotalCollection = this.ShippingByTotalService.GetAll();
            gvShippingByTotals.DataSource = shippingByTotalCollection;
            gvShippingByTotals.DataBind();
        }

        private void BindSettings()
        {
            cbLimitMethodsToCreated.Checked = this.SettingManager.GetSettingValueBoolean("ShippingByTotal.LimitMethodsToCreated");
        }
        
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                int shippingMethodId = int.Parse(this.ddlShippingMethod.SelectedItem.Value);
                var shippingByTotal = new ShippingByTotal()
                {
                    ShippingMethodId = shippingMethodId,
                    From = txtFrom.Value,
                    To = txtTo.Value,
                    UsePercentage = cbUsePercentage.Checked,
                    ShippingChargePercentage = txtShippingChargePercentage.Value,
                    ShippingChargeAmount = txtShippingChargeAmount.Value
                };
                this.ShippingByTotalService.InsertShippingByTotal(shippingByTotal);

                BindData();
            }
            catch (Exception exc)
            {
                processAjaxError(exc);
            }
        }

        protected void gvShippingByTotals_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "UpdateShippingByTotal")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvShippingByTotals.Rows[index];

                HiddenField hfShippingByTotalId = row.FindControl("hfShippingByTotalId") as HiddenField;
                DropDownList ddlShippingMethod = row.FindControl("ddlShippingMethod") as DropDownList;
                DecimalTextBox txtFrom = row.FindControl("txtFrom") as DecimalTextBox;
                DecimalTextBox txtTo = row.FindControl("txtTo") as DecimalTextBox;
                CheckBox cbUsePercentage = row.FindControl("cbUsePercentage") as CheckBox;
                DecimalTextBox txtShippingChargePercentage = row.FindControl("txtShippingChargePercentage") as DecimalTextBox;
                DecimalTextBox txtShippingChargeAmount = row.FindControl("txtShippingChargeAmount") as DecimalTextBox;

                int shippingByTotalId = int.Parse(hfShippingByTotalId.Value);
                int shippingMethodId = int.Parse(ddlShippingMethod.SelectedItem.Value);
                ShippingByTotal shippingByTotal = this.ShippingByTotalService.GetById(shippingByTotalId);

                if (shippingByTotal != null)
                {
                    shippingByTotal.ShippingMethodId = shippingMethodId;
                    shippingByTotal.From = txtFrom.Value;
                    shippingByTotal.To = txtTo.Value;
                    shippingByTotal.UsePercentage = cbUsePercentage.Checked;
                    shippingByTotal.ShippingChargePercentage = txtShippingChargePercentage.Value;
                    shippingByTotal.ShippingChargeAmount = txtShippingChargeAmount.Value;
                    this.ShippingByTotalService.UpdateShippingByTotal(shippingByTotal);
                }
                BindData();
            }
        }

        protected void gvShippingByTotals_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ShippingByTotal shippingByTotal = (ShippingByTotal)e.Row.DataItem;
             
                Button btnUpdate = e.Row.FindControl("btnUpdate") as Button;
                if (btnUpdate != null)
                    btnUpdate.CommandArgument = e.Row.RowIndex.ToString();

                DropDownList ddlShippingMethod = e.Row.FindControl("ddlShippingMethod") as DropDownList;
                ddlShippingMethod.Items.Clear();
                var shippingMethodCollection = this.ShippingService.GetAllShippingMethods();
                foreach (ShippingMethod shippingMethod in shippingMethodCollection)
                {
                    ListItem item = new ListItem(shippingMethod.Name, shippingMethod.ShippingMethodId.ToString());
                    ddlShippingMethod.Items.Add(item);
                    if (shippingByTotal.ShippingMethodId == shippingMethod.ShippingMethodId)
                        item.Selected = true;
                }
            }
        }

        protected void gvShippingByTotals_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int shippingByTotalId = (int)gvShippingByTotals.DataKeys[e.RowIndex]["ShippingByTotalId"];
            ShippingByTotal shippingByTotal = this.ShippingByTotalService.GetById(shippingByTotalId);
            if (shippingByTotal != null)
            {
                this.ShippingByTotalService.DeleteShippingByTotal(shippingByTotal.ShippingByTotalId);
                BindData();
            }
        }

        protected void processAjaxError(Exception exc)
        {
            ProcessException(exc, false);
            pnlError.Visible = true;
            lErrorTitle.Text = exc.Message;
        }

        public void Save()
        {
            this.SettingManager.SetParam("ShippingByTotal.LimitMethodsToCreated", cbLimitMethodsToCreated.Checked.ToString());
        }
    }
}

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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.ExportImport;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common.Utils;
using System.Web.UI.DataVisualization.Charting;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CustomerReportsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                chartCustomersByOrderTotal.Visible = false;
                chartCustomersByNumberOfOrder.Visible = false;
                FillDropDowns();
                BindGridByLanguage();
                BindGridByGender();
                BindGridByCountry();
            }
        }

        protected List<CustomerBestReportLine> GetReportByOrderTotal()
        {
            DateTime? startDate = ctrlStartDatePickerByOrderTotal.SelectedDate;
            DateTime? endDate = ctrlEndDatePickerByOrderTotal.SelectedDate;
            if (startDate.HasValue)
            {
                startDate = DateTimeHelper.ConvertToUtcTime(startDate.Value, DateTimeHelper.CurrentTimeZone);
            }
            if (endDate.HasValue)
            {
                endDate = DateTimeHelper.ConvertToUtcTime(endDate.Value, DateTimeHelper.CurrentTimeZone).AddDays(1);
            }

            OrderStatusEnum? orderStatus = null;
            int orderStatusId = int.Parse(ddlOrderStatusByOrderTotal.SelectedItem.Value);
            if (orderStatusId > 0)
                orderStatus = (OrderStatusEnum)Enum.ToObject(typeof(OrderStatusEnum), orderStatusId);

            PaymentStatusEnum? paymentStatus = null;
            int paymentStatusId = int.Parse(ddlPaymentStatusByOrderTotal.SelectedItem.Value);
            if (paymentStatusId > 0)
                paymentStatus = (PaymentStatusEnum)Enum.ToObject(typeof(PaymentStatusEnum), paymentStatusId);

            ShippingStatusEnum? shippingStatus = null;
            int shippingStatusId = int.Parse(ddlShippingStatusByOrderTotal.SelectedItem.Value);
            if (shippingStatusId > 0)
                shippingStatus = (ShippingStatusEnum)Enum.ToObject(typeof(ShippingStatusEnum), shippingStatusId);

            return this.CustomerService.GetBestCustomersReport(startDate,
                endDate, orderStatus, paymentStatus, shippingStatus, 1);
        }

        protected List<CustomerBestReportLine> GetReportByNumberOfOrder()
        {
            DateTime? startDate = ctrlStartDatePickerByNumberOfOrder.SelectedDate;
            DateTime? endDate = ctrlEndDatePickerByNumberOfOrder.SelectedDate;
            if (startDate.HasValue)
            {
                startDate = DateTimeHelper.ConvertToUtcTime(startDate.Value, DateTimeHelper.CurrentTimeZone);
            }
            if (endDate.HasValue)
            {
                endDate = DateTimeHelper.ConvertToUtcTime(endDate.Value, DateTimeHelper.CurrentTimeZone).AddDays(1);
            }

            OrderStatusEnum? orderStatus = null;
            int orderStatusId = int.Parse(ddlOrderStatusByNumberOfOrder.SelectedItem.Value);
            if (orderStatusId > 0)
                orderStatus = (OrderStatusEnum)Enum.ToObject(typeof(OrderStatusEnum), orderStatusId);

            PaymentStatusEnum? paymentStatus = null;
            int paymentStatusId = int.Parse(ddlPaymentStatusByNumberOfOrder.SelectedItem.Value);
            if (paymentStatusId > 0)
                paymentStatus = (PaymentStatusEnum)Enum.ToObject(typeof(PaymentStatusEnum), paymentStatusId);

            ShippingStatusEnum? shippingStatus = null;
            int shippingStatusId = int.Parse(ddlShippingStatusByNumberOfOrder.SelectedItem.Value);
            if (shippingStatusId > 0)
                shippingStatus = (ShippingStatusEnum)Enum.ToObject(typeof(ShippingStatusEnum), shippingStatusId);

            return this.CustomerService.GetBestCustomersReport(startDate,
                endDate, orderStatus, paymentStatus, shippingStatus, 2);
        }

        protected void FillDropDowns()
        {
            OrderStatusEnum[] orderStatuses = (OrderStatusEnum[])Enum.GetValues(typeof(OrderStatusEnum));
            PaymentStatusEnum[] paymentStatuses = (PaymentStatusEnum[])Enum.GetValues(typeof(PaymentStatusEnum));
            ShippingStatusEnum[] shippingStatuses = (ShippingStatusEnum[])Enum.GetValues(typeof(ShippingStatusEnum));

            //by order total
            this.ddlOrderStatusByOrderTotal.Items.Clear();
            ListItem itemOrderStatusByOrderTotal = new ListItem(GetLocaleResourceString("Admin.Common.All"), "0");
            this.ddlOrderStatusByOrderTotal.Items.Add(itemOrderStatusByOrderTotal);
            foreach (OrderStatusEnum orderStatus in orderStatuses)
            {
                ListItem item2 = new ListItem(orderStatus.GetOrderStatusName(), ((int)orderStatus).ToString());
                this.ddlOrderStatusByOrderTotal.Items.Add(item2);
            }

            this.ddlPaymentStatusByOrderTotal.Items.Clear();
            ListItem itemPaymentStatusByOrderTotal = new ListItem(GetLocaleResourceString("Admin.Common.All"), "0");
            this.ddlPaymentStatusByOrderTotal.Items.Add(itemPaymentStatusByOrderTotal);
            foreach (PaymentStatusEnum paymentStatus in paymentStatuses)
            {
                ListItem item2 = new ListItem(paymentStatus.GetPaymentStatusName(), ((int)paymentStatus).ToString());
                this.ddlPaymentStatusByOrderTotal.Items.Add(item2);
            }

            this.ddlShippingStatusByOrderTotal.Items.Clear();
            ListItem itemShippingStatusByOrderTotal = new ListItem(GetLocaleResourceString("Admin.Common.All"), "0");
            this.ddlShippingStatusByOrderTotal.Items.Add(itemShippingStatusByOrderTotal);
            foreach (ShippingStatusEnum shippingStatus in shippingStatuses)
            {
                ListItem item2 = new ListItem(shippingStatus.GetShippingStatusName(), ((int)shippingStatus).ToString());
                this.ddlShippingStatusByOrderTotal.Items.Add(item2);
            }
            

            //by number of orders
            this.ddlOrderStatusByNumberOfOrder.Items.Clear();
            ListItem itemOrderStatusByNumberOfOrder = new ListItem(GetLocaleResourceString("Admin.Common.All"), "0");
            this.ddlOrderStatusByNumberOfOrder.Items.Add(itemOrderStatusByNumberOfOrder);
            foreach (OrderStatusEnum orderStatus in orderStatuses)
            {
                ListItem item2 = new ListItem(orderStatus.GetOrderStatusName(), ((int)orderStatus).ToString());
                this.ddlOrderStatusByNumberOfOrder.Items.Add(item2);
            }

            this.ddlPaymentStatusByNumberOfOrder.Items.Clear();
            ListItem itemPaymentStatusByNumberOfOrder = new ListItem(GetLocaleResourceString("Admin.Common.All"), "0");
            this.ddlPaymentStatusByNumberOfOrder.Items.Add(itemPaymentStatusByNumberOfOrder);
            foreach (PaymentStatusEnum paymentStatus in paymentStatuses)
            {
                ListItem item2 = new ListItem(paymentStatus.GetPaymentStatusName(), ((int)paymentStatus).ToString());
                this.ddlPaymentStatusByNumberOfOrder.Items.Add(item2);
            }

            this.ddlShippingStatusByNumberOfOrder.Items.Clear();
            ListItem itemShippingStatusByNumberOfOrder = new ListItem(GetLocaleResourceString("Admin.Common.All"), "0");
            this.ddlShippingStatusByNumberOfOrder.Items.Add(itemShippingStatusByNumberOfOrder);
            foreach (ShippingStatusEnum shippingStatus in shippingStatuses)
            {
                ListItem item2 = new ListItem(shippingStatus.GetShippingStatusName(), ((int)shippingStatus).ToString());
                this.ddlShippingStatusByNumberOfOrder.Items.Add(item2);
            }
        }

        protected void BindGridByOrderTotal()
        {
            var report = GetReportByOrderTotal();
            if (report.Count == 0)
            {
                chartCustomersByOrderTotal.Visible = false;
            }
            else
            {
                chartCustomersByOrderTotal.Visible = true;
                chartCustomersByOrderTotal.Series[0].Points.Clear();
                foreach (CustomerBestReportLine repLine in report)
                {
                    var dp = new DataPoint();
                    dp.YValues = new double[1] { (double)repLine.OrderTotal };
                    dp.LegendText = GetCustomerName(repLine.CustomerId);
                    dp.ToolTip = dp.LegendText;
                    chartCustomersByOrderTotal.Series[0].Points.Add(dp);
                }
                chartCustomersByOrderTotal.DataBind();
            }
            this.gvByOrderTotal.DataSource = report;
            this.gvByOrderTotal.DataBind();
        }

        protected void BindGridByNumberOfOrder()
        {
            var report = GetReportByNumberOfOrder();
            if (report.Count == 0)
            {
                chartCustomersByNumberOfOrder.Visible = false;
            }
            else
            {
                chartCustomersByNumberOfOrder.Visible = true;
                chartCustomersByNumberOfOrder.Series[0].Points.Clear();
                foreach (CustomerBestReportLine repLine in report)
                {
                    var dp = new DataPoint();
                    dp.YValues = new double[1] { repLine.OrderCount };
                    dp.LegendText = GetCustomerName(repLine.CustomerId);
                    dp.ToolTip = dp.LegendText;
                    chartCustomersByNumberOfOrder.Series[0].Points.Add(dp);
                }
                chartCustomersByNumberOfOrder.DataBind();
            }
            this.gvByNumberOfOrder.DataSource = report;
            this.gvByNumberOfOrder.DataBind();
        }

        protected void BindGridByLanguage()
        {
            var report = this.CustomerService.GetCustomerReportByLanguage();
            if (report.Count == 0)
            {
                chartCustomersByLanguage.Visible = false;
            }
            else
            {
                chartCustomersByLanguage.Visible = true;
                chartCustomersByLanguage.Series[0].Points.Clear();
                foreach (CustomerReportByLanguageLine repLine in report)
                {
                    var dp = new DataPoint();
                    dp.YValues = new double[1] { repLine.CustomerCount };
                    dp.LegendText = GetLanguageInfo(repLine.LanguageId);
                    dp.ToolTip = dp.LegendText;
                    chartCustomersByLanguage.Series[0].Points.Add(dp);
                }
                chartCustomersByLanguage.DataBind();
            }
            this.gvByLanguage.DataSource = report;
            this.gvByLanguage.DataBind();
        }

        protected void BindGridByGender()
        {
            var report = this.CustomerService.GetCustomerReportByAttributeKey("Gender");
            if (report.Count == 0)
            {
                chartCustomerByGender.Visible = false;
            }
            else
            {
                chartCustomerByGender.Visible = true;
                chartCustomerByGender.Series[0].Points.Clear();
                foreach (CustomerReportByAttributeKeyLine repLine in report)
                {
                    var dp = new DataPoint();
                    dp.YValues = new double[1] { repLine.CustomerCount };
                    dp.LegendText = GetGenderInfo(repLine.AttributeKey);
                    dp.ToolTip = dp.LegendText;
                    chartCustomerByGender.Series[0].Points.Add(dp);
                }
                chartCustomerByGender.DataBind();
            }
            this.gvByGender.DataSource = report;
            this.gvByGender.DataBind();
        }

        protected void BindGridByCountry()
        {
            var report = this.CustomerService.GetCustomerReportByAttributeKey("CountryId");
            if (report.Count == 0)
            {
                chartCustomerByCountry.Visible = false;
            }
            else
            {
                chartCustomerByCountry.Visible = true;
                chartCustomerByCountry.Series[0].Points.Clear();
                foreach (CustomerReportByAttributeKeyLine repLine in report)
                {
                    var dp = new DataPoint();
                    dp.YValues = new double[1] { repLine.CustomerCount };
                    dp.LegendText = GetCountryInfo(repLine.AttributeKey);
                    dp.ToolTip = dp.LegendText;
                    chartCustomerByCountry.Series[0].Points.Add(dp);
                }
                chartCustomerByCountry.DataBind();
            }
            this.gvByCountry.DataSource = report;
            this.gvByCountry.DataBind();
        }

        protected string GetCustomerInfo(int customerId)
        {
            string customerInfo = string.Empty;
            Customer customer = this.CustomerService.GetCustomerById(customerId);
            if (customer != null)
            {
                if (customer.IsGuest)
                {
                    customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerId={0}\">{1}</a>", customer.CustomerId, GetLocaleResourceString("Admin.Common.CustomerGuest"));
                }
                else
                {
                    customerInfo = string.Format("{0} (<a href=\"CustomerDetails.aspx?CustomerID={1}\">{2}</a>)", Server.HtmlEncode(customer.FullName), customer.CustomerId, Server.HtmlEncode(customer.Email));
                }
            }
            return customerInfo;
        }

        protected string GetCustomerName(int customerId)
        {
            string customerName = string.Empty;
            Customer customer = this.CustomerService.GetCustomerById(customerId);
            if (customer != null)
            {
                if (customer.IsGuest)
                {
                    customerName = GetLocaleResourceString("Admin.Common.CustomerGuest");
                }
                else
                {
                    customerName = string.Format("{0} ({1})", Server.HtmlEncode(customer.FullName), Server.HtmlEncode(customer.Email));
                }
            }
            return customerName;
        }

        protected string GetLanguageInfo(int languageId)
        {
            string languageInfo = string.Empty;
            var language = this.LanguageService.GetLanguageById(languageId);
            if (language != null)
            {
                languageInfo = language.Name;
            }
            else
            {
                languageInfo = GetLocaleResourceString("Admin.Common.Unknown");
            }
            return Server.HtmlEncode(languageInfo);
        }

        protected string GetGenderInfo(string attributeKey)
        {
            string genderInfo = string.Empty;
            switch (attributeKey.ToLowerInvariant())
            {
                case "m":
                    genderInfo = GetLocaleResourceString("Admin.CustomerReports.ByGender.Male");
                    break;
                case "f":
                    genderInfo = GetLocaleResourceString("Admin.CustomerReports.ByGender.Female");
                    break;
                default:
                    genderInfo = GetLocaleResourceString("Admin.Common.Unknown");
                    break;
            }
            return Server.HtmlEncode(genderInfo);
        }

        protected string GetCountryInfo(string attributeKey)
        {
            string countryInfo = string.Empty;
            if (String.IsNullOrEmpty(attributeKey))
            {
                countryInfo = GetLocaleResourceString("Admin.Common.Unknown");
            }
            else
            {
                int countryId = 0;
                if (int.TryParse(attributeKey, out countryId))
                {
                    var country = this.CountryService.GetCountryById(countryId);
                    if (country != null)
                    {
                        countryInfo = country.Name;
                    }
                    else
                    {
                        countryInfo = GetLocaleResourceString("Admin.Common.Unknown");
                    }
                }
                else
                {
                    countryInfo = GetLocaleResourceString("Admin.Common.Unknown");
                }
            }
            return Server.HtmlEncode(countryInfo);
        }

        protected void btnSearchByOrderTotal_Click(object sender, EventArgs e)
        {
            try
            {
                BindGridByOrderTotal();
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        protected void btnSearchByNumberOfOrder_Click(object sender, EventArgs e)
        {
            try
            {
                BindGridByNumberOfOrder();
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }
    }
}
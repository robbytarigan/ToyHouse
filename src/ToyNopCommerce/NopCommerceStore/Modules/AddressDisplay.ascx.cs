// Not original
// Dispaly Address
// Do not display Phone Number, Email, Fax Number or Company whenever they are empty
namespace NopSolutions.NopCommerce.Web.Modules {
    #region Using
    using System;
    using System.ComponentModel;
    using System.Web.UI.WebControls;
    using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
    #endregion Using
    public partial class AddressDisplay : BaseNopFrontendUserControl
    {
        public Address Address
        {
            set
            {
                Address address = value;
                if (address != null)
                {
                    this.btnEditAddress.CommandArgument = address.AddressId.ToString();
                    this.btnDeleteAddress.CommandArgument = address.AddressId.ToString();
                    this.lFullName.Text = Server.HtmlEncode(string.Format("{0} {1}", address.FirstName, address.LastName));
                    this.lFirstName.Text = Server.HtmlEncode(address.FirstName);
                    this.lLastName.Text = Server.HtmlEncode(address.LastName);

                    if (!String.IsNullOrEmpty(address.PhoneNumber)) {
                        this.lPhoneNumber.Text = Server.HtmlEncode(address.PhoneNumber);
                    } else {
                        phonePanel.Visible = false;
                    }
                    
                    if (!String.IsNullOrEmpty(address.Email)) {
                        this.lEmail.Text = Server.HtmlEncode(address.Email);
                    } else {
                        emailPanel.Visible = false;
                    }

                    if (!String.IsNullOrEmpty(address.FaxNumber)) {
                        this.lFaxNumber.Text = Server.HtmlEncode(address.FaxNumber);
                    } else {
                        faxPanel.Visible = false;
                    }

                    if (!String.IsNullOrEmpty(address.Company))
                        this.lCompany.Text = Server.HtmlEncode(address.Company);
                    else
                        this.companyPanel.Visible = false;

                    this.lAddress1.Text = Server.HtmlEncode(address.Address1);
                    if (!String.IsNullOrEmpty(address.Address2))
                        this.lAddress2.Text = Server.HtmlEncode(address.Address2);
                    else
                        this.pnlAddress2.Visible = false;
                    this.lCity.Text = Server.HtmlEncode(address.City);
                    var country = address.Country;
                    if (country != null)
                        this.lCountry.Text = Server.HtmlEncode(country.Name);
                    else
                        this.pnlCountry.Visible = false;
                    var stateProvince = address.StateProvince;
                    if (stateProvince != null)
                        this.lStateProvince.Text = Server.HtmlEncode(stateProvince.Name);
                    this.lZipPostalCode.Text = Server.HtmlEncode(address.ZipPostalCode);
                }
            }
        }

        protected void btnEditAddress_Click(object sender, CommandEventArgs e)
        {
            int addressId = Convert.ToInt32(e.CommandArgument);
            Response.Redirect(string.Format("~/addressedit.aspx?addressid={0}", addressId));
        }

        protected void btnDeleteAddress_Click(object sender, CommandEventArgs e)
        {
            int addressId = Convert.ToInt32(e.CommandArgument);
            Response.Redirect(string.Format("~/addressedit.aspx?addressid={0}&delete={1}", addressId, true));
        }

        [DefaultValue(true)]
        public bool ShowDeleteButton
        {
            get
            {
                object obj2 = this.ViewState["ShowDeleteButton"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                this.ViewState["ShowDeleteButton"] = value;
            }
        }

        [DefaultValue(true)]
        public bool ShowEditButton
        {
            get
            {
                object obj2 = this.ViewState["ShowEditButton"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                this.ViewState["ShowEditButton"] = value;
            }
        }
    }
}
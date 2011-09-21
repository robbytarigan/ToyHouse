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
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CreditCardTypeInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            CreditCardType creditCardType = this.PaymentService.GetCreditCardTypeById(this.CreditCardTypeId);
            if (creditCardType != null)
            {
                this.txtName.Text = creditCardType.Name;
                this.txtSystemKeyword.Text = creditCardType.SystemKeyword;
                this.txtDisplayOrder.Value = creditCardType.DisplayOrder;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        public CreditCardType SaveInfo()
        {
            CreditCardType creditCardType = this.PaymentService.GetCreditCardTypeById(this.CreditCardTypeId);
            if (creditCardType != null)
            {
                creditCardType.Name = txtName.Text;
                creditCardType.SystemKeyword = txtSystemKeyword.Text;
                creditCardType.DisplayOrder = txtDisplayOrder.Value;
                this.PaymentService.UpdateCreditCardType(creditCardType);
            }
            else
            {
                creditCardType = new CreditCardType()
                {
                    Name = txtName.Text,
                    SystemKeyword = txtSystemKeyword.Text,
                    DisplayOrder = txtDisplayOrder.Value
                };
                this.PaymentService.InsertCreditCardType(creditCardType);
            }

            return creditCardType;
        }

        public int CreditCardTypeId
        {
            get
            {
                return CommonHelper.QueryStringInt("CreditCardTypeId");
            }
        }
    }
}
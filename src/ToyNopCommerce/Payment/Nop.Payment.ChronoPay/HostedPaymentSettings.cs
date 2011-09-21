﻿//------------------------------------------------------------------------------
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
// Contributor(s): 
//------------------------------------------------------------------------------

using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Payment.Methods.ChronoPay
{
    public class HostedPaymentSettings
    {
        #region Properties
        public static string GatewayUrl
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.ChronoPay.HostedPayment.GatewayUrl", "https://secure.chronopay.com/index_shop.cgi");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.ChronoPay.HostedPayment.GatewayUrl", value);
            }
        }

        public static string ProductId
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.ChronoPay.HostedPayment.ProductID");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.ChronoPay.HostedPayment.ProductID", value);
            }
        }

        public static string ProductName
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.ChronoPay.HostedPayment.ProductName");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.ChronoPay.HostedPayment.ProductName", value);
            }
        }

        public static string SharedSecrect
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.ChronoPay.HostedPayment.SharedSecrect");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.ChronoPay.HostedPayment.SharedSecrect", value);
            }
        }

        public static decimal AdditionalFee
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.ChronoPay.HostedPayment.AdditionalFee");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParamNative("PaymentMethod.ChronoPay.HostedPayment.AdditionalFee", value);
            }
        }
        #endregion
    }
}

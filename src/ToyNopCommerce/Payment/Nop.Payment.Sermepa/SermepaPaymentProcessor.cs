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
// Contributor(s): Noel Revuelta _______. 
//------------------------------------------------------------------------------

using System;
using System.Security.Cryptography;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Payment.Methods.Sermepa
{
    /// <summary>
    /// Sermepa payment processor
    /// </summary>
    public class SermepaPaymentProcessor : IPaymentMethod
    {
        #region Fields

        private bool pruebas = true;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public SermepaPaymentProcessor()
        {
            pruebas = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.Sermepa.Pruebas");
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets Sermepa URL
        /// </summary>
        /// <returns></returns>
        private string GetSermepaUrl()
        {
            return pruebas ? "https://sis-t.sermepa.es:25443/sis/realizarPago" : 
                "https://sis.sermepa.es/sis/realizarPago";
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process payment
        /// </summary>
        /// <param name="paymentInfo">Payment info required for an order processing</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderGuid">Unique order identifier</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void ProcessPayment(PaymentInfo paymentInfo, Customer customer, Guid orderGuid, ref ProcessPaymentResult processPaymentResult)
        {
            processPaymentResult.PaymentStatus = PaymentStatusEnum.Pending;
        }

        /// <summary>
        /// Post process payment (payment gateways that require redirecting)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>The error status, or String.Empty if no errors</returns>
        public string PostProcessPayment(Order order)
        {

            //Notificación On-Line
            string strDs_Merchant_MerchantURL = Common.Utils.CommonHelper.GetStoreLocation(false) + "SermepaReturn.aspx";

            //URL OK
            string strDs_Merchant_UrlOK = Common.Utils.CommonHelper.GetStoreLocation(false) + "CheckoutCompleted.aspx";

            //URL KO
            string strDs_Merchant_UrlKO = Common.Utils.CommonHelper.GetStoreLocation(false) + "SermepaError.aspx";

            //Numero de pedido
            string strDs_Merchant_Order = order.OrderId.ToString();

            //Nombre del comercio
            string strDs_Merchant_MerchantName = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Sermepa.NombreComercio");

            //Importe
            string amount = ((int)Convert.ToInt64(order.OrderTotal * 100)).ToString();
            string strDs_Merchant_Amount = amount;

            //Código de comercio
            string strDs_Merchant_MerchantCode = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Sermepa.FUC");

            //Moneda
            string strDs_Merchant_Currency = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Sermepa.Moneda");

            //Terminal
            string strDs_Merchant_Terminal = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Sermepa.Terminal");

            //Tipo de transaccion (0 - Autorización)
            string strDs_Merchant_TransactionType = "0";

            //Clave
            string clave = "";
            if (pruebas) 
            { 
                clave = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Sermepa.ClavePruebas"); 
            }
            else 
            { 
                clave = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Sermepa.ClaveReal"); 
            }

            //Calculo de la firma
            string SHA = string.Format("{0}{1}{2}{3}{4}{5}{6}",
                strDs_Merchant_Amount,
                strDs_Merchant_Order,
                strDs_Merchant_MerchantCode,
                strDs_Merchant_Currency,
                strDs_Merchant_TransactionType,
                strDs_Merchant_MerchantURL,
                clave);

            byte[] SHAresult;
            SHA1 shaM = new SHA1Managed();
            SHAresult = shaM.ComputeHash(System.Text.Encoding.Default.GetBytes(SHA));
            string SHAresultStr = BitConverter.ToString(SHAresult).Replace("-", "");

            //Creamos el POST
            RemotePost remotePostHelper = new RemotePost();
            remotePostHelper.FormName = "form1";
            remotePostHelper.Url = GetSermepaUrl();

            remotePostHelper.Add("Ds_Merchant_Amount", strDs_Merchant_Amount);
            remotePostHelper.Add("Ds_Merchant_Currency", strDs_Merchant_Currency);
            remotePostHelper.Add("Ds_Merchant_Order", strDs_Merchant_Order);
            remotePostHelper.Add("Ds_Merchant_MerchantCode", strDs_Merchant_MerchantCode);
            remotePostHelper.Add("Ds_Merchant_TransactionType", strDs_Merchant_TransactionType);
            remotePostHelper.Add("Ds_Merchant_MerchantURL", strDs_Merchant_MerchantURL);
            remotePostHelper.Add("Ds_Merchant_MerchantSignature", SHAresultStr);
            remotePostHelper.Add("Ds_Merchant_Terminal", strDs_Merchant_Terminal);
            remotePostHelper.Add("Ds_Merchant_MerchantName", strDs_Merchant_MerchantName);
            remotePostHelper.Add("Ds_Merchant_UrlOK", strDs_Merchant_UrlOK);
            remotePostHelper.Add("Ds_Merchant_UrlKO", strDs_Merchant_UrlKO);

            remotePostHelper.Post();

            return string.Empty;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee()
        {
            return IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.Sermepa.AdditionalFee");
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void Capture(Order order, ref ProcessPaymentResult processPaymentResult)
        {
            throw new NopException("Capture method not supported");
        }

        /// <summary>
        /// Refunds payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>        
        public void Refund(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            throw new NopException("Refund method not supported");
        }

        /// <summary>
        /// Voids payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>        
        public void Void(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            throw new NopException("Void method not supported");
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="paymentInfo">Payment info required for an order processing</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderGuid">Unique order identifier</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void ProcessRecurringPayment(PaymentInfo paymentInfo, Customer customer, Guid orderGuid, ref ProcessPaymentResult processPaymentResult)
        {
            throw new NopException("Recurring payments not supported");
        }

        /// <summary>
        /// Cancels recurring payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>        
        public void CancelRecurringPayment(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
        }
        #endregion

        #region Properies

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool CanCapture
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool CanPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool CanRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool CanVoid
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        /// <returns>A recurring payment type of payment method</returns>
        public RecurringPaymentTypeEnum SupportRecurringPayments
        {
            get
            {
                return RecurringPaymentTypeEnum.NotSupported;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        /// <returns>A payment method type</returns>
        public PaymentMethodTypeEnum PaymentMethodType
        {
            get
            {
                return PaymentMethodTypeEnum.Standard;
            }
        }
        #endregion
    }
}

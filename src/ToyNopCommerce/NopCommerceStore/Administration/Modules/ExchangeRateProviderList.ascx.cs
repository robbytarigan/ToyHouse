﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Directory.ExchangeRates;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ExchangeRateProviderListControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        private void BindData()
        {
            string prv1 = this.SettingManager.GetSettingValue("ExchangeRateProvider1.Classname");
            string prv2 = this.SettingManager.GetSettingValue("ExchangeRateProvider2.Classname");
            string prv3 = this.SettingManager.GetSettingValue("ExchangeRateProvider3.Classname");

            if (!String.IsNullOrEmpty(prv1))
            {
                Type t = Type.GetType(prv1, false);

                if (t != null)
                {
                    IExchangeRateProvider instance = Activator.CreateInstance(t) as IExchangeRateProvider;

                    if (instance != null)
                    {
                        ddlExchangeRateProviders.Items.Add(new ListItem(instance.Name, "1"));
                    }
                }
            }
            if (!String.IsNullOrEmpty(prv2))
            {
                Type t = Type.GetType(prv2, false);

                if (t != null)
                {
                    IExchangeRateProvider instance = Activator.CreateInstance(t) as IExchangeRateProvider;

                    if (instance != null)
                    {
                        ddlExchangeRateProviders.Items.Add(new ListItem(instance.Name, "2"));
                    }
                }
            }
            if (!String.IsNullOrEmpty(prv3))
            {
                Type t = Type.GetType(prv3, false);

                if (t != null)
                {
                    IExchangeRateProvider instance = Activator.CreateInstance(t) as IExchangeRateProvider;

                    if (instance != null)
                    {
                        ddlExchangeRateProviders.Items.Add(new ListItem(instance.Name, "3"));
                    }
                }
            }
            ddlExchangeRateProviders.DataBind();

            ddlExchangeRateProviders.SelectedValue = this.SettingManager.GetSettingValue("ExchangeRateProvider.Current");
        }

        public string SelectedProvider
        {
            get 
            {
                return ddlExchangeRateProviders.SelectedItem.Value;
            }
        }
    }
}
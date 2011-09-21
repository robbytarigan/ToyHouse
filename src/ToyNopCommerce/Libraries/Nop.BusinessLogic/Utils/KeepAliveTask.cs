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
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Net;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Tasks;

namespace NopSolutions.NopCommerce.BusinessLogic.Utils
{
    /// <summary>
    /// Represents a task for pinger
    /// </summary>
    public partial class KeepAliveTask : ITask
    {
        private string _path = string.Empty;

        /// <summary>
        /// Executes a task
        /// </summary>
        /// <param name="node">Xml node that represents a task description</param>
        public void Execute(XmlNode node)
        {
            var attribute1 = node.Attributes["path"];
            if (attribute1 != null && !String.IsNullOrEmpty(attribute1.Value))
            {
                this._path = attribute1.Value;
            }
            string url = IoC.Resolve<ISettingManager>().StoreUrl;
            url += _path;

            using (var wc = new WebClient())
            {
                string response = wc.DownloadString(url);
            }
        }
    }
}

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

namespace NopSolutions.NopCommerce.BusinessLogic.CustomerManagement
{
    /// <summary>
    /// Represents a customer report by language line
    /// </summary>
    public partial class CustomerReportByLanguageLine : BaseEntity
    {
        /// <summary>
        /// Gets or sets the language ID
        /// </summary>
        public int LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the customer count
        /// </summary>
        public int CustomerCount { get; set; }
    }
}
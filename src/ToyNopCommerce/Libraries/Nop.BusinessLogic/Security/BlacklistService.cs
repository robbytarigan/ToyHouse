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
using System.Collections.Generic;
using System.Linq;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Security
{
    /// <summary>
    /// IP Blacklist service implementation
    /// </summary>
    public partial class BlacklistService : IBlacklistService
    {
        #region Constants
        private const string BLACKLIST_ALLIP_KEY = "Nop.blacklist.ip.all";
        private const string BLACKLIST_ALLNETWORK_KEY = "Nop.blacklist.network.all";
        private const string BLACKLIST_IP_PATTERN_KEY = "Nop.blacklist.ip.";
        private const string BLACKLIST_NETWORK_PATTERN_KEY = "Nop.blacklist.network.";
        #endregion

        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        private readonly NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public BlacklistService(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopRequestCache();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// This encodes the string representation of an IP address to a uint, but
        /// backwards so that it can be used to compare addresses. This function is
        /// used internally for comparison and is not valid for valid encoding of
        /// IP address information.
        /// </summary>
        /// <param name="ipAddress">A string representation of the IP address to convert</param>
        /// <returns>Returns a backwards uint representation of the string.</returns>
        private uint IpAddressToLongBackwards(string ipAddress)
        {
            byte[] byteIp = System.Net.IPAddress.Parse(ipAddress).GetAddressBytes();

            uint ip = (uint)byteIp[0] << 24;
            ip += (uint)byteIp[1] << 16;
            ip += (uint)byteIp[2] << 8;
            ip += (uint)byteIp[3];

            return ip;
        }

        /// <summary>
        /// Compares two IP addresses for equality. 
        /// </summary>
        /// <param name="ipAddress1">The first IP to compare</param>
        /// <param name="ipAddress2">The second IP to compare</param>
        /// <returns>True if equal, false if not.</returns>
        private bool AreEqual(string ipAddress1, string ipAddress2)
        {
            // convert to long in case there is any zero padding in the strings
            return IpAddressToLongBackwards(ipAddress1) == IpAddressToLongBackwards(ipAddress2);
        }

        /// <summary>
        /// Compares two string representations of an Ip address to see if one
        /// is greater than the other
        /// </summary>
        /// <param name="toCompare">The IP address on the left hand side of the greater 
        /// than operator</param>
        /// <param name="compareAgainst">The Ip address on the right hand side of the 
        /// greater than operator</param>
        /// <returns>True if ToCompare is greater than CompareAgainst, else false</returns>       
        private bool IsGreater(string toCompare, string compareAgainst)
        {
            // convert to long in case there is any zero padding in the strings
            return IpAddressToLongBackwards(toCompare) > IpAddressToLongBackwards(compareAgainst);
        }

        /// <summary>
        /// Compares two string representations of an Ip address to see if one
        /// is less than the other
        /// </summary>
        /// <param name="toCompare">The IP address on the left hand side of the less 
        /// than operator</param>
        /// <param name="compareAgainst">The Ip address on the right hand side of the 
        /// less than operator</param>
        /// <returns>True if ToCompare is greater than CompareAgainst, else false</returns>
        private bool IsLess(string toCompare, string compareAgainst)
        {
            // convert to long in case there is any zero padding in the strings
            return IpAddressToLongBackwards(toCompare) < IpAddressToLongBackwards(compareAgainst);
        }

        /// <summary>
        /// Determines whether a specified object is equal to another object
        /// </summary>
        /// <param name="toCompare">The IP address on the left hand side of the less 
        /// than operator</param>
        /// <param name="compareAgainst">The Ip address on the right hand side of the 
        /// less than operator</param>
        /// <returns>Result</returns>
        private bool IsEqual(string toCompare, string compareAgainst)
        {
            return IpAddressToLongBackwards(toCompare) == IpAddressToLongBackwards(compareAgainst);
        }

        /// <summary>
        /// Compares two string representations of an Ip address to see if one
        /// is greater than or equal to the other.
        /// </summary>
        /// <param name="toCompare">The IP address on the left hand side of the greater 
        /// than or equal operator</param>
        /// <param name="compareAgainst">The Ip address on the right hand side of the 
        /// greater than or equal operator</param>
        /// <returns>True if ToCompare is greater than or equal to CompareAgainst, else false</returns>
        private bool IsGreaterOrEqual(string toCompare, string compareAgainst)
        {
            // convert to long in case there is any zero padding in the strings
            return IpAddressToLongBackwards(toCompare) >= IpAddressToLongBackwards(compareAgainst);
        }

        /// <summary>
        /// Compares two string representations of an Ip address to see if one
        /// is less than or equal to the other.
        /// </summary>
        /// <param name="toCompare">The IP address on the left hand side of the less 
        /// than or equal operator</param>
        /// <param name="compareAgainst">The Ip address on the right hand side of the 
        /// less than or equal operator</param>
        /// <returns>True if ToCompare is greater than or equal to CompareAgainst, else false</returns>
        private bool IsLessOrEqual(string toCompare, string compareAgainst)
        {
            // convert to long in case there is any zero padding in the strings
            return IpAddressToLongBackwards(toCompare) <= IpAddressToLongBackwards(compareAgainst);
        }

        /// <summary>
        /// Converts a uint representation of an Ip address to a string.
        /// </summary>
        /// <param name="ipAddress">The IP address to convert</param>
        /// <returns>A string representation of the IP address.</returns>
        private string LongToIpAddress(uint ipAddress)
        {
            return new System.Net.IPAddress(ipAddress).ToString();
        }

        /// <summary>
        /// Converts a string representation of an IP address to a uint. This
        /// encoding is proper and can be used with other networking functions such
        /// as the System.Net.IPAddress class.
        /// </summary>
        /// <param name="ipAddress">The Ip address to convert.</param>
        /// <returns>Returns a uint representation of the IP address.</returns>
        private uint IpAddressToLong(string ipAddress)
        {
            byte[] byteIp = System.Net.IPAddress.Parse(ipAddress).GetAddressBytes();

            uint ip = (uint)byteIp[3] << 24;
            ip += (uint)byteIp[2] << 16;
            ip += (uint)byteIp[1] << 8;
            ip += (uint)byteIp[0];

            return ip;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets an IP address by its identifier
        /// </summary>
        /// <param name="ipAddressId">IP Address unique identifier</param>
        /// <returns>An IP address</returns>
        public BannedIpAddress GetBannedIpAddressById(int ipAddressId)
        {
            if (ipAddressId == 0)
                return null;

            
            var query = from ba in _context.BannedIpAddresses
                        where ba.BannedIpAddressId == ipAddressId
                        select ba;
            var bannedIpAddress = query.SingleOrDefault();

            return bannedIpAddress;
        }

        /// <summary>
        /// Gets all IP addresses
        /// </summary>
        /// <returns>An IP address collection</returns>
        public List<BannedIpAddress> GetBannedIpAddressAll()
        {
            string key = BLACKLIST_ALLIP_KEY;
            object obj2 = _cacheManager.Get(key);
            if (CacheEnabled && (obj2 != null))
            {
                return (List<BannedIpAddress>)obj2;
            }

            
            var query = from ba in _context.BannedIpAddresses
                        orderby ba.BannedIpAddressId
                        select ba;
            var collection = query.ToList();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, collection);
            }
            return collection;
        }

        /// <summary>
        /// Inserts an IP address
        /// </summary>
        /// <param name="ipAddress">IP address</param>
        /// <returns>IP Address</returns>
        public void InsertBannedIpAddress(BannedIpAddress ipAddress)
        {
            if (ipAddress == null)
                throw new ArgumentNullException("ipAddress");
            
            ipAddress.Address = CommonHelper.EnsureNotNull(ipAddress.Address);
            ipAddress.Address = ipAddress.Address.Trim();
            ipAddress.Address = CommonHelper.EnsureMaximumLength(ipAddress.Address, 50);
            ipAddress.Comment = CommonHelper.EnsureNotNull(ipAddress.Comment);
            ipAddress.Comment = CommonHelper.EnsureMaximumLength(ipAddress.Comment, 500);

            

            _context.BannedIpAddresses.AddObject(ipAddress);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(BLACKLIST_IP_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates an IP address
        /// </summary>
        /// <param name="ipAddress">IP address</param>
        /// <returns>IP address</returns>
        public void UpdateBannedIpAddress(BannedIpAddress ipAddress)
        {
            if (ipAddress == null)
                throw new ArgumentNullException("ipAddress");
            
            ipAddress.Address = CommonHelper.EnsureNotNull(ipAddress.Address);
            ipAddress.Address = ipAddress.Address.Trim();
            ipAddress.Address = CommonHelper.EnsureMaximumLength(ipAddress.Address, 50);
            ipAddress.Comment = CommonHelper.EnsureNotNull(ipAddress.Comment);
            ipAddress.Comment = CommonHelper.EnsureMaximumLength(ipAddress.Comment, 500);

            
            if (!_context.IsAttached(ipAddress))
                _context.BannedIpAddresses.Attach(ipAddress);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(BLACKLIST_IP_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Deletes an IP address by its identifier
        /// </summary>
        /// <param name="ipAddressId">IP address unique identifier</param>
        public void DeleteBannedIpAddress(int ipAddressId)
        {
            var ipAddress = GetBannedIpAddressById(ipAddressId);
            if (ipAddress == null)
                return;

            
            if (!_context.IsAttached(ipAddress))
                _context.BannedIpAddresses.Attach(ipAddress);
            _context.DeleteObject(ipAddress);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(BLACKLIST_IP_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets an IP network by its Id
        /// </summary>
        /// <param name="bannedIpNetworkId">IP network unique identifier</param>
        /// <returns>IP network</returns>
        public BannedIpNetwork GetBannedIpNetworkById(int bannedIpNetworkId)
        {
            if (bannedIpNetworkId == 0)
                return null;

            
            var query = from bn in _context.BannedIpNetworks
                        where bn.BannedIpNetworkId == bannedIpNetworkId
                        select bn;
            var ipNetwork = query.SingleOrDefault();

            return ipNetwork;
        }

        /// <summary>
        /// Gets all IP networks
        /// </summary>
        /// <returns>IP network collection</returns>
        public List<BannedIpNetwork> GetBannedIpNetworkAll()
        {
            string key = BLACKLIST_ALLNETWORK_KEY;
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<BannedIpNetwork>)obj2;
            }

            
            var query = from bn in _context.BannedIpNetworks
                        orderby bn.BannedIpNetworkId
                        select bn;
            var collection = query.ToList();
            
            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, collection);
            }
            return collection;
        }

        /// <summary>
        /// Inserts an IP network
        /// </summary>
        /// <param name="ipNetwork">IP network</param>
        public void InsertBannedIpNetwork(BannedIpNetwork ipNetwork)
        {
            if (ipNetwork == null)
                throw new ArgumentNullException("ipNetwork");
            
            ipNetwork.StartAddress = CommonHelper.EnsureNotNull(ipNetwork.StartAddress);
            ipNetwork.StartAddress = ipNetwork.StartAddress.Trim();
            ipNetwork.StartAddress = CommonHelper.EnsureMaximumLength(ipNetwork.StartAddress, 50);
            ipNetwork.EndAddress = CommonHelper.EnsureNotNull(ipNetwork.EndAddress);
            ipNetwork.EndAddress = ipNetwork.EndAddress.Trim();
            ipNetwork.EndAddress = CommonHelper.EnsureMaximumLength(ipNetwork.EndAddress, 50);
            ipNetwork.Comment = CommonHelper.EnsureNotNull(ipNetwork.Comment);
            ipNetwork.Comment = CommonHelper.EnsureMaximumLength(ipNetwork.Comment, 500);
            ipNetwork.IpException = CommonHelper.EnsureNotNull(ipNetwork.IpException);

            
            
            _context.BannedIpNetworks.AddObject(ipNetwork);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(BLACKLIST_NETWORK_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates an IP network
        /// </summary>
        /// <param name="ipNetwork">IP network</param>
        public void UpdateBannedIpNetwork(BannedIpNetwork ipNetwork)
        {
            if (ipNetwork == null)
                throw new ArgumentNullException("ipNetwork");

            ipNetwork.StartAddress = CommonHelper.EnsureNotNull(ipNetwork.StartAddress);
            ipNetwork.StartAddress = ipNetwork.StartAddress.Trim();
            ipNetwork.StartAddress = CommonHelper.EnsureMaximumLength(ipNetwork.StartAddress, 50);
            ipNetwork.EndAddress = CommonHelper.EnsureNotNull(ipNetwork.EndAddress);
            ipNetwork.EndAddress = ipNetwork.EndAddress.Trim();
            ipNetwork.EndAddress = CommonHelper.EnsureMaximumLength(ipNetwork.EndAddress, 50);
            ipNetwork.Comment = CommonHelper.EnsureNotNull(ipNetwork.Comment);
            ipNetwork.Comment = CommonHelper.EnsureMaximumLength(ipNetwork.Comment, 500);
            ipNetwork.IpException = CommonHelper.EnsureNotNull(ipNetwork.IpException);

            
            if (!_context.IsAttached(ipNetwork))
                _context.BannedIpNetworks.Attach(ipNetwork);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(BLACKLIST_NETWORK_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Deletes an IP network
        /// </summary>
        /// <param name="bannedIpNetwork">IP network unique identifier</param>
        public void DeleteBannedIpNetwork(int bannedIpNetwork)
        {
            var ipNetwork = GetBannedIpNetworkById(bannedIpNetwork);
            if (ipNetwork == null)
                return;

            
            if (!_context.IsAttached(ipNetwork))
                _context.BannedIpNetworks.Attach(ipNetwork);
            _context.DeleteObject(ipNetwork);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(BLACKLIST_NETWORK_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Checks if an IP from the IpAddressCollection or the IpNetworkCollection is banned
        /// </summary>
        /// <param name="ipAddress">IP address</param>
        /// <returns>False or true</returns>
        public bool IsIpAddressBanned(BannedIpAddress ipAddress)
        {
            // Check if the IP is valid
            if (!IsValidIp(ipAddress.Address.Trim()))
                throw new NopException("The following isn't a valid IP address: " + ipAddress.Address);

            // Check if the IP is in the banned IP addresses
            var ipAddressCollection = GetBannedIpAddressAll();
            //if (ipAddressCollection.Contains(ipAddress))
            foreach (var ip in ipAddressCollection)
                if (IsEqual(ipAddress.Address, ip.Address))
                    return true;

            // Check if the IP is in the banned IP networks
            var ipNetworkCollection = GetBannedIpNetworkAll();

            foreach (var ipNetwork in ipNetworkCollection)
            {
                // Get the first and last IPs in the network
                string[] rangeItem = ipNetwork.ToString().Split("-".ToCharArray());
                
                // Get the exceptions as a list
                var exceptionItem = new List<string>();
                exceptionItem.AddRange(ipNetwork.IpException.Split(";".ToCharArray()));
                // Check if the IP is an exception 
                if(exceptionItem.Contains(ipAddress.Address))
                    return false;

                // Check if the 1st IP is valid
                if (!IsValidIp(rangeItem[0].Trim()))
                    throw new NopException("The following isn't a valid IP address: " + rangeItem[0]);

                // Check if the 2nd IP is valid
                if (!IsValidIp(rangeItem[1].Trim()))
                    throw new NopException("The following isn't a valid IP address: " + rangeItem[1]);

                //Check if the IP is in the given range
                if (IsGreaterOrEqual(ipAddress.Address, rangeItem[0].Trim())
                    && IsLessOrEqual(ipAddress.Address, rangeItem[1].Trim()))
                    return true;
            }
            // Return false otherwise
            return false;
        }

        /// <summary>
        /// Check if the ip is valid.
        /// </summary>
        /// <param name="ipAddress">The string representation of an IP address</param>
        /// <returns>True if the IP is valid.</returns>
        public bool IsValidIp(string ipAddress)
        {
            try
            {
                System.Net.IPAddress.Parse(ipAddress);
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.BlacklistManager.CacheEnabled");
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Configuration;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using IPNumbers;
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;


namespace Boleto.Infra.Security
{
    /// <summary>
    /// Filter by IP address
    /// </summary>
    public class FilterIpAttribute : AuthorizeAttribute
    {

        #region Allowed
        /// <summary>
        /// Comma seperated string of allowable IPs. Example "10.2.5.41,192.168.0.22"
        /// </summary>
        /// <value></value>
        public string AllowedSingleIPs { get; set; }

        /// <summary>
        /// Comma seperated string of allowable IPs with masks. Example "10.2.0.0;255.255.0.0,10.3.0.0;255.255.0.0"
        /// </summary>
        /// <value>The masked I ps.</value>
        public string AllowedMaskedIPs { get; set; }

        /// <summary>
        /// Gets or sets the configuration key for allowed single IPs
        /// </summary>
        /// <value>The configuration key single I ps.</value>
        public string ConfigurationKeyAllowedSingleIPs { get; set; }

        /// <summary>
        /// Gets or sets the configuration key allowed mmasked IPs
        /// </summary>
        /// <value>The configuration key masked I ps.</value>
        public string ConfigurationKeyAllowedMaskedIPs { get; set; }

        /// <summary>
        /// List of allowed IPs
        /// </summary>
        readonly IPList _allowedIpListToCheck = new IPList();
        #endregion

        #region Denied
        /// <summary>
        /// Comma seperated string of denied IPs. Example "10.2.5.41,192.168.0.22"
        /// </summary>
        /// <value></value>
        public string DeniedSingleIPs { get; set; }

        /// <summary>
        /// Comma seperated string of denied IPs with masks. Example "10.2.0.0;255.255.0.0,10.3.0.0;255.255.0.0"
        /// </summary>
        /// <value>The masked I ps.</value>
        public string DeniedMaskedIPs { get; set; }


        /// <summary>
        /// Gets or sets the configuration key for denied single IPs
        /// </summary>
        /// <value>The configuration key single I ps.</value>
        public string ConfigurationKeyDeniedSingleIPs { get; set; }

        /// <summary>
        /// Gets or sets the configuration key for denied masked IPs
        /// </summary>
        /// <value>The configuration key masked I ps.</value>
        public string ConfigurationKeyDeniedMaskedIPs { get; set; }

        /// <summary>
        /// List of denied IPs
        /// </summary>
        readonly IPList _deniedIpListToCheck = new IPList();
        #endregion


        /// <summary>
        /// Determines whether access to the core framework is authorized.
        /// </summary>
        /// <param name="actionContext">The HTTP context, which encapsulates all HTTP-specific information about an individual HTTP request.</param>
        /// <returns>
        /// true if access is authorized; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="httpContext"/> parameter is null.</exception>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (actionContext == null)
                throw new ArgumentNullException("actionContext");

            try
            {
                var userIpAddress =
                    HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',')
                        .Select(s => s.Trim())
                        .First();
                
                // Check that the IP is allowed to access
                bool ipAllowed = CheckAllowedIPs(userIpAddress);

                // Check that the IP is not denied to access
                bool ipDenied = CheckDeniedIPs(userIpAddress);

                // Only allowed if allowed and not denied
                bool finallyAllowed = ipAllowed && !ipDenied;

                return finallyAllowed;
            }
            catch (Exception e)
            {
                Log.Write(e.Message + ": " + e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Checks the allowed IPs.
        /// </summary>
        /// <param name="userIpAddress">The user ip address.</param>
        /// <returns></returns>
        private bool CheckAllowedIPs(string userIpAddress)
        {
            // Populate the IPList with the Single IPs
            if (!string.IsNullOrEmpty(AllowedSingleIPs))
            {
                SplitAndAddSingleIPs(AllowedSingleIPs, _allowedIpListToCheck);
            }

            // Populate the IPList with the Masked IPs
            if (!string.IsNullOrEmpty(AllowedMaskedIPs))
            {
                SplitAndAddMaskedIPs(AllowedMaskedIPs, _allowedIpListToCheck);
            }

            // Check if there are more settings from the configuration (Web.config)
            if (!string.IsNullOrEmpty(ConfigurationKeyAllowedSingleIPs))
            {
                string configurationAllowedAdminSingleIPs = ConfigurationManager.AppSettings[ConfigurationKeyAllowedSingleIPs];
                if (!string.IsNullOrEmpty(configurationAllowedAdminSingleIPs))
                {
                    SplitAndAddSingleIPs(configurationAllowedAdminSingleIPs, _allowedIpListToCheck);
                }
            }

            if (!string.IsNullOrEmpty(ConfigurationKeyAllowedMaskedIPs))
            {
                string configurationAllowedAdminMaskedIPs = ConfigurationManager.AppSettings[ConfigurationKeyAllowedMaskedIPs];
                if (!string.IsNullOrEmpty(configurationAllowedAdminMaskedIPs))
                {
                    SplitAndAddMaskedIPs(configurationAllowedAdminMaskedIPs, _allowedIpListToCheck);
                }
            }

            return _allowedIpListToCheck.CheckNumber(userIpAddress);
        }

        /// <summary>
        /// Checks the denied IPs.
        /// </summary>
        /// <param name="userIpAddress">The user ip address.</param>
        /// <returns></returns>
        private bool CheckDeniedIPs(string userIpAddress)
        {
            // Populate the IPList with the Single IPs
            if (!string.IsNullOrEmpty(DeniedSingleIPs))
            {
                SplitAndAddSingleIPs(DeniedSingleIPs, _deniedIpListToCheck);
            }

            // Populate the IPList with the Masked IPs
            if (!string.IsNullOrEmpty(DeniedMaskedIPs))
            {
                SplitAndAddMaskedIPs(DeniedMaskedIPs, _deniedIpListToCheck);
            }

            // Check if there are more settings from the configuration (Web.config)
            if (!string.IsNullOrEmpty(ConfigurationKeyDeniedSingleIPs))
            {
                string configurationDeniedAdminSingleIPs = ConfigurationManager.AppSettings[ConfigurationKeyDeniedSingleIPs];
                if (!string.IsNullOrEmpty(configurationDeniedAdminSingleIPs))
                {
                    SplitAndAddSingleIPs(configurationDeniedAdminSingleIPs, _deniedIpListToCheck);
                }
            }

            if (!string.IsNullOrEmpty(ConfigurationKeyDeniedMaskedIPs))
            {
                string configurationDeniedAdminMaskedIPs = ConfigurationManager.AppSettings[ConfigurationKeyDeniedMaskedIPs];
                if (!string.IsNullOrEmpty(configurationDeniedAdminMaskedIPs))
                {
                    SplitAndAddMaskedIPs(configurationDeniedAdminMaskedIPs, _deniedIpListToCheck);
                }
            }

            return _deniedIpListToCheck.CheckNumber(userIpAddress);
        }

        /// <summary>
        /// Splits the incoming ip string of the format "IP,IP" example "10.2.0.0,10.3.0.0" and adds the result to the IPList
        /// </summary>
        /// <param name="ips">The ips.</param>
        /// <param name="list">The list.</param>
        private void SplitAndAddSingleIPs(string ips, IPList list)
        {
            var splitSingleIPs = ips.Split(',');
            foreach (string ip in splitSingleIPs)
                list.Add(ip);
        }

        /// <summary>
        /// Splits the incoming ip string of the format "IP;MASK,IP;MASK" example "10.2.0.0;255.255.0.0,10.3.0.0;255.255.0.0" and adds the result to the IPList
        /// </summary>
        /// <param name="ips">The ips.</param>
        /// <param name="list">The list.</param>
        private void SplitAndAddMaskedIPs(string ips, IPList list)
        {
            var splitMaskedIPs = ips.Split(',');
            foreach (string maskedIp in splitMaskedIPs)
            {
                var ipAndMask = maskedIp.Split(';');
                list.Add(ipAndMask[0], ipAndMask[1]); // IP;MASK
            }
        }
    }
}

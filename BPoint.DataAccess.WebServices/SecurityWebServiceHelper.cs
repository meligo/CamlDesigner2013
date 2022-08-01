using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPoint.SharePoint.DataAccess
{
    public class SecurityWebServiceHelper
    {
        private string sharePointUrl = null;
        private  string username = null;
        private string password = null;
        private string domain = null;
        private bool useDefaultCredentials = false;

        private SecurityWebService.Security securityWebService = null;
        private System.Net.CookieCollection authCookies = null;

        #region Constructors
        public SecurityWebServiceHelper(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new Exception("SharePoint URL cannot be null or empty.");

            this.useDefaultCredentials = true;
            InitializeWebService(url);
        }

        public SecurityWebServiceHelper(string url, string username, string password, string domain)
        {
            if (string.IsNullOrEmpty(url))
                throw new Exception("SharePoint URL cannot be null or empty.");

            this.useDefaultCredentials = false;
            this.username = username;
            this.password = password;
            this.domain = domain;

            // Initialize web services
            InitializeWebService(url);
        }
        #endregion

        #region Public Properties

        public string SharePointUrl
        {
            get { return sharePointUrl; }
            set 
            {
                RefreshWebService(value);
            }
        }

        public SecurityWebService.Security SecurityWebService
        {
            get
            {
                if (securityWebService == null)
                {
                    InitializeWebService(null);
                }
                return securityWebService;
            }
        }

        public System.Net.CookieCollection AuthenticatedCookies
        {
            get { return authCookies; }
            set
            {
                if (value != null)
                {
                    SecurityWebService.CookieContainer = new System.Net.CookieContainer();
                    SecurityWebService.CookieContainer.Add(value);
                }
                else
                    SecurityWebService.CookieContainer = null;
            }
        }
        #endregion

        #region Public Methods
        public string GetFolderPermissions(string folderUrl)
        {
            return SecurityWebService.GetFolderPermissions(folderUrl);
        }
        #endregion

        #region Private Methods
        private void InitializeWebService(string url)
        {
            if (!string.IsNullOrEmpty(url))
                sharePointUrl = String.Format("{0}/_vti_bin/SpikesSecurity.asmx", url);

            if (string.IsNullOrEmpty(sharePointUrl))
                throw new Exception("SharePoint URL cannot be null or empty.");

            securityWebService = new SecurityWebService.Security();
            securityWebService.Url = sharePointUrl;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                if (string.IsNullOrEmpty(domain))
                    securityWebService.Credentials = new System.Net.NetworkCredential(username, password);
                else
                    securityWebService.Credentials = new System.Net.NetworkCredential(username, password, domain);
            }
            else
            {
                securityWebService.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }

            // TODO: Test a method to see if the user has access
        }

        private void RefreshWebService(string url)
        {
            if (!string.IsNullOrEmpty(sharePointUrl) && sharePointUrl != url)
            {
                sharePointUrl = String.Format("{0}/_vti_bin/SecuritySpikes.asmx", url);
                if (securityWebService.Url != sharePointUrl)
                    securityWebService.Url = sharePointUrl;
            }
        }
        #endregion
    }
}

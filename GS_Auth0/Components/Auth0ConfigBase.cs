using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Authentication;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace GS.Auth0.Components
{
    /// <summary>
    /// The Config class provides a central area for management of Module Configuration Settings.
    /// </summary>
    [Serializable]
    public class Auth0ConfigBase : AuthenticationConfigBase
    {
        #region "Constructors"        
        protected Auth0ConfigBase(string service, int portalId)
            : base(portalId)
        {
            Service = service;

            Domain = PortalController.GetPortalSetting(this.Service + "_Domain", portalId, "");
            ClientID = PortalController.GetPortalSetting(Service + "_ClientID", portalId, "");
            ClientSecret = PortalController.GetPortalSetting(Service + "_ClientSecret", portalId, "");
            RedirectUri = PortalController.GetPortalSetting(Service + "_RedirectUri", portalId, "");
            PostLogoutRedirectUri = PortalController.GetPortalSetting(Service + "_PostLogoutRedirectUri", portalId, "");
            IsEnabled = PortalController.GetPortalSettingAsBoolean(Service + "_Enabled", portalId, false);
            IsDiagnosticModeEnabled = PortalController.GetPortalSettingAsBoolean(Service + "_IsDiagnosticModeEnabled", portalId, true);
        }
        #endregion

        #region "Private properties"
        private string Service { get; set; }
        #endregion

        #region "Public properties"
        /// <summary>
        /// Flag that determines whether provider is enabled or disabled.
        /// </summary>
        [DotNetNuke.UI.WebControls.SortOrder(0)]
        public bool IsEnabled { get; set; }

        [DotNetNuke.UI.WebControls.SortOrder(1)]
        public string Domain { get; set; }

        [DotNetNuke.UI.WebControls.SortOrder(2)]
        public string ClientID { get; set; }

        [DotNetNuke.UI.WebControls.SortOrder(3)]
        public string ClientSecret { get; set; }

        [DotNetNuke.UI.WebControls.IsReadOnly(false)]
        [DotNetNuke.UI.WebControls.SortOrder(4)]
        public string RedirectUri { get; set; }

        [DotNetNuke.UI.WebControls.IsReadOnly(false)]
        [DotNetNuke.UI.WebControls.SortOrder(5)]
        public string PostLogoutRedirectUri { get; set; }

        /// <summary>
        /// Flag that determines if diagnostic info should be saved in DNN log file
        /// </summary>
        [DotNetNuke.UI.WebControls.SortOrder(6)]
        public bool IsDiagnosticModeEnabled { get; set; }


        /// <summary>
        /// Check if provider has defined all coordinates that are necessary to initialize login flow.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public bool AreProviderSettingsCorrect
        {
            get
            {
                return !(
                       string.IsNullOrEmpty(Domain)
                    || string.IsNullOrEmpty(ClientID)
                    || string.IsNullOrEmpty(ClientSecret)
                    || string.IsNullOrEmpty(RedirectUri)
                    || string.IsNullOrEmpty(PostLogoutRedirectUri)
                    );
            }
        }

        #endregion

        #region "Public methods"
        public static void ClearConfig(string service, int portalId)
        {
            DataCache.RemoveCache(GetCacheKey(service, portalId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service">Unique string required to create cache key</param>
        /// <param name="portalId"></param>
        /// <returns></returns>
        public static Auth0ConfigBase GetConfig(string service, int portalId)
        {
            string key = GetCacheKey(service, portalId);
            object _cachedConfig = DataCache.GetCache(key);
            Auth0ConfigBase config = (Auth0ConfigBase)_cachedConfig;
            if (config == null)
            {
                config = new Auth0ConfigBase(service, portalId);
                DataCache.SetCache(key, config);
            }
            return config;
        }

        public static void UpdateConfig(Auth0ConfigBase config)
        {
            PortalController.UpdatePortalSetting(config.PortalID, config.Service + "_Domain", config.Domain);
            PortalController.UpdatePortalSetting(config.PortalID, config.Service + "_ClientID", config.ClientID);
            PortalController.UpdatePortalSetting(config.PortalID, config.Service + "_ClientSecret", config.ClientSecret);
            PortalController.UpdatePortalSetting(config.PortalID, config.Service + "_RedirectUri", config.RedirectUri);
            PortalController.UpdatePortalSetting(config.PortalID, config.Service + "_PostLogoutRedirectUri", config.PostLogoutRedirectUri);
            PortalController.UpdatePortalSetting(config.PortalID, config.Service + "_Enabled", config.IsEnabled.ToString(CultureInfo.InvariantCulture));
            PortalController.UpdatePortalSetting(config.PortalID, config.Service + "_IsDiagnosticModeEnabled", config.IsDiagnosticModeEnabled.ToString(CultureInfo.InvariantCulture));

            ClearConfig(config.Service, config.PortalID);
        }
        #endregion

        #region "Private Methods"
        private static string GetCacheKey(string service, int portalId)
        {
            const string _cacheKey = "Authentication";
            return _cacheKey + "." + service + "_" + portalId;
        }
        #endregion

    }
}
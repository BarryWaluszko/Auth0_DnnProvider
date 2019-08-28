/*
' Copyright (c) 2019 Glanton
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Security;
using DotNetNuke.Security.Membership;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Common;
using System.Web;
using DotNetNuke.Entities.Host;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Authentication.OAuth;
using System.Net;
using GS.Auth0.Components;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using DotNetNuke.Instrumentation;
using System.Text.RegularExpressions;

namespace GS.Auth0
{
    public partial class LogIn : OAuthLoginBase
    {
        private static readonly ILog logger = LoggerSource.Instance.GetLogger(typeof(LogIn));

        #region Properties
        protected override string AuthSystemApplicationName
        {
            get { return Constants.PROVIDER_NAME; }
        }

        /// <summary>
        /// Flag that determines whether 'Auth0 Connector' is enabled for this portal
        /// </summary>
        public override bool Enabled
        {
            get
            {
                Auth0ConfigBase _providerConfig = Auth0ConfigBase.GetConfig(Constants.PROVIDER_NAME, PortalId);
                return _providerConfig.AreProviderSettingsCorrect && _providerConfig.IsEnabled;
            }
        }


        /// <summary>
        /// Return alert message if there is any
        /// </summary>
        private string AlertMessage
        {
            get
            {
                return Request.QueryString[Constants.ALERT_QUERY_STRING] != null ?
                    Request.QueryString[Constants.ALERT_QUERY_STRING] :
                string.Empty;
            }
        }

        /// <summary>
        /// Check if OWIN pipeline is enabled in DNN web.config file
        /// </summary>
        private bool OwinStatus
        {
            get
            {
                return
                    System.Web.Configuration.WebConfigurationManager.AppSettings["owin:appStartup"] != null
                    && System.Web.Configuration.WebConfigurationManager.AppSettings["owin:appStartup"].ToString() == Constants.AUTH_TYPE;
            }
        }
        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            this.ModuleConfiguration.ModuleControl.SupportsPartialRendering = false;
            base.OnInit(e);

            #region "SSL settings"
            // Remove insecure protocols (SSL3, TLS 1.0, TLS 1.1)
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Ssl3;
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls;
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls11;
            // Add TLS 1.2
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            #endregion

            this.loginButton.Click += new EventHandler(loginButton_Click);

            OAuthClient = new Auth0Client(PortalId, Mode);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            #region "Adjust 'Query String' for OIDC"
            //OIDC needs "ReturnUrl" in the url. I don't know why, anyone can help?
            string newUrl = string.Empty;
            if (Regex.IsMatch(Request.RawUrl, Constants.OIDC_RETURN_URL, RegexOptions.IgnoreCase)
                && !Regex.IsMatch(Request.RawUrl, Constants.OIDC_RETURN_URL))
                newUrl = Regex.Replace(Request.RawUrl, Constants.OIDC_RETURN_URL, Constants.OIDC_RETURN_URL, RegexOptions.IgnoreCase); //make ReturnUrl upercases

            else if (!Regex.IsMatch(Request.RawUrl, Constants.OIDC_RETURN_URL, RegexOptions.IgnoreCase))
                newUrl = Request.RawUrl + (Regex.IsMatch(Request.RawUrl, "\\?") ? "&" : "?") + Constants.OIDC_RETURN_URL + "=%2f"; //add query string param "ReturnUrl"

            if (!string.IsNullOrEmpty(newUrl))
                Response.Redirect(newUrl);    //TODO: why QueryString needs to have 'ReturnUrl' parameter??
            #endregion

            if (!this.Page.IsPostBack)
            {
                //display OIDC errors, if there are any
                if (!string.IsNullOrEmpty(AlertMessage))
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, AlertMessage, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                    logger.Warn(AlertMessage);
                }
                if (!OwinStatus && Enabled)
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Enable OWIN in web.config file to utilize Auth0 provider!", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
                }
            }
        }

        protected void loginButton_Click(object sender, EventArgs e)
        {
            try
            {
                IOwinContext ctx = Request.GetOwinContext();
                if (ctx == null)
                    logger.Warn("Request.GetOwinContext() not found!");
                else if (ctx.Authentication == null)
                    logger.Warn("Request.GetOwinContext().Authentication is null");
                else
                {
                    string returnUri = Request.QueryString[Constants.OIDC_RETURN_URL] != null ? Request.QueryString[Constants.OIDC_RETURN_URL] : "/";
                    ctx.Authentication.Challenge(new Microsoft.Owin.Security.AuthenticationProperties { RedirectUri = returnUri }, Constants.AUTH_TYPE); // Send an OpenID Connect sign-in request.
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        #endregion

        #region "Private methods"
        protected override UserData GetCurrentUser()
        {
            return OAuthClient.GetCurrentUser<Auth0UserData>();
        }
        #endregion
    }
}

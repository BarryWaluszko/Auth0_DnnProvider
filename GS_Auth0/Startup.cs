using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using GS.Auth0.Components;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

[assembly: Microsoft.Owin.OwinStartupAttribute(GS.Auth0.Constants.PROVIDER_NAME, typeof(GS.Auth0.Startup))]

namespace GS.Auth0
{
    public class Startup
    {
        private static readonly ILog logger = LoggerSource.Instance.GetLogger(typeof(Startup));

        public void Configuration(IAppBuilder app)
        {
            try
            {
                #region "SSL settings"
                // Remove insecure protocols (SSL3, TLS 1.0, TLS 1.1)
                ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Ssl3;
                ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls;
                ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls11;
                // Add TLS 1.2
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                #endregion

                Auth0ConfigBase config = Auth0ConfigBase.GetConfig(Constants.PROVIDER_NAME, Helpers.FirstPortalID);

                System.Web.Helpers.AntiForgeryConfig.UniqueClaimTypeIdentifier = System.Security.Claims.ClaimTypes.NameIdentifier;

                // Configure Auth0 parameters
                string auth0Domain = config.Domain;
                string auth0ClientId = config.ClientID;

                // Enable the Cookie saver middleware to work around a bug in the OWIN implementation
                app.UseKentorOwinCookieSaver();

                // Set Cookies as default authentication type
                app.SetDefaultSignInAsAuthenticationType(Constants.AUTH_TYPE);
                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationType = Constants.AUTH_TYPE,
                    CookieName = Constants.AUTH_COOKIE_NAME,
                });


                // Configure Auth0 authentication
                app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
                {
                    AuthenticationType = Constants.AUTH_TYPE,
                    Authority = $"https://{auth0Domain}",
                    ClientId = auth0ClientId,
                    Scope = "openid profile email",
                    ResponseType = OpenIdConnectResponseType.CodeIdToken,

                    TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier
                    },

                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        RedirectToIdentityProvider = notification =>
                        {
                            DotNetNuke.Entities.Portals.PortalSettings _portalSettings = null;

                            #region "Get settings from current DNN portal"                            
                            if (notification.OwinContext.Environment["System.Web.HttpContextBase"] != null
                            && notification.OwinContext.Environment["System.Web.HttpContextBase"] is System.Web.HttpContextWrapper)
                            {
                                System.Web.HttpContextWrapper context = notification.OwinContext.Environment["System.Web.HttpContextBase"] as System.Web.HttpContextWrapper;
                                if (context.Items["PortalSettings"] != null
                                && context.Items["PortalSettings"] is DotNetNuke.Entities.Portals.PortalSettings)
                                {
                                    _portalSettings = context.Items["PortalSettings"] as DotNetNuke.Entities.Portals.PortalSettings;
                                }
                            }
                            #endregion

                            #region "Get provider settings"                               
                            Auth0ConfigBase _providerConfig = null;
                            if (_portalSettings != null)
                                _providerConfig = Auth0ConfigBase.GetConfig(Constants.PROVIDER_NAME, _portalSettings.PortalId);
                            else
                                logger.Debug("Can't obtain DNN settings, login process terminated!!");
                            #endregion

                            #region "Set Auth0 coordinates according to the current DNN portal"
                            if (_portalSettings != null && notification.ProtocolMessage.RequestType != OpenIdConnectRequestType.Logout)
                            {
                                notification.Options.Authority = $"https://{_providerConfig.Domain}";
                                notification.Options.ClientId = _providerConfig.ClientID;
                                notification.Options.ClientSecret = _providerConfig.ClientSecret;
                                notification.Options.RedirectUri = _providerConfig.RedirectUri;
                                notification.Options.CallbackPath = Microsoft.Owin.PathString.FromUriComponent("/Default.aspx");

                                notification.ProtocolMessage.ClientId = _providerConfig.ClientID;
                                notification.ProtocolMessage.ClientSecret = _providerConfig.ClientSecret;
                                notification.ProtocolMessage.RedirectUri = _providerConfig.RedirectUri;
                            }
                            #endregion

                            #region "Log-off code snippet"
                            else if (notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                            {
                                var logoutUri = $"https://{_providerConfig.Domain}/v2/logout?client_id={_providerConfig.ClientID}";
                                var postLogoutUri = _providerConfig.PostLogoutRedirectUri;
                                if (!string.IsNullOrEmpty(postLogoutUri))
                                {
                                    if (postLogoutUri.StartsWith("/"))
                                    {
                                        // transform to absolute
                                        var request = notification.Request;
                                        postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                                    }
                                    logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
                                }
                                notification.Response.Redirect(logoutUri);
                                notification.HandleResponse();
                            }
                            #endregion

                            #region "Output some diagnistic info"
                            if (_providerConfig != null && _providerConfig.IsDiagnosticModeEnabled)
                            {
                                logger.Debug(string.Format("Redirecting to '{0}' using following coordinates:", notification.Options.Authority));
                                logger.Debug("Client id: " + notification.Options.ClientId);
                                logger.Debug("Redirect uri: " + notification.Options.RedirectUri);
                                logger.Debug("Callback path: " + notification.Options.CallbackPath);
                            }
                            #endregion

                            return Task.FromResult(0);
                        },

                        AuthorizationCodeReceived = async context =>
                        {
                            DotNetNuke.Entities.Portals.PortalSettings _portalSettings = null;

                            #region "Get settings from current DNN portal"
                            if (context.OwinContext.Environment["System.Web.HttpContextBase"] != null
                            && context.OwinContext.Environment["System.Web.HttpContextBase"] is System.Web.HttpContextWrapper)
                            {
                                System.Web.HttpContextWrapper _context = context.OwinContext.Environment["System.Web.HttpContextBase"] as System.Web.HttpContextWrapper;
                                if (_context.Items["PortalSettings"] != null
                                && _context.Items["PortalSettings"] is DotNetNuke.Entities.Portals.PortalSettings)
                                {
                                    _portalSettings = _context.Items["PortalSettings"] as DotNetNuke.Entities.Portals.PortalSettings;
                                }
                            }
                            #endregion

                            #region "Get provider settings"
                            Auth0ConfigBase _providerConfig = null;
                            if (_portalSettings != null)
                                _providerConfig = Auth0ConfigBase.GetConfig(Constants.PROVIDER_NAME, _portalSettings.PortalId);
                            else
                                throw new ArgumentNullException("Can't obtain DNN settings, login process terminated!!");
                            #endregion

                            GS.Auth0.Components.UserController userController = new GS.Auth0.Components.UserController();

                            //get or create DNN user
                            DotNetNuke.Entities.Users.UserInfo _userInfo = userController.User_Create(context.AuthenticationTicket.Identity.Name, _portalSettings, _providerConfig.IsDiagnosticModeEnabled);

                            if (_userInfo != null)
                            {
                                //update DNN user profile
                                userController.User_Update(
                                _userInfo,
                                context.AuthenticationTicket.Identity?.FindFirst(c => c.Type == "nickname")?.Value,
                                context.AuthenticationTicket.Identity?.FindFirst(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value,
                                _portalSettings.PortalId,
                                _providerConfig.IsDiagnosticModeEnabled);

                                DotNetNuke.Security.Membership.UserLoginStatus loginStatus = DotNetNuke.Security.Membership.UserLoginStatus.LOGIN_FAILURE;
                                UserInfo objUserInfo = DotNetNuke.Entities.Users.UserController.ValidateUser(_portalSettings.PortalId, context.AuthenticationTicket.Identity.Name, "",
                                                                Constants.PROVIDER_NAME, "",
                                                                _portalSettings.PortalName, "",
                                                                ref loginStatus);

                                //set type of current authentication provider
                                DotNetNuke.Services.Authentication.AuthenticationController.SetAuthenticationType(Constants.AUTH_TYPE);
                                DotNetNuke.Entities.Users.UserController.UserLogin(_portalSettings.PortalId, _userInfo, _portalSettings.PortalName, context.OwinContext.Request.RemoteIpAddress, false);
                            }
                            else
                                throw new ArgumentNullException(string.Format("Can't create or get user '{0}' from DNN.", context.AuthenticationTicket.Identity.Name));

                            await Task.FromResult(0);
                        },

                        AuthenticationFailed = (context) =>
                        {
                            //get the error message and send it to the DNN login page
                            DotNetNuke.Entities.Portals.PortalSettings _portalSettings = null;

                            #region "Get settings from current DNN portal"                            
                            if (context.OwinContext.Environment["System.Web.HttpContextBase"] != null
                            && context.OwinContext.Environment["System.Web.HttpContextBase"] is System.Web.HttpContextWrapper)
                            {
                                System.Web.HttpContextWrapper _context = context.OwinContext.Environment["System.Web.HttpContextBase"] as System.Web.HttpContextWrapper;
                                if (_context.Items["PortalSettings"] != null
                                && _context.Items["PortalSettings"] is DotNetNuke.Entities.Portals.PortalSettings)
                                {
                                    _portalSettings = _context.Items["PortalSettings"] as DotNetNuke.Entities.Portals.PortalSettings;
                                }
                            }
                            #endregion

                            #region "Get provider settings"                            
                            Auth0ConfigBase _providerConfig = null;
                            if (_portalSettings != null)
                                _providerConfig = Auth0ConfigBase.GetConfig(Constants.PROVIDER_NAME, _portalSettings.PortalId);
                            else
                                logger.Error("Can't obtain DNN settings from 'AuthenticationFailed' event, login process terminated!!");
                            #endregion

                            if (_providerConfig.IsDiagnosticModeEnabled)
                                logger.Error(string.Format("OIDC authentication failed, details: {0}", context.Exception));

                            string redirectUrl = DotNetNuke.Common.Globals.NavigateURL(_portalSettings.LoginTabId, "Login", new string[] { Constants.ALERT_QUERY_STRING + "=" + context.Exception.Message });
                            context.Response.Redirect(redirectUrl);
                            context.HandleResponse();
                            return Task.FromResult(0);
                        },

                        #region "Rest of 'Notification' methods, not in use for now."
                        //SecurityTokenValidated = notification =>
                        //{
                        //    return Task.FromResult(0);
                        //},
                        //MessageReceived = (context) =>
                        //{

                        //    return Task.FromResult(0);
                        //},
                        #endregion
                    },


                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
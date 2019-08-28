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
using DotNetNuke.Common;
using Microsoft.Owin;
using System.Web;
using DotNetNuke.Instrumentation;

namespace GS.Auth0
{
    #region Properties

    #endregion

    public partial class LogOff : DotNetNuke.Services.Authentication.AuthenticationLogoffBase
    {
        private static readonly ILog logger = LoggerSource.Instance.GetLogger(typeof(LogOff));

        #region Event Handlers

        override protected void OnInit(EventArgs e)
        {
            this.Load += new System.EventHandler(this.Page_Load);
            base.OnInit(e);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// -----------------------------------------------------------------------------
        private void Page_Load(object sender, System.EventArgs e)
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
                    //Remove user from DNN cache
                    if (this.UserInfo != null)
                    {
                        DotNetNuke.Common.Utilities.DataCache.ClearUserCache(this.PortalSettings.PortalId, Context.User.Identity.Name);
                        if (Context.User.Identity.Name != this.UserInfo.Username)
                            DotNetNuke.Common.Utilities.DataCache.ClearUserCache(this.PortalSettings.PortalId, this.UserInfo.Username);
                    }

                    //remove 'authentication' cookie
                    HttpContext.Current.Response.Cookies.Set(new HttpCookie("authentication", "")
                    {
                        Expires = DateTime.Now.AddDays(-1)
                    });

                    new PortalSecurity().SignOut();

                    // Send an OpenID Connect sign-out request.
                    ctx.Authentication.SignOut(
                        new Microsoft.Owin.Security.AuthenticationProperties { RedirectUri = Request.UrlReferrer.AbsoluteUri },
                        Constants.AUTH_TYPE);
                }
            }
            catch (Exception ex) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        #endregion
    }
}
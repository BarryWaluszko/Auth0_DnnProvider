using DotNetNuke.Services.Authentication.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GS.Auth0.Components
{
    public class Auth0Client : OAuthClientBase
    {
        public Auth0Client(int portalId, DotNetNuke.Services.Authentication.AuthMode mode)
            : base(portalId, mode, "xx")
        {
            Service = Constants.PROVIDER_NAME;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GS.Auth0.Api.Auth
{
    public class Auth0AuthMessageHandler : DotNetNuke.Web.Api.Auth.AuthMessageHandlerBase
    {
        public override string AuthScheme => Constants.AUTH_TYPE;

        public Auth0AuthMessageHandler(bool includeByDefault, bool forceSsl)
            : base(includeByDefault, forceSsl)
        {
        }
    }
}
using DotNetNuke.Services.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GS.Auth0.Components
{
    public class Auth0LoginBase : AuthenticationLoginBase
    {
        public override bool Enabled
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
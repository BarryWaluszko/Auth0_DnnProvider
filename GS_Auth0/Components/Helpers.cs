using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GS.Auth0.Components
{
    public class Helpers
    {
        /// <summary>
        /// Find the smallest portalID in the DNN instance
        /// </summary>
        public static int FirstPortalID
        {
            get
            {
                int portalID = Int32.MaxValue;
                DotNetNuke.Entities.Portals.PortalController pc = new DotNetNuke.Entities.Portals.PortalController();
                System.Collections.ArrayList portalList = pc.GetPortals();
                if (portalList != null)
                    foreach (DotNetNuke.Entities.Portals.PortalInfo portal in portalList)
                        if (portal.PortalID < portalID)
                            portalID = portal.PortalID;

                return portalID;
            }
        }
    }
}
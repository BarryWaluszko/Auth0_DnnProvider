using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GS.Auth0.Components
{
    public class UserController
    {
        private static readonly ILog logger = LoggerSource.Instance.GetLogger(typeof(UserController));

        /// <summary>
        /// Get user from DNN cache, if it doesn't exist create him
        /// </summary>
        /// <param name="username"></param>
        /// <param name="portalSettings"></param>
        /// <param name="isDiagnosticModeEnabled">Enable that flag to create logs with info about what this method do</param>
        /// <returns></returns>
        public UserInfo User_Create(string username, PortalSettings portalSettings, bool isDiagnosticModeEnabled)
        {
            DotNetNuke.Entities.Users.UserInfo _userInfo = null;

            if (!string.IsNullOrEmpty(username) && portalSettings != null)
            {
                //check if user already exists in DNN
                _userInfo = DotNetNuke.Entities.Users.UserController.GetCachedUser(portalSettings.PortalId, username);

                //user not exists in DNN, so let's create a user
                if (_userInfo == null)
                {
                    //create user
                    _userInfo = new UserInfo();
                    _userInfo.Username = username;
                    _userInfo.PortalID = portalSettings.PortalId;
                    _userInfo.Membership.Approved = true;
                    _userInfo.Membership.CreatedDate = DateTime.Now;
                    _userInfo.AffiliateID = DotNetNuke.Common.Utilities.Null.NullInteger;
                    _userInfo.IsDeleted = false;
                    _userInfo.Membership.IsDeleted = false;
                    _userInfo.Membership.Password = DotNetNuke.Entities.Users.UserController.GeneratePassword(13);
                    DotNetNuke.Security.Membership.UserCreateStatus userCreateStatus = DotNetNuke.Entities.Users.UserController.CreateUser(ref _userInfo);
                    if (userCreateStatus != DotNetNuke.Security.Membership.UserCreateStatus.Success)
                    {
                        throw new ArgumentException(userCreateStatus.ToString());
                    }
                    if (isDiagnosticModeEnabled)
                        logger.Debug(string.Format("User '{0}' created in DNN portal", _userInfo.Username));
                } else if (isDiagnosticModeEnabled)
                    logger.Debug(string.Format("User '{0}' found in DNN portal", _userInfo.Username));
            }
            return _userInfo;
        }

        /// <summary>
        /// Update the DNN user profile
        /// </summary>
        /// <param name="dnnUser"></param> 
        /// <param name="displayName"></param>
        /// <param name="emailAddress"></param>
        /// <param name="portalID"></param>
        /// <param name="isDiagnosticModeEnabled">Enable that flag to create logs with info about what this method do.</param>
        /// <returns></returns>
        public UserInfo User_Update(UserInfo dnnUser, string displayName, string emailAddress, int portalID, bool isDiagnosticModeEnabled)
        {
            if (dnnUser != null)
            {
                if (isDiagnosticModeEnabled)
                    logger.Debug(string.Format("Updating profile for user '{0}'", dnnUser.Username));

                //dnnUser.FirstName = "xxx";    //for future use
                //dnnUser.LastName = "xxx";     //for future use
                dnnUser.DisplayName = displayName;
                dnnUser.IsDeleted = false;
                dnnUser.Membership.IsDeleted = false;
                dnnUser.Email = emailAddress;              
                dnnUser.Membership.Password = DotNetNuke.Entities.Users.UserController.GeneratePassword();
                dnnUser.Membership.Approved = true;
                dnnUser.AffiliateID = DotNetNuke.Common.Utilities.Null.NullInteger;

                if (string.IsNullOrEmpty(dnnUser.DisplayName))
                    dnnUser.DisplayName = dnnUser.Username;

                if (string.IsNullOrEmpty(dnnUser.FirstName))
                    dnnUser.FirstName = dnnUser.DisplayName;

                DotNetNuke.Entities.Users.UserController.UpdateUser(portalID, dnnUser);
            }
            return dnnUser;
        }
    }
}
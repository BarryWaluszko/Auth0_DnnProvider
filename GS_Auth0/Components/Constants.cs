
namespace GS.Auth0
{
    public class Constants
    {
        /// <summary>
        /// Unique name that was assigned to this Auth0 provider
        /// </summary>
        internal const string PROVIDER_NAME = "GS_Auth0";
        
        internal const string OIDC_RETURN_URL = "ReturnUrl";

        internal const string AUTH_TYPE = "GS_Auth0";
        
        /// <summary>
        /// Name of the authentication cookie where authentication ticket will persist
        /// </summary>
        internal const string AUTH_COOKIE_NAME = ".DOTNETNUKE";

        /// <summary>
        /// Name of the query string key that contains error message
        /// </summary>
        internal const string ALERT_QUERY_STRING = "am";
    }
}
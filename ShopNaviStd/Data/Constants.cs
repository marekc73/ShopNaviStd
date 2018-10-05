using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopNavi.Data
{
    public static class Constants
    {
        public static string AppName = "ShopNaviClient";
        public static string WebAppName = "ShopNaviWebClient";

        // OAuth
        // For Google login, configure at https://console.developers.google.com/
        public const string AndroidClientId = /*"625447915918-7ivpo6od7n02r48gb0i30n4i5t5c2ami"*/"600668473149-uu538hb9gv2dgpeh7mdog6uepu173boh";
        public static string iOSClientId = "<insert IOS client ID here>";
        public static string PostFix = ".apps.googleusercontent.com";

        // These values do not need changing
        public static string Scope = "https://www.googleapis.com/auth/userinfo.email";
        public static string ScopeGmailReadonly = "https://www.googleapis.com/auth/gmail.readonly";
        public static string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
        public static string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        public static string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";
        public static string GmailUrl = "https://www.googleapis.com/gmail/v1/users/me/messages/{0}?key={{1}}";
        public static string MessageList = "https://www.googleapis.com/gmail/v1/users/me/messages?q=subject%3A{0}+from%3A{1}+after%3A{2}";
        public static string MessageGet = "https://www.googleapis.com/gmail/v1/users/me/messages/{0}";
        // Set these to reversed iOS/Android client ids, with :/oauth2redirect appended
        public static string iOSRedirectUrl = "<insert IOS redirect URL here>:/oauth2redirect";
//        public static string AndroidRedirectUrl = "urn:ietf:wg:oauth:2.0:oob";
        public static string AndroidRedirectUrl = string.Format("com.googleusercontent.apps.{0}:/oauth2redirect", StoreFactory.Settings.GmailClientId);
        //public static string AndroidRedirectUrl = "xamarin-auth://localhost";
    }
}

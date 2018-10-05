using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using ShopNavi.Data;
using Xamarin.Auth;
using Xamarin.Forms;

namespace ShopNavi.Droid
{
    public class LoginOAuth : ILoginOAuth
    {
        public LoginOAuth()
        {
            store = AccountStore.Create();
            account = store.FindAccountsForService(Constants.AppName).FirstOrDefault();
        }

        public LoginOAuth(ICreateListVM vm)
        {
            store = AccountStore.Create();
            account = store.FindAccountsForService(Constants.AppName).FirstOrDefault();
            this.vm = vm;
        }

        Account account;
        AccountStore store;
        ICreateListVM vm = null;
        bool isAuthenticated = false;

        public async Task<bool> OnLoginClicked(object sender, EventArgs e)
        {
            //StoreFactory.HalProxy.SignIn();

            //return;
            if (!this.isAuthenticated)
            {
                string clientId = null;
                string redirectUri = null;
                string secret = null;
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        clientId = Constants.iOSClientId;
                        redirectUri = Constants.iOSRedirectUrl;
                        break;

                    case Device.Android:
                        clientId = StoreFactory.Settings.GmailClientId + Constants.PostFix;
                        redirectUri = Constants.AndroidRedirectUrl;
                        break;
                }

                var authenticator = new OAuth2Authenticator(
                    clientId,
                    secret,
                    Constants.Scope + " " + Constants.ScopeGmailReadonly,
                    new Uri(Constants.AuthorizeUrl),
                    new Uri(redirectUri),
                    new Uri(Constants.AccessTokenUrl),
                    null,
                    true);

                AuthenticationState.Authenticator = (OAuth2Authenticator)authenticator;
                authenticator.Completed += OnAuthCompleted;
                authenticator.Error += OnAuthError;

                var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
                presenter.Login(authenticator);
            }
            else
            {
                await ReadFromGmail(store.FindAccountsForService(Constants.AppName).FirstOrDefault(), this.vm);
            }

            return true;
        }

        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            vm.IsRunning = true;

            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }


            if (e.IsAuthenticated)
            {
                StoreFactory.CurrentVM.Logs.Add("Gmail authentcation ok");
                this.isAuthenticated = true;
                await ReadFromGmail(e.Account, this.vm);
            }
        }

        private async Task<int> ReadFromGmail(Account acc, ICreateListVM vm)
        {
            User user = null;

            // If the user is authenticated, request their basic user data from Google
            // UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
            var request = new OAuth2Request("GET", new Uri(Constants.UserInfoUrl), null, acc);
            var response = await request.GetResponseAsync();
            if (response != null)
            {
                StoreFactory.CurrentVM.Logs.Add("Gmail user info received");

                // Deserialize the data and store it in the account store
                // The users email address will be used to identify data in SimpleDB
                string userJson = await response.GetResponseTextAsync();
                user = JsonConvert.DeserializeObject<User>(userJson);
            }

            if (account != null)
            {
                store.Delete(account, Constants.AppName);
            }

            DateTime date = DateTime.Now.Subtract(new TimeSpan(vm.Days * 24, 0, 0));
            var request2 = new OAuth2Request("GET", new Uri(string.Format(Constants.MessageList, "\"" + vm.Subject + "\"", vm.From, date.ToString("yyyy/MM/dd"))), null, acc);
            var response2 = await request2.GetResponseAsync();
            if (response2 != null)
            {
                StoreFactory.CurrentVM.Logs.Add("Gmail message list received");
                // Deserialize the data and store it in the account store
                // The users email address will be used to identify data in SimpleDB
                string userJson = await response2.GetResponseTextAsync();

                vm.Percentage = 0;
                vm.Messages.Clear();
                foreach (string id in StoreFactory.HalProxy.ReadMailIds(userJson))
                {
                    StoreFactory.CurrentVM.Logs.Add("Gmail message id = " + id);
                    var email = await ReadEmail(id, acc, vm);
                    vm.Messages.Add(email);
                    StoreFactory.HalProxy.RunOnUiThread(
                         new Action(() => { vm.Percentage++; }));
                }
                vm.RaiseChanges();
            }


            await store.SaveAsync(acc, Constants.AppName);
            vm.IsRunning = false;
            StoreFactory.CurrentVM.Logs.Add("Gmail message count = " + vm.Messages.Count);

            return vm.Messages.Count;
        }

        async Task<Email> ReadEmail(string id, Account account, ICreateListVM vm)
        {
            Email email = null;
            var request = new OAuth2Request("GET", new Uri(string.Format(Constants.MessageGet, id)), null, account);
            var response = await request.GetResponseAsync();
            if (response != null)
            {
                // Deserialize the data and store it in the account store
                // The users email address will be used to identify data in SimpleDB
                string userJson = await response.GetResponseTextAsync();

                email = StoreFactory.HalProxy.ReadMailContent(userJson, vm);
                StoreFactory.CurrentVM.Logs.Add("Gmail message received " + email.Subject);

            }

            return email;

        }
        void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

            Console.WriteLine("Authentication error: " + e.Message);
        }

    }
}
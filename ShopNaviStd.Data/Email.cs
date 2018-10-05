using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ShopNavi.Data
{

    [JsonObject]
    public class Email
    {
        public Email(ICreateListVM vm)
        {
            this.InputFromGmailCmd = new Command((nothing) =>
            {
                StoreFactory.Items.InitInput(this.Body.Split(new String[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries), true, StoreFactory.CurrentVM);
                StoreFactory.CurrentVM.RefreshOutputList();
                this.GoToInputCmd.Execute(null);

            });

            this.GoToInputCmd = StoreFactory.HalProxy.GetGoToInputCmd(vm);
        }

        public string Subject
        {
            get;
            set;
        }

        public string From
        {
            get;
            set;
        }

        public string Body
        {
            get;
            set;
        }

        public DateTime Received
        {
            get;
            set;
        }

        public ICommand InputFromGmailCmd { get; protected set; }

        public ICommand GoToInputCmd { get; set; }

    }
}

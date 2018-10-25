using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using ShopNavi.Data;
using ShopNavi.Resources;
using Xamarin.Forms;

namespace ShopNavi
{
    public class CreateListVM : BaseVM, ICreateListVM
    {

        public CreateListVM()
        {
            this.menu = new CreateListMenu(this)
            {
                Name = Resources.TextResource.CreateListMenuItem,
                Description = Resources.TextResource.CreateListMenuItemDescr
            };

            this.AddItemCmd = new Command(async (nothing) =>
            {
                if(string.IsNullOrEmpty(this.NewItem))
                {
                    await this.Navi.PushModalAsync(new ItemListPage(this), true); 
                }

                if (this.CanAddItem)
                {
                    if (!string.IsNullOrEmpty(this.NewItem))
                    {
                        this.Lines.Add(new LineItem(this.NewItem, this));
                        //this.CanMove = true;
                        this.NewItem = string.Empty;
                    }
                    else if (this.PoolItem != null)
                    {
                        this.Lines.Add(new LineItem(this.PoolItem.Name, this));
                        //this.CanMove = true;
                        this.PoolItem = null;
                    }

                }
                else
                {
                    StoreFactory.HalProxy.ShowMessage(Resources.TextResource.EmptyCell);
                }
            });

            this.LoginCmd = new Command(async (nothing) =>
            {
                await this.login.OnLoginClicked(this, null);
                StoreFactory.HalProxy.MakeToast(TextResource.Finished);
            });

            this.NewItemCmd = new Command((nothing) =>
            {
                this.NewItem = string.Empty;
            });

            this.GoToItemListCmd = new Command(async () =>
            {
                await this.Navi.PushModalAsync(new ItemListPage(this), true); 
            });

            this.GmailSettingsCmd = new Command(() =>
            {
                this.GmailSettingsOn = !this.GmailSettingsOn;

            });

            this.OkCmd = new Command((arg) =>
                {
                    if (this.PoolItem != null)
                    {
                        this.NewItem = this.PoolItem.ToString();
                    }
                    this.Navi.PopModalAsync();
                });
            this.CancelCmd = new Command((arg) =>
            {
                this.Navi.PopModalAsync();
            });

            this.Subject = StoreFactory.Settings.GmailSubject;
            this.From = StoreFactory.Settings.GmailFrom;
            //this.Messages.Add(new Email(this)
            //{
            //    Subject = "nakup",
            //    Body = "test1 test2"
            //});
        }

        public void RefreshInputList()
        {
            var lines = this.Lines;
            this.Lines = null;
            this.Lines = lines;
        }


        private string newItem = string.Empty;

        private Item poolItem = null;
        CreateListMenu menu;
        ILoginOAuth login;

        public ICommand AddItemCmd { get; protected set; }
        public ICommand NewItemCmd { get; protected set; }
        public ICommand TextChangedCmd { get; protected set; }
        public ICommand GoToItemListCmd { get; set; }
        public ICommand GmailSettingsCmd { get; set; }
        public ICommand LoginCmd { get; protected set; }

        ObservableCollection<LineItem> lines = new ObservableCollection<LineItem>();
        private string errorMsg = Resources.TextResource.NoError;
        private int percentage = 0;
        private bool removeDuplicities = true;
        private ObservableCollection<Email> messages = new ObservableCollection<Email>();
        private string subject = "nakup";
        private string from;
        private int days = 3;
        private Email selectedMessage;
        public bool gmailSettingsOn = false;

        public CreateListMenu Menu
        {
            get
            {
                return this.menu;
            }
        }

        public ObservableCollection<LineItem> Lines
        {
            get
            {
                return this.lines;
            }
            set
            {
                this.Lines = value;
                this.OnPropertyChanged("Lines");
            }
        }

        public string NewItem
        {
            get
            {
                return this.newItem;
            }
            set
            {
                this.newItem = value;
                this.OnPropertyChanged("NewItem");
                this.OnPropertyChanged("CanAddItem");
            }
        }

        public bool CanAddItem
        {
            get
            {
                return !string.IsNullOrEmpty(this.NewItem) || this.PoolItem != null;
            }
        }


        public string ErrorMsg
        {
            get
            {
                return this.errorMsg;
            }
            set
            {
                this.errorMsg = value;
                this.OnPropertyChanged("ErrorMsg");
            }
        }

        public Item PoolItem 
        {
            get
            {
                return this.poolItem;
            }
            set
            {
                this.poolItem = value;
                this.OnPropertyChanged("PoolItem");
                this.OnPropertyChanged("CanAddItem");
            }
        }

        public ObservableCollection<Item> AllItems
        {
            get
            {
                return StoreFactory.Items.Items;
            }
        }

        public int Percentage
        {
            get
            {
                return this.percentage;
            }
            set
            {
                this.percentage = value;
                OnPropertyChanged("Percentage");
            }
        }

        public bool RemoveDuplicities
        {
            get
            {
                return this.removeDuplicities;
            }
            set
            {
                this.removeDuplicities = value;
                OnPropertyChanged("RemoveDuplicities");
            }
        }


        public int ListHeight
        {
            get
            {
                return StoreFactory.HalProxy.LineHeight * 3;
            }
        }

        public override void Save()
        {
            
        }

        [XmlAttribute]
        public string Subject
        {
            get
            {
                return this.subject;
            }
            set
            {
                this.subject = value;
                this.OnPropertyChanged("Subject");
            }
        }

        [XmlAttribute]
        public string From
        {
            get
            {
                return this.from;
            }
            set
            {
                this.from = value;
                this.OnPropertyChanged("From");
            }
        }

        [XmlAttribute]
        public int Days
        {
            get
            {
                return this.days;
            }
            set
            {
                this.days = value;
                this.OnPropertyChanged("Days");
            }
        }

        public ObservableCollection<Email> Messages
        {
            get
            {
                return this.messages;
            }
        }

        public Email SelectedMessage
        {
            get
            {
                return this.selectedMessage;
            }
            set
            {
                this.selectedMessage = value;
                this.OnPropertyChanged("SelectedMessage");
            }
        }

        public override void RaiseChanges()
        {
            base.RaiseChanges();
            this.OnPropertyChanged("Messages");
        }


        public bool GmailSettingsOn
        {
            get
            {
                return this.gmailSettingsOn;
            }
            set
            {
                this.gmailSettingsOn = value;
                this.OnPropertyChanged("GmailSettingsOn");
                this.OnPropertyChanged("GmailSettingsIcon");
            }
        }
        public string GmailSettingsIcon
        {
            get
            {
                return StoreFactory.HalProxy.ResourcePrefix + (this.GmailSettingsOn ?  "up2.png" : "down2.png");
            }
        }

        public int RowSize
        {
            get
            {
                return 35;// (int)Font.Default.FontSize + 10;
            }
        }

        public ILoginOAuth Login
        {
            get
            {
                return this.login;
            }
            set
            {
                this.login = value;
            }
        }
    }
}

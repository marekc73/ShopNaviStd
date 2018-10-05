using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public class CommonVM : CommonBaseVM
    {
        private bool inputExtraOn = false;

        public CommonVM() : base()
        {
            Labels = new string[] { Resources.TextResource.StartMovingLine, Resources.TextResource.FinishMovingLine };

            this.Order = new OutputLineVisitor(this);

            this.LoadCmd = new Command(() =>
            {
            });

            this.SortCmd = new Command(() =>
            {
                this.OutputList.Clear();
                if (this.CurrentStore != null)
                {
                    this.IsRunning = true;
                    foreach (var it in this.CurrentStore.SortInputList(
                        this.InputList.ToList(), 
                        this))
                    {
                        //it.DelLineCmd = StoreFactory.HalProxy.GetDelLineCmd(this, it);
                        this.OutputList.Add(it);
                    }
                    this.RefreshOutputList();

                    StoreFactory.HalProxy.FinalizeData();

                    this.IsRunning = false;
                    (this.Parent as MainVM).SetPageIndex(4); 
                }
            });

            this.PasteCmd = new Command(() =>
            {
                this.InputList.Clear();
                StoreFactory.Items.InitInput(StoreFactory.HalProxy.GetClipboardList(), true, this);
                

                this.OutputList.Clear();
                foreach (var it in this.CurrentStore.SortInputList(this.InputList.ToList(), this))
                {
                    //it.DelLineCmd = StoreFactory.HalProxy.GetDelLineCmd(this, it);
                    this.OutputList.Add(it);
                }

                this.RefreshOutputList();
            });

            this.DeleteListCmd = new Command(() =>
            {
                this.InputList.Clear();

                this.RefreshInputList();
            });

            this.PrintCmd = new Command(() =>
            {
                StoreFactory.HalProxy.Print(this.OutputList.ToList());
            });

            this.DelFinishedCmd = new Command(() =>
            {
                List<OutputLine> remaining = this.OutputList.Where(x => x.Valid).ToList<OutputLine>();

                this.OutputList.Clear();

                foreach (var it in remaining)
                {
                    this.OutputList.Add(it);
                }

                this.RefreshOutputList();
            });

            this.GoToStoreListCmd = new Command(async () => 
            {
                var page = new ShopListPage(this);
                await this.Navi.PushModalAsync(page, true); 
            });

            this.GoToStoreEditCmd = new Command(async (arg) =>
            {
                Store store = arg as Store;
                store.Swipe = PanOperation.All;
                foreach(var sect in store.Sections)
                {
                    sect.Swipe = store.Swipe;
                }
                var page = new StoreEditPage(store);
                await this.Navi.PushModalAsync(page, true);
            });

            this.NewStoreCmd = new Command(async (arg) =>
            {
                Store store = StoreFactory.CreateStore("new", "none");
                var page = new NewStorePage(store);
                await this.Navi.PushModalAsync(page, true);
            });


            this.CurrentStore = this.StoreList.FirstOrDefault();

            this.InputExtraCmd = new Command(() =>
            {
                this.InputExtraOn = !this.InputExtraOn;

            });
        }

        public ICommand InputExtraCmd { get; set; }

        public bool InputExtraOn
        {
            get
            {
                return this.inputExtraOn;
            }
            set
            {
                this.inputExtraOn = value;
                this.OnPropertyChanged("InputExtraOn");
                this.OnPropertyChanged("InputExtraIcon");
            }
        }
        public string InputExtraIcon
        {
            get
            {
                return this.inputExtraOn ? "up2.png" : "down2.png";
            }
        }

    }
}

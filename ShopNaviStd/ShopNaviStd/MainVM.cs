using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{

    public class MainVM : BaseVM
    {

        public MainVM(MainPage pageArg)
        {
            var vm = new CommonVM() { Parent = this };

            SettingsBaseVM settingVm = new SettingsVM(StoreFactory.HalProxy.ReadSettings());
            CreateListVM listVm = new CreateListVM();
            vm.Order = new OutputLineVisitor(vm);
            StoreFactory.CurrentVM = vm;

            listVm.Parent = vm.Parent;

//            foreach(var line in StoreFactory.HalProxy.ReadGmail())
//                vm.Logs.Add(line);

            this.Pages.Add(new PageItem() { Title = Resources.TextResource.Gmail, Icon = StoreFactory.HalProxy.ResourcePrefix + "mail.png", Type = typeof(LoginPage), VM = listVm, Page = new LoginPage(listVm) });
            this.Pages.Add(new PageItem() { Title = Resources.TextResource.CreateList, Icon = StoreFactory.HalProxy.ResourcePrefix + "edit.png", Type = typeof(CreateListPage), VM = listVm, Page = new CreateListPage(listVm) });
            this.Pages.Add(new PageItem() { Title = Resources.TextResource.InputList, Icon = StoreFactory.HalProxy.ResourcePrefix + "in.png", Type = typeof(InputPage), VM = vm, Page = new InputPage(vm) });
            this.Pages.Add(new PageItem() { Title = Resources.TextResource.OutputList, Icon = StoreFactory.HalProxy.ResourcePrefix + "out.png", Type = typeof(OutputPage), VM = vm, Page = new OutputPage(vm) });
            this.Pages.Add(new PageItem() { Title = Resources.TextResource.CarouselList, Icon = StoreFactory.HalProxy.ResourcePrefix + "carousel.png", Type = typeof(CarouselOutput), VM = vm, Page = new CarouselOutput(vm) });
            this.Pages.Add(new PageItem() { Title = Resources.TextResource.Settings, Icon = StoreFactory.HalProxy.ResourcePrefix + "settings.png", Type = typeof(SettingsPage), VM = settingVm, Page = new SettingsPage(settingVm) });
            this.Pages.Add(new PageItem() { Title = Resources.TextResource.Logs, Icon = StoreFactory.HalProxy.ResourcePrefix + "logs.png", Type = typeof(LogsPage), VM = vm, Page = new LogsPage()});
            
            OnPropertyChanged("Pages");

            this.GoToPageCmd = new Command(() =>
            {
                string typeName = "InputPage";
                switch (typeName.ToString())
                {
                    case "LoginPage":
                        this.Navi.PushAsync(this.Pages[0].Page, true);
                        break;
                    case "CreateListPage":
                        this.Navi.PushAsync(this.Pages[1].Page, true);
                        break;
                    case "InputPage":
                        this.Navi.PushAsync(this.Pages[2].Page, true);
                        break;
                    case "Output":
                        this.Navi.PushAsync(this.Pages[3].Page, true);
                        break;
                    case "Carousel":
                        this.Navi.PushAsync(this.Pages[4].Page, true);
                        break;
                    case "SettingsPage":
                        this.Navi.PushAsync(this.Pages[5].Page, true);
                        break;
                    case "LogsPage":
                        this.Navi.PushAsync(this.Pages[6].Page, true);
                        break;
                }
            });

            StoreFactory.MainVM = this;
            this.page = pageArg;
        }

        public void InitPages()
        {
            this.CurrentPageItem = this.Pages[1];
            this.CurrentPageItem = this.Pages[StoreFactory.HalProxy.InitialPageIndex];
        }


        public ICommand GoToPageCmd { get; set; }

        public PageItem CurrentPageItem
        {
            get
            {
                return this.currentPageItem;
            }
            set
            {
                var oldPage = this.currentPageItem;
                this.currentPageItem = value;
                SetCurrentPage(this.CurrentPage);
            }
        }

        public void SetPageIndex(int ix)
        {
            this.CurrentPageItem = this.Pages[ix];            
        }

        private void SetCurrentPage(ContentPage page)
        {
            try
            {
                if(page is CarouselOutput)//carousel bug
                {
                    var tmp = this.Pages[4];
                    this.Pages.RemoveAt(4);
                    var vm = tmp.VM;
                    var pageItem = new PageItem() { Title = Resources.TextResource.CarouselList, Icon = StoreFactory.HalProxy.ResourcePrefix + "carousel.png", Type = typeof(CarouselOutput), VM = vm, Page = new CarouselOutput(vm as CommonVM) };    
                    this.Pages.Insert(4, pageItem);
                    page = pageItem.Page;

                }

                this.page.Detail = new NavigationPage(page);
                this.page.IsPresented = false;
                OnPropertyChanged("CurrentPageItem");
                OnPropertyChanged("CurrentPage");
            }
            catch(System.InvalidCastException e0)
            {
                StoreFactory.CurrentVM.Logs.Add(e0.Message);
            }
            catch (Exception ex)
            {
                StoreFactory.CurrentVM.Logs.Add(ex.Message);
            }
        }
        public ContentPage CurrentPage
        {
            get
            {
                Type pt = this.currentPageItem != null ? this.currentPageItem.Type : this.Pages[StoreFactory.HalProxy.InitialPageIndex].Type;
                BaseVM vm = this.currentPageItem != null ? this.currentPageItem.VM : this.Pages[StoreFactory.HalProxy.InitialPageIndex].VM;
                if (this.pagesMap.ContainsKey(pt))
                {
                    return this.pagesMap[this.currentPageItem.Type];
                }
                else 
                {
                    var page = Activator.CreateInstance(pt, new object[] { vm }) as ContentPage;
                    this.pagesMap[pt] = page;
                    return page;
                }
            }
        }

        public ObservableCollection<PageItem> Pages
        {
            get
            {
                return this.pages;
            }
        }

        private ObservableCollection<PageItem> pages = new ObservableCollection<PageItem>();
        private PageItem currentPageItem = null;
        private Dictionary<Type, ContentPage> pagesMap = new Dictionary<Type, ContentPage>();
        private MainPage page;

        //public Main Page
        //{
        //    get
        //    {
        //        return this.page;
        //    }
        //    set
        //    {
        //        this.page = value;
        //        this.page.Detail = new NavigationPage(this.Pages[StoreFactory.HalProxy.InitialPageIndex].Page);
        //    }
        //}

        public override void Save()
        {
            
        }
    }
}

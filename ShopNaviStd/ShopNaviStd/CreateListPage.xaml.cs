using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public partial class CreateListPage : ContentPage
    {
        public CreateListPage()
        {            
            InitializeComponent();
            //(this.BindingContext as CreateListVM).Navi = this.Navigation;
        }

        public CreateListPage(CreateListVM vm)
        {
            InitializeComponent();
            this.BindingContext = vm;

            (this.BindingContext as CreateListVM).Navi = this.Navigation;
        }

        //public void OnTextChanged(object sender, TextChangedEventArgs args)
        //{
        //    List<Item> items = new List<Item>();
        //    if (this.allItems != null && this.allItems.ItemsSource != null && 
        //        (args.OldTextValue.Length < args.NewTextValue.Length))                
        //    {
        //        foreach (var it in this.allItems.ItemsSource)
        //        {   
        //            items.Add(it as Item);
        //        }

        //        var selected = items.FirstOrDefault(x => x.Name.StartsWith(args.NewTextValue));
        //        if (selected != null)
        //        {
        //            //this.allItems.IsVisible = true;
        //            this.allItems.Focus();
        //            this.allItems.SelectedItem = selected;
        //        }
        //    }
        //}

        //public void OnClicked(object sender, EventArgs args)
        //{
        //    this.allItems.Focus();
        //}

        //public void OnSelectedItem(object sender, object item)
        //{
        //    if (item != null)
        //    {
        //        this.newItem.Text = item.ToString();
        //    }
        //    this.allItems.IsVisible = false;
        //}
    }
}

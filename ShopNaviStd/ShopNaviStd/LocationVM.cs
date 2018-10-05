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
    public class LocationVM : BaseVM
    {
        private List<Store> storeList;
        private ItemLocation location;
        private Item src;
        private Item selectedItem;
        private Store store;
        private string nearItemName;
        
        public LocationVM(Item item, Store st) : base()
        {
            this.store = st;
            this.src = item;
            this.location = this.store.GetLocationForItem(this.src);
            OnPropertyChanged("LocationValue");
            OnPropertyChanged("ItemList");

            foreach(var it in this.ItemList)
            {
                it.LinkNearItemCmd = new Command(() =>
                {
                    this.NearItemName = it.Name;
                    this.location.NearItemId = it.Id;
                });
            }
        }


        public Item SelectedItem
        {
            get
            {
                return this.selectedItem;
            }
            set
            {
                this.selectedItem = value;
                if (this.selectedItem != null && this.selectedItem.LinkNearItemCmd != null)
                {
                    this.selectedItem.LinkNearItemCmd.Execute(this.selectedItem);
                }
                OnPropertyChanged("SelectedItem");
            }
        }

        ItemLocation Location 
        { 
            get
            {
                return this.location;
            }
        }

        public string LocationLabel
        {
            get
            {
                return this.location == null ? string.Empty : this.location.Label;
            }
            set
            {
                this.location.Label = value;
                OnPropertyChanged("LocationLabel");
            }
        }

        public string LocationText
        {
            get
            {
                return this.location == null ? string.Empty : this.location.Text;
            }
        }

        public string LocationValue
        {
            get
            {
                return this.location == null ? string.Empty : this.location.Location;
            }
            set
            {
                this.location.Location = value;
                OnPropertyChanged("LocationValue");
            }
        }

        public ObservableCollection<Item> ItemList 
        { 
            get
            {
                return StoreFactory.Items.Items;
            }
        }

        public string NearItemName
        {
            get
            {
                return this.nearItemName;
            }
            set
            {
                this.nearItemName = value;
                OnPropertyChanged("NearItemName");
            }
        }

        public override void Save()
        {
            this.store.UpdateLocationForItem(this.location, this.src);
            StoreFactory.CurrentVM.RefreshInputList();
        }
    }
}

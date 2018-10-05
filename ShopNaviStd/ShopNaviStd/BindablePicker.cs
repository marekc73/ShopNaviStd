namespace ShopNavi
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using Xamarin.Forms;
    using ShopNavi.Data;

    public class BindablePicker : Picker
    {
        public BindablePicker()
        {
            this.SelectedIndexChanged += OnSelectedIndexChanged;
        }

        Boolean _disableNestedCalls = false;

        public static BindableProperty ItemsSourceProperty =
            BindableProperty.Create<BindablePicker, IList>(o => o.ItemsSource, default(IList), propertyChanged: OnItemsSourceChanged);

        public static BindableProperty SelectedItemProperty =
            BindableProperty.Create<BindablePicker, object>(o => o.SelectedItem, default(object));

        public static BindableProperty ParentProperty =
            BindableProperty.Create<BindablePicker, IUpdatable>(o => o.ParentDataSource, default(IUpdatable));

        public delegate void OnSelectedItemDlg(object sender, object item);

        public event OnSelectedItemDlg OnParentUpdate;
        public event OnSelectedItemDlg OnSelectedItem;

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set 
            { 
                SetValue(SelectedItemProperty, value);
                if (this.ParentDataSource != null)
                {
                    this.ParentDataSource.Update(this.SelectedItem);
                }
                UpdateSelectedIndex(value);
            }
        }

        private void UpdateSelectedIndex(object it)
        {
            for (int i = 0; this.ItemsSource != null && i < this.ItemsSource.Count; i++)
            {
                if(it.ToString() == this.ItemsSource[i].ToString())
                {
                    _disableNestedCalls = true;
                    this.SelectedIndex = i;

                    _disableNestedCalls = false;
                    break;
                }
            }
        }

        public IUpdatable ParentDataSource
        {
            get { return (IUpdatable)GetValue(ParentProperty); }
            set
            {
                SetValue(ParentProperty, value);
            }
        }

        private static void OnItemsSourceChanged(BindableObject bindable, IList oldvalue, IList newvalue)
        {
            var picker = bindable as BindablePicker;

            if (picker != null)
            {
                picker.Items.Clear();
                if (newvalue == null) return;
                //now it works like "subscribe once" but you can improve
                foreach (var item in newvalue)
                {
                    //var value = 
                    picker.Items.Add(item.ToString());
                }
            }
        }

        private void OnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            if (!_disableNestedCalls)
            {
                if (SelectedIndex < 0 || SelectedIndex > Items.Count - 1)
                {
                    SelectedItem = null;
                }
                else
                {
                    SelectedItem = ItemsSource[SelectedIndex];
                }

                if (this.OnSelectedItem != null)
                {
                    this.OnSelectedItem(this, SelectedItem);
                }
            }
        }
    }
}

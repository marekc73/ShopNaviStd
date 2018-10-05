using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi.Data
{
    public abstract class CommonBaseVM : OrderContainerVM<CommonBaseVM, OutputLine>
    {
        ObservableCollection<Item> inputList = new ObservableCollection<Item>();
        ObservableCollection<Item> delInputList = new ObservableCollection<Item>();

        ObservableCollection<OutputLine> outputList = new ObservableCollection<OutputLine>();

        ObservableCollection<Store> storeList = new ObservableCollection<Store>();
        private bool itemEditMode, cleanList = true;

        private bool showDeleted = false;
        private ObservableCollection<string> logs = new ObservableCollection<string>();

        public ICommand SortCmd { get; protected set; }
        public ICommand LoadCmd { get; protected set; }
        public ICommand PasteCmd { get; protected set; }
        public ICommand DeleteListCmd { get; protected set; }
        public ICommand PrintCmd { get; protected set; }
        public ICommand DelFinishedCmd { get; protected set; }

        public ICommand GoToStoreListCmd { get; set; }
        public ICommand GoToStoreEditCmd { get; set; }
        public ICommand NewStoreCmd { get; set; }

        public Action<string> ShowAlert;
        private Section currentSection;
        private bool isStoreEdit = false;
        protected string[] labels;

        public CommonBaseVM()
        {
            StoreFactory.CurrentVM = this;
            Section.Labels = this.labels;

            foreach (var store in StoreFactory.HalProxy.GetStoreList())
            {
                this.storeList.Add(store);
            }

            this.CurrentStore = this.storeList.FirstOrDefault();

            StoreFactory.Items = StoreFactory.HalProxy.GetAllItems();
            foreach(var it in StoreFactory.Items.Items)
            {
                it.Parent = this;
            }

            foreach (var it in StoreFactory.Items.LastInput)
            {
                it.Parent = this;
                this.AddItemWithLocationInit(it);
            }

        }

        ~CommonBaseVM()
        {
            StoreFactory.HalProxy.FinalizeData();
        }

        [XmlIgnore]
        public ObservableCollection<Item> InputList
        {
            get
            {
                return this.inputList;
            }
            set
            {
                this.inputList = value;
                OnPropertyChanged("InputList");
            }
        }

        [XmlIgnore]
        public ObservableCollection<OutputLine> OutputList
        {
            get
            {
                return this.outputList;
            }
            set
            {
                this.outputList = value;
                OnPropertyChanged("OutputList");
                OnPropertyChanged("VisibleOutputList");
            }
        }

        [XmlIgnore]
        public ObservableCollection<OutputLine> VisibleOutputList
        {
            get
            {
                return new ObservableCollection<OutputLine>(this.outputList.Where(x => x.IsVisible));
            }
        }

        [XmlIgnore]
        public ObservableCollection<string> Logs
        {
            get
            {
                return this.logs;
            }
            set
            {
                this.logs = value;
                OnPropertyChanged("Logs");
            }
        }

        [XmlArray]
        public ObservableCollection<Store> StoreList
        {
            get
            {
                return this.storeList;
            }
        }

        [XmlIgnore]
        public Section CurrentSection
        {
            get
            {
                return this.currentSection;
            }
            set
            {
                this.currentSection = value;
                OnPropertyChanged("CurrentSection");
            }
        }

        public ObservableCollection<Section> CurrentSections
        {
            get
            {
                if (this.CurrentStore != null)
                {
                    return this.CurrentStore.Sections;
                }
                else
                {
                    return new ObservableCollection<Section>();
                }
            }
        }

        [XmlIgnore]
        public Store PendingStore
        { get; set; }

        [XmlIgnore]
        public Store CurrentStore
        {
            get
            {
                return StoreFactory.CurrentStore;
            }
            set
            {
                if (value != null && value != StoreFactory.CurrentStore)
                {
                    StoreFactory.CurrentStore = value;
                    OnPropertyChanged("CurrentStore");
                    OnPropertyChanged("CurrentStoreText");
                    if (StoreFactory.CurrentStore != null)
                    {
                        StoreFactory.Items.InitInput(this.InputList.Select(x => x.Name).ToList(), true);

                        this.OutputList.Clear();
                        foreach (var it in StoreFactory.CurrentStore.SortInputList(this.inputList.ToList(), this))
                        {
                            this.OutputList.Add(it);
                        }
                    }
                }
            }
        }

        public string CurrentStoreText
        {
            get
            {
                return this.CurrentStore != null ? this.CurrentStore.ToString() : string.Empty;
            }
        }
        
        [XmlAttribute]
        public bool CleanList
        {
            get
            {
                return this.cleanList;
            }
            set
            {
                this.cleanList = value;
                OnPropertyChanged("CleanList");
            }
        }

        [XmlAttribute]
        public bool ItemEditMode
        {
            get
            {
                return this.itemEditMode;
            }
            set
            {
                this.itemEditMode = value;

                this.RefreshInputList();

                OnPropertyChanged("ItemEditMode");
                OnPropertyChanged("InputList");
            }
        }

        public bool ShowDeleted
        {
            get
            {
                return this.showDeleted;
            }
            set
            {
                this.showDeleted = value;
                OnPropertyChanged("ShowDeleted");
                OnPropertyChanged("VisibleOutputList");
                OnPropertyChanged("FilteredOutputList");
                foreach (var it in this.OutputList)
                {
                    it.RaiseChanges();
                }
            }
        }

        public override void RaiseChanges()
        {
            OnPropertyChanged("VisibleOutputList");
            OnPropertyChanged("FilteredOutputList");
        }

        public void RefreshInputList()
        {
            foreach(var item in this.InputList)
            {
                item.RaiseChanges();
            }
        }

        public void RefreshOutputList()
        {
            int i = 0;
            OnPropertyChanged("VisibleOutputList");
            foreach (var line in this.OutputList)
            {
                line.Index = i++;
                line.RaiseChanges();
            }
        }

        public void InitInput(IList<string> input)
        {
            if(input == null || input.Count == 0)
            {
                return;
            }

            bool reinitOk = true;

            if (reinitOk)
            {
                if (this.CleanList)
                {
                    this.InputList.Clear();
                }

                StoreFactory.Items.InitInput(input, true, this);
            }
            else
            {
                this.ShowAlert("Operation denied");
            }
        }

        public void AddItemWithLocationInit(Item item)
        {
            item.LocationOpCmd = StoreFactory.HalProxy.GetLocationCmd(item, this.CurrentStore);
            this.InputList.Add(item);
        }

        public Dictionary<string,IList<string>> SaveState()
        {
            Dictionary<string, IList<string>> ret = new Dictionary<string, IList<string>>();

            ret.Add("data", this.InputList.Select(x => x.Name).ToList());
            ret.Add("output", this.OutputList.Select(x => x.Name).ToList());

            return ret;
        }

        public override void Save()
        {
            StoreFactory.Items.LastInput = this.InputList;
            if (this.PendingStore != null)
            {
                this.CurrentStore = this.PendingStore;
            }
        }

        public void RestoreState(Dictionary<string,IList<string>> data)
        {
            this.InitInput(data["data"]);
        }

        public override void DisableLine(OrderLineVM line)
        {
            OutputLine outLine = line as OutputLine;
            Item it = line as Item;
            if (outLine != null)
            {
                outLine.Valid = false;
                this.RefreshOutputList();
            }
            else if(it != null)
            {
                this.delInputList.Add(it);
                this.inputList.Remove(it);
                this.RefreshInputList();
            }
        }

        public override void EnableLine(OrderLineVM line)
        {
            OutputLine outLine = line as OutputLine;
            Item it = line as Item;

            if (outLine != null)
            {
                outLine.Valid = true;
                this.RefreshOutputList();
            }
            else if (it != null && this.delInputList.Remove(it))
            {
                this.InputList.Add(it);
                this.RefreshInputList();
            }
        }

        public void DeleteOutputLine(OutputLine line)
        {
            this.OutputList.Remove(line);
            this.RefreshOutputList();
        }


        internal void MoveOutputLine(IOrdered outputLine, SwipeType? swipe)
        {
            var oldIndex = outputLine.Index;

            if(swipe.Value == SwipeType.Up)
            {
                var el = this.outputList.ElementAt(oldIndex - 1);
                if (el != null)
                {
                    el.Index++;
                    outputLine.Index--;
                    this.outputList.Move(oldIndex, oldIndex - 1);
                    outputLine.TimeStamp = DateTime.Now;
                }
            }
            else if (swipe.Value == SwipeType.Down)
            {
                var el = this.outputList.ElementAt(oldIndex + 1);
                if (el != null)
                {
                    el.Index--;
                    outputLine.Index++;
                    this.outputList.Move(oldIndex, oldIndex + 1);
                    outputLine.TimeStamp = DateTime.Now;
                }
            }

            this.RefreshOutputList();

        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;

namespace ShopNavi.Data
{
    public class ItemLocation
    {
        [XmlIgnore]
        public string Label
        {
            get;
            set;
        }

        [XmlAttribute]
        public int Id { get; set; }
        [XmlAttribute]
        public string Location { get; set; }
        [XmlAttribute]
        public int NearItemId { get; set; }

        [XmlIgnore]
        public string Text
        {
            get
            {
                try
                {
                    int nearId = this.NearItemId;
                    var item = StoreFactory.Items.Items.FirstOrDefault(x => x.Id == nearId);
                    return this.Location + "; " + (item != null ? item.ToString() : string.Empty);
                }
                catch(Exception ex)
                {
                    return string.Empty + ";" + ex.Message;
                }
            }
        }
    }

    [XmlRoot]
    [XmlInclude(typeof(Section))]
    public class Store : OrderContainerVM<Store, Section>
    {
        class ItemComparer : IEqualityComparer<Item>
        {
            public bool Equals(Item x, Item y)
            {
                return x.Name == y.Name;
            }
            public int GetHashCode(Item codeh)
            {
                return codeh.Name.GetHashCode();
            }
        }

        public Store():base()
        {
            this.StoreEditCmd = StoreFactory.CurrentVM.GoToStoreEditCmd;

            this.NewStoreCmd = StoreFactory.CurrentVM.NewStoreCmd;
        }

        private ObservableCollection<Section> sections = new ObservableCollection<Section>();
        private Section selectedSection;
        private ObservableCollection<Section> deletedSections = new ObservableCollection<Section>();
        private string imageName;
        private ObservableCollection<ItemLocation> itemLocations = new ObservableCollection<ItemLocation>();
        private string name;
        private string location;
        private int index;

        [XmlAttribute]
        public string ImageName
        {
            get
            {
                try
                {

                    if (string.IsNullOrEmpty(this.imageName))
                    {
                        this.imageName = StoreFactory.HalProxy.ResourcePrefix + this.Name.ToLowerInvariant() + ".png";
                    }
                }
                catch
                {
                    this.imageName = string.Empty;
                }

                return this.imageName;
            }
            set
            {
                this.imageName = value;
            }
        }

        [XmlAttribute]
        public string Name 
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                OnPropertyChanged("Name");
            }
        }

        [XmlAttribute]
        public string Location
        {
            get
            {
                return this.location;
            }
            set
            {
                this.location = value;
                OnPropertyChanged("Location");
            }
        }

        [XmlAttribute]
        public int Index
        {
            get
            {
                return this.index;
            }
            set
            {
                this.index = value;
                OnPropertyChanged("Index");
            }
        }

        [XmlArray]
        public ObservableCollection<Section> Sections
        {
            get
            {
                return this.sections;
            }
            set
            {
                this.sections = value;
                OnPropertyChanged("Sections");
            }
        }

        [XmlIgnore]
        public ObservableCollection<Section> DeletedSections
        {
            get
            {
                return this.deletedSections;
            }
        }

        [XmlIgnore]
        public Section SelectedSection
        {
            get
            {
                return this.selectedSection;
            }
            set
            {
                this.selectedSection = value;
                OnPropertyChanged("SelectedSection");
            }
        }

        [XmlArray]
        public ObservableCollection<ItemLocation> ItemLocations
        {
            get
            {
                return this.itemLocations;
            }
            set
            {
                this.itemLocations = value;
                OnPropertyChanged("ItemLocations");
            }
        }

        public override void RaiseChanges()
        {
            OnPropertyChanged("Sections");
            OnPropertyChanged("SelectedSection");
            OnPropertyChanged("Me");
        }

        public override string ToString()
        {
            return this.Name + " : " + this.Location;
        }

        public void InitStore(AllItems dict)
        {            
            foreach(var item in dict.Items)
            {
                item.Link.ItemLinks.Add(item.Id);
            }

            this.Order = new StoreVisitor(this);

        }

        public List<OutputLine> SortInputList(List<Item> input, CommonBaseVM vm)
        {
            StoreFactory.CurrentStore = this;
            List<Item> output = new List<Item>();

            foreach(var it in input.Distinct(new ItemComparer()))
            {
                if (output.Count == 0)
                {
                    output.Insert(0, it);
                }
                else
                {
                    int ix = -1;
                    for (int i = 0; i < output.Count; i++)
                    {
                        if (it.Link.Id < output[i].Link.Id)
                        {

                            ix = i;
                            break;
                        }
                    }
                    if(ix < 0)
                    {
                        output.Add(it);
                    }
                    else
                    {
                        output.Insert(ix, it);
                    }
                }
            }

            int k = 0;
            return output.Select( x => new OutputLine(x.Name, x.Link.Name, k++, vm, "(" + this.GetLocationForItem(x).Text + ")")).ToList();
        }

        public ItemLocation GetLocationForItem(Item it)
        {
            var ret = this.itemLocations                
                .FirstOrDefault(x => x.Id == it.Id);

            if(ret == null)
            {
                ret = new ItemLocation() { Label = it.Name, Id = it.Id, Location = string.Empty, NearItemId = -1};
            }

            return ret;
        }

        public void UpdateLocationForItem(ItemLocation location, Item it)
        {
            var loc = this.itemLocations                
                .FirstOrDefault(x => x.Id == it.Id);

            if(loc == null)
            {
                location.Id = it.Id;
                this.ItemLocations.Add(location);
            }
            else
            {
                loc = location;
            }
        }

        public void Save(Stream file)
        {
            try
            {
                using (TextWriter textWriter = new StreamWriter(file))
                {
                    var xmlSerializer = new XmlSerializer(typeof(Store));
                    xmlSerializer.Serialize(textWriter, this);
                }
            }
            catch(Exception ex)
            {
                StoreFactory.CurrentVM.Logs.Add(ex.Message);
            }
        }

        public void AssignParent(BaseVM vm)
        {
            foreach (var sect in this.Sections)
            {
                sect.Parent = this;
            }

            this.Parent = vm;
            this.StoreEditCmd = this.CommonVM.GoToStoreEditCmd;
            this.NewStoreCmd = this.CommonVM.NewStoreCmd;
            this.Navi = vm.Navi;
        }

        public static Store Read(Stream file)
        {
            var store =  StoreFactory.HalProxy.ReadStore(file);
            store.InitStore(StoreFactory.Items);
            return store;
        }

        [XmlIgnore]
        public string FileName 
        { 
            get
            {
                return string.Format("Store.{0}.{1}.xml", this.Name, this.Location);
            }
        }

        [XmlIgnore]
        public ICommand StoreEditCmd { get; set; }

        [XmlIgnore]
        public ICommand NewStoreCmd { get; set; }

        [XmlIgnore]
        public CommonBaseVM CommonVM
        {
            get
            {
                return this.Parent as CommonBaseVM;
            }
        }

        public Section LastSelectedSection { get; set; }

        public void MoveOutputLine(IOrdered outputLine, SwipeType? swipe)
        {
            var oldIndex = outputLine.Index;

            if (swipe.Value == SwipeType.Up)
            {
                var el = this.Sections.ElementAt(oldIndex - 1) as IOrdered;
                if (el != null)
                {
                    el.Index++;
                    outputLine.Index--;
                    this.Sections.Move(oldIndex, oldIndex - 1);
                    outputLine.TimeStamp = DateTime.Now;
                }
            }
            else if (swipe.Value == SwipeType.Down)
            {
                var el = this.Sections.ElementAt(oldIndex + 1) as IOrdered;
                if (el != null)
                {
                    el.Index--;
                    outputLine.Index++;
                    this.Sections.Move(oldIndex, oldIndex + 1);
                    outputLine.TimeStamp = DateTime.Now;
                }
            }
            this.RaiseChanges();
        }

        public override void Save()
        {
            StoreFactory.CurrentVM.StoreList.Add(this);
            this.Save(StoreFactory.HalProxy.GetWriteStream(this.FileName));
        }

        public override void EnableLine(OrderLineVM line)
        {
            if (this.DeletedSections.Remove(line as Section))
            {
                this.Sections.Add(line as Section);
                OnPropertyChanged("Sections");
            }
        }

        public override void DisableLine(OrderLineVM line)
        {
            if (this.Sections.Remove(line as Section))
            {
                this.DeletedSections.Add(line as Section);
                OnPropertyChanged("Sections");
            }
        }

    }
}

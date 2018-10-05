using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ShopNavi.Data
{
    public class AllItems : NotifyPropertyChanged
    {
        private string name = string.Empty;

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
                this.OnPropertyChanged("Name");
            }
        }

        ObservableCollection<Item> items = new ObservableCollection<Item>();
        ObservableCollection<Item> lastInput = new ObservableCollection<Item>();

        [XmlArray]
        public ObservableCollection<Item> Items
        {
            get
            {
                return this.items;
            }
        }

        [XmlArray]
        public ObservableCollection<Item> LastInput
        {
            get
            {
                return this.lastInput;
            }
            set
            {
                this.lastInput = value;
            }
        }

        public List<Item> InitInput(IList<string> input, bool removeDuplicities = true, CommonBaseVM vm = null, bool clean = true)
        {
            List<Item> ret = new List<Item>();
            IEnumerable<string> list = removeDuplicities ? input.Distinct() : input.AsEnumerable();
            foreach (var expr in list)
            {
                var item = this.GetItem(expr, true);
                item.Link.ItemLinks.Add(item.Id);
                ret.Add(item);
            }

            if (vm != null)
            {
                if(clean)
                {
                    vm.InputList.Clear();
                }

                foreach (var it in ret)
                {
                    vm.AddItemWithLocationInit(it);
                }
            }
            return ret;
        }

        public void Save(Stream file)
        {
            using (TextWriter textWriter = new StreamWriter(file))
            {
                var xmlSerializer = new XmlSerializer(typeof(AllItems));
                xmlSerializer.Serialize(textWriter, this);
            }
        }

        public static AllItems Read(Stream file)
        {
            AllItems ret = null;
            using (TextReader textReader = new StreamReader(file))
            {
                var xmlSerializer = new XmlSerializer(typeof(AllItems));
                ret = xmlSerializer.Deserialize(textReader) as AllItems;
            }

            StoreFactory.SetLastId(ret.Items.Max(x => x.Id));
            return ret;
        }

        public Item GetItem(string expr, bool initLink = false)
        {
            var ret = this.Items.FirstOrDefault(x => Search(expr, x));  
            if(ret == null)
            {
                if (initLink)
                {
                    ret = new Item()
                    {
                        Name = expr,
                        NormalizedName = expr,
                        LinkId = StoreFactory.GetSectionIndexForName(expr, string.Empty),
                        Parent = StoreFactory.CurrentVM
                    };
                }
                else
                {
                    ret = new Item()
                    {
                        Name = expr,
                        NormalizedName = expr,
                        Parent = StoreFactory.CurrentVM
                    };
                }

                ret.LinkOpCmd = StoreFactory.HalProxy.NewLinkCmd(ret, StoreFactory.CurrentVM);
                ret.DeleteMeCmd = CommandFactory.GetDelInputLineCmd(StoreFactory.CurrentVM, ret);
                ret.OkCmd = CommandFactory.AcceptLinkCmd(ret);

                this.Items.Add(ret);

            }
            else if(initLink)
            {
                ret.LinkId = StoreFactory.GetSectionIndexForName(expr, ret.Link != null ? ret.Link.Name : string.Empty);
            }

            return ret;
        }

        private static bool Search(string expr, Item it)
        {
            //WildcardOptions options = WildcardOptions.IgnoreCase |
            //          WildcardOptions.Compiled;
            //WildcardPattern wildcard = new WildcardPattern(name, options);
            //return wildcard.IsMatch(feature.Name);

            return it.Name.StartsWith(expr, StringComparison.CurrentCultureIgnoreCase) || it.NormalizedName == expr || it.Description.Contains(expr);
        }
    }
}

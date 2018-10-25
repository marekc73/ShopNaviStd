using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Xamarin.Forms;

namespace ShopNavi.Data
{
    public interface IUpdatable
    {
        void Update(object src);
    }

    public class Item : OrderLineVM, IUpdatable
    {
        public Item() : base()
        {
            this.Id = StoreFactory.NewId();
            this.OkCmd = CommandFactory.AcceptLinkCmd(this);
            this.CancelCmd = new Command((arg) =>
            {
                this.Parent.Navi.PopModalAsync();
            });

            this.DeleteCmd = new Command(() =>
            {
                (this.Parent as CommonBaseVM).DisableLine(this);
            });

            this.LinkOpCmd = StoreFactory.HalProxy.NewLinkCmd(this, StoreFactory.CurrentVM);
            this.DeleteMeCmd = CommandFactory.GetDelInputLineCmd(StoreFactory.CurrentVM, this);
            this.IsAutoOk = StoreFactory.Settings.IsAutoOk;
        }

        public Item(Item src)
        {
            this.Id = src.Id;
            this.Name = src.Name;
            this.NormalizedName = src.NormalizedName;
            this.Description = src.Description;

            this.OkCmd = CommandFactory.AcceptLinkCmd(this);
            this.CancelCmd = new Command((arg) =>
            {
                this.Parent.Navi.PopModalAsync();
            });

            this.LinkOpCmd = StoreFactory.HalProxy.NewLinkCmd(this, StoreFactory.CurrentVM);
            this.DeleteMeCmd = CommandFactory.GetDelInputLineCmd(StoreFactory.CurrentVM, this);
        }

        private Section link = null;
        private string name = string.Empty;
        private string normalizeName = string.Empty;
        private string description = string.Empty;
        private int id;
        private int linkId = 0;

        [XmlIgnore]
        public ICommand LinkOpCmd { get; set; }

        [XmlIgnore]
        public ICommand LocationOpCmd { get; set; }

        [XmlIgnore]
        public ICommand LinkNearItemCmd { get; set; }

        [XmlIgnore]
        public ICommand DeleteMeCmd { get; set; }

        [XmlAttribute]
        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
                OnPropertyChanged("Id");
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
                OnPropertyChanged("Name");
                this.name = value;
            }
        }

        [XmlIgnore]
        public string Icon
        {
            get
            {
                if (this.Link == null || this.Link.Id == 0)
                {
                    return StoreFactory.HalProxy.ResourcePrefix + "assoc.png";
                }
                else if (this.Parent != null && this.CommonVM.ItemEditMode)
                {
                    return StoreFactory.HalProxy.ResourcePrefix + "dissoc.png";
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                OnPropertyChanged("Icon");
            }
        }

        [XmlIgnore]
        public string LocationIcon
        {
            get
            {
                return StoreFactory.HalProxy.ResourcePrefix + "location.png";
            }
        }

        [XmlIgnore]
        public string LocationText
        {
            get
            {
                return StoreFactory.CurrentStore.GetLocationForItem(this).Text;
            }
        }

        [XmlAttribute]
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                OnPropertyChanged("Description");
                this.description = value;
            }
        }

        [XmlAttribute]
        public string NormalizedName
        {
            get
            {
                return this.normalizeName;
            }
            set
            {
                OnPropertyChanged("NormalizedName");
                this.normalizeName = value;
            }
        }

        [XmlAttribute]
        public int LinkId
        {
            get
            {
                return this.linkId;
            }
            set
            {
                this.linkId = value;
                OnPropertyChanged("Link");
                OnPropertyChanged("LinkText");
                OnPropertyChanged("LinkId");
                OnPropertyChanged("Icon");
                OnPropertyChanged("LocationIcon");
                OnPropertyChanged("Me");
                OnPropertyChanged("IsEditable");
            }
        }

        [XmlIgnore]
        public string LinkText
        {
            get
            {
                if (this.Link != null)
                {
                    return this.Link.Name.Length > 10 ? this.Link.Name.Substring(0, 10) + "..." : this.Link.Name;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        [XmlIgnore]
        public Section Link
        {
            get
            {
                if(this.link == null || this.link.Id != this.LinkId || this.LinkId == -1)
                {
                    this.link = StoreFactory.FindSection(this);
                }

                return this.link;
            }
            set
            {
                this.link = value;
                if (this.link == null)
                {
                    this.LinkId = -1;
                }
                else
                {
                    this.LinkId = value.Id;
                }
                OnPropertyChanged("Link");
                OnPropertyChanged("LinkText");
                OnPropertyChanged("Icon");
                OnPropertyChanged("LocationIcon");
                OnPropertyChanged("Me");
                OnPropertyChanged("IsEditable");
            }
        }

        [XmlIgnore]
        public List<Section> AvailableSections
        {
            get
            {
                return this.CommonVM.CurrentSections.ToList();
            }
        }

        public void RaisePropertyChanged()
        {
            OnPropertyChanged("Parent");
            OnPropertyChanged("Link");
            OnPropertyChanged("Name");
            OnPropertyChanged("Id");
            OnPropertyChanged("IsEditable");
            OnPropertyChanged("LocationIcon");
            OnPropertyChanged("LocationText");
            OnPropertyChanged("Icon");
        }

        public override void RaiseChanges()
        {
            base.RaiseChanges();
            this.RaisePropertyChanged();
        }
        public override string ToString()
        {
            return this.Name;
        }

        public void Update(object src)
        {
            Section sect = src as Section;
            if(sect != null)
            {
                this.Link = sect;
            }
        }

        public CommonBaseVM CommonVM
        {
            get
            {
                return this.Parent as CommonBaseVM;
            }
        }

        public override void Save()
        {
        }

        public bool IsEditable
        {
            get
            {
                return this.CommonVM.ItemEditMode || this.LinkId == -1 || this.Link == null;
            }
        }
    }
}

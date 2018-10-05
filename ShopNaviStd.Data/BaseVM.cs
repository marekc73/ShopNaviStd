using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Xamarin.Forms;

namespace ShopNavi.Data
{
    public enum SwipeType
    {
        None = 0,
        Up = 0x01,
        Down = 0x02,
        Left = 0x10,
        Right = 0x20
    }

    public enum PanOperation { None, Move, Delete, All };

    public struct SwipeAction
    {
        public SwipeType Type;
        public bool Finished;
    }

    public abstract class BaseVM : NotifyPropertyChanged, IMasterDetailNavi
    {
        public BaseVM()
        {
            this.OkCmd = new Command((arg) =>
            {
                this.Save();
                this.Navi.PopModalAsync();
            });
            this.CancelCmd = new Command((arg) =>
            {
                this.Navi.PopModalAsync();
            });
        }

        protected BaseVM parent;
        public static string[] Labels;
        private bool isRunning = false;
        private bool isAutoOk = true;
        private PanOperation swipe = PanOperation.Delete;

        [XmlIgnore]
        public BaseVM Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                this.parent = value;
                OnPropertyChanged("Parent");
            }
        }

        [XmlIgnore]
        public ICommand OkCmd { get; set; }
        [XmlIgnore]
        public ICommand CancelCmd { get; set; }

        public abstract void Save();

        [XmlIgnore]
        public INavigation Navi
        {
            get;
            set;
        }

        [XmlIgnore]
        public BaseVM Me
        {
            get
            {
                return this;
            }
        }

        public virtual void RaiseChanges()
        { }

        public bool IsRunning
        {
            get
            {
                return this.isRunning;
            }
            set
            {
                this.isRunning = value;
                this.OnPropertyChanged("IsRunning");
            }
        }

        [XmlAttribute]
        public bool IsAutoOk
        {
            get
            {
                return this.isAutoOk;
            }
            set
            {
                this.isAutoOk = value;
                this.OnPropertyChanged("IsAutoOk");
            }
        }

        [XmlAttribute]
        public PanOperation Swipe
        {
            get
            {
                return this.swipe;
            }
            set
            {
                this.swipe = value;
                this.OnPropertyChanged("Swipe");
            }

        }
    }

    public class OrderLineVM : BaseVM, IOrdered
    {
        public OrderLineVM()
        {
            this.OnDragCmd = CommandFactory.GetMoveSectionCmd(StoreFactory.CurrentVM, this);
        }

        private MoveStatus lineMoveStatus = MoveStatus.Finished;
        private string orderImageName = "empty.png";
        private int index;
        private bool valid = true;
        private Image onOffImage;

        [XmlIgnore]
        public ICommand DeleteCmd { get; set; }

        [XmlIgnore]
        public string OnOffImageName
        {
            get
            {
                return this.Valid ? "on.png" : "off.png";
            }
            set
            {
                OnPropertyChanged("OnOffImageName");
            }
        }

        [XmlAttribute]
        public bool Valid
        {
            get
            {
                return this.valid;
            }
            set
            {
                this.valid = value;
                //if (!this.valid)
                //{
                //    StoreFactory.HalProxy.GetDelLineCmd(this.Parent, this).Execute(this);
                //}

                OnPropertyChanged("Valid");
                OnPropertyChanged("IsVisible");
                OnPropertyChanged("OnOffImage");
                OnPropertyChanged("OnOffImageName");
                if (this.Parent != null)
                {
                    this.Parent.RaiseChanges();
                }
            }
        }

        public override void RaiseChanges()
        {
            OnPropertyChanged("Valid");
            OnPropertyChanged("IsVisible");
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

        [XmlAttribute]
        public DateTime TimeStamp
        {
            get;
            set;
        }

        [XmlAttribute]
        public string OrderImageName
        {
            get
            {
                return this.orderImageName;
            }
            set
            {
                this.orderImageName = value;
                OnPropertyChanged("OrderImageName");
            }
        }

        [XmlIgnore]
        public ICommand OnDragCmd { get; set; }

        [XmlIgnore]
        public MoveStatus LineMoveStatus
        {
            get
            {
                return this.lineMoveStatus;
            }
            set
            {
                this.lineMoveStatus = value;
                this.OnPropertyChanged("LineMoveStatus");
                this.OnPropertyChanged("MoveLabel");
                this.OnPropertyChanged("Me");
            }
        }

        [XmlIgnore]
        public virtual string MoveLabel
        {
            get
            {
                return string.Empty;
            }
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class OrderContainerVM<T, U> : BaseVM
    {
        private OrderVisitor<T, U> order;

        [XmlIgnore]
        public OrderVisitor<T, U> Order
        {
            get
            {
                return this.order;
            }
            set
            {
                this.order = value;
            }
        }

        public virtual void Accept(OrderVisitor<T, U> visitor)
        {
            this.order = visitor;
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }

        public abstract void EnableLine(OrderLineVM line);
        public abstract void DisableLine(OrderLineVM line);

    }
}

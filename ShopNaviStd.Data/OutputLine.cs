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

    public class OutputLine : OrderLineVM
    {
        public OutputLine() : base()
        { 
        }

        public OutputLine(string name, string section, int index, BaseVM vm, string location)
        {
            this.name = name;
            this.Index = index;
            this.Parent = vm;
            this.section = section;
            this.Location = location;

            CommonBaseVM commonVm = this.Parent as CommonBaseVM;

            this.DeleteCmd = new Command(() =>
            {
                if (this.Valid)
                {
                    this.Delete(null, null);
                }
                else
                {
                    this.Valid = true;
                    commonVm.EnableLine(this);
                }
                this.RaiseChanges();

                if(this.Previous != null)
                    this.Previous.RaiseChanges();

                if (this.Next != null)
                    this.Next.RaiseChanges();
            });

            this.OnDragCmd = new Command((arg) =>
            {
                SwipeType? swipe = arg as SwipeType?;
                if((swipe.Value & SwipeType.Up) != 0)
                {
                    this.CommonVM.Order.MoveOutputLine(this, -1, SwipeType.Up);
                }
                else if ((swipe.Value & SwipeType.Down) != 0)
                {
                    this.CommonVM.Order.MoveOutputLine(this, -1, SwipeType.Down);
                }
                else if ((swipe.Value & SwipeType.Left) != 0)
                {
                    this.CommonVM.Order.MoveOutputLine(this, -1, SwipeType.Up);
                }
                else if ((swipe.Value & SwipeType.Right) != 0)
                {
                    this.CommonVM.Order.MoveOutputLine(this, -1, SwipeType.Down);
                }

                commonVm.RaiseChanges();
                
            });
        }

        private string name = string.Empty;
        private Guid id;
        private string section = string.Empty;
        private string location = string.Empty;
        private static OutputLine empty = new OutputLine();

        [XmlIgnore]
        public string PreviousName
        {
            get
            {
                return this.Previous == null ? string.Empty : this.Previous.Name;
            }
        }

        public string PreviousLocation
        {
            get
            {
                return this.Previous == null ? string.Empty : this.Previous.Location;
            }
        }

        public string PreviousSection
        {
            get
            {
                return this.Previous == null ? string.Empty : this.Previous.Section;
            }
        }

        [XmlIgnore]
        public string NextName
        {
            get
            {
                return this.Next == null ? string.Empty : this.Next.Name;
            }
        }

        public string NextLocation
        {
            get
            {
                return this.Next == null ? string.Empty : this.Next.Location;
            }
        }

        public string NextSection
        {
            get
            {
                return this.Next == null ? string.Empty : this.Next.Section;
            }
        }

        [XmlIgnore]
        private OutputLine Previous
        {
            get
            {
                try
                {
                    CommonBaseVM vm = this.parent as CommonBaseVM;
                    OutputLine n;
                    int i = 1;
                    do
                    {
                        n = vm.OutputList[this.Index - i++];
                    } while (n != null && !n.IsVisible && i < vm.OutputList.Count);

                    return n;
                }
                catch
                {
                    return OutputLine.Empty();
                }
            }
        }

        private static OutputLine Empty()
        {
            return empty;
        }

        [XmlIgnore]
        private OutputLine Next
        {
            get
            {
                try
                {
                    CommonBaseVM vm = this.parent as CommonBaseVM;
                    OutputLine n;
                    int i = 1;
                    do
                    {
                        n = vm.OutputList[this.Index + i++];
                    } while (n != null && !n.IsVisible && i< vm.OutputList.Count);

                    return n;
                }
                catch
                {
                    return OutputLine.Empty();
                }
            }
        }

        [XmlIgnore]
        public OutputLine Me
        {
            get
            {
                return this;
            }
        }

        public ICommand OnPinchCmd { get; set; }

        [XmlAttribute]
        public Guid Id
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

        [XmlAttribute]
        public string Section
        {
            get
            {
                return this.section;
            }
            set
            {
                OnPropertyChanged("Name");
                this.section = value;
            }
        }

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
        public bool IsVisible
        {
            get
            {
                return this.Valid || this.CommonVM.ShowDeleted;
            }
        }

        [XmlAttribute]
        public bool LocationVisible
        {
            get
            {
                return !string.IsNullOrEmpty(this.Location);
            }
        }
        void Delete(object sender, EventArgs args) 
        {
            this.Valid = !this.Valid;
            this.CommonVM.DisableLine(this);
        }

        private CommonBaseVM CommonVM
        {
            get
            {
                return this.Parent as CommonBaseVM;
            }
        }

        public override void Save()
        {
        }

        public override string MoveLabel
        {
            get
            {
                OrderContainerVM<CommonBaseVM, OutputLine> element = this.Parent as OrderContainerVM<CommonBaseVM, OutputLine>;
                if (element != null && BaseVM.Labels != null && BaseVM.Labels.Length >= 2)
                {
                    return (element.Order.LineForMove != null && element.Order.LineForMove.LineMoveStatus == MoveStatus.Started) ? BaseVM.Labels[1] : BaseVM.Labels[0];
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public override void RaiseChanges()
        {
            base.RaiseChanges();
            this.OnPropertyChanged("Next");
            this.OnPropertyChanged("Prev");
            this.OnPropertyChanged("NextName");
            this.OnPropertyChanged("PrevName");
            this.OnPropertyChanged("NextLocation");
            this.OnPropertyChanged("PrevLocation");
            this.OnPropertyChanged("NextSection");
            this.OnPropertyChanged("PrevSection");
        }

    }
}

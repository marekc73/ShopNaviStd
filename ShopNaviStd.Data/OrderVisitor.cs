using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ShopNavi.Data
{
    public enum MoveStatus
    {
        Finished,
        Started,
        Running
    }

    /// <summary>
    /// line editor abstract (moving lines)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public abstract class OrderVisitor<T, U> : NotifyPropertyChanged
    {
        public OrderVisitor()
        {
        }

        public OrderVisitor(T element)
        {
            this.element = element;
        }

        protected T element;
        private OrderLineVM lineForMove; 

        public OrderLineVM LineForMove 
        { 
            get
            {
                return this.lineForMove;
            }
            set
            {
                this.lineForMove = value;
                OnPropertyChanged("LineForMove");
            }
        }

        public abstract ObservableCollection<U> Lines { get; }

        public virtual void MoveOutputLine(IOrdered line, int destinationIndex = -1, SwipeType swipe = SwipeType.Up)
        {
            if (destinationIndex == -1)
            {
                destinationIndex = swipe == SwipeType.Up ? line.Index - 1 : line.Index + 1;
            }

            if (swipe == SwipeType.Up)
            {
                this.Lines.Move(line.Index, destinationIndex - (line.Index < destinationIndex ? 1 : 0));
            }
            else if (swipe == SwipeType.Down)
            {
                this.Lines.Move(line.Index, destinationIndex + (line.Index < destinationIndex ? 0 : 1));
            }
            this.Reindex();
            this.RaiseChanges();
            line.LineMoveStatus = MoveStatus.Finished;
            this.LineForMove = null;
        }

        public virtual void RaiseChanges()
        {
            BaseVM vm = this.element as BaseVM;
            if(vm != null)
            {
                vm.RaiseChanges();
            }

            if (this.LineForMove != null)
            {
                this.LineForMove.RaiseChanges();
            }
        }

        private void Reindex()
        {
            int i = 0;
            foreach(IOrdered it in this.Lines)
            {
                it.Index = i++;
                it.TimeStamp = DateTime.Now;
            }
        }
    }

    public class StoreVisitor : OrderVisitor<Store, Section>
    {
        public StoreVisitor(Store el):base(el)
        { }

        public override ObservableCollection<Section> Lines
        {
            get
            {
                Store store = this.element as Store;
                return store.Sections;
            }
        }
    }


    public class OutputLineVisitor : OrderVisitor<CommonBaseVM, OutputLine>
    {
        public OutputLineVisitor(CommonBaseVM el)
            : base(el)
        { }

        public override ObservableCollection<OutputLine> Lines
        {
            get
            {
                CommonBaseVM store = this.element as CommonBaseVM;
                return store.OutputList;
            }
        }
    }
}

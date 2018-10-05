using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public class InputContentPage : MenuContentPage<CommonBaseVM, OutputLine>
    {

    }

    public class OutputContentPage : MenuContentPage<CommonBaseVM, OutputLine>
    {

    }

    public class StoreContentPage : MenuContentPage<Store, Section>
    {

    }

    public class MenuContentPage<T, U> : ContentPage
    {
        public MenuContentPage()
        {
        }

        public MenuContentPage(T vm)
        {
            this.BindingContext = vm;
        }

        public OrderContainerVM<T, U> OrderVM
        {
            get
            {
                return this.BindingContext as OrderContainerVM<T, U>;
            }
        }

        public virtual void OnMoveStart(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            OrderContainerVM<T, U> vm = this.BindingContext as OrderContainerVM<T, U>;
            OrderLineVM line = (sender as MenuItem).CommandParameter as OrderLineVM;
            if (vm != null && line != null && vm.Order.LineForMove == null && line.LineMoveStatus == MoveStatus.Finished)
            {
                line.LineMoveStatus = MoveStatus.Started;
                vm.Order.LineForMove = line;
                vm.Order.RaiseChanges();
            }
            else if (vm != null && line != null && vm.Order.LineForMove != null && vm.Order.LineForMove.LineMoveStatus == MoveStatus.Started)
            {
                int index = line.Index;
                vm.Order.MoveOutputLine(vm.Order.LineForMove, index, SwipeType.Down);
            }
        }

        public virtual void OnDelete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            OrderContainerVM<T, U> vm = this.BindingContext as OrderContainerVM<T, U>;
            OrderLineVM line = (sender as MenuItem).CommandParameter as OrderLineVM;
            vm.DisableLine(line);
        }

    }
}

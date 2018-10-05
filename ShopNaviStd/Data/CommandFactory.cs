using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ShopNavi;

namespace ShopNavi.Data
{
    public class CommandFactory
    {
        /// <summary>
        /// commands
        /// </summary>
        /// <param name="it"></param>
        /// <param name="vm"></param>
        /// <returns></returns>

        public static System.Windows.Input.ICommand AcceptLinkCmd(Item it)
        {
            return new Command((section) =>
            {
                it.Link = section as Section;
                it.Parent.Navi.PopModalAsync();
            });
        }

        public static System.Windows.Input.ICommand AcceptChangeCmd()
        {
            return new Command((it) =>
            {
                BaseVM vm = it as BaseVM;
                if (vm != null)
                {
                    vm.Parent.Navi.PopModalAsync();
                }
            });
        }

        public static System.Windows.Input.ICommand GetDelInputLineCmd(CommonBaseVM vm, Item key)
        {
            return new Command(() =>
            {
                vm.InputList.Remove(key);
            });
        }
        public static System.Windows.Input.ICommand GetMoveSectionCmd(CommonBaseVM vm, IOrdered line)
        {
            return new Command((arg) =>
            {
                line.TimeStamp = DateTime.Now;

                Task.Factory.StartNew(() => Task.Delay(3000))
                    .ContinueWith((t, x) =>
                    {
                        if ((DateTime.Now.Subtract((x as IOrdered).TimeStamp).TotalSeconds > 3))
                        {
                            (x as IOrdered).OrderImageName = "empty.png";
                        }
                    },
                        line);


                SwipeAction swipe = (arg as SwipeAction?).Value;
                SwipeType swipeType = SwipeType.None;
                if ((swipe.Type & SwipeType.Up) != 0 || (swipe.Type & SwipeType.Left) != 0)
                {
                    line.OrderImageName = "up.png";
                    swipeType = SwipeType.Up;
                }
                else if ((swipe.Type & SwipeType.Down) != 0 || (swipe.Type & SwipeType.Right) != 0)
                {
                    line.OrderImageName = "down.png";
                    swipeType = SwipeType.Down;
                }
                else
                {
                    line.OrderImageName = "empty.png";
                }

                if (swipe.Finished)
                {
                    vm.Order.MoveOutputLine(line, -1, swipeType);
                    line.OrderImageName = "empty.png";
                }

                vm.RaiseChanges();
            });
        }

        public static System.Windows.Input.ICommand SwipeTimeout(IOrdered line)
        {
            return new Command(() =>
            {
            });
        }

        public static System.Windows.Input.ICommand GetDelLineCmd(CommonBaseVM vm, OutputLine key)
        {
            return new Command(() =>
            {
                vm.DeleteOutputLine(vm.OutputList.FirstOrDefault(x => x.Name == key.Name));
            });
        }
    }
}

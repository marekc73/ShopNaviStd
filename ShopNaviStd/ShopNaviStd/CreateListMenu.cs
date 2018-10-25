using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
//using Android.Widget;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public class CreateListMenu : NotifyPropertyChanged
    {
        public CreateListMenu(CreateListVM vm)
        {
            this.parentVm = vm;

            this.NewListCmd = new Command((nothing) =>
            {
                this.CanMove = true;
                vm.Lines.Clear();
            });

            this.PublishCmd = new Command(async (nothing) =>
            {
                try
                {
                    this.InProgress = true;

                    if (this.PublishActive)
                    {
                        try
                        {
                            this.parentVm.Percentage = 0;
                            this.PublishActive = false;
                            if ((StoreFactory.HalProxy.NetworkConnected && StoreFactory.Settings.FromCloud) || StoreFactory.Settings.FromSMS)
                            {
                                StoreFactory.CurrentVM.Logs.Add("Sending Message");
                                await MessageSender.SendText(vm.Lines.Select( x=> x.Text).ToArray(), this.OnProgress);
                            }
                            else
                            {
                                StoreFactory.CurrentVM.Logs.Add("Sending notification");
                                StoreFactory.HalProxy.SendNotification("nakup " + DateTime.Now, vm.Lines.Select( x=> x.Text).ToArray());
                            }
                        }
                        catch (Exception ex)
                        {
                            StoreFactory.CurrentVM.Logs.Add(ex.Message);
                        }
                        finally
                        {
                            this.PublishActive = true;
                            StoreFactory.CurrentVM.Logs.Add(vm.ErrorMsg);
                        }
                    }
                    else
                    {
                        this.parentVm.ErrorMsg = Resources.TextResource.EmptyList;
                        StoreFactory.HalProxy.ShowMessage(Resources.TextResource.EmptyList);
                    }
                }
                finally
                {
                    this.InProgress = false;
                }
            });

            this.MoveToInputCmd = new Command((nothing) =>
            {
                if (this.CanMove)
                {
                    StoreFactory.Items.InitInput(vm.Lines.Select(x => x.Text).ToList(), vm.RemoveDuplicities, StoreFactory.CurrentVM);

                    (StoreFactory.CurrentVM.Parent as MainVM).SetPageIndex(2);
                }
                else
                {
                    this.parentVm.ErrorMsg = Resources.TextResource.EmptyList;
                    StoreFactory.HalProxy.ShowMessage(Resources.TextResource.EmptyList);
                }
            });

            this.RecordCmd = new Command((nothing) =>
            {
                try
                {
                    if (this.InProgress)
                    {
                        this.InProgress = false;
                        this.RecordButtonLabel = Resources.TextResource.StartListening;
                    }
                    else
                    {
                        this.InProgress = true;
                        this.RecordButtonLabel = Resources.TextResource.StopListening;
                        vm.Lines.Clear();
                        var err = StoreFactory.HalProxy.RecordLines(this.OnRecognizedText, StoreFactory.Settings.CurrentLanguage);
                        if (err.IsError)
                        {
                            this.InProgress = false;
                            this.RecordButtonLabel = Resources.TextResource.StartListening;
                            vm.ErrorMsg = err.ToString();
                            StoreFactory.HalProxy.MakeToast(this.parentVm.ErrorMsg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    StoreFactory.CurrentVM.Logs.Add(ex.Message);
                    StoreFactory.CurrentVM.Logs.Add(ex.StackTrace);
                }
            });
        }

        public void OnRecognizedText(IList<string> lines, ErrorResult err)
        {
            if (InProgress)
            {
                if (err.IsError)
                {
                    this.InProgress = false;
                    this.RecordButtonLabel = Resources.TextResource.StartListening;
                }
                else
                {
                    foreach (string line in lines)
                    {
                        string[] delimiters = new string[] { StoreFactory.Settings.SpeechDelimiter };
                        if (!line.Contains(StoreFactory.Settings.SpeechDelimiter))
                        {
                            delimiters = new string[] { StoreFactory.Settings.SpeechDelimiter, " " };
                        }

                        foreach (var word in line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries))
                        {
                            this.parentVm.Lines.Add(new LineItem(word, this.parentVm));
                        }
                    }
                    this.CanMove = true;
                }
            }
        }

        public void OnProgress(int percentage, ErrorResult result)
        {
            if (result.IsError)
            {
                StoreFactory.CurrentVM.Logs.Add(result.ToString());
            }


            if (percentage == 100)
            {                
                StoreFactory.HalProxy.RunOnUiThread(
                    new Action(() => { this.parentVm.ErrorMsg = result.ToString(); this.PublishActive = true; this.parentVm.IsRunning = false;
                        StoreFactory.HalProxy.MakeToast(this.parentVm.ErrorMsg); }));
            }
            else if (percentage == 0)
            {
                StoreFactory.HalProxy.RunOnUiThread(
                    new Action(() => { this.parentVm.ErrorMsg = Resources.TextResource.Running; this.PublishActive = false; this.parentVm.IsRunning = true;
                        StoreFactory.HalProxy.MakeToast(this.parentVm.ErrorMsg);
                    }));
            }
        }


        public string Icon
        {
            get
            {
                return StoreFactory.HalProxy.ResourcePrefix + "menu.png";
            }
        }

        public string Name 
        { 
            get; 
            set; 
        }

        public string Description
        {
            get;
            set;
        }

        public bool InProgress
        {
            get
            {
                return inProgress;
            }
            set
            {
                this.inProgress = value;
                this.parentVm.ErrorMsg = this.inProgress ? Resources.TextResource.Listening : Resources.TextResource.Finished;

                this.OnPropertyChanged("InProgress");
                this.OnPropertyChanged("IsMenuVisible");
            }
        }

        public bool IsMenuVisible
        {
            get
            {
                return !this.InProgress;
            }
        }

        public bool CanMove
        {
            get
            {
                return this.parentVm.Lines.Count > 0;
            }
            set
            {
                this.OnPropertyChanged("CanMove");
                this.OnPropertyChanged("PublishActive");
            }
        }

        public string RecordButtonLabel
        {
            get
            {
                return this.recordButtonLabel;
            }
            set
            {
                this.recordButtonLabel = value;
                this.OnPropertyChanged("RecordButtonLabel");
            }
        }

        public bool PublishActive
        {
            get
            {
                return this.publishActive && this.CanMove;
            }
            set
            {
                this.publishActive = value;
                this.OnPropertyChanged("PublishActive");
            }
        }

        public ICommand RecordCmd { get; protected set; }
        public ICommand NewListCmd { get; protected set; }
        public ICommand PublishCmd { get; protected set; }
        public ICommand MoveToInputCmd { get; protected set; }

        private CreateListVM parentVm = null;
        private string recordButtonLabel = Resources.TextResource.StartListening;
        private bool publishActive = true;
        private bool inProgress = false;

        public override string ToString()
        {
            return this.Name;
        }


    }
}

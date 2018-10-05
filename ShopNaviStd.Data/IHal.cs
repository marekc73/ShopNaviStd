using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShopNavi.Data
{
    public interface ISwipeGestureRecognizer : IGestureRecognizer
    {
        event EventHandler SwipeLeft;
        event EventHandler SwipeRight;
        event EventHandler SwipeUp;
        event EventHandler SwipeDown;
        void OnTouchEvent(object e);
    }

    public struct GestureData
    {
        public GestureStatus status;
        public double totalX;
        public double totalY;
        public object sender;
    }

    public interface IMasterDetailNavi
    {
        INavigation Navi
        {
            get;
            set;
        }
    }

    public enum LogSeverity
    {
        None,
        Error,
        Warning,
        Info,
        Debug
    }

    public interface IHal
    {
        List<Store> GetStoreList();

        List<string> GetClipboardList();

        AllItems GetAllItems();

        void Print(List<OutputLine> output);

        bool SupportsGcm
        { get; }

        ErrorResult FinalizeData();

        Stream GetWriteStream(string fileName);

        ErrorResult RecordLines(Action<IList<string>, ErrorResult> onRecognizedText, CultureInfo language);

        void OnRecognizedText(IList<string> lines, ErrorResult res);

        void SendNotification(string message, string[] lines);

        object GetImage(string name);

        SettingsBaseVM ReadSettings();

        bool NetworkConnected { get; }

        //ISwipeGestureRecognizer SwipeRecognizer
        //{
        //    get;
        //}

        void HandlePanUpdated(GestureData data);

        void RunOnUiThread(Action action);

        ErrorResult SendSMS(string number, string subject, string[] lines, Action<int> onProgress);

        string UserName
        {
            get;
        }

        int InitialPageIndex
        {
            get;
            set;
        }

        Store ReadStore(Stream file);

        void WriteLog(LogSeverity severity, string text);

        void ShowMessage(string text);

        int LineHeight
        { get;  }

        IEnumerable<string> ReadGmail();

        IList<String> ReadMailIds(string userJson);

        Email ReadMailContent(string userJson, ICreateListVM vm);

        void SignIn();

        object GoogleClient { get; set; }

        object MainActivity { get; }
        void MakeToast(string msg);

        ICommand NewLinkCmd(Item it, BaseVM vm);

        ICommand GetLocationCmd(Item it, Store store);

        ICommand GetGoToInputCmd(ICreateListVM vm);

        ILoginOAuth GetLogin(ICreateListVM vm);
    }
}

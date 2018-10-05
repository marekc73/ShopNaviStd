using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ShopNavi.Data
{
    public interface ICreateListVM
    {
        string Subject
        {
            get;
            set;
        }

        string From
        {
            get;
            set;
        }

        int Days
        {
            get;
            set;
        }

        int Percentage
        {
            get;
            set;
        }

        bool IsRunning
        {
            get;
            set;
        }

        ObservableCollection<Email> Messages
        {
            get;
        }

        BaseVM Parent
        {
            get;
            set;
        }

        void RaiseChanges();
    }
}

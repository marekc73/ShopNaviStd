using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopNavi.Data
{
    public interface IOrdered
    {
        int Index
        {
            get;
            set;
        }

        DateTime TimeStamp
        {
            get;
            set;
        }

        string OrderImageName
        {
            get;
            set;
        }

        MoveStatus LineMoveStatus 
        { 
            get; 
            set; 
        }
    }
}

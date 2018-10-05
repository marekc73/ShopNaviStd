using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShopNavi.Data
{
    public interface ILoginOAuth
    {
        Task<bool> OnLoginClicked(object sender, EventArgs e);
    }
}

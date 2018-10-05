using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShopNavi.Data
{
    public class WebProxy : IWebProxy
    {
        private System.Net.ICredentials creds;

        public ICredentials Credentials
        {
            get
            {
                return creds;
            }
            set
            {
                creds = value;
            }
        }

        public Uri GetProxy(Uri destination)
        {
            return new Uri("wwwproxy.sbs.sk:3128");
        }

        public bool IsBypassed(Uri host)
        {
            return true;
        }
    }
}

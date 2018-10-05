using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopNavi.Data
{
    public enum ErrorCodes
    {
        OK = 0,
        Error = 1,
        CommError = 2,
        SmsError = 3,
        InvalidLanguage,
        Cancelled
    }

    public class ErrorResult
    {
        public ErrorResult()
        { }

        public ErrorResult(int code, string msg)
        {
            this.Code = code;
            this.Message = msg;
        }

        public ErrorResult(ErrorCodes code, string msg)
        {
            this.Code = (int)code;
            this.Message = msg;
        }

        private int code = 0;
        private string message = string.Empty;

        public ErrorCodes ECode
        {
            get
            {
                return (ErrorCodes)this.code;
            }
        }

        public int Code
        {
            get
            {
                return this.code;
            }
            set
            {
                this.code = value;
            }
        }

        public string Message
        {
            get
            {
                return this.message;
            }
            set
            {
                this.message = value;
            }
        }

        public bool IsError
        {
            get
            {
                return this.Code != 0;
            }
        }
        public override string ToString()
        {
            return this.Code != 0 ? this.Message : "No error";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserReg.Models
{
    public class UserClass
    {
        public string Uname { get; set; }

        public string Uemail { get; set; }

        public string Upwd { get; set; }

        public char Gender { get; set; }

        //public string Uimg { get; set; }
        public HttpPostedFileBase Uimg { get; set; }


    }
}
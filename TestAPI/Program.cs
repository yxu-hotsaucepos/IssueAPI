using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using WebAPI.Controllers;
namespace TestAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            IssueController control = new IssueController(null, null, null);
            control.Request = new HttpRequestMessage();
            control.Request.SetConfiguration(new HttpConfiguration());
            var result = control.Get();
        }
    }
}

using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Runtime.Serialization;

namespace MoCS.Service
{
    [DataContract(Namespace = "")]
    public class HttpStatusDetails
    {
        [DataMember(Order = 1)]
        public int HttpStatusCode { get; set; }
        [DataMember(Order = 2)]
        public string Details { get; set; }
    }
}

using System;

namespace Routing
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var lang = string.Empty;
            if (Request.RawUrl.Contains("?"))
            {
                lang = Request.RawUrl.Split('?')[1];
            }

            if (this.Request.Browser.IsMobileDevice)
            {
                Response.Redirect("/View/Mobile/html/index.html" + "?" + lang);
            }
            else
            {
                //Response.Redirect("/View/Mobile/html/index.html");
                Response.Redirect("/View/PC/html/index.html" + "?" + lang);
            }
        }
    }
}
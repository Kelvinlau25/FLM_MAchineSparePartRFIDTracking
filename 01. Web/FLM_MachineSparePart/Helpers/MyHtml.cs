using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
public static class MyHtml
{
    public static MvcHtmlString BackButton(this HtmlHelper html,string url="back")
    {
        var tb = new TagBuilder("button");
        //var jsact = "window.history." + url + "();return false;";
        //if (url != "back")
        //{
            var jsact = "window.location.href ='" + url + "'; return false;";
        //}
        //
        tb.AddCssClass("btn");
        tb.AddCssClass("btn-primary");
        tb.Attributes.Add("onclick", jsact);
        tb.SetInnerText("Back");
        return new MvcHtmlString(tb.ToString());
    }
    
    public static MvcHtmlString WebAlert(this HtmlHelper html, string message)
    {
        var tb = new TagBuilder("script");
        tb.InnerHtml = string.Format("alert('{0}');", message);
        return new MvcHtmlString(tb.ToString());
    }
}
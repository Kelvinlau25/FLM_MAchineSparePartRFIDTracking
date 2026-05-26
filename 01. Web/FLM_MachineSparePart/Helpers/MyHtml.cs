using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;

namespace FILM_Sparepart_MVC.Helpers
{
    public static class MyHtml
    {
        public static IHtmlContent BackButton(this IHtmlHelper html, string url = "back")
        {
            var tb = new TagBuilder("button");
            var jsact = "window.location.href ='" + url + "'; return false;";
            
            tb.AddCssClass("btn");
            tb.AddCssClass("btn-primary");
            tb.MergeAttribute("onclick", jsact);
            tb.InnerHtml.Append("Back");
            
            return tb;
        }

        public static IHtmlContent WebAlert(this IHtmlHelper html, string message)
        {
            var tb = new TagBuilder("script");
            tb.InnerHtml.AppendHtml(string.Format("alert('{0}');", message));
            return tb;
        }
    }
}
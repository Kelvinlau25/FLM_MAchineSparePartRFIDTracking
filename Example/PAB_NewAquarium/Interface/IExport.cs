using PAB_NewAquarium.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PAB_NewAquarium.Interface
{
    public interface IExport
    {
        FileContentResult ExportToExcel(DataTable dt, string FileName);
        byte[] ExportToPDF(string ViewName, ControllerContext ControllerContext,
                           object modelMaster, iText.Kernel.Geom.PageSize pagesize,
                           EnumType.PDF_TYPE PDF_TYPE);
        string RenderViewToString(ControllerContext context, string viewPath, object model,
            bool partial);
    }
}

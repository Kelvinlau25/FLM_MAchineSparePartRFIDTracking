using Newtonsoft.Json;
using PAB_NewAquarium.DAL;
using PAB_NewAquarium.Filters;
using PAB_NewAquarium.Helpers;
using PAB_NewAquarium.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PAB_NewAquarium.Controllers
{
    public class MM_StockInventoryEntryController : Controller
    {
        #region Whole Controller temporally closed due to functional specs change
        CommonFunction common = new CommonFunction();
        MM_STOCK_INVENTORY_DAL dal = new MM_STOCK_INVENTORY_DAL();

        [SessionExpire]
        public async Task<ActionResult> MM_StockInventoryEntry(string stock = "")
        {
            MM_STOCK_INVENTORY model = new MM_STOCK_INVENTORY();
            var obj1 = new { pStoreType = "IN" };
            var obj2 = new { pStoreType = "OUT" };
            model.STOCK_IN_LISTING = await common.PSP_COMMON_DAPPER<MM_STOCK_IN_LISTING>("PSP_GET_STOCK_INVENTORY", CommandType.StoredProcedure, obj1);
            model.STOCK_OUT_LISTING = await common.PSP_COMMON_DAPPER<MM_STOCK_OUT_LISTING>("PSP_GET_STOCK_INVENTORY", CommandType.StoredProcedure, obj2);
            model.STOCK_TYPE = stock;
            return View(model);
        }

        [SessionExpire]
        public async Task<ActionResult> MM_StockInventory_View(string stock = "")
        {
            MM_STOCK_INVENTORY model = new MM_STOCK_INVENTORY();
            var obj = new { pStoreType = "IN" };
            model.STOCK_IN_LISTING = await common.PSP_COMMON_DAPPER<MM_STOCK_IN_LISTING>("PSP_GET_STOCK_INVENTORY", CommandType.StoredProcedure, obj);
            model.STOCK_TYPE = stock;
            ViewBag.STOCK_IN = JsonConvert.SerializeObject(model.STOCK_IN_LISTING);
            ViewBag.REFERENCE_NO_DDL = await common.PSP_GET_DDL("MM_PRODUCT_H", "REFERENCE_NO", "");
            ViewBag.STORE_LOC_DDL = await common.PSP_GET_DDL("MM_LOCATION", "LOC_CODE", "AND LOC_TYPE='H'");
            return View(model);
        }

        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> MM_StockInventory_View(MM_STOCK_INVENTORY model)
        {
            ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);
            model.CREATED_BY = aclObj.EMP_NO + " - " + aclObj.EMP_NAME;
            model.CREATED_DATE = DateTime.Now;
            model.CREATED_LOC = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
            model = await dal.PSP_STOCK_INVENTORY_MAINT(model);
            ViewBag.REFERENCE_NO_DDL = await common.PSP_GET_DDL("MM_PRODUCT_H", "REFERENCE_NO", "");
            ViewBag.STORE_LOC_DDL = await common.PSP_GET_DDL("MM_LOCATION", "LOC_CODE", "AND LOC_TYPE='H'");
            ViewBag.ReturnMessage = (model.RETURN_MESSAGE == "Saved") ? "Success" : "Fail";
            return View(model);
        }

        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> MM_StockInventory_GetInventoryDtl(string referenceNo)
        {
            MM_STOCK_INVENTORY model = new MM_STOCK_INVENTORY();
            List<SqlParameter> _pMssql = new List<SqlParameter>();
            _pMssql.Add(new SqlParameter("@pReferenceNo", referenceNo));
            var obj = new { pReferenceNo = referenceNo };
            model = await common.PSP_COMMON_DAPPER_SINGLE<MM_STOCK_INVENTORY>("PSP_GET_AJAX_CHOP_NO", CommandType.StoredProcedure, obj);
            model.COLOR_WAY_DDL = await common.PSP_GET_DDL3("PSP_GET_AJAX_COLORWAY_DDL", _pMssql);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
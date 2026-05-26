using System;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.IdentityModel.Logging;
using PAB_NewAquarium.Filters;
using PAB_NewAquarium.Helpers;
using PAB_NewAquarium.Models;

namespace PAB_NewAquarium.Controllers
{
    public class CustomerDashboardController : BaseController
    {
        readonly BlobHelper blobHelper = new BlobHelper();

        [SessionExpire]
        public async Task<ActionResult> CustomerDashboard()
        {
            try
            {
                SalesDashboardModel model = new SalesDashboardModel();
                var param = new { pUser = "customer" };
                var (objModelA, objModelB, objModelC) = await common.PSP_COMMON_DAPPER_MULTIPLE<FABRIC_HOT_SALES, LATEST_PRODUCTS, CAROUSEL_DASHBOARD>("PSP_GET_IMAGE_SALES_DASHBOARD", CommandType.StoredProcedure, param);
                model.FABRIC_HOT_SALES = objModelA;
                model.LATEST_PRODUCTS = objModelB;
                model.CAROUSEL_DASHBOARD = objModelC;

                // Loop each image and set secure token
                foreach (var item in model.FABRIC_HOT_SALES)
                {
                    item.IMAGE_URL = await blobHelper.SetBlobUrlToken(item.IMAGE_URL);
                }
                foreach (var item in model.LATEST_PRODUCTS)
                {
                    item.IMAGE_URL = await blobHelper.SetBlobUrlToken(item.IMAGE_URL);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                await errorLog.ErrorLog_Add_V2(MethodBase.GetCurrentMethod().Name, ex, username);
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
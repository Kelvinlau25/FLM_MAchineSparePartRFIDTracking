using PAB_NewAquarium.DAL;
using PAB_NewAquarium.Filters;
using PAB_NewAquarium.Helpers;
using PAB_NewAquarium.Models;
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
    public class MM_UserController : Controller
    {
        #region Whole Controller temporally closed due to functional specs change
        CommonFunction common = new CommonFunction();
        MM_User_DAL dal = new MM_User_DAL();
        ReaderRFID readerRFID = new ReaderRFID();

        [SessionExpire]
        public async Task<ActionResult> MM_User()
        {
            User model = new User();
            model.UserListing = new List<UserList>();
            model.UserListing = await common.PSP_COMMON_DAPPER<UserList>("PSP_GET_USER_LIST", CommandType.StoredProcedure);
            return View(model);
        }

        [SessionExpire]
        public async Task<ActionResult> MM_User_View(int id)
        {
            UserDetail model = await GetUserDetailAsync(id);

            return View(model);
        }

        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> MM_User_View(UserDetail model, string btn)
        {
            ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);
            string recordTyp = "5";
            UserDetail returnModel = await GetUserDetailAsync(model.USER_ID);

            List<SqlParameter> _pMssql = new List<SqlParameter>();
            _pMssql.Add(new SqlParameter("@pID", model.USER_ID));
            _pMssql.Add(new SqlParameter("@pDeleteID", model.USER_ID));
            _pMssql.Add(new SqlParameter("@pEmployeeID", ""));
            _pMssql.Add(new SqlParameter("@pEmployeeName", ""));
            _pMssql.Add(new SqlParameter("@pRFIDTag", ""));
            _pMssql.Add(new SqlParameter("@pRecordTyp", recordTyp));
            _pMssql.Add(new SqlParameter("@pCreatedBy", aclObj.EMP_NO + " - " + aclObj.EMP_NAME));
            _pMssql.Add(new SqlParameter("@pCreatedLoc", Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()));
            _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

            string result = await common.PSP_COMMON_SQL("PSP_USER_DETAIL_MAINT", CommandType.StoredProcedure, _pMssql, "", "", true);

            ViewBag.Result = result;

            return View(returnModel);
        }

        [SessionExpire]
        public async Task<ActionResult> MM_User_Create()
        {
            ViewBag.Result = null;
            UserDetail model = new UserDetail();
            model.EmployeeIDDdl = await dal.PSP_GET_EMPLOYEE_ID();
            //model.RFID_READER = await readerRFID.GetReader("PReaderRFIDTagReg");

            return View(model);
        }

        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> MM_User_Create(UserDetail model)
        {
            if (ModelState.IsValid)
            {
                ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);
                model.USER_ID = 0;

                List<SqlParameter> _pMssql = new List<SqlParameter>();
                _pMssql.Add(new SqlParameter("@pID", model.USER_ID));
                _pMssql.Add(new SqlParameter("@pDeleteID", model.USER_ID));
                _pMssql.Add(new SqlParameter("@pEmployeeID", model.EMPLOYEE_ID));
                _pMssql.Add(new SqlParameter("@pEmployeeName", model.EMPLOYEE_NAME));
                _pMssql.Add(new SqlParameter("@pRFIDTag", model.RFID_TAG));
                _pMssql.Add(new SqlParameter("@pRecordTyp", "1"));
                _pMssql.Add(new SqlParameter("@pCreatedBy", aclObj.EMP_NO + " - " + aclObj.EMP_NAME));
                _pMssql.Add(new SqlParameter("@pCreatedLoc", Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()));
                _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

                string result = await common.PSP_COMMON_SQL("PSP_USER_DETAIL_MAINT", CommandType.StoredProcedure, _pMssql, "", "", true);

                ViewBag.Result = result;
            }
            model.EmployeeIDDdl = await dal.PSP_GET_EMPLOYEE_ID();
            //model.RFID_READER = await readerRFID.GetReader("PReaderRFIDTagReg");

            return View(model);
        }

        [SessionExpire]
        public async Task<ActionResult> MM_User_Edit(int id)
        {
            ViewBag.Result = null;
            UserDetail model = await GetUserDetailAsync(id);
            //model.RFID_READER = await readerRFID.GetReader("PReaderRFIDTagReg");

            return View(model);
        }

        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> MM_User_Edit(UserDetail model)
        {
            if (ModelState.IsValid)
            {
                ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);

                List<SqlParameter> _pMssql = new List<SqlParameter>();
                _pMssql.Add(new SqlParameter("@pID", model.USER_ID));
                _pMssql.Add(new SqlParameter("@pDeleteID", model.USER_ID));
                _pMssql.Add(new SqlParameter("@pEmployeeID", model.EMPLOYEE_ID));
                _pMssql.Add(new SqlParameter("@pEmployeeName", model.EMPLOYEE_NAME));
                _pMssql.Add(new SqlParameter("@pRFIDTag", model.RFID_TAG));
                _pMssql.Add(new SqlParameter("@pRecordTyp", "3"));
                _pMssql.Add(new SqlParameter("@pCreatedBy", aclObj.EMP_NO + " - " + aclObj.EMP_NAME));
                _pMssql.Add(new SqlParameter("@pCreatedLoc", Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()));
                _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

                string result = await common.PSP_COMMON_SQL("PSP_USER_DETAIL_MAINT", CommandType.StoredProcedure, _pMssql, "", "", true);

                ViewBag.Result = result;
            }
            model.EmployeeIDDdl = await dal.PSP_GET_EMPLOYEE_ID();
            //model.RFID_READER = await readerRFID.GetReader("PReaderRFIDTagReg");

            return View(model);
        }

        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> MM_FabricLocation_Edit(UserDetail model)
        {
            if (ModelState.IsValid)
            {
                ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);

                List<SqlParameter> _pMssql = new List<SqlParameter>();
                _pMssql.Add(new SqlParameter("@pID", model.USER_ID));
                _pMssql.Add(new SqlParameter("@pDeleteID", model.USER_ID));
                _pMssql.Add(new SqlParameter("@pEmployeeID", model.EMPLOYEE_ID));
                _pMssql.Add(new SqlParameter("@pEmployeeName", model.EMPLOYEE_NAME));
                _pMssql.Add(new SqlParameter("@pRFIDTag", /*model.RFID_TAG*/""));
                _pMssql.Add(new SqlParameter("@pRecordTyp", "1"));
                _pMssql.Add(new SqlParameter("@pCreatedBy", aclObj.EMP_NO + " - " + aclObj.EMP_NAME));
                _pMssql.Add(new SqlParameter("@pCreatedLoc", Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()));
                _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

                string result = await common.PSP_COMMON_SQL("PSP_USER_DETAIL_MAINT", CommandType.StoredProcedure, _pMssql, "", "", true);

                ViewBag.Result = result;
            }

            return View(model);
        }

        [SessionExpire]
        [HttpPost]
        public async Task<ActionResult> MM_User_DeleteListing(string[] deleteID)
        {
            ACL_UserObj aclObj = (Session["AclUser"] as ACL_UserObj);
            string deleteIDStr = string.Join(",", deleteID);

            List<SqlParameter> _pMssql = new List<SqlParameter>();
            _pMssql.Add(new SqlParameter("@pID", ""));
            _pMssql.Add(new SqlParameter("@pDeleteID", deleteIDStr));
            _pMssql.Add(new SqlParameter("@pEmployeeID", ""));
            _pMssql.Add(new SqlParameter("@pEmployeeName", ""));
            _pMssql.Add(new SqlParameter("@pRFIDTag", /*model.RFID_TAG*/""));
            _pMssql.Add(new SqlParameter("@pRecordTyp", "5"));
            _pMssql.Add(new SqlParameter("@pCreatedBy", aclObj.EMP_NO + " - " + aclObj.EMP_NAME));
            _pMssql.Add(new SqlParameter("@pCreatedLoc", Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString()));
            _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

            string result = await common.PSP_COMMON_SQL("PSP_USER_DETAIL_MAINT", CommandType.StoredProcedure, _pMssql, "", "", true);
            string isSuccessStr = "";
            if (result == "Success")
            {
                isSuccessStr = "Success";
            }
            else
            {
                isSuccessStr = "Failed";
            }

            return Json(new { isSuccess = isSuccessStr });
        }

        [SessionExpire]
        public async Task<ActionResult> MM_User_AuditTrail(int id)
        {
            AuditTrailModel auditTrailModel = new AuditTrailModel();

            List<SqlParameter> _pMssql = new List<SqlParameter>();
            _pMssql.Add(new SqlParameter("@TABLE", "MM_USER"));
            _pMssql.Add(new SqlParameter("@KEY_VALUE", id));
            _pMssql.Add(new SqlParameter("@SortColumn", "UPDATED_DATE"));
            _pMssql.Add(new SqlParameter("@SortType", "DESC"));
            auditTrailModel = await AuditTrailHelper.AuditTrailStoreProcedureSqlAsync("PSP_GET_AUDIT_TRAIL", CommandType.StoredProcedure, _pMssql, "PAB_NEW_AQUA");


            ViewBag.JsonResult = auditTrailModel.JsonData;
            ViewBag.KeyNames = auditTrailModel.ListData;
            ViewBag.USER_ID = id;

            return View();
        }

        public async Task<UserDetail> GetUserDetailAsync(int id)
        {
            var obj = new { pID = id };
            UserDetail model = await common.PSP_COMMON_DAPPER_SINGLE<UserDetail>("PSP_GET_USER_DETAIL", CommandType.StoredProcedure, obj);
            return model;
        }

        public async Task<JsonResult> GetEmployeeNameAjaxAsync(string employeeID)
        {
            string employeeName = await dal.PSP_GET_EMPLOYEE_NAME(employeeID);
            return Json(employeeName, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
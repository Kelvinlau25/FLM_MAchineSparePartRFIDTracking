using System;
using System.Text;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using ReportModel;
using FILM_Sparepart_MVC.Helper_Code.Objects;
using System.Data.OleDb;
using System.Security.Principal;
using System.DirectoryServices;

namespace FILM_Sparepart_MVC.Controllers
{
    public class ReportController : Controller
    {
        public static class CommonMethod
        {
            public static List<T> ConvertToList<T>(DataTable dt)
            {
                var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
                var properties = typeof(T).GetProperties();
                return dt.AsEnumerable().Select(row => {
                    var objT = Activator.CreateInstance<T>();
                    foreach (var pro in properties)
                    {
                        if (columnNames.Contains(pro.Name.ToLower()))
                        {
                            try
                            {
                                if (pro.PropertyType.Name.Equals("Boolean"))
                                {
                                    if (row[pro.Name].ToString().ToUpper().Equals("TRUE")) { pro.SetValue(objT, true); }
                                    else { pro.SetValue(objT, false); }
                                }
                                else { pro.SetValue(objT, row[pro.Name]); }
                            }
                            catch (Exception ex) { }
                        }
                    }
                    return objT;
                }).ToList();
            }

            public static string ConvertSearchValue(string[] Scol, string str)
            {
                var val = "'%" + str + "%'";
                str = "";
                var additonal = "";
                foreach (var col in Scol)
                {
                    string[] c = col.Split(new Char[] { '/' });
                    str += (additonal + " UPPER(" + c[1] + ") " + "LIKE" + " UPPER(" + val + ") ");
                    additonal = " OR";
                }
                return str;
            }

        }

        public ActionResult movement_rpt()
        {

            MovementRpt MovementRpt = new MovementRpt();
            MovementRpt.DropdownReportName = getReportName();
            MovementRpt.DropdownLocationName = getLocationName();
            MovementRpt.tableParent = new DataTable();
            //MovementRpt.tableParent = GetMoveReport("SP_FILM_ERROR_RPT", "20190101", "20190201");
            ViewBag.ReportType = "Select";
            return View(MovementRpt);
        }

        [HttpPost]
        public ActionResult movement_rpt(string ActionType, MovementRpt model)
        {
            try
            {
                model.DropdownReportName = getReportName();
                model.DropdownLocationName = getLocationName();
                //if ((model.DateFrom.HasValue == false && model.DateTo.HasValue) || (model.DateTo.HasValue == false && model.DateFrom.HasValue))
                //{
                //    ViewBag.Error = "Either both date are empty or both date are choosen.";
                //    ViewBag.ReportType = "Select";
                //    model.tableParent = new DataTable();
                //}

                if (ActionType == "Export")
                {
                    string spName = "";
                    int idtype = model.selectedId;
                    //string dateFrom = (model.DateFrom.HasValue) ? model.DateFrom.Value.ToString("yyyyMMdd") : "0";
                    //string dateTo = (model.DateTo.HasValue) ? model.DateTo.Value.ToString("yyyyMMdd") : "0";
                    string dateFrom = model.DateFrom;
                    string dateTo = model.DateTo;
                    int readerid = model.selectedLocId;

                    if (idtype == 1)
                    { spName = "SP_FILM_ERROR_RPT"; }
                    else if (idtype == 2)
                    { spName = "SP_FILM_MOVEMENT_RPT"; }
                    else
                    { spName = "SP_FILM_ERROR_LOG_RPT"; }

                    //Fill dataset with records
                    DataSet dataSet = GetRecords(spName, dateFrom, dateTo, readerid);

                    StringBuilder sb = new StringBuilder();


                    sb.Append("<table>");

                    if (idtype == 1)
                    {
                        sb.Append("<tr><td colspan='15' style='background-color:#0000FF; color:white; font-weight:bold;'><b>Unknown Transaction Report</b></td></tr>");
                    }
                    else if (idtype == 2)
                    {
                        sb.Append("<tr><td colspan='13' style='background-color:#0000FF; color:white; font-weight:bold;'><b>Spare Part Transaction Report</b></td></tr>");
                    }
                    else
                    {
                        sb.Append("<tr><td colspan='14' style='background-color:#0000FF; color:white; font-weight:bold;'><b>Detail Transaction Report</b></td></tr>");
                    }

                    sb.Append("<tr><td colspan='15'></td></tr>");

                    //LINQ to get Column names
                    var columnName = dataSet.Tables[0].Columns.Cast<DataColumn>()
                                         .Select(x => x.ColumnName)
                                         .ToArray();
                    sb.Append("<tr>");
                    //Looping through the column names
                    foreach (var col in columnName)
                        sb.Append("<td style='color:black; font-weight:bold; border:solid;'>" + col + "</td>");
                    sb.Append("</tr>");

                    //Looping through the records
                    foreach (DataRow dr in dataSet.Tables[0].Rows)
                    {
                        sb.Append("<tr>");
                        foreach (DataColumn dc in dataSet.Tables[0].Columns)
                        {
                            sb.Append("<td style='border:solid;'>" + dr[dc] + "</td>");
                        }
                        sb.Append("</tr>");
                    }

                    sb.Append("</table>");



                    //Writing StringBuilder content to an excel file.
                    Response.Clear();
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.Charset = "";
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.ms-excel";

                    if (idtype == 1)
                    {
                        Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", "Unknown Transaction Report_" + DateTime.Now.ToString("ddMMyyyy") + ".xls"));
                    }
                    else if (idtype == 2)
                    {
                        Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", "Spare Part Transaction Report_" + DateTime.Now.ToString("ddMMyyyy") + ".xls"));
                    }
                    else
                    {
                        Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", "Detail Transaction Report_" + DateTime.Now.ToString("ddMMyyyy") + ".xls"));
                    }


                    Response.Write(sb.ToString());
                    Response.Flush();
                    Response.Close();

                }
                if (ActionType == "Reset")
                {

                    ViewBag.ReportType = "Reset";
                    model.tableParent = new DataTable();
                }
                if (ModelState.IsValid)  //checking model is valid or not
                {
                    if (ActionType == "Search")
                    {
                        string spName = "";
                        int idtype = model.selectedId;
                        //string dateFrom = (model.DateFrom.HasValue) ? model.DateFrom.Value.ToString("yyyyMMdd") : "0";
                        //string dateTo = (model.DateTo.HasValue) ? model.DateTo.Value.ToString("yyyyMMdd") : "0";
                        string dateFrom = model.DateFrom;
                        string dateTo = model.DateTo;
                        int readerid = model.selectedLocId;

                        if (String.IsNullOrEmpty(dateTo))
                        {
                            dateTo = "Null";
                        }
                        if (String.IsNullOrEmpty(dateFrom))
                        {
                            dateFrom = "Null";
                        }


                        if (idtype == 1)
                        { spName = "SP_FILM_ERROR_RPT"; }
                        else if (idtype == 2)
                        { spName = "SP_FILM_MOVEMENT_RPT"; }
                        else
                        { spName = "SP_FILM_ERROR_LOG_RPT"; }

                        model.tableParent = GetMoveReport(spName, dateFrom, dateTo, readerid);

                        if (idtype == 1)
                        { ViewBag.ReportType = "Unknown"; }
                        else if (idtype == 2)
                        { ViewBag.ReportType = "SparePart"; }
                        else
                        { ViewBag.ReportType = "Detail"; }

                        model.DropdownReportName = getReportName();

                       
                    }

                    ModelState.Clear(); //clearing model
                }
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }
            return View(model);

        }

        public ActionResult ExportMoveReport(MovementRpt model)
        {
            //string returnurl = @Url.Action("movement_rpt", "Report").ToString();
            string spName = "";
            int idtype = model.selectedId;
            //string dateFrom = (model.DateFrom.HasValue) ? model.DateFrom.Value.ToString("yyyyMMdd") : "0";
            //string dateTo = (model.DateTo.HasValue) ? model.DateTo.Value.ToString("yyyyMMdd") : "0";
            string dateFrom = model.DateFrom;
            string dateTo = model.DateTo;

            if (idtype == 1)
            { spName = "SP_FILM_ERROR_RPT"; }
            else if (idtype == 2)
            { spName = "SP_FILM_MOVEMENT_RPT"; }
            else
            { spName = "SP_FILM_ERROR_LOG_RPT"; }

            int readerid = model.selectedLocId;

            //Fill dataset with records
            DataSet dataSet = GetRecords(spName, dateFrom, dateTo, readerid);

            StringBuilder sb = new StringBuilder();


            sb.Append("<table>");

            if (idtype == 1)
            {
                sb.Append("<tr><td colspan='15' style='background-color:#E0F8F7; color:white; font-weight:bold;'><b>Unknown Transaction Report</b></td></tr>");
            }
            else if (idtype == 2)
            {
                sb.Append("<tr><td colspan='13' style='background-color:#E0F8F7; color:white; font-weight:bold;'><b>Spare Part Transaction Report</b></td></tr>");
            }
            else
            {
                sb.Append("<tr><td colspan='14' style='background-color:#E0F8F7; color:white; font-weight:bold;'><b>Detail Transaction Report</b></td></tr>");
            }
           
            sb.Append("<tr><td colspan='15'></td></tr>");

            //LINQ to get Column names
            var columnName = dataSet.Tables[0].Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();
            sb.Append("<tr>");
            //Looping through the column names
            foreach (var col in columnName)
                sb.Append("<td style='background-color:#36CBE6; color:black; font-weight:bold;'>" + col + "</td>");
            sb.Append("</tr>");

            //Looping through the records
            foreach (DataRow dr in dataSet.Tables[0].Rows)
            {
                sb.Append("<tr>");
                foreach (DataColumn dc in dataSet.Tables[0].Columns)
                {
                    sb.Append("<td>" + dr[dc] + "</td>");
                }
                sb.Append("</tr>");
            }

            sb.Append("</table>");



            //Writing StringBuilder content to an excel file.
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Charset = "";
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";

            if (idtype == 1)
            {
                Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", "Unknown Transaction Report_" + DateTime.Now.ToString("ddMMyyyy") + ".xls"));
            }
            else if (idtype == 2)
            {
                Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", "Spare Part Transaction Report_" + DateTime.Now.ToString("ddMMyyyy") + ".xls"));
            }
            else
            {
                Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", "Detail Transaction Report_" + DateTime.Now.ToString("ddMMyyyy") + ".xls"));
            }

           
            Response.Write(sb.ToString());
            //Response.Flush();
            //Response.Close();
          
            return Redirect("movement_rpt");
        }

        DataSet GetRecords(string spName, string dateFrom, string dateTo, int readerid)
        {
            DataSet datatbl = new DataSet();

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["SQLCon"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = spName;
            cmd.Parameters.Add(new SqlParameter("@dateFrom", dateFrom)).Direction = System.Data.ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@dateto", dateTo)).Direction = System.Data.ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@readerid", readerid)).Direction = System.Data.ParameterDirection.Input;
            cmd.Connection = conn;

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            sqlDataAdapter.SelectCommand = cmd;
            sqlDataAdapter.Fill(datatbl);

            return datatbl;
        }
        //Dropdownlist
        private static List<SelectListItem> getReportName()
        {          
            List<SelectListItem> list = new List<SelectListItem>() {
                new SelectListItem(){ Value="1", Text="Unknown Movement"},
                new SelectListItem(){ Value="2", Text="Spare Part Transaction Report"},
                new SelectListItem(){ Value="3", Text="Detail Transaction Report"},

            };      
            return list;
        }

        private static List<SelectListItem> getLocationName()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            string constr = ConfigurationManager.ConnectionStrings["SQLCon"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * from MM_SPARE_PART_READER WHERE RECORD_TYPE <> '5'";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr["STORAGE_CODE"].ToString(),
                                Value = sdr["READER_ID"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }
        //get Report Data
        DataTable GetMoveReport(string spName, string dateFrom, string dateTo, int readerid)
        {
            DataTable datatbl = new DataTable();

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["SQLCon"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = spName;
            cmd.Parameters.Add(new SqlParameter("@dateFrom", dateFrom)).Direction = System.Data.ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@dateto", dateTo)).Direction = System.Data.ParameterDirection.Input;
            cmd.Parameters.Add(new SqlParameter("@readerid", readerid)).Direction = System.Data.ParameterDirection.Input;
            cmd.Connection = conn;

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            sqlDataAdapter.SelectCommand = cmd;
            sqlDataAdapter.Fill(datatbl);

            return datatbl;
        }

    


    }
}
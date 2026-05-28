using DBModel;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FILM_Sparepart_MVC.Repositories
{
     public class CommonRepo
    {
        DatabaseModel.Database1 db = new DatabaseModel.Database1();
        public List<T> ConvertToList<T>(DataTable dt)
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
                            else if (pro.PropertyType.Name.Equals("Integer"))
                            {
                                pro.SetValue(objT, Convert.ToInt32(row[pro.Name]));
                            }
                            else if (pro.PropertyType.Name.Equals("DateTime"))
                            {
                                pro.SetValue(objT, Convert.ToDateTime(row[pro.Name]));
                            }
                            else { pro.SetValue(objT, row[pro.Name]); }
                        }
                        catch (Exception ex) { }
                    }
                }
                return objT;
            }).ToList();
        }

        public string ConvertSearchValue(string[] Scol, string str)
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

      public DataTable List(string Table, string TableID, string Search,
      string Value, string SortField, string Direction,
      string FrmRowno, string ToRowno, string Deleted, string Conn = "SQLCon")
        {
            db.OpenConnection();
            db.command.CommandText = "PSP_COMMON_LIST";
            db.command.CommandType = CommandType.StoredProcedure;
            db.command.CommandTimeout = 0;
            db.command.Parameters.Clear();
            db.command.Parameters.Add(new SqlParameter("@Table", Table)).Direction = System.Data.ParameterDirection.Input;
            db.command.Parameters.Add(new SqlParameter("@TableID", TableID)).Direction = System.Data.ParameterDirection.Input;
            db.command.Parameters.Add(new SqlParameter("@Search", Search)).Direction = System.Data.ParameterDirection.Input;
            db.command.Parameters.Add(new SqlParameter("@Value", Value)).Direction = System.Data.ParameterDirection.Input;
            db.command.Parameters.Add(new SqlParameter("@SortField", SortField)).Direction = System.Data.ParameterDirection.Input;
            db.command.Parameters.Add(new SqlParameter("@Direction", Direction)).Direction = System.Data.ParameterDirection.Input;
            db.command.Parameters.Add(new SqlParameter("@FrmRowno", FrmRowno)).Direction = System.Data.ParameterDirection.Input;
            db.command.Parameters.Add(new SqlParameter("@ToRowno", ToRowno)).Direction = System.Data.ParameterDirection.Input;
            db.command.Parameters.Add(new SqlParameter("@Deleted", Deleted)).Direction = System.Data.ParameterDirection.Input;
            db.reader = db.command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(db.reader);

            db.CloseReader();
            db.CloseConnection();

            return dt;
        }


        #region ACL System

        public UserRegModel getUserData(string id)
        {
            db.OpenConnection();
            db.command.CommandText = "PSP_UserReg_SEL";
            db.command.CommandType = CommandType.StoredProcedure;
            db.command.CommandTimeout = 0;
            db.command.Parameters.Clear();
            db.command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
            db.reader = db.command.ExecuteReader();

            UserRegModel UserRegModel = null;

            while (db.reader.Read())
            {
                UserRegModel = new UserRegModel();
                UserRegModel.ID_ACL_USER = Convert.ToInt32(db.reader["ID_ACL_USER"]);
                UserRegModel.USER_ID = db.reader["USER_ID"].ToString();
                UserRegModel.USR_EMAIL = db.reader["USR_EMAIL"].ToString();
                UserRegModel.EMP_NO = db.reader["EMP_NO"].ToString();
                UserRegModel.EMP_NAME = db.reader["EMP_NAME"].ToString();
                UserRegModel.COMPANY = db.reader["COMPANY"].ToString();
                //UserRegModel.STATUS_IND = (Status)Enum.Parse(typeof(Status), reader["STATUS_IND"].ToString());
                UserRegModel.STATUS_IND = db.reader["STATUS_IND"].ToString();
                UserRegModel.RECORD_TYP = db.reader["RECORD_TYP"].ToString();
                UserRegModel.CREATED_BY = db.reader["CREATED_BY"].ToString();
                UserRegModel.CREATED_DATE = Convert.ToDateTime(db.reader["CREATED_DATE"]);
                UserRegModel.CREATED_LOC = db.reader["CREATED_LOC"].ToString();
                UserRegModel.UPDATED_BY = db.reader["UPDATED_BY"].ToString();
                UserRegModel.UPDATED_DATE = Convert.ToDateTime(db.reader["UPDATED_DATE"]);
                UserRegModel.UPDATED_LOC = db.reader["UPDATED_LOC"].ToString();
            }

            db.CloseReader();
            db.CloseConnection();


            return UserRegModel;

        }

        public DataTable getUserLst(Int64 roleID)
        {
            db.OpenConnection();
            db.command.CommandText = "PSP_GetUserList";
            db.command.CommandType = CommandType.StoredProcedure;
            db.command.CommandTimeout = 0;
            db.command.Parameters.Clear();
            db.command.Parameters.Add(new SqlParameter("@pID", roleID)).Direction = System.Data.ParameterDirection.Input;
            db.reader = db.command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(db.reader);

            db.CloseReader();
            db.CloseConnection();

            return dt;
        }
        public DataTable getUserLst2(int roleID, int resourceID)
        {
            db.OpenConnection();
            db.command.CommandText = "PSP_GetUserList2";
            db.command.CommandType = CommandType.StoredProcedure;
            db.command.CommandTimeout = 0;
            db.command.Parameters.Clear();
            db.command.Parameters.Add(new SqlParameter("@pRoleID", roleID)).Direction = System.Data.ParameterDirection.Input;
            db.command.Parameters.Add(new SqlParameter("@pResourceID", resourceID)).Direction = System.Data.ParameterDirection.Input;
            db.reader = db.command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(db.reader);

            db.CloseReader();
            db.CloseConnection();

            return dt;
        }

        public DataTable getAclLst(Int64 roleID)
        {
            string createdby = "Admin";
            string loc = "127.0.0.1";
            db.OpenConnection();
            db.command.CommandText = "PSP_ACL_LIST";
            db.command.CommandType = CommandType.StoredProcedure;
            db.command.CommandTimeout = 0;
            db.command.Parameters.Clear();
            db.command.Parameters.Add(new SqlParameter("@pRoleID", roleID)).Direction = System.Data.ParameterDirection.Input;
            db.command.Parameters.Add(new SqlParameter("@Record_typ", "1")).Direction = System.Data.ParameterDirection.Input;
            db.command.Parameters.Add(new SqlParameter("@createdby", createdby)).Direction = System.Data.ParameterDirection.Input;
            db.command.Parameters.Add(new SqlParameter("@loc", loc)).Direction = System.Data.ParameterDirection.Input;
            db.reader = db.command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(db.reader);

            db.CloseReader();
            db.CloseConnection();

            return dt;
        }



        public DataTable getPopUpResource(Int64 ParentID)
        {
            db.OpenConnection();
            db.command.CommandText = "PSP_GetPopUpResourceList";
            db.command.CommandType = CommandType.StoredProcedure;
            db.command.CommandTimeout = 0;
            db.command.Parameters.Clear();
            db.command.Parameters.Add(new SqlParameter("@pParentID", ParentID)).Direction = System.Data.ParameterDirection.Input;
            db.reader = db.command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(db.reader);

            db.CloseReader();
            db.CloseConnection();

            return dt;
        }
        #endregion

    }
}
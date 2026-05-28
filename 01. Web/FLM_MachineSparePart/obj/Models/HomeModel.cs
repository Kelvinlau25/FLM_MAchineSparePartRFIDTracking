using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Web.Mvc;
using FILM_Sparepart_MVC.Helper_Code.Objects;
using System.ComponentModel.DataAnnotations.Schema;
using CompareAttribute = System.ComponentModel.DataAnnotations.CompareAttribute;

namespace HomeModel
{
    public class SideBarContent
    {

        public int ID_ACL_RESOURCE { get; set; }
        public int RESOURCE_PARENT_ID { get; set; }
        public string RESOURCE_DESC { get; set; }
        public string RESOURCE_name { get; set; }
        public string RESOURCE_VIEW { get; set; }
        public string RESOURCE_CONTROLLER { get; set; }
        public int LAYER { get; set; }
        public int ACTION { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "* Please Enter Old Password.")]
        [Display(Name = "Old Password")]
        public string OLD_PASSWORD { get; set; }
        [Required(ErrorMessage = "* Please Enter New Password.")]
        [Display(Name = "New Password")]
        public string NEW_PASSWORD { get; set; }
        [Required(ErrorMessage = "* Please Confirm Password by Enter New Password again.")]
        [NotMapped]
        [Compare("NEW_PASSWORD")]        
        [Display(Name = "Confirm New Password")]
        public string CONFIRM_NEW_PASSWORD { get; set; }     
    }
    public class AuthenticatorModel
    {
        public int ID_ACL_USER { get; set; }
        public string USER_ID { get; set; }
        public string USR_EMAIL { get; set; }
        public string COMPANY { get; set; }
        public string EMP_NO { get; set; }
        public string EMP_NAME { get; set; }
        public int ID_ACL_ROLE { get; set; }
        public string ROLE_NAME { get; set; }
        public string ROLE_DESC { get; set; }
        public int ID_ACL_RESOURCE { get; set; }
        public string RESOURCE_NAME { get; set; }
        public string RESOURCE_DESC { get; set; }
        public bool VALID_USER { get; set; }
        [Required(ErrorMessage = "* Please Enter Username.")]
        [Display(Name = "Username")]
        public string LOGIN_ID { get; set; }
        [Required(ErrorMessage = "* Please Enter password.")]
        [Display(Name = "Password")]
        public string PASSWORD { get; set; }

    }

    public class DB : DatabaseModel.Database
    {
        public DB()
        {

        }

        public AuthenticatorModel ValidateUserInfo(string userAD, string systemName)
        {
            OpenConnection();
            command.CommandText = "PSP_ACL_USER_SEL";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pUserID", userAD)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@pSystemName", systemName)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            AuthenticatorModel AuthenticatorModel = null;
            AuthenticatorModel = new AuthenticatorModel();
            AuthenticatorModel.VALID_USER = false;
            while (reader.Read())
            {
                if (reader.HasRows)
                {

                    AuthenticatorModel.ID_ACL_USER = Convert.ToInt16(reader["ID_ACL_USER"]);
                    AuthenticatorModel.ID_ACL_ROLE = Convert.ToInt16(reader["ID_ACL_ROLE"]);
                    AuthenticatorModel.ID_ACL_RESOURCE = Convert.ToInt16(reader["ID_ACL_RESOURCE"]);
                    AuthenticatorModel.USER_ID = reader["USER_ID"].ToString();
                    AuthenticatorModel.USR_EMAIL = reader["USR_EMAIL"].ToString();
                    AuthenticatorModel.COMPANY = reader["COMPANY"].ToString();
                    AuthenticatorModel.EMP_NO = reader["EMP_NO"].ToString();
                    AuthenticatorModel.EMP_NAME = reader["EMP_NAME"].ToString();
                    AuthenticatorModel.ROLE_NAME = reader["ROLE_NAME"].ToString();
                    AuthenticatorModel.ROLE_DESC = reader["ROLE_DESC"].ToString();
                    AuthenticatorModel.RESOURCE_NAME = reader["RESOURCE_NAME"].ToString();
                    AuthenticatorModel.RESOURCE_DESC = reader["RESOURCE_DESC"].ToString();
                    AuthenticatorModel.PASSWORD = reader["USR_PASSWORD"].ToString();
                    AuthenticatorModel.VALID_USER = true;
                }
                else
                {
                    AuthenticatorModel.VALID_USER = false;
                }
            }


            CloseReader();
            CloseConnection();


            return AuthenticatorModel;

        }

        #region menu
        public DataTable sideBarDB(Int64 roleID, string SystemName)
        {
            OpenConnection();
            //command.CommandText = "PSP_ACL_SIDEBAR";
            command.CommandText = "PSP_ACL_SIDEBAR_FILM";
            
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@ID_ACL_ROLE", roleID)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@pSystemName", SystemName)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();

            return dt;
        }

        public DataTable oldPassword(int userID)
        {
            OpenConnection();
            command.CommandText = "PSP_ACL_CHANGE_PASSWORD";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@userID", userID)).Direction = System.Data.ParameterDirection.Input;
            reader =  command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load( reader);

            CloseReader();
            CloseConnection();

            return dt;
        }

        public string NewPassWord(int userID, string newPassword)
        {
            OpenConnection();
            command.CommandText = "PSP_ACL_CHANGE_PASSWORD_MAINT";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@userID", userID)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@newPassword", newPassword)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@returnID", SqlDbType.VarChar, 1)).Direction = System.Data.ParameterDirection.Output;
            reader =  command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load( reader);

            CloseReader();
            CloseConnection();

            return  command.Parameters["@returnID"].Value.ToString();


        }
        #endregion
    }
}
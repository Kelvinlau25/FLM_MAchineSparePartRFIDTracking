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
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Net.Mail;
using FILM_Sparepart_MVC.DAL;

namespace DBModel
{

    #region Models

    public class UserRegModel
    {
        public Int64 ID_ACL_USER { get; set; }

        [Display(Name = "User ID")]
        public string USER_ID { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string USR_EMAIL { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string EMP_NAME { get; set; }

        [Required]
        [Display(Name = "Emp No")]
        public string EMP_NO { get; set; }

        [Required]
        [Display(Name = "Company")]
        public string COMPANY { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string STATUS_IND { get; set; }
        public string RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public List<SelectListItem> DropdownCompany { get; set; }
        public List<string> SelectedSingleUserId { get; set; }
        public List<AdUserObj> SelectedUserLst { get; set; }

    }
    public class ResourceRegModel
    {
        public Int64 ID_ACL_RESOURCE { get; set; }

        [Required]
        [Display(Name = "Resource Name")]
        public string RESOURCE_NAME { get; set; }

        [Required]
        [Display(Name = "Resource Description")]
        public string RESOURCE_DESC { get; set; }

        [Required]
        [Display(Name = "Resource View")]
        public string RESOURCE_VIEW { get; set; }

        [Required]
        [Display(Name = "Resource Controller")]
        public string RESOURCE_CONTROLLER { get; set; }

        [Required]
        [Display(Name = "Parent ID")]
        public Int64 RESOURCE_PARENT_ID { get; set; }

        [Required]
        [Display(Name = "Resource Seq")]
        public Int64 RESOURCE_SEQ { get; set; }

        [Required]
        [Display(Name = "Resource Status")]
        public string RESOURCE_STATUS { get; set; }

        public string RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string SEARCH_VALUE { get; set; }

    }
    public class RoleRegModel
    {
        public Int64 ID_ACL_ROLE { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        public string ROLE_NAME { get; set; }

        [Required]
        [Display(Name = "Role Description")]
        public string ROLE_DESC { get; set; }

        public Int64 ID_ACL_RESOURCE { get; set; }

        [Display(Name = "Resource")]
        public string RESOURCE_NAME { get; set; }

        //[Required]
        [Display(Name = "Role Status")]
        public string ROLE_STATUS { get; set; }
        public string RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string SEARCH_VALUE { get; set; }

        public List<SelectListItem> DropdownResource { get; set; }

        [Required]
        [Display(Name = "User Role")]
        public List<int> SelectedMultiUserId { get; set; }

        public List<UserObj> SelectedUserLst { get; set; }

    }
    public class ResourceAclModel
    {
        public Int64 ID_ACL_RESOURCE { get; set; }

        [Display(Name = "Resource")]
        public string RESOURCE_NAME { get; set; }

        public string RESOURCE_DESC { get; set; }

        public Boolean View_Right { get; set; }
        public Boolean Add_Right { get; set; }
        public Boolean Edit_Right { get; set; }
        public Boolean Delete_Right { get; set; }

    }
    public class RoleModel
    {
        public Int64 ID_ACL_ROLE { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        public string ROLE_NAME { get; set; }

        [Required]
        [Display(Name = "Role Description")]
        public string ROLE_DESC { get; set; }
    }
    public class AclViewModel
    {
        public Int64 ID_ACL_Access_Control { get; set; }
        public RoleModel roleModel { get; set; }
        public IEnumerable<ResourceAclModel> resourceAclModel { get; set; }
        //public ResourceAclModel resourceAclModel { get; set; }
    }

    public class RootResourceModel
    {
        public Int64 ID_ACL_RESOURCE { get; set; }
        public string RESOURCE_NAME { get; set; }
        public Int64 RESOURCE_PARENT_ID { get; set; }

    }

    public class Layer1ResourceModel
    {
        public Int64 ID_ACL_RESOURCE { get; set; }
        public string RESOURCE_NAME { get; set; }
        public Int64 RESOURCE_PARENT_ID { get; set; }

    }

    public class Layer2ResourceModel
    {
        public Int64 ID_ACL_RESOURCE { get; set; }
        public string RESOURCE_NAME { get; set; }
        public Int64 RESOURCE_PARENT_ID { get; set; }

    }

    public class PopUpResourceModel
    {
        //public IEnumerable<RootResourceModel> RootResourceModel { get; set; }
        //public IEnumerable<Layer1ResourceModel> Layer1ResourceModel { get; set; }
        //public IEnumerable<Layer2ResourceModel> Layer2ResourceModel { get; set; }
        public List<RootResourceModel> RootResourceModel { get; set; }
        public List<Layer1ResourceModel> Layer1ResourceModel { get; set; }
        public List<Layer2ResourceModel> Layer2ResourceModel { get; set; }
    }



    //public class CheckBoxModel
    //{
    //    //Value of checkbox 
    //    public int Value { get; set; }
    //    //description of checkbox 
    //    public string Text { get; set; }
    //    //whether the checkbox is selected or not
    //    public bool IsChecked { get; set; }
    //}
    public enum Stat
    {
        Active,
        Inactive
    }
    public enum Status
    {
        [Display(Name = "Active")]
        A,
        [Display(Name = "Inactive")]
        I

    }

    #endregion

    public class Database
    {

        protected string sql { get; set; }
        protected SqlCommand command;
        protected SqlConnection c;
        public SqlDataReader reader;
        protected SqlTransaction tran;
        protected SqlDataAdapter sqladp;
        protected string Message;

        // constructor
        public void database()
        {
        }
        // method
        public void OpenConnection()
        {
            command = new SqlCommand();
            c = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLCon"].ConnectionString);
            command.Connection = c;
            c.Open();
        }
        public string ExecuteNonQuery()
        {
            string i = null;
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                i = e.Message;
            }
            return i;
        }
        public void ExecuteReader()
        {
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException e)
            {
                Message = e.Message;
            }
        }
        public void CloseReader()
        {
            reader.Close();
            reader.Dispose();
            reader = null;
        }
        public void CloseConnection()
        {
            c.Close();
            c.Dispose();
            c = null;
        }
    }

    public class DB : Database
    {
        RfidAuditContext _rfidAuditCtx;
        public DB()
        {
            _rfidAuditCtx = new RfidAuditContext();
        }

        public DataTable List(string Table, string TableID, string Search,
        string Value, string SortField, string Direction,
        string FrmRowno, string ToRowno, string Deleted)
        {
            OpenConnection();
            command.CommandText = "PSP_COMMON_LIST";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@Table", Table)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@TableID", TableID)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@Search", Search)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@Value", Value)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@SortField", SortField)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@Direction", Direction)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@FrmRowno", FrmRowno)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@ToRowno", ToRowno)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@Deleted", Deleted)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();

            return dt;
        }

       
        public string UserRegMaint(UserRegModel user, String record_typ)
        {
            string result = "";
            string return_value = "0";
            string createdby = "Admin";
            string loc = "127.0.0.1";


            OpenConnection();
            try
            {
                command.CommandText = "PSP_UserReg_Maint";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@ID", user.ID_ACL_USER)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@USER_ID", user.SelectedSingleUserId[0])).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@COMPANY", user.COMPANY)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@EMP_NO", user.EMP_NO)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@EMP_NAME", user.EMP_NAME)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@USR_EMAIL", user.USR_EMAIL)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@STATUS_IND", user.STATUS_IND)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@Record_typ", record_typ)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@createdby", createdby)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@loc", loc)).Direction = System.Data.ParameterDirection.Input;

                return_value = command.ExecuteScalar().ToString();
                if (Convert.ToInt32(return_value) > 0)
                {
                    result = "Data save successfully.";
                }
                return result;
            }
            catch (Exception ex)
            {
                return result = ex.Message;
            }
            finally
            {

                CloseConnection();
            }

        }

        public UserRegModel getUserData(string id)
        {
            OpenConnection();
            command.CommandText = "PSP_UserReg_SEL";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            UserRegModel UserRegModel = null;

            while (reader.Read())
            {
                UserRegModel = new UserRegModel();
                UserRegModel.ID_ACL_USER = Convert.ToInt32(reader["ID_ACL_USER"]);
                UserRegModel.USER_ID = reader["USER_ID"].ToString();
                UserRegModel.USR_EMAIL = reader["USR_EMAIL"].ToString();
                UserRegModel.EMP_NO = reader["EMP_NO"].ToString();
                UserRegModel.EMP_NAME = reader["EMP_NAME"].ToString();
                UserRegModel.COMPANY = reader["COMPANY"].ToString();
                //UserRegModel.STATUS_IND = (Status)Enum.Parse(typeof(Status), reader["STATUS_IND"].ToString());
                UserRegModel.STATUS_IND = reader["STATUS_IND"].ToString();
                UserRegModel.RECORD_TYP = reader["RECORD_TYP"].ToString();
                UserRegModel.CREATED_BY = reader["CREATED_BY"].ToString();
                UserRegModel.CREATED_DATE = Convert.ToDateTime(reader["CREATED_DATE"]);
                UserRegModel.CREATED_LOC = reader["CREATED_LOC"].ToString();
                UserRegModel.UPDATED_BY = reader["UPDATED_BY"].ToString();
                UserRegModel.UPDATED_DATE = Convert.ToDateTime(reader["UPDATED_DATE"]);
                UserRegModel.UPDATED_LOC = reader["UPDATED_LOC"].ToString();
            }

            CloseReader();
            CloseConnection();


            return UserRegModel;

        }

        public string ResourceRegMaint(ResourceRegModel resource, String record_typ)
        {
            string result = "";
            string return_value = "0";
            string createdby = "Admin";
            string loc = "127.0.0.1";


            OpenConnection();
            try
            {

                command.CommandText = "PSP_ResourceReg_Maint";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@ID_ACL_RESOURCE", resource.ID_ACL_RESOURCE)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@RESOURCE_NAME", resource.RESOURCE_NAME)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@RESOURCE_DESC", resource.RESOURCE_DESC)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@RESOURCE_VIEW", resource.RESOURCE_VIEW)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@RESOURCE_CONTROLLER", resource.RESOURCE_CONTROLLER)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@RESOURCE_PARENT_ID", resource.RESOURCE_PARENT_ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@RESOURCE_SEQ", resource.RESOURCE_SEQ)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@RESOURCE_STATUS", resource.RESOURCE_STATUS)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@Record_typ", record_typ)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@createdby", createdby)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@loc", loc)).Direction = System.Data.ParameterDirection.Input;

                return_value = command.ExecuteScalar().ToString();
                if (Convert.ToInt32(return_value) > 0)
                {
                    result = "Data save successfully.";
                }
                return result;
            }
            catch (Exception ex)
            {
                return result = ex.Message;
            }
            finally
            {

                CloseConnection();
            }

        }

        public ResourceRegModel getResourceData(string id)
        {
            OpenConnection();
            command.CommandText = "PSP_ResouceReg_SEL";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            ResourceRegModel ResourceRegModel = null;

            while (reader.Read())
            {
                ResourceRegModel = new ResourceRegModel();
                ResourceRegModel.ID_ACL_RESOURCE = Convert.ToInt32(reader["ID_ACL_RESOURCE"]);
                ResourceRegModel.RESOURCE_NAME = reader["RESOURCE_NAME"].ToString();
                ResourceRegModel.RESOURCE_DESC = reader["RESOURCE_DESC"].ToString();
                ResourceRegModel.RESOURCE_VIEW = reader["RESOURCE_VIEW"].ToString();
                ResourceRegModel.RESOURCE_CONTROLLER = reader["RESOURCE_CONTROLLER"].ToString();
                ResourceRegModel.RESOURCE_PARENT_ID = Convert.ToInt32(reader["RESOURCE_PARENT_ID"]);
                ResourceRegModel.RESOURCE_SEQ = Convert.ToInt32(reader["RESOURCE_SEQ"]);
                ResourceRegModel.RESOURCE_STATUS = reader["RESOURCE_STATUS"].ToString();
                ResourceRegModel.RECORD_TYP = reader["RECORD_TYP"].ToString();
                ResourceRegModel.CREATED_BY = reader["CREATED_BY"].ToString();
                ResourceRegModel.CREATED_DATE = Convert.ToDateTime(reader["CREATED_DATE"]);
                ResourceRegModel.CREATED_LOC = reader["CREATED_LOC"].ToString();
                ResourceRegModel.UPDATED_BY = reader["UPDATED_BY"].ToString();
                ResourceRegModel.UPDATED_DATE = Convert.ToDateTime(reader["UPDATED_DATE"]);
                ResourceRegModel.UPDATED_LOC = reader["UPDATED_LOC"].ToString();
            }

            CloseReader();
            CloseConnection();


            return ResourceRegModel;

        }

        public string RoleRegMaint(RoleRegModel role, String record_typ)
        {
            string result = "";
            string return_value = "0";
            string createdby = "Admin";
            string loc = "127.0.0.1";

            OpenConnection();
            try
            {
                command.CommandText = "PSP_RoleReg_Maint";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@ID_ACL_ROLE", role.ID_ACL_ROLE)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@ROLE_NAME", role.ROLE_NAME)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@ROLE_DESC", role.ROLE_DESC)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@ID_ACL_RESOURCE", role.ID_ACL_RESOURCE)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@ROLE_STATUS", role.ROLE_STATUS)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@Record_typ", record_typ)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@createdby", createdby)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@loc", loc)).Direction = System.Data.ParameterDirection.Input;

                return_value = command.ExecuteScalar().ToString();
                if (Convert.ToInt32(return_value) > 0)
                {
                    result = UserRoleMaint(Convert.ToInt32(return_value), role, record_typ);
                }
                return result;
            }
            catch (Exception ex)
            {
                return result = ex.Message;
            }
            finally
            {
                CloseConnection();
            }

        }
        public string UserRoleMaint(Int64 newRoleID, RoleRegModel userRole, String record_typ)
        {
            string result = "";
            string return_value = "0";
            string createdby = "Admin";
            string loc = "127.0.0.1";

            if (userRole.SelectedMultiUserId.Count > 0)
            {
                for (int i = 0; i < userRole.SelectedMultiUserId.Count; i++)
                {
                    int id_ACL_user = userRole.SelectedMultiUserId[i];

                    try
                    {
                        command.CommandText = "PSP_UserRole_Maint";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 0;
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter("@ID_ACL_ROLE", newRoleID)).Direction = System.Data.ParameterDirection.Input;
                        command.Parameters.Add(new SqlParameter("@ID_ACL_USER", id_ACL_user)).Direction = System.Data.ParameterDirection.Input;
                        command.Parameters.Add(new SqlParameter("@row", i)).Direction = System.Data.ParameterDirection.Input;
                        command.Parameters.Add(new SqlParameter("@Record_typ", record_typ)).Direction = System.Data.ParameterDirection.Input;
                        command.Parameters.Add(new SqlParameter("@createdby", createdby)).Direction = System.Data.ParameterDirection.Input;
                        command.Parameters.Add(new SqlParameter("@loc", loc)).Direction = System.Data.ParameterDirection.Input;
                        return_value = command.ExecuteScalar().ToString();
                        if (Convert.ToInt32(return_value) > 0)
                        {
                            result = "Data save successfully.";
                        }
                    }
                    catch (Exception ex)
                    {
                        return result = ex.Message;
                    }
                }
            }
            return result;
        }
        public RoleRegModel getRoleData(string id)
        {
            OpenConnection();
            command.CommandText = "PSP_RoleReg_SEL";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            RoleRegModel RoleRegModel = null;

            while (reader.Read())
            {
                RoleRegModel = new RoleRegModel();
                RoleRegModel.ID_ACL_ROLE = Convert.ToInt32(reader["ID_ACL_ROLE"]);
                RoleRegModel.ROLE_NAME = reader["ROLE_NAME"].ToString();
                RoleRegModel.ROLE_DESC = reader["ROLE_DESC"].ToString();
                RoleRegModel.ID_ACL_RESOURCE = Convert.ToInt32(reader["ID_ACL_RESOURCE"]);
                RoleRegModel.RESOURCE_NAME = reader["RESOURCE_NAME"].ToString();
                RoleRegModel.ROLE_STATUS = reader["ROLE_STATUS"].ToString();
                RoleRegModel.RECORD_TYP = reader["RECORD_TYP"].ToString();
                RoleRegModel.CREATED_BY = reader["CREATED_BY"].ToString();
                RoleRegModel.CREATED_DATE = Convert.ToDateTime(reader["CREATED_DATE"]);
                RoleRegModel.CREATED_LOC = reader["CREATED_LOC"].ToString();
                RoleRegModel.UPDATED_BY = reader["UPDATED_BY"].ToString();
                RoleRegModel.UPDATED_DATE = Convert.ToDateTime(reader["UPDATED_DATE"]);
                RoleRegModel.UPDATED_LOC = reader["UPDATED_LOC"].ToString();
            }

            CloseReader();
            CloseConnection();

            return RoleRegModel;
        }
        public DataTable getUserLst(Int64 roleID)
        {
            OpenConnection();
            command.CommandText = "PSP_GetUserList";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pID", roleID)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();

            return dt;
        }

        public DataTable getAclLst(Int64 roleID)
        {
            string createdby = "Admin";
            string loc = "127.0.0.1";
            OpenConnection();
            command.CommandText = "PSP_ACL_LIST";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pRoleID", roleID)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@Record_typ", "1")).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@createdby", createdby)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@loc", loc)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();

            return dt;
        }

        public Boolean ACL_Reset(Int64 roleID)
        {
            Boolean reset_status = false;
            string return_value = "0";

            OpenConnection();
            try
            {
                command.CommandText = "PSP_ACL_Reset";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@ID_ACL_ROLE", roleID)).Direction = System.Data.ParameterDirection.Input;

                return_value = command.ExecuteScalar().ToString();
                if (Convert.ToInt32(return_value) > 0)
                {
                    reset_status = true;
                }
                return reset_status;
            }
            catch (Exception ex)
            {
                return reset_status = false;
            }
            finally
            {
                CloseConnection();
            }
        }
        public string RoleAccessRightMaint(Int64 resourceID, AclViewModel aclView, string accessRight)
        {
            string result = "";
            string return_value = "0";
            string createdby = "Admin";
            string loc = "127.0.0.1";

            OpenConnection();
            try
            {
                command.CommandText = "PSP_ACL_Maint";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@ID_ACL_ROLE", aclView.roleModel.ID_ACL_ROLE)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@ID_ACL_RESOURCE", resourceID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@ACCESS_TYPE", accessRight)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@Record_typ", "1")).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@createdby", createdby)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@loc", loc)).Direction = System.Data.ParameterDirection.Input;

                return_value = command.ExecuteScalar().ToString();
                if (Convert.ToInt32(return_value) > 0)
                {
                    result = "Data save successfully.";
                }
                return result;
            }
            catch (Exception ex)
            {
                return result = ex.Message;
            }
            finally
            {

                CloseConnection();
            }
        }

        public DataTable getPopUpResource(Int64 ParentID)
        {
            OpenConnection();
            command.CommandText = "PSP_GetPopUpResourceList";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pParentID", ParentID)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();

            return dt;
        }

    }


    public class SparePartModel
    {
        public int SPARE_PART_MODEL_ID { get; set; }
        public string MODEL_NO { get; set; }
        public string REMARKS { get; set; }
        public string RECORD_TYPE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string SEARCH_VALUE { get; set; }


    }

    // new 
    #region Models
    public class LabelModel
    {
        [DisplayName("Cell No From")]
        public string cellNoFrom { get; set; }

        [DisplayName("Cell No To")]
        public string cellNoTo { get; set; }
        public string CustID { get; set; }
        public string PrtContct { get; set; }
        public string DateInstalled { get; set; }
        public string Company { get; set; }
        public string QRCode { get; set; }
        public string img { get; set; }
    }
    public class LabelList
    {
        public List<LabelModel> label { get; set; }
    }
    public class PCAuditModel
    {
        public string SITE_TXT { get; set; }
        public string DEPT { get; set; }
        public string MICROSOFTSURFACE { get; set; }
        public string IPC { get; set; }
        public string NB { get; set; }
        public string TEST { get; set; }
        public string NETWORKSWITCH { get; set; }
        public string TESTAUDIT { get; set; }
        public string MPC { get; set; }
        public string RFID { get; set; }
        public string VMC { get; set; }
        public string PC { get; set; }
        public string INTANGIBLEASSET_SOFTWARE { get; set; }
        public string NETWORKROUTER { get; set; }
        public string MACBOOK { get; set; }
        public string NETWORKFIREWALL { get; set; }
        public string IPHONE { get; set; }
        public string FIREWALL { get; set; }
        public string PRINTER { get; set; }
        public string TOTAL_COMPLETED { get; set; }
        public string TOTAL_TO_COMPLETE { get; set; }
        public DataTable reportTbl { get; set; }
        public string SEARCH { get; set; }
        public List<string> selectedAsset { get; set; }
        public DateTime? reportMonth { get; set; }
    }
    public class EmailModel
    {
        public Int64 ID { get; set; }

        [Required]
        [Display(Name = "EmpNo")]
        public string EmpNo { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string NAME { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string EMAIL { get; set; }


        [Required]
        [Display(Name = "Status")]
        public string STATUS_IND { get; set; }
        public string RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string SEARCH_VALUE { get; set; }

    }

    //---------------------------Audit----------------------------------
    public class AuditModel
    {
        public Int64 ID { get; set; }

        [Required]
        [StringLength(24, MinimumLength = 24)]
        [Display(Name = "RFID")]
        public string RFID { get; set; }
        public DateTime audit_date { get; set; }

        [Required]
        [Display(Name = "Cel No")]
        public string Cel_No { get; set; } //mainKey

        [Required]
        [Display(Name = "Owner")]
        public string Owner { get; set; } //col2

        [Required]
        [Display(Name = "Computer Name")]
        public string Computer_Name { get; set; } //col1


        [Display(Name = "Type")]
        public string Type { get; set; } //col3


        [Display(Name = "Purchase Date")]
        public string Purchase_Date { get; set; } //col4

        [Display(Name = "Department")]
        public string Department { get; set; } //col5

        [Display(Name = "Company")]
        public string Company { get; set; } //col6

        [Display(Name = "Location")]
        public string Location { get; set; } //col7

        [Display(Name = "RFID Type")]
        public string RFID_Type { get; set; } //col8

        [Display(Name = "Revolution Lab Date")]
        public string Revolution_Lab_Date { get; set; } //col9

        [Display(Name = "Fixed Asset Tag")]
        public string Fixed_Asset_Tag { get; set; } //col10

        [Display(Name = "Department_Name")]
        public string DEPARTMENT_NAME { get; set; }
        [Display(Name = "Company_Name")]
        public string COMPANY_NAME { get; set; }

        [Display(Name = "Location_Name")]
        public string LOCATION_NAME { get; set; }
        public string status { get; set; }
        public string auditby { get; set; }
        public string remark { get; set; }

        public string auditdatetime { get; set; }

        [Display(Name = "Status")]
        public string STATUS_IND { get; set; }
        public string RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public List<AuditModel> auditinfo { get; set; }
        public string Total { get; set; }
        public int count { get; set; }
        public string SITE { get; set; }
        public string LOCATION1 { get; set; }
        public int TTL_VALID { get; set; }
        public int TTL_ITEM { get; set; }
    }
    public class MyViewModel
    {
        public string status { get; set; }
        public int count { get; set; }
    }
    //------------------------Renewal-----------------------------------
    public class RenewalModel
    {
        public decimal Total { get; set; }
        public decimal Total_Exp { get; set; }
        public decimal Total_NtExp { get; set; }
        public decimal Percentage_Exp { get; set; }
        public decimal Percentage_NtExp { get; set; }
        public List<RenewalModel> rnwinfo { get; set; }

    }
    //------------------------Registration------------------------------
    public class RegistrationModel
    {
        public Int64 ID { get; set; }
        public String SITE_TXT { get; set; }
        public String ITEM_TXT { get; set; }
        public String STATUS_TXT { get; set; }
        [Required]
        public String LOC { get; set; }
        [Required]
        public String DEPT { get; set; }
        [Required]
        public String Site { get; set; }
        [Required]
        public String Item_Type { get; set; }
        [Required]
        public String CEL_Number { get; set; }
        [Required]
        public String Status { get; set; }
        [Required]
        public String Owner { get; set; }
        [Required]
        public String Fixed_Asset { get; set; }
        [Required]
        public String Location { get; set; }
        [Required]
        public String RFID { get; set; }
        [Required]
        public String Department { get; set; }
        [Required]
        public String Model { get; set; }
        [Required]
        public DateTime? Purchase_Date { get; set; }
        [Required]
        public String Manufacturer { get; set; }
        [Required]
        public DateTime? Waranty_Expiry { get; set; }
        [Required]
        public String OS_version { get; set; }
        [Required]
        public DateTime? EOL_Support { get; set; }
        [Required]
        public String Serial_Number { get; set; }
        [Required]
        public String Description { get; set; }
        public String Asset_Type { get; set; }
        public String IP_Address { get; set; }
        public String MAC_Address { get; set; }
        public String Host_Name { get; set; }
        public String Sys_App { get; set; }
        public DateTime? Installation_Date { get; set; }
        public String RECORD_TYP { get; set; }
        public String STATUS_IND { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public List<RegistrationModel> reginfo { get; set; }
        public decimal Total { get; set; }
        public decimal Total_Exp { get; set; }
        public decimal Total_NtExp { get; set; }
        public decimal Percentage_Exp { get; set; }
        public decimal Percentage_NtExp { get; set; }
        //public bool RadioBtn { get; set; }
        //public bool RadioBtn1 { get; set; }
        //public string RadioType { get; set; }
        //public string RadioName { get; set; }
        public string AgeByYear { get; set; }
        public string AgeByMonth { get; set; }
        public string AgeByDay { get; set; }
        public RegistrationModel RegModel { get; set; }

    }

    public class RegistrationVIEWmodel
    {
        public RegistrationModel RegistrationModel { get; set; }
        public IEnumerable<SelectListItem> Statusdropdownitem { get; set; }
        public IEnumerable<SelectListItem> Sitedropdownitem { get; set; }
        public IEnumerable<SelectListItem> Locdropdownitem { get; set; }
        public IEnumerable<SelectListItem> Ctgdropdownitem { get; set; }
        public IEnumerable<SelectListItem> Deptdropdownitem { get; set; }
        public RegistrationVIEWmodel()
        {
            this.Sitedd = new List<SelectListItem>();
            this.Locdd = new List<SelectListItem>();
            this.Deptdd = new List<SelectListItem>();

        }
        public List<SelectListItem> Sitedd { get; set; }
        public List<SelectListItem> Locdd { get; set; }
        public List<SelectListItem> Deptdd { get; set; }
        public string CelNum { get; set; }
        public string SiteCode { get; set; }
        public string ItemCode { get; set; }
        public string Message { get; set; }
    }
    //--------------------------Multiple Model--------------

    public class RevRegModel
    {
        public Int64 ID_REV_LAB { get; set; }
        [Required]
        [Display(Name = "Value")]
        public Int64 ID_MM_MAIN_DROPDOWN { get; set; }
        public string MAIN_DROPDOWN_TEXT { get; set; }   //get action dropdown
        [Required]
        [Display(Name = "Value")]
        public Int64 ID_MM_SUB_DROPDOWN { get; set; }
        public string SUB_DROPDOWN_TEXT { get; set; }   //get location dropdown
        public DateTime SCANNED_DATE { get; set; }

        public String RFID { get; set; }
        public String Department { get; set; }
        public String Model { get; set; }
        public String Site { get; set; }
        [Required]
        public String Item_Type { get; set; }
        public String CEL_Number { get; set; }
        [Required]
        public String Status { get; set; }
        public String Owner { get; set; }
        [Required]
        [Display(Name = "Value")]
        public string REMARK { get; set; }
        [Required]
        [Display(Name = "Value")]
        //public Int64 UPDATEBY { get; set; }
        public string UPDATEBY { get; set; }
        public string UPDATEBY_TEXT { get; set; }   //get updateby dropdown
        public string SCAN_STATUS { get; set; }
        public string STATUS_IND { get; set; }
        public Int64 RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        //  public string ID_Asset_Reg { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public List<RevRegModel> revinfo { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string MOVEMENT_STATUS { get; set; }
        public String MOVSTT_TXT { get; set; }

    }

    //------------------------------------------------------------------
    public class RevolutionLabViewModel
    {
        // public RevolutionLabModel RevolutionLabModel { get; set; }
        public RevRegModel RevRegModel { get; set; }
        public IEnumerable<SelectListItem> DropdownLoc { get; set; }
        public IEnumerable<SelectListItem> DropdownStt { get; set; }
        public IEnumerable<SelectListItem> DropdownUpBy { get; set; }
        public IEnumerable<SelectListItem> DropdownMovStt { get; set; }
    }
    //------------------------MainDropdown----------------------------
    public class MainDropDownModel
    {
        public Int64 ID_MM_MAIN_DROPDOWN { get; set; }
        [Required]
        [Display(Name = "Dropdown")]
        public String MAIN_DROPDOWN_TYPE { get; set; }
        [Required]
        [Display(Name = "Value")]
        public String MAIN_DROPDOWN_TEXT { get; set; }
        [Required]
        [Display(Name = "Description")]
        public String DESCRIPTION { get; set; }
        //[Required]
        //[Display(Name = "MM_DROPDOWN_STATUS")]
        //public String MM_DROPDOWN_STATUS { get; set; }
        public string REC_TYPE { get; set; }
        [Required]
        [Display(Name = "Status")]
        public string STATUS_IND { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string SEARCH_VALUE { get; set; }
        [Required]
        public string CODE { get; set; }
    }
    //---------------------MainDropdown Audit Trail-------------------
    public class MainDropdown_AT
    {
        public long SQ_ID { get; set; }
        public string KEY_FIELD { get; set; }
        public Nullable<long> KEY_VALUE { get; set; }
        public string FIELD_NAME { get; set; }
        public string B4_UPDATE { get; set; }
        public string AF_UPDATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string HEADER_ID { get; set; }
    }
    //------------------------SubDropdown-----------------------------
    public class SubDropdownModel
    {
        public Int64 ID_MM_SUB_DROPDOWN { get; set; }
        [Required]
        [Display(Name = "Dropdown")]
        public Int64 ID_MM_MAIN_DROPDOWN { get; set; }
        [Required]
        public string MAIN_DROPDOWN_TEXT { get; set; }
        [Required]
        public string SUB_DROPDOWN_TYPE { get; set; }
        [Required]
        [Display(Name = "Value")]
        public string SUB_DROPDOWN_TEXT { get; set; }
        [Required]
        [Display(Name = "Status")]
        public string STATUS_IND { get; set; }
        public string MAIN_DROPDOWN_TYPE { get; set; }
        [Display(Name = "Record Type")]
        public Int64 RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string SEARCH_VALUE { get; set; }
        [Required]
        public string CODE { get; set; }
    }
    public class SubDropdownViewModel
    {
        public SubDropdownModel SubDropdownModel { get; set; }
        public IEnumerable<SelectListItem> DropdownItem { get; set; }
        public IEnumerable<SubDropdownModel> SubModel { get; set; }
    }
    //---------------------SubDropdown Audit Trail-------------------
    public class SubDropdown_AT
    {
        public long SQ_ID { get; set; }
        public string KEY_FIELD { get; set; }
        public Nullable<long> KEY_VALUE { get; set; }
        public string FIELD_NAME { get; set; }
        public string B4_UPDATE { get; set; }
        public string B4_DROPDOWN_TEXT { get; set; }
        public string AF_UPDATE { get; set; }
        public string AF_DROPDOWN_TEXT { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string HEADER_ID { get; set; }
    }
    //---------------------------RFID SCAN------------------------------------
    #region RFID Scan
    public class CompanyModel
    {
        public Int64 ID { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string CODE { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string NAME { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string DESCRIPTION { get; set; }


        [Required]
        [Display(Name = "Status")]
        public string STATUS_IND { get; set; }
        public string RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string SEARCH_VALUE { get; set; }

    }

    public class DepartmentModel
    {
        public Int64 ID { get; set; }

        [Required]
        [Display(Name = "COMPANY_ID")]
        public Int64 ID_COMPANY { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string CODE { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string NAME { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string DESCRIPTION { get; set; }


        [Required]
        [Display(Name = "Status")]
        public string STATUS_IND { get; set; }

        [Required]
        [Display(Name = "Company_Name")]
        public string COMPANY_NAME { get; set; }
        public string RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public List<SelectListItem> DropdownCompany { get; set; }

    }

    public class LocationModel
    {
        public Int64 ID { get; set; }

        [Required]
        [Display(Name = "DEPARTMENT_ID")]
        public Int64 ID_DEPARTMENT { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string CODE { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string NAME { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string DESCRIPTION { get; set; }


        [Required]
        [Display(Name = "Status")]
        public string STATUS_IND { get; set; }

        [Required]
        [Display(Name = "Department_Name")]
        public string DEPARTMENT_NAME { get; set; }
        [Required]
        [Display(Name = "Company_Name")]
        public string COMPANY_NAME { get; set; }

        public string RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public List<SelectListItem> DropdownCompany { get; set; }
        public List<SelectListItem> DropdownDepartment { get; set; }

    }

    public class RFIDModel
    {
        public Int64 ID { get; set; }

        [Required]
        [Display(Name = "RFID")]
        public string RFID { get; set; }

        [Required]
        [Display(Name = "Key")]
        public string mainKey { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string col1 { get; set; }

        [Required]
        [Display(Name = "Company")]
        public string col2 { get; set; }


        [Required]
        [Display(Name = "IP")]
        public string col3 { get; set; }

        [Required]
        [Display(Name = "MAC")]
        public string col4 { get; set; }
        [Required]
        [Display(Name = "COMPNAME")]
        public string col5 { get; set; }

        [Display(Name = "Department_Name")]
        public string DEPARTMENT_NAME { get; set; }
        [Display(Name = "Company_Name")]
        public string COMPANY_NAME { get; set; }
        [Required]
        [Display(Name = "Location_Name")]
        public string LOCATION_NAME { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string STATUS_IND { get; set; }
        public string RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string SEARCH_VALUE { get; set; }

        public List<SelectListItem> DropdownCompany { get; set; }
        public List<SelectListItem> DropdownDepartment { get; set; }
        public List<SelectListItem> DropdownLocation { get; set; }

    }

    #endregion
    //-------------------------Record Movement---------------------------------
    #region RECORD MOVEMENT
    public class search
    {
        public string Sr_no { get; set; }
        public string Name { get; set; }
    }
    public class RecordMovement
    {
        public Int64 ID { get; set; }
        [Required]
        [Display(Name = "CEL Number")]
        public string CEL_LABEL { get; set; }
        [Required]
        [Display(Name = "RFID Number")]
        public string RFID_NUM { get; set; }
        [Required]
        public string OWNER { get; set; }
        [Required]
        public string MOV_STT { get; set; }
        [Required]
        [Display(Name = "Movement Status")]
        public String MOV_TXT { get; set; }
        public string LOCATION { get; set; }
        public string UPDATED_DATE_ASSET { get; set; }
        [Required]
        public string PIC { get; set; }
        public string ROOT_CAUSE { get; set; }
        public String REC_TYPE { get; set; }
        public String STATUS_IND { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string PIC_NAME { get; set; }
        public string STATUS { get; set; }
        public int TOTAL { get; set; }
        public int TOTAL_In_Status { get; set; }
        public List<RecordMovement> recinfo { get; set; }

    }
    public class RecordMovementViewModel
    {
        public RecordMovement RecordMovement { get; set; }
        public IEnumerable<SelectListItem> RecMovSttdropdownitem { get; set; }
        public IEnumerable<SelectListItem> PICdropdownitem { get; set; }
        public IEnumerable<SelectListItem> CELdropdownitem { get; set; }
        public IEnumerable<SelectListItem> RFIDdropdownitem { get; set; }
        public IEnumerable<SelectListItem> Locdropdownitem { get; set; }

    }
    public class RecMov_AT
    {
        public long SQ_ID { get; set; }
        public string KEY_FIELD { get; set; }
        public Nullable<long> KEY_VALUE { get; set; }
        public string FIELD_NAME { get; set; }
        public string B4_UPDATE { get; set; }
        public string AF_UPDATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string HEADER_ID { get; set; }
    }
    #endregion

    public enum Stat2
    {
        Active,
        Dispose
    }
    #endregion

    public class Database1
    {
        protected string sql { get; set; }
        protected SqlCommand command;
        protected SqlConnection c;
        public SqlDataReader reader;
        protected SqlTransaction tran;
        protected SqlDataAdapter sqladp;
        protected string Message;

        // constructor
        public void database()
        {
        }
        // method
        public void OpenConnection()
        {
            command = new SqlCommand();
            c = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLCon"].ConnectionString);
            command.Connection = c;
            c.Open();
        }
        public string ExecuteNonQuery()
        {
            string i = null;
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                i = e.Message;
            }
            return i;
        }
        public void ExecuteReader()
        {
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException e)
            {
                Message = e.Message;
            }
        }
        public void CloseReader()
        {
            reader.Close();
            reader.Dispose();
            reader = null;
        }
        public void CloseConnection()
        {
            c.Close();
            c.Dispose();
            c = null;
        }
    }
    public class DB1 : Database1
    {
        public DB1()
        {

        }

        public DataTable List(string Table, string TableID, string Search,
        string Value, string SortField, string Direction,
        string FrmRowno, string ToRowno, string Deleted)
        {
            OpenConnection();
            command.CommandText = "PSP_COMMON_LIST";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@Table", Table)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@TableID", TableID)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@Search", Search)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@Value", Value)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@SortField", SortField)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@Direction", Direction)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@FrmRowno", FrmRowno)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@ToRowno", ToRowno)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@Deleted", Deleted)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();

            return dt;
        }

        public DataTable List2(string Table, string TableID, string Search,
        string Value, string SortField, string Direction,
        string FrmRowno, string ToRowno, string Deleted)
        {
            OpenConnection();
            command.CommandText = "PSP_COMMON_LIST_DTL";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@Table", Table)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@TableID", TableID)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@Search", Search)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@Value", Value)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@SortField", SortField)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@Direction", Direction)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@FrmRowno", FrmRowno)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@ToRowno", ToRowno)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@Deleted", Deleted)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();

            return dt;
        }

        public DataTable getPopUpResource(Int64 ParentID)
        {
            OpenConnection();
            command.CommandText = "PSP_GetPopUpResourceList";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pParentID", ParentID)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();

            return dt;
        }



        #region Asset Management
        //---------------------------------MainDropdown-------------------------------------------
        #region Maindropdown
        public MainDropDownModel getMainDropDownData(string id)
        {
            MainDropDownModel MainDropDownModel = null;
            try
            {
                OpenConnection();
                command.CommandText = "PSP_MainDropdown_SEL";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pID_MAINDROP", id)).Direction = System.Data.ParameterDirection.Input;
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    MainDropDownModel = new MainDropDownModel();
                    MainDropDownModel.ID_MM_MAIN_DROPDOWN = Convert.ToInt32(reader["ID_MM_MAIN_DROPDOWN"]);
                    MainDropDownModel.MAIN_DROPDOWN_TYPE = reader["MAIN_DROPDOWN_TYPE"].ToString();
                    MainDropDownModel.MAIN_DROPDOWN_TEXT = reader["MAIN_DROPDOWN_TEXT"].ToString();
                    MainDropDownModel.DESCRIPTION = reader["DESCRIPTION"].ToString().Trim();
                    MainDropDownModel.CODE = reader["CODE"].ToString();
                    MainDropDownModel.REC_TYPE = reader["REC_TYPE"].ToString();
                    MainDropDownModel.STATUS_IND = reader["STATUS_IND"].ToString();
                    MainDropDownModel.CREATED_BY = reader["CREATED_BY"].ToString();
                    MainDropDownModel.CREATED_DATE = Convert.ToDateTime(reader["CREATED_DATE"]);
                    MainDropDownModel.CREATED_LOC = reader["CREATED_LOC"].ToString();
                    MainDropDownModel.UPDATED_BY = reader["UPDATED_BY"].ToString();
                    MainDropDownModel.UPDATED_DATE = Convert.ToDateTime(reader["UPDATED_DATE"]);
                    MainDropDownModel.UPDATED_LOC = reader["UPDATED_LOC"].ToString();
                }
                CloseReader();
                CloseConnection();
            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }

            return MainDropDownModel;
        }
        //-----------------------------------------------------------------
        //                       GET ALL DATA  + SEARCH DATA
        //-----------------------------------------------------------------
        public List<MainDropDownModel> getALLtypelistFROMmaindropdown(string type, string search)
        {
            List<MainDropDownModel> DataMainDropdown = new List<MainDropDownModel>();

            try
            {
                OpenConnection();
                command.CommandText = "PSP_GetMainDropdownLIST";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pMAINDROPTYPE", type)).Direction = ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@search", search)).Direction = ParameterDirection.Input;

                reader = command.ExecuteReader();
                while (reader.Read())
                {

                    MainDropDownModel DataMainDropdown1 = new MainDropDownModel();
                    DataMainDropdown1.ID_MM_MAIN_DROPDOWN = Convert.ToInt32(reader["ID_MM_MAIN_DROPDOWN"]);
                    DataMainDropdown1.MAIN_DROPDOWN_TYPE = reader["MAIN_DROPDOWN_TYPE"].ToString();
                    DataMainDropdown1.MAIN_DROPDOWN_TEXT = reader["MAIN_DROPDOWN_TEXT"].ToString();
                    DataMainDropdown1.DESCRIPTION = reader["DESCRIPTION"].ToString().Trim();
                    DataMainDropdown1.CODE = reader["CODE"].ToString();
                    DataMainDropdown1.REC_TYPE = reader["REC_TYPE"].ToString();
                    DataMainDropdown1.STATUS_IND = reader["STATUS_IND"].ToString();
                    DataMainDropdown1.CREATED_BY = reader["CREATED_BY"].ToString();
                    DataMainDropdown1.CREATED_DATE = Convert.ToDateTime(reader["CREATED_DATE"]);
                    DataMainDropdown1.CREATED_LOC = reader["CREATED_LOC"].ToString();
                    DataMainDropdown1.UPDATED_BY = reader["UPDATED_BY"].ToString();
                    DataMainDropdown1.UPDATED_DATE = Convert.ToDateTime(reader["UPDATED_DATE"]);
                    DataMainDropdown1.UPDATED_LOC = reader["UPDATED_LOC"].ToString();
                    DataMainDropdown.Add(DataMainDropdown1);

                }
                CloseReader();
                CloseConnection();
            }
            catch (Exception ex)
            {

                string x = ex.Message;
            }

            return DataMainDropdown;
        }
        //-----------------------------------------------------------------
        //                       INSERT DATA TO MainDropDownModel
        //-----------------------------------------------------------------
        public string AllTypeMainDropDownMaint(MainDropDownModel comp, String RECORD_TYPE, string type)
        {
            string result = "";
            string return_value = "0";

            OpenConnection();
            try
            {
                ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
                //string userID = userobj.ID_ACL_USER.ToString();
                //string userID = userobj.EMP_NO.ToString();
                string userID = userobj.USER_ID.ToString();
                string createdby = userID;
                string loc = HttpContext.Current.Request.UserHostAddress;

                command.CommandText = "PSP_MainDropdown_Maint";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pID_MAINDROP", comp.ID_MM_MAIN_DROPDOWN)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pMAINDROPTYPE", type)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pMAINDROPTEXT", comp.MAIN_DROPDOWN_TEXT)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pDESCRIPTION", "null")).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@Code", comp.CODE)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pRECORDTYPE", RECORD_TYPE)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pSTATUS_IND", comp.STATUS_IND)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@Createdby", createdby)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@Loc", loc)).Direction = System.Data.ParameterDirection.Input;

                return_value = command.ExecuteScalar().ToString();

                if (Convert.ToInt32(return_value) > 0)
                {
                    if (Convert.ToInt32(RECORD_TYPE) == 1)
                    {
                        result = "Data Save Successfully.";
                    }
                    else if (Convert.ToInt32(RECORD_TYPE) == 3)
                    {
                        result = "Data Amend Successfully.";
                    }
                    else
                    {
                        result = "Data Delete Successfully.";
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return result = ex.Message;
            }
            finally
            {
                CloseConnection();
            }
        }

        public string AssetMainDropDownMaint(MainDropDownModel comp, String RECORD_TYPE, string type)
        {
            string result = "";
            string return_value = "0";

            OpenConnection();
            try
            {
                ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
                //string userID = userobj.ID_ACL_USER.ToString();
                //string userID = userobj.EMP_NO.ToString();
                string userID = userobj.USER_ID.ToString();
                string createdby = userID;
                string loc = HttpContext.Current.Request.UserHostAddress;

                command.CommandText = "PSP_MainDropdown_Maint";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pID_MAINDROP", comp.ID_MM_MAIN_DROPDOWN)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pMAINDROPTYPE", type)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pMAINDROPTEXT", comp.MAIN_DROPDOWN_TEXT)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pDESCRIPTION", comp.DESCRIPTION)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@Code", comp.CODE)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pRECORDTYPE", RECORD_TYPE)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pSTATUS_IND", comp.STATUS_IND)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@Createdby", createdby)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@Loc", loc)).Direction = System.Data.ParameterDirection.Input;

                return_value = command.ExecuteScalar().ToString();

                if (Convert.ToInt32(return_value) > 0)
                {
                    if (Convert.ToInt32(RECORD_TYPE) == 1)
                    {
                        result = "Data Save Successfully.";
                    }
                    else if (Convert.ToInt32(RECORD_TYPE) == 3)
                    {
                        result = "Data Amend Successfully.";
                    }
                    else
                    {
                        result = "Data Delete Successfully.";
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return result = ex.Message;
            }
            finally
            {
                CloseConnection();
            }
        }
        //-------------------------------------------MainDropdown Audit Trail--------------------------------------------
        public MainDropdown_AT getMainDropDownATData(string id)
        {
            MainDropdown_AT MainDropdown_AdT = null;
            try
            {
                OpenConnection();
                command.CommandText = "PSP_MAIN_DROPDOWN_LIST_A";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    MainDropdown_AdT = new MainDropdown_AT();
                    MainDropdown_AdT.SQ_ID = Convert.ToInt32(reader["SQ_ID"]);
                    MainDropdown_AdT.KEY_FIELD = reader["KEY_FIELD"].ToString();
                    MainDropdown_AdT.KEY_VALUE = Convert.ToInt32(reader["KEY_VALUE"]);
                    MainDropdown_AdT.FIELD_NAME = reader["FIELD_NAME"].ToString();
                    MainDropdown_AdT.B4_UPDATE = reader["B4_UPDATE"].ToString();
                    MainDropdown_AdT.AF_UPDATE = reader["AF_UPDATE"].ToString();
                    MainDropdown_AdT.UPDATED_BY = reader["UPDATED_BY"].ToString();
                    MainDropdown_AdT.UPDATED_DATE = Convert.ToDateTime(reader["UPDATED_DATE"]);
                    MainDropdown_AdT.UPDATED_LOC = reader["UPDATED_LOC"].ToString();
                    //MainDropdown_AdT.HEADER_ID = reader["HEADER_ID"].ToString();
                }
                CloseReader();
                CloseConnection();
            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }

            return MainDropdown_AdT;
        }
        #endregion
        //-------------------------------------------------SubDropdown---------------------------------------------------
        #region Subdropdown
        public List<SubDropdownModel> getListData(string type, string search)
        {
            List<SubDropdownModel> testing = new List<SubDropdownModel>();
            try
            {
                OpenConnection();
                command.CommandText = "PSP_GetSubDropdownLIST";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pTYPE", type)).Direction = ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@search", search)).Direction = ParameterDirection.Input;
                //command.Parameters.AddWithValue("@search", search);
                //string res = search;
                reader = command.ExecuteReader();


                while (reader.Read())
                {
                    SubDropdownModel model = new SubDropdownModel();
                    model.ID_MM_SUB_DROPDOWN = Convert.ToInt32(reader["ID_MM_SUB_DROPDOWN"]);
                    model.ID_MM_MAIN_DROPDOWN = Convert.ToInt32(reader["ID_MM_MAIN_DROPDOWN"]);
                    model.MAIN_DROPDOWN_TEXT = reader["MAIN_DROPDOWN_TEXT"].ToString();
                    model.SUB_DROPDOWN_TYPE = reader["SUB_DROPDOWN_TYPE"].ToString();
                    model.SUB_DROPDOWN_TEXT = reader["SUB_DROPDOWN_TEXT"].ToString();
                    model.CODE = reader["CODE"].ToString();
                    model.STATUS_IND = reader["STATUS_IND"].ToString();
                    model.RECORD_TYP = Convert.ToInt32(reader["RECORD_TYP"]);
                    model.CREATED_BY = reader["CREATED_BY"].ToString();
                    model.CREATED_DATE = Convert.ToDateTime(reader["CREATED_DATE"]);
                    model.CREATED_LOC = reader["CREATED_LOC"].ToString();
                    model.UPDATED_BY = reader["UPDATED_BY"].ToString();
                    model.UPDATED_DATE = Convert.ToDateTime(reader["UPDATED_DATE"]);
                    model.UPDATED_LOC = reader["UPDATED_LOC"].ToString();
                    testing.Add(model);
                }
                CloseReader();
                CloseConnection();
            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }

            return testing;
        }

        public SubDropdownModel getSubDropdownData(string id)
        {
            SubDropdownModel SubDropdownModel = null;
            try
            {
                OpenConnection();
                command.CommandText = "PSP_SubDropdown_SEL";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    SubDropdownModel = new SubDropdownModel();
                    SubDropdownModel.ID_MM_SUB_DROPDOWN = Convert.ToInt32(reader["ID_MM_SUB_DROPDOWN"]);
                    SubDropdownModel.ID_MM_MAIN_DROPDOWN = Convert.ToInt32(reader["ID_MM_MAIN_DROPDOWN"]);
                    SubDropdownModel.MAIN_DROPDOWN_TEXT = reader["MAIN_DROPDOWN_TEXT"].ToString();
                    SubDropdownModel.SUB_DROPDOWN_TYPE = reader["SUB_DROPDOWN_TYPE"].ToString();
                    SubDropdownModel.SUB_DROPDOWN_TEXT = reader["SUB_DROPDOWN_TEXT"].ToString();
                    SubDropdownModel.CODE = reader["CODE"].ToString();
                    SubDropdownModel.STATUS_IND = reader["STATUS_IND"].ToString();
                    SubDropdownModel.RECORD_TYP = Convert.ToInt32(reader["RECORD_TYP"]);
                    SubDropdownModel.CREATED_BY = reader["CREATED_BY"].ToString();
                    SubDropdownModel.CREATED_DATE = Convert.ToDateTime(reader["CREATED_DATE"]);
                    SubDropdownModel.CREATED_LOC = reader["CREATED_LOC"].ToString();
                    SubDropdownModel.UPDATED_BY = reader["UPDATED_BY"].ToString();
                    SubDropdownModel.UPDATED_DATE = Convert.ToDateTime(reader["UPDATED_DATE"]);
                    SubDropdownModel.UPDATED_LOC = reader["UPDATED_LOC"].ToString();
                }
                CloseReader();
                CloseConnection();
            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }

            return SubDropdownModel;
        }

        public string SubDropdownMaint(SubDropdownModel comp, String record_typ, String type)
        {
            string result = "";
            string return_value = "0";

            OpenConnection();
            try
            {
                ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
                //string userID = userobj.ID_ACL_USER.ToString();                         //make change in here to chg id into emp number
                //string userID = userobj.EMP_NO.ToString();
                string userID = userobj.USER_ID.ToString();
                string createdby = userID;
                string loc = HttpContext.Current.Request.UserHostAddress;

                command.CommandText = "PSP_SubDropdown_Maint";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;

                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pID", comp.ID_MM_SUB_DROPDOWN)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pID_DD", comp.ID_MM_MAIN_DROPDOWN)).Direction = System.Data.ParameterDirection.Input;
                //command.Parameters.Add(new SqlParameter("@pTYPE", "STORE LOCATION")).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pTYPE", type)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pTEXT", comp.SUB_DROPDOWN_TEXT)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pSTATUS", comp.STATUS_IND)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@Code", comp.CODE)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@Record_typ", record_typ)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@Createdby", createdby)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@Loc", loc)).Direction = System.Data.ParameterDirection.Input;

                return_value = command.ExecuteScalar().ToString();

                if (Convert.ToInt32(return_value) > 0)
                {
                    if (Convert.ToInt32(record_typ) == 1)
                    {
                        result = "Data Save Successfully.";
                    }
                    else if (Convert.ToInt32(record_typ) == 3)
                    {
                        result = "Data Amend Successfully.";
                    }
                    else
                    {
                        result = "Data Delete Successfully.";
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return result = ex.Message;
            }
            finally
            {
                CloseConnection();
            }
        }
        //-------------------------------------------SubDropdown Audit Trail--------------------------------------------
        public SubDropdown_AT getSubDropDownATData(string id)
        {
            SubDropdown_AT SubDropdown_AdT = null;
            try
            {
                OpenConnection();
                command.CommandText = "PSP_SUB_DROPDOWN_LIST_A";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    SubDropdown_AdT = new SubDropdown_AT();
                    SubDropdown_AdT.SQ_ID = Convert.ToInt32(reader["SQ_ID"]);
                    SubDropdown_AdT.KEY_FIELD = reader["KEY_FIELD"].ToString();
                    SubDropdown_AdT.KEY_VALUE = Convert.ToInt32(reader["KEY_VALUE"]);
                    SubDropdown_AdT.FIELD_NAME = reader["FIELD_NAME"].ToString();
                    SubDropdown_AdT.B4_UPDATE = reader["B4_UPDATE"].ToString();
                    SubDropdown_AdT.B4_DROPDOWN_TEXT = reader["B4_DROPDOWN_TEXT"].ToString();
                    SubDropdown_AdT.AF_UPDATE = reader["AF_UPDATE"].ToString();
                    SubDropdown_AdT.AF_DROPDOWN_TEXT = reader["AF_DROPDOWN_TEXT"].ToString();
                    SubDropdown_AdT.UPDATED_BY = reader["UPDATED_BY"].ToString();
                    SubDropdown_AdT.UPDATED_DATE = Convert.ToDateTime(reader["UPDATED_DATE"]);
                    SubDropdown_AdT.UPDATED_LOC = reader["UPDATED_LOC"].ToString();
                }
                CloseReader();
                CloseConnection();
            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }

            return SubDropdown_AdT;
        }
        #endregion
        //----------------------------Revolution Lab-------------------------------------------------------
        #region Revolution Lab
        public List<RevRegModel> getRevData(string search)
        {
            List<RevRegModel> testing = new List<RevRegModel>();
            try
            {
                OpenConnection();
                command.CommandText = "PSP_GetRevLabLIST";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@search", search)).Direction = ParameterDirection.Input;
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    RevRegModel model = new RevRegModel();
                    model.ID_REV_LAB = Convert.ToInt32(reader["ID_REV_LAB"]);
                    model.SCANNED_DATE = Convert.ToDateTime(reader["SCANNED_DATE"]);
                    model.CEL_Number = reader["CEL_NUM"].ToString();
                    model.RFID = reader["RFID"].ToString();
                    model.Site = reader["SITE"].ToString();
                    model.Department = reader["DEPARTMENT"].ToString();
                    model.ID_MM_SUB_DROPDOWN = Convert.ToInt32(reader["ID_MM_SUB_DROPDOWN"]); //location
                    model.SUB_DROPDOWN_TEXT = reader["SUB_DROPDOWN_TEXT"].ToString();         //location
                    model.Owner = reader["OWNER"].ToString();
                    model.Item_Type = reader["ITEM_TYP"].ToString();
                    model.Model = reader["MODEL"].ToString();
                    model.ID_MM_MAIN_DROPDOWN = Convert.ToInt32(reader["ID_MM_MAIN_DROPDOWN"]); //action
                    model.MAIN_DROPDOWN_TEXT = reader["MAIN_DROPDOWN_TEXT"].ToString();         //action
                    model.REMARK = reader["REMARK"].ToString();
                    model.SCAN_STATUS = reader["SCAN_STATUS"].ToString();
                    //model.MOVEMENT_STATUS = reader["MOVEMENT_STATUS"].ToString();     //Movement Status
                    //model.MOVSTT_TXT = reader["MOVSTT_TXT"].ToString();               //Movement Status
                    //model.UPDATEBY = Convert.ToInt32(reader["UPDATEBY"]);             //HelpDesk Name
                    model.UPDATEBY = reader["UPDATEBY"].ToString();
                    model.UPDATEBY_TEXT = reader["UPDATEBY_TEXT"].ToString();         //HelpDesk Name
                    //model.UPDATEBY = Convert.ToInt32(reader["UPDATEBY"]);
                    model.STATUS_IND = reader["STATUS_IND"].ToString();
                    model.RECORD_TYP = Convert.ToInt32(reader["RECORD_TYP"]);
                    model.CREATED_BY = reader["CREATED_BY"].ToString();
                    model.CREATED_DATE = Convert.ToDateTime(reader["CREATED_DATE"]);
                    model.CREATED_LOC = reader["CREATED_LOC"].ToString();
                    model.UPDATED_BY = reader["UPDATED_BY"].ToString();
                    model.UPDATED_DATE = Convert.ToDateTime(reader["UPDATED_DATE"]);
                    model.UPDATED_LOC = reader["UPDATED_LOC"].ToString();
                    testing.Add(model);
                }
                CloseReader();
                CloseConnection();
            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }

            return testing;
        }

        public RevRegModel getRevLabData(string id)
        {
            RevRegModel RevRegModel = null;
            try
            {
                OpenConnection();
                command.CommandText = "PSP_Registration_RevolutionLab";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pRFID", id)).Direction = System.Data.ParameterDirection.Input;
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    RevRegModel = new RevRegModel();
                    RevRegModel.ID_REV_LAB = Convert.ToInt32(reader["ID_REV_LAB"]);
                    RevRegModel.SCANNED_DATE = Convert.ToDateTime(reader["SCANNED_DATE"]);
                    RevRegModel.CEL_Number = reader["CEL_NUM"].ToString();
                    RevRegModel.RFID = reader["RFID"].ToString();
                    RevRegModel.Site = reader["SITE"].ToString();
                    RevRegModel.Department = reader["DEPARTMENT"].ToString();
                    RevRegModel.ID_MM_SUB_DROPDOWN = Convert.ToInt32(reader["ID_MM_SUB_DROPDOWN"]); //location
                    RevRegModel.SUB_DROPDOWN_TEXT = reader["SUB_DROPDOWN_TEXT"].ToString();         //location
                    RevRegModel.Owner = reader["OWNER"].ToString();
                    RevRegModel.Item_Type = reader["ITEM_TYP"].ToString();
                    RevRegModel.Model = reader["MODEL"].ToString();
                    RevRegModel.ID_MM_MAIN_DROPDOWN = Convert.ToInt32(reader["ID_MM_MAIN_DROPDOWN"]); //action
                    RevRegModel.MAIN_DROPDOWN_TEXT = reader["MAIN_DROPDOWN_TEXT"].ToString();         //action
                    RevRegModel.REMARK = reader["REMARK"].ToString();
                    RevRegModel.RFID = reader["RFID"].ToString();
                    //RevRegModel.MOVEMENT_STATUS = reader["MOVEMENT_STATUS"].ToString();     //Movement Status
                    //RevRegModel.MOVSTT_TXT = reader["MOVSTT_TXT"].ToString();               //Movement Status
                    //RevRegModel.UPDATEBY = Convert.ToInt32(reader["UPDATEBY"]);             //HelpDesk Name
                    RevRegModel.UPDATEBY = reader["UPDATEBY"].ToString();
                    RevRegModel.UPDATEBY_TEXT = reader["UPDATEBY_TEXT"].ToString();         //HelpDesk Name
                    //RevRegModel.UPDATEBY = Convert.ToInt32(reader["UPDATEBY"]);
                    RevRegModel.STATUS_IND = reader["STATUS_IND"].ToString();
                    RevRegModel.RECORD_TYP = Convert.ToInt32(reader["RECORD_TYP"]);
                    RevRegModel.CREATED_BY = reader["CREATED_BY"].ToString();
                    RevRegModel.CREATED_DATE = Convert.ToDateTime(reader["CREATED_DATE"]);
                    RevRegModel.CREATED_LOC = reader["CREATED_LOC"].ToString();
                    RevRegModel.UPDATED_BY = reader["UPDATED_BY"].ToString();
                    RevRegModel.UPDATED_DATE = Convert.ToDateTime(reader["UPDATED_DATE"]);
                    RevRegModel.UPDATED_LOC = reader["UPDATED_LOC"].ToString();
                }
                CloseReader();
                CloseConnection();
            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }

            return RevRegModel;
        }

        // zulaikha17122020

        public string RevLabMaint(RevRegModel comp, String record_typ)
        {
            string result = "";
            string return_value = "0";

            OpenConnection();
            try
            {
                ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
                //string userID = userobj.ID_ACL_USER.ToString();
                //string userID = userobj.EMP_NO.ToString();
                string userID = userobj.USER_ID.ToString();
                string createdby = userID;
                string loc = HttpContext.Current.Request.UserHostAddress;

                command.CommandText = "PSP_RevolutionLab_Maint";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pRFID", comp.ID_REV_LAB)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pLOC", comp.ID_MM_SUB_DROPDOWN)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pACTION", comp.ID_MM_MAIN_DROPDOWN)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pREMARK", comp.REMARK)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pUPDATEBY", comp.UPDATEBY)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@Record_typ", record_typ)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@Createdby", createdby)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@Loc", loc)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@MovStt", comp.MOVEMENT_STATUS)).Direction = System.Data.ParameterDirection.Input;

                return_value = command.ExecuteScalar().ToString();

                if (Convert.ToInt32(return_value) > 0)
                {
                    if (Convert.ToInt32(record_typ) == 1)
                    {
                        result = "Data Save Successfully.";
                    }
                    else if (Convert.ToInt32(record_typ) == 3)
                    {
                        result = "Data Amend Successfully.";
                    }
                    else
                    {
                        result = "Data Delete Successfully.";
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return result = ex.Message;
            }
            finally
            {
                CloseConnection();
            }
        }
        #endregion
        //---------------------------Record Movement----------------------------------------------
        #region Record Movement
        public List<RecordMovement> getRecMovData(string search)
        {
            List<RecordMovement> RecMocData = new List<RecordMovement>();

            try
            {
                OpenConnection();
                command.CommandText = "PSP_RecordMovement_List";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@search", search)).Direction = ParameterDirection.Input;
                reader = command.ExecuteReader();
                while (reader.Read())
                {

                    RecordMovement RecMov = new RecordMovement();
                    RecMov.ID = Convert.ToInt32(reader["ID"]);
                    RecMov.CEL_LABEL = reader["CEL_LABEL"].ToString();
                    RecMov.RFID_NUM = reader["RFID_NUM"].ToString();
                    RecMov.OWNER = reader["OWNER"].ToString();
                    RecMov.MOV_STT = reader["MOV_STT"].ToString();
                    RecMov.MOV_STT = reader["MOV_TXT"].ToString();
                    RecMov.LOCATION = reader["LOCATION"].ToString();
                    RecMov.UPDATED_DATE_ASSET = reader["UPDATE_DATE_ASSET"].ToString(); // == DBNull.Value ? (DateTime?)null : (DateTime)reader["UPDATED_DATE_ASSET"];
                    RecMov.PIC = reader["PIC"].ToString();
                    RecMov.PIC_NAME = reader["PIC_NAME"].ToString();
                    RecMov.ROOT_CAUSE = reader["ROOT_CAUSE"].ToString();
                    RecMov.REC_TYPE = reader["REC_TYPE"].ToString();
                    RecMov.STATUS_IND = reader["STATUS_IND"].ToString();
                    RecMov.CREATED_BY = reader["CREATED_BY"].ToString();
                    RecMov.CREATED_DATE = reader["CREATED_DATE"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["CREATED_DATE"];
                    RecMov.CREATED_LOC = reader["CREATED_LOC"].ToString();
                    RecMov.UPDATED_BY = reader["UPDATED_BY"].ToString();
                    RecMov.UPDATED_DATE = reader["UPDATED_DATE"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["UPDATED_DATE"];
                    RecMov.UPDATED_LOC = reader["UPDATED_LOC"].ToString();
                    RecMocData.Add(RecMov);
                }
                CloseReader();
                CloseConnection();
            }
            catch (Exception ex)
            {

                string x = ex.Message;

            }
            return RecMocData;
        }
        public RecordMovement getRecMov(string id, string cel)
        {
            RecordMovement RecordMovement = null;
            try
            {
                if (id != null && cel == null)
                {
                    OpenConnection();
                    command.CommandText = "PSP_RECORD_MOVEMENT_SEL";
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 0;
                    command.Parameters.Clear();
                    command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
                    command.Parameters.Add(new SqlParameter("@pType", "id")).Direction = System.Data.ParameterDirection.Input;
                    reader = command.ExecuteReader();
                }
                else if (cel != null && id == null)
                {
                    OpenConnection();
                    command.CommandText = "PSP_RECORD_MOVEMENT_SEL";
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 0;
                    command.Parameters.Clear();
                    command.Parameters.Add(new SqlParameter("@pCel", cel)).Direction = System.Data.ParameterDirection.Input;
                    command.Parameters.Add(new SqlParameter("@pType", "cel")).Direction = System.Data.ParameterDirection.Input;
                    reader = command.ExecuteReader();
                }

                while (reader.Read())
                {
                    RecordMovement = new RecordMovement();
                    RecordMovement.ID = Convert.ToInt32(reader["ID"]);
                    RecordMovement.CEL_LABEL = reader["CEL_LABEL"].ToString();
                    RecordMovement.RFID_NUM = reader["RFID_NUM"].ToString();
                    RecordMovement.OWNER = reader["OWNER"].ToString();
                    RecordMovement.MOV_STT = reader["MOV_STT"].ToString();
                    RecordMovement.LOCATION = reader["LOCATION"].ToString(); RecordMovement.UPDATED_DATE_ASSET = reader["UPDATE_DATE_ASSET"].ToString(); //== DBNull.Value ? (DateTime?)null : (DateTime)reader["UPDATED_DATE_ASSET"];
                    RecordMovement.PIC = reader["PIC"].ToString();
                    RecordMovement.ROOT_CAUSE = reader["ROOT_CAUSE"].ToString();
                    RecordMovement.STATUS_IND = reader["STATUS_IND"].ToString();
                    RecordMovement.REC_TYPE = reader["REC_TYPE"].ToString();
                    RecordMovement.CREATED_BY = reader["CREATED_BY"].ToString();
                    RecordMovement.CREATED_DATE = Convert.ToDateTime(reader["CREATED_DATE"]);
                    RecordMovement.CREATED_LOC = reader["CREATED_LOC"].ToString();
                    RecordMovement.UPDATED_BY = reader["UPDATED_BY"].ToString();
                    RecordMovement.UPDATED_DATE = Convert.ToDateTime(reader["UPDATED_DATE"]);
                    RecordMovement.UPDATED_LOC = reader["UPDATED_LOC"].ToString();
                }
                CloseReader();
                CloseConnection();
            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }
            return RecordMovement;
        }

        public string RecMovMaint(RecordMovement comp, String RECORD_TYPE)
        {
            string result = "";
            string return_value = "0";
            OpenConnection();
            try
            {
                ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
                //string userID = userobj.ID_ACL_USER.ToString();
                //string userID = userobj.EMP_NO.ToString();
                string userID = userobj.USER_ID.ToString();
                string createdby = userID;
                string loc = HttpContext.Current.Request.UserHostAddress;

                command.CommandText = "PSP_RECORD_MOVEMENT_MAINT";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pID", comp.ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCEL", comp.CEL_LABEL)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pRFID", comp.RFID_NUM)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pOWNER", comp.OWNER)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pMOV_STT", comp.MOV_STT)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pLOCATION", comp.LOCATION)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pUPDATE_DATE_A", comp.UPDATED_DATE_ASSET)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pPIC", comp.PIC)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pROOT_CAUSE", comp.ROOT_CAUSE)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pSTATUS_IND", comp.STATUS_IND)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pREC_TYPE", RECORD_TYPE)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCREATED_BY", createdby)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@ploc", loc)).Direction = System.Data.ParameterDirection.Input;

                return_value = command.ExecuteScalar().ToString();

                if (Convert.ToInt32(return_value) > 0)
                {
                    if (Convert.ToInt32(RECORD_TYPE) == 1)
                    {
                        result = "Data Save Successfully.";
                    }
                    else if (Convert.ToInt32(RECORD_TYPE) == 3)
                    {
                        result = "Data Amend Successfully.";
                    }
                    //else
                    //{
                    //    result = "Data Delete Successfully.";
                    //}
                }
                return result;
            }
            catch (Exception ex)
            {
                return result = ex.Message;
            }
            finally
            {
                CloseConnection();
            }
        }
        public string RecMovMaintRmv(RecordMovement comp, String RECORD_TYPE)
        {
            string result = "";
            string return_value = "0";
            OpenConnection();
            try
            {
                ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
                string userID = userobj.ID_ACL_USER.ToString();
                string createdby = userID;
                string loc = HttpContext.Current.Request.UserHostAddress;

                command.CommandText = "PSP_RECORD_MOVEMENT_MAINT";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pID", comp.ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pREC_TYPE", RECORD_TYPE)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCREATED_BY", createdby)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@ploc", loc)).Direction = System.Data.ParameterDirection.Input;

                return_value = command.ExecuteScalar().ToString();

                if (Convert.ToInt32(return_value) > 0)
                {
                    result = "Data Delete Successfully.";
                }
                return result;
            }
            catch (Exception ex)
            {
                return result = ex.Message;
            }
            finally
            {
                CloseConnection();
            }
        }
        public RecordMovement getAssetData(string cel, string rfid)
        {
            RecordMovement RecordMovement = null;
            try
            {
                OpenConnection();
                command.CommandText = "PSP_GetData_SEL";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pCEL_Number", cel)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pRFID", rfid)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pTyp", "GetData")).Direction = System.Data.ParameterDirection.Input;
                //command.Parameters.Add(new SqlParameter("@pID", "")).Direction = System.Data.ParameterDirection.Input;
                reader = command.ExecuteReader();
                //DataTable dt = new DataTable();
                //dt.Load(reader);
                while (reader.Read())
                {
                    if (cel != "")
                    {
                        RecordMovement = new RecordMovement();
                        RecordMovement.OWNER = reader["Owner"].ToString();
                        RecordMovement.LOCATION = reader["LOC"].ToString();
                        RecordMovement.RFID_NUM = reader["RFID"].ToString();
                        RecordMovement.UPDATED_DATE_ASSET = reader["Update_Date"].ToString();//Convert.ToDateTime(reader["Update_Date"]);//reader["Update_Date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Update_Date"];
                    }
                    else if (rfid != "")
                    {
                        RecordMovement = new RecordMovement();
                        RecordMovement.OWNER = reader["Owner"].ToString();
                        RecordMovement.LOCATION = reader["LOC"].ToString();
                        RecordMovement.CEL_LABEL = reader["CEL_Number"].ToString();
                        RecordMovement.UPDATED_DATE_ASSET = reader["Update_Date"].ToString();//Convert.ToDateTime(reader["Update_Date"]);//reader["Update_Date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Update_Date"];
                    }
                }
                CloseReader();
                CloseConnection();
                //return dt;
            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }
            return RecordMovement;
        }


        public RecMov_AT getRecMovATData(string id)
        {
            RecMov_AT RecMov_AT = null;
            try
            {
                OpenConnection();
                command.CommandText = "PSP_RecMovDtl_LIST_A";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    RecMov_AT = new RecMov_AT();
                    RecMov_AT.SQ_ID = Convert.ToInt32(reader["SQ_ID"]);
                    RecMov_AT.KEY_FIELD = reader["KEY_FIELD"].ToString();
                    RecMov_AT.KEY_VALUE = Convert.ToInt32(reader["KEY_VALUE"]);
                    RecMov_AT.FIELD_NAME = reader["FIELD_NAME"].ToString();
                    RecMov_AT.B4_UPDATE = reader["B4_UPDATE"].ToString();
                    RecMov_AT.AF_UPDATE = reader["AF_UPDATE"].ToString();
                    RecMov_AT.UPDATED_BY = reader["UPDATED_BY"].ToString();
                    RecMov_AT.UPDATED_DATE = Convert.ToDateTime(reader["UPDATED_DATE"]);
                    RecMov_AT.UPDATED_LOC = reader["UPDATED_LOC"].ToString();
                    //RecMov_AT.HEADER_ID = reader["HEADER_ID"].ToString();
                }
                CloseReader();
                CloseConnection();
            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }

            return RecMov_AT;
        }
        #endregion
        //-------------------------Registration----------------------------
        #region Registration
        public RegistrationVIEWmodel getAutoCelData(string site, string item)
        {
            RegistrationVIEWmodel RegistrationVIEWmodel = null;
            try
            {
                OpenConnection();
                command.CommandText = "PSP_GetCel";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pSite", site)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pItem", item)).Direction = System.Data.ParameterDirection.Input;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    RegistrationVIEWmodel = new RegistrationVIEWmodel();
                    RegistrationVIEWmodel.CelNum = reader["CelNum"].ToString();
                    //RegistrationVIEWmodel.CelNum = reader["SiteCode"].ToString();
                    //RegistrationVIEWmodel.CelNum = reader["ItemCode"].ToString();
                }
                CloseReader();
                CloseConnection();
                //return dt;
            }
            catch (Exception ex)
            {
                RegistrationVIEWmodel = new RegistrationVIEWmodel();
                string x = ex.Message;
                RegistrationVIEWmodel.Message = x;
            }
            return RegistrationVIEWmodel;
        }

        public string getImport(RegistrationModel comp, String RECORD_TYPE)
        {
            string result = "";
            string return_value = "0";
            OpenConnection();
            try
            {
                ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
                string userID = userobj.ID_ACL_USER.ToString();
                string createdby = userID;
                string loc = HttpContext.Current.Request.UserHostAddress;
                //DateTime? newdate = null;

                command.CommandText = "PSP_ImportExcelToReg";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pID", comp.ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pSite", comp.Site)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pItem_Type", comp.Item_Type)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCEL_Number", comp.CEL_Number)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pStatus", comp.Status)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pOwner", comp.Owner)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pFixed_Asset", comp.Fixed_Asset)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pLocation", comp.Location)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pRFID", comp.RFID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pDepartment", comp.Department)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pModel", comp.Model)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pPurchase_Date", comp.Purchase_Date)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pManufacturer", comp.Manufacturer)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pWaranty_expiry", comp.Waranty_Expiry)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pOS_version", comp.OS_version)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pEOL_Support", comp.EOL_Support)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pDescription", comp.Description)).Direction = System.Data.ParameterDirection.Input;
                //-------------------------------ip, mac, host name, sys_app-------------------------------------------------------
                command.Parameters.Add(new SqlParameter("@pIP", comp.IP_Address)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pMAC", comp.MAC_Address)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pHostName", comp.Host_Name)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pSys_App", comp.Sys_App)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pInstal_Date", comp.Installation_Date)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pAsset_Type", comp.Asset_Type)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pSerial_Number", comp.Serial_Number)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pRec_type", RECORD_TYPE)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCreated_By", createdby)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@ploc", loc)).Direction = System.Data.ParameterDirection.Input;
                command.ExecuteScalar();
                result = "Records Imported Successfully";

                return result;
            }
            catch (Exception ex)
            {
                return result = ex.Message;
                //string x = ex.Message;
            }
            finally
            {
                CloseConnection();
            }
        }
        //----------------------get value show in edit page------------------
        public RegistrationModel getRegIT(string id)
        {
            RegistrationModel RegistrationModel = null;
            try
            {
                OpenConnection();
                command.CommandText = "PSP_Registration_Asset_Management_SEL";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    RegistrationModel = new RegistrationModel();
                    RegistrationModel.ID = Convert.ToInt32(reader["ID"]);
                    RegistrationModel.Site = reader["Site"].ToString();
                    RegistrationModel.SITE_TXT = reader["SITE_TXT"].ToString();
                    RegistrationModel.Item_Type = reader["Item_Type"].ToString();
                    RegistrationModel.ITEM_TXT = reader["ITEM_TXT"].ToString();
                    RegistrationModel.CEL_Number = reader["CEL_Number"].ToString();
                    RegistrationModel.Status = reader["Status"].ToString();
                    RegistrationModel.STATUS_TXT = reader["STATUS_TXT"].ToString();
                    //RegistrationModel.STATUS_IND = reader["STATUS_IND"].ToString();
                    RegistrationModel.Owner = reader["Owner"].ToString();
                    RegistrationModel.Fixed_Asset = reader["Fixed_Asset"].ToString();
                    RegistrationModel.Location = reader["Location"].ToString();
                    RegistrationModel.LOC = reader["LOC"].ToString();
                    RegistrationModel.RFID = reader["RFID"].ToString();
                    RegistrationModel.Department = reader["Department"].ToString();
                    RegistrationModel.DEPT = reader["DEPT"].ToString();
                    RegistrationModel.Model = reader["Model"].ToString();
                    RegistrationModel.Purchase_Date = reader["Purchase_Date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Purchase_Date"];
                    RegistrationModel.Manufacturer = reader["Manufacturer"].ToString();
                    RegistrationModel.EOL_Support = reader["EOL_Support"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["EOL_Support"];
                    RegistrationModel.Waranty_Expiry = reader["Waranty_Expiry"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Waranty_Expiry"];
                    RegistrationModel.OS_version = reader["OS_version"].ToString();
                    RegistrationModel.Serial_Number = reader["Serial_Number"].ToString();
                    RegistrationModel.Description = reader["Description"].ToString();
                    //--------------------------ip, mac and host name-------------------------------
                    RegistrationModel.IP_Address = reader["IP_Address"].ToString();
                    RegistrationModel.MAC_Address = reader["MAC_Address"].ToString();
                    RegistrationModel.Host_Name = reader["Host_Name"].ToString();
                    RegistrationModel.Sys_App = reader["System_Application"].ToString();
                    RegistrationModel.Installation_Date = reader["Installation_Date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Installation_Date"];
                    RegistrationModel.Asset_Type = reader["Asset_Type"].ToString();
                    RegistrationModel.RECORD_TYP = reader["Record_Type"].ToString();
                    RegistrationModel.CREATED_BY = reader["Created_By"].ToString();
                    RegistrationModel.CREATED_DATE = Convert.ToDateTime(reader["Created_Date"]);
                    RegistrationModel.CREATED_LOC = reader["Created_Loc"].ToString();
                    RegistrationModel.UPDATED_BY = reader["Update_By"].ToString();
                    RegistrationModel.UPDATED_DATE = Convert.ToDateTime(reader["Update_Date"]);
                    RegistrationModel.UPDATED_LOC = reader["Update_Loc"].ToString();
                }
                CloseReader();
                CloseConnection();
            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }
            return RegistrationModel;
        }
        //---------------------------get data shwo in table list & search------------------------------------
        public List<RegistrationModel> getRegITdata(string search)
        {
            List<RegistrationModel> RegITdata = new List<RegistrationModel>();

            try
            {
                OpenConnection();
                command.CommandText = "PSP_GetRegistrationIT_Asset_Management_List";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@search", search)).Direction = ParameterDirection.Input;
                reader = command.ExecuteReader();
                while (reader.Read())
                {

                    RegistrationModel DataRegIT = new RegistrationModel();
                    DataRegIT.ID = Convert.ToInt32(reader["ID"]);
                    DataRegIT.Site = reader["Site"].ToString();
                    DataRegIT.SITE_TXT = reader["SITE_TXT"].ToString();
                    DataRegIT.Item_Type = reader["Item_Type"].ToString();
                    DataRegIT.ITEM_TXT = reader["ITEM_TXT"].ToString();
                    DataRegIT.CEL_Number = reader["CEL_Number"].ToString();
                    DataRegIT.Status = reader["Status"].ToString();
                    DataRegIT.STATUS_TXT = reader["STATUS_TXT"].ToString();
                    //DataRegIT.STATUS_IND = reader["STATUS_IND"].ToString();
                    DataRegIT.Owner = reader["Owner"].ToString();
                    DataRegIT.Fixed_Asset = reader["Fixed_Asset"].ToString();
                    DataRegIT.Location = reader["Location"].ToString();
                    DataRegIT.LOC = reader["LOC"].ToString();
                    DataRegIT.RFID = reader["RFID"].ToString();
                    DataRegIT.Department = reader["Department"].ToString();
                    DataRegIT.DEPT = reader["DEPT"].ToString();
                    DataRegIT.Model = reader["Model"].ToString();
                    DataRegIT.Purchase_Date = reader["Purchase_Date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Purchase_Date"];
                    DataRegIT.Manufacturer = reader["Manufacturer"].ToString();
                    DataRegIT.Purchase_Date = reader["EOL_Support"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["EOL_Support"];
                    DataRegIT.Purchase_Date = reader["Waranty_Expiry"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Waranty_Expiry"];
                    DataRegIT.OS_version = reader["OS_version"].ToString();
                    DataRegIT.Serial_Number = reader["Serial_Number"].ToString();
                    DataRegIT.Description = reader["Description"].ToString();
                    DataRegIT.Asset_Type = reader["Asset_Type"].ToString();
                    //----------------------ip, mac and host name------------------------
                    DataRegIT.IP_Address = reader["IP_Address"].ToString();
                    DataRegIT.MAC_Address = reader["MAC_Address"].ToString();
                    DataRegIT.Host_Name = reader["Host_Name"].ToString();
                    DataRegIT.Sys_App = reader["System_Application"].ToString();
                    DataRegIT.Installation_Date = reader["Installation_Date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Installation_Date"];
                    DataRegIT.RECORD_TYP = reader["Record_Type"].ToString();
                    DataRegIT.CREATED_BY = reader["Created_By"].ToString();
                    DataRegIT.CREATED_DATE = reader["Created_Date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Created_Date"]; //10052021
                    DataRegIT.CREATED_LOC = reader["Created_Loc"].ToString();
                    DataRegIT.UPDATED_BY = reader["Update_By"].ToString();
                    DataRegIT.UPDATED_DATE = reader["Update_Date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Update_Date"]; //10052021
                    DataRegIT.UPDATED_LOC = reader["Update_Loc"].ToString();
                    RegITdata.Add(DataRegIT);

                }
                CloseReader();
                CloseConnection();
            }
            catch (Exception ex)
            {

                string x = ex.Message;

            }
            return RegITdata;
        }
        //--------------------------------add data into table-------------------------------------
        public string RegITMaint(RegistrationModel comp, String RECORD_TYPE)
        {
            string result = "";
            string return_value = "0";
            OpenConnection();
            try
            {
                ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
                //string userID = userobj.ID_ACL_USER.ToString();
                //string userID = userobj.EMP_NO.ToString();
                string userID = userobj.USER_ID.ToString();
                string createdby = userID;
                string loc = HttpContext.Current.Request.UserHostAddress;

                command.CommandText = "PSP_Registration_Asset_Management_Maint";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pID", comp.ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pSite", comp.Site)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pItem_Type", comp.Item_Type)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCEL_Number", comp.CEL_Number)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pStatus", comp.Status)).Direction = System.Data.ParameterDirection.Input;
                //command.Parameters.Add(new SqlParameter("@pStatus", comp.STATUS_IND)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pOwner", comp.Owner)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pFixed_Asset", comp.Fixed_Asset)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pLocation", comp.Location)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pRFID", comp.RFID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pDepartment", comp.Department)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pModel", comp.Model)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pPurchase_Date", comp.Purchase_Date)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pManufacturer", comp.Manufacturer)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pWaranty_expiry", comp.Waranty_Expiry)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pOS_version", comp.OS_version)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pEOL_Support", comp.EOL_Support)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pSerial_Number", comp.Serial_Number)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pDescription", comp.Description)).Direction = System.Data.ParameterDirection.Input;
                //-------------------------------ip, mac host name-------------------------------------------------------
                command.Parameters.Add(new SqlParameter("@pIP", comp.IP_Address)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pMAC", comp.MAC_Address)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pHostName", comp.Host_Name)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pSys_App", comp.Sys_App)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pInstal_Date", comp.Installation_Date)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pAsset_Type", "IT Asset")).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pRec_type", RECORD_TYPE)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCreated_By", createdby)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@ploc", loc)).Direction = System.Data.ParameterDirection.Input;

                return_value = command.ExecuteScalar().ToString();

                if (Convert.ToInt32(return_value) > 0)
                {
                    if (Convert.ToInt32(RECORD_TYPE) == 1)
                    {
                        result = "Data Save Successfully.";
                    }
                    else if (Convert.ToInt32(RECORD_TYPE) == 3)
                    {
                        result = "Data Amend Successfully.";
                    }
                    //else
                    //{
                    //    result = "Data Delete Successfully.";
                    //}
                }
                return result;
            }
            catch (Exception ex)
            {
                return result = ex.Message;
            }
            finally
            {
                CloseConnection();
            }
        }
        //add in 10052021 RegITMaintRmv
        public string RegITMaintRmv(RegistrationModel comp, String RECORD_TYPE)
        {
            string result = "";
            string return_value = "0";
            OpenConnection();
            try
            {
                ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
                string userID = userobj.ID_ACL_USER.ToString();
                string createdby = userID;
                string loc = HttpContext.Current.Request.UserHostAddress;

                command.CommandText = "PSP_Registration_Asset_Management_Maint";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pID", comp.ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pAsset_Type", "IT Asset")).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pRec_type", RECORD_TYPE)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCreated_By", createdby)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@ploc", loc)).Direction = System.Data.ParameterDirection.Input;

                return_value = command.ExecuteScalar().ToString();

                if (Convert.ToInt32(return_value) > 0)
                {
                    result = "Data Delete Successfully.";
                }
                return result;
            }
            catch (Exception ex)
            {
                return result = ex.Message;
            }
            finally
            {
                CloseConnection();
            }
        }
        //---------------------------Reg NonIT---------------------------------------
        //---------------------get data show in table list & search------------------
        public List<RegistrationModel> getRegNonITdata(string search)
        {
            List<RegistrationModel> RegNonITdata = new List<RegistrationModel>();

            try
            {
                OpenConnection();
                command.CommandText = "PSP_GetRegistrationNonIT_Asset_Management_List";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@search", search)).Direction = ParameterDirection.Input;
                reader = command.ExecuteReader();
                while (reader.Read())
                {

                    RegistrationModel DataRegNonIT = new RegistrationModel();
                    DataRegNonIT.ID = Convert.ToInt32(reader["ID"]);
                    DataRegNonIT.Site = reader["Site"].ToString();
                    DataRegNonIT.SITE_TXT = reader["SITE_TXT"].ToString();
                    DataRegNonIT.Item_Type = reader["Item_Type"].ToString();
                    DataRegNonIT.ITEM_TXT = reader["ITEM_TXT"].ToString();
                    DataRegNonIT.CEL_Number = reader["CEL_Number"].ToString();
                    DataRegNonIT.Status = reader["Status"].ToString();
                    DataRegNonIT.STATUS_TXT = reader["STATUS_TXT"].ToString();
                    //DataRegNonIT.STATUS_IND = reader["STATUS_IND"].ToString();
                    DataRegNonIT.Owner = reader["Owner"].ToString();
                    DataRegNonIT.Fixed_Asset = reader["Fixed_Asset"].ToString();
                    DataRegNonIT.Location = reader["Location"].ToString();
                    DataRegNonIT.LOC = reader["LOC"].ToString();
                    DataRegNonIT.RFID = reader["RFID"].ToString();
                    DataRegNonIT.Department = reader["Department"].ToString();
                    DataRegNonIT.DEPT = reader["DEPT"].ToString();
                    DataRegNonIT.Model = reader["Model"].ToString();
                    DataRegNonIT.Purchase_Date = reader["Purchase_Date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Purchase_Date"];
                    DataRegNonIT.Manufacturer = reader["Manufacturer"].ToString();
                    DataRegNonIT.EOL_Support = reader["EOL_Support"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["EOL_Support"];
                    DataRegNonIT.Serial_Number = reader["Serial_Number"].ToString();
                    DataRegNonIT.Description = reader["Description"].ToString();
                    DataRegNonIT.Asset_Type = reader["Asset_Type"].ToString();
                    DataRegNonIT.Waranty_Expiry = reader["Waranty_Expiry"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Waranty_Expiry"];
                    DataRegNonIT.OS_version = reader["OS_version"].ToString();
                    //----------------ip, mac and host name--------------------
                    DataRegNonIT.IP_Address = reader["IP_Address"].ToString();
                    DataRegNonIT.MAC_Address = reader["MAC_Address"].ToString();
                    DataRegNonIT.Host_Name = reader["Host_Name"].ToString();
                    DataRegNonIT.Sys_App = reader["System_Application"].ToString();
                    DataRegNonIT.Installation_Date = reader["Installation_Date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Installation_Date"];
                    DataRegNonIT.RECORD_TYP = reader["Record_Type"].ToString();
                    DataRegNonIT.CREATED_BY = reader["Created_By"].ToString();
                    DataRegNonIT.CREATED_DATE = reader["Created_Date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Created_Date"]; //10052021
                    DataRegNonIT.CREATED_LOC = reader["Created_Loc"].ToString();
                    DataRegNonIT.UPDATED_BY = reader["Update_By"].ToString();
                    DataRegNonIT.UPDATED_DATE = reader["Update_Date"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Update_Date"]; //10052021
                    DataRegNonIT.UPDATED_LOC = reader["Update_Loc"].ToString();
                    RegNonITdata.Add(DataRegNonIT);

                }
                CloseReader();
                CloseConnection();
            }
            catch (Exception ex)
            {

                string x = ex.Message;

            }
            return RegNonITdata;
        }
        //-----------------------add data into table-------------------------
        public string RegNonITMaint(RegistrationModel comp, String RECORD_TYPE)
        {
            string result = "";
            string return_value = "0";
            OpenConnection();
            try
            {
                ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
                //string userID = userobj.ID_ACL_USER.ToString();
                //string userID = userobj.EMP_NO.ToString();
                string userID = userobj.USER_ID.ToString();
                string createdby = userID;
                string loc = HttpContext.Current.Request.UserHostAddress;

                //command.CommandText = "PSP_Registration_Asset_Management_Maint";
                command.CommandText = "PSP_Registration_NonIT_Maint";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pID", comp.ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pSite", comp.Site)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pItem_Type", comp.Item_Type)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCEL_Number", "NULL")).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pStatus", comp.Status)).Direction = System.Data.ParameterDirection.Input;
                //command.Parameters.Add(new SqlParameter("@pStatus", comp.STATUS_IND)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pOwner", comp.Owner)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pFixed_Asset", comp.Fixed_Asset)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pLocation", comp.Location)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pRFID", comp.RFID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pDepartment", comp.Department)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pModel", comp.Model)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pPurchase_Date", comp.Purchase_Date)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pManufacturer", comp.Manufacturer)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pWaranty_expiry", comp.Waranty_Expiry)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pOS_version", "NULL")).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pEOL_Support", comp.EOL_Support)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pSerial_Number", comp.Serial_Number)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pDescription", comp.Description)).Direction = System.Data.ParameterDirection.Input;
                //-------------------------------ip, mac host name-------------------------------------------------------
                command.Parameters.Add(new SqlParameter("@pIP", comp.IP_Address)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pMAC", comp.MAC_Address)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pHostName", comp.Host_Name)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pSys_App", comp.Sys_App)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pInstal_Date", comp.Installation_Date)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pAsset_Type", "Non-IT Asset")).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pRec_type", RECORD_TYPE)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCreated_By", createdby)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@ploc", loc)).Direction = System.Data.ParameterDirection.Input;

                return_value = command.ExecuteScalar().ToString();

                if (Convert.ToInt32(return_value) > 0)
                {
                    if (Convert.ToInt32(RECORD_TYPE) == 1)
                    {
                        result = "Data Save Successfully.";
                    }
                    else if (Convert.ToInt32(RECORD_TYPE) == 3)
                    {
                        result = "Data Amend Successfully.";
                    }
                    //else
                    //{
                    //    result = "Data Delete Successfully.";
                    //}
                }
                return result;
            }
            catch (Exception ex)
            {
                return result = ex.Message;
            }
            finally
            {
                CloseConnection();
            }
        }
        //for remove only 10052021
        public string RegNonITMaintRmv(RegistrationModel comp, String RECORD_TYPE)
        {
            string result = "";
            string return_value = "0";
            OpenConnection();
            try
            {
                ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
                string userID = userobj.ID_ACL_USER.ToString();
                string createdby = userID;
                string loc = HttpContext.Current.Request.UserHostAddress;

                command.CommandText = "PSP_Registration_NonIT_Maint";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@pID", comp.ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pAsset_Type", "Non-IT Asset")).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pRec_type", RECORD_TYPE)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCreated_By", createdby)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@ploc", loc)).Direction = System.Data.ParameterDirection.Input;

                return_value = command.ExecuteScalar().ToString();

                if (Convert.ToInt32(return_value) > 0)
                {
                    result = "Data Delete Successfully.";
                }
                return result;
            }
            catch (Exception ex)
            {
                return result = ex.Message;
            }
            finally
            {
                CloseConnection();
            }
        }
        #endregion
        //-------------------------Summary report Cascading DD-------------------------------
        #region summary report cascading dd
        public List<SelectListItem> getStockTakeLocDropDown(string pID)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            string constr = ConfigurationManager.ConnectionStrings["SQLCon"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT DISTINCT col7 FROM [dbo].[rfid_audit_data] WHERE col6 = '" + pID + "' AND col7 <> '' AND col7 IS NOT NULL;";

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
                                Text = sdr["col7"].ToString(),
                                Value = sdr["col7"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }
        public List<SelectListItem> getAssetSummaryItemDropDown(string pID)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            string constr = ConfigurationManager.ConnectionStrings["SQLCon"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT DISTINCT ITEM_TXT FROM [dbo].[PVIEW_REGISTER_LST] WHERE Asset_Type = '" + pID + "' AND Record_Type <> '5' AND ITEM_TXT <> '' AND ITEM_TXT IS NOT NULL;";

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
                                Text = sdr["ITEM_TXT"].ToString(),
                                Value = sdr["ITEM_TXT"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        public List<SelectListItem> getYearDropDown(string pID)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            string constr = ConfigurationManager.ConnectionStrings["SQLCon"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT DISTINCT AgeByYear FROM [dbo].[PVIEW_AGE_OF_IT] WHERE ITEM_TXT = '" + pID + "' AND AgeByYear <> '';";

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
                                Text = sdr["AgeByYear"].ToString(),
                                Value = sdr["AgeByYear"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }
        public List<SelectListItem> getRecMovSttDropDown(string pID)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            string constr = ConfigurationManager.ConnectionStrings["SQLCon"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT DISTINCT MOV_STT FROM [dbo].[PVIEW_RECORD_MOVEMENT_LST] WHERE LOCATION = '" + pID + "';";

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
                                Text = sdr["MOV_STT"].ToString(),
                                Value = sdr["MOV_STT"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }
        #endregion

        #endregion

        #region location
        [HttpPost]
        public List<SelectListItem> getDepartmentDropDown(string pID)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            string constr = ConfigurationManager.ConnectionStrings["SQLCon"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query = "SELECT distinct col5 FROM [dbo].[rfid_data] WHERE RECORD_TYP<>'5' AND UPPER(STATUS_IND)='ACTIVE' and col6 = '" + pID + "' ;";
                //string query = "SELECT distinct Col5 FROM [dbo].[PVIEW_RFID_DATA] WHERE Record_Type <> '5' AND UPPER(Status_ind)='ACTIVE' and col6 = '" + pID + "' ;";
                string query = "SELECT distinct Col5 FROM [dbo].[PVIEW_RFID_DATA] WHERE Record_Type <> '5' AND UPPER(Status_ind)='ACTIVE' and col6 = '" + pID + "' and col5 = 'ENGINEERING';";




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
                                Text = sdr["Col5"].ToString(),
                                Value = sdr["Col5"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }
        #endregion

        #region RFID
        public List<SelectListItem> getLocationtDropDown(string pID, string pID2)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            string constr = ConfigurationManager.ConnectionStrings["SQLCon"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query = "SELECT distinct col7 FROM [dbo].[rfid_data] WHERE RECORD_TYP<>'5' AND UPPER(STATUS_IND)='ACTIVE' and col6 = '" + pID + "' and col5 = '" + pID2 + "';";
                string query = "SELECT distinct Col7 FROM [dbo].[PVIEW_RFID_DATA] WHERE Record_Type<>'5' AND UPPER(Status_ind)='ACTIVE' and col6 = '" + pID + "' and Col5 = '" + pID2 + "';";

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
                                Text = sdr["Col7"].ToString(),
                                Value = sdr["Col7"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        [HttpPost]
        public string CheckLocationStatus(string pID)
        {
            string return_value = "";
            string today = DateTime.Now.ToString("dd/MM/yyyy");
            string constr = ConfigurationManager.ConnectionStrings["SQLCon"].ConnectionString;
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT status FROM [dbo].[rfid_audit_status] WHERE id_location = '" + pID + "' and format(audit_date,'dd/MM/yyyy') = '" + today + "'";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    dt.Load(sdr);

                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0]["status"].ToString() == "IN PROGRESS")
                        { return_value = "2"; }//4 future do for if close accidently and login same id can start over
                        else if (dt.Rows[0]["status"].ToString() == "DONE")
                        { return_value = "3"; }
                    }
                    else
                    {
                        return_value = "1";
                    }

                    con.Close();
                }
            }

            return return_value;
        }

        [HttpPost]
        public string UPDATERFID(string pTYPE, string pID, string pRFID, string pUSER)
        {
            ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
            string userID = userobj.USER_ID.ToString();
            //please open back after do testing
            string createdby = userID;
            //string createdby = "ADMIN";

            string result = "";
            int tempcount = 0;
            OpenConnection();
            command.CommandText = "PSP_RFID_SCAN";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pType", pTYPE)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@pID", pID)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@pRFID", pRFID)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@pUser", createdby)).Direction = System.Data.ParameterDirection.Input;

            result = "NO PROBLEM";

            if (pTYPE == "6")
            {
                reader = command.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);

                if (dt.Rows.Count > 0)
                {

                    StringBuilder ltr = new StringBuilder();
                    ltr.AppendLine("<html><body>");
                    ltr.AppendLine("<div style='width:95%; margin:0 auto; height:auto;'>");
                    ltr.AppendLine("<h2 style='background-color:black;color:black;'>Audit Scanning Report </h2><br/> ");
                    ltr.AppendLine("<h2 style='background-color:black;color:black;'>PC/Notebook Audit Record as at " + DateTime.Now.ToString("dd/MM/yyyy") + "</h2><br/>");
                    ltr.AppendLine("<h2 style='background-color:black;color:black;'>LOCATION : " + dt.Rows[0]["Location"] + "</h2><br/>");
                    ltr.AppendLine("<div style='width:1280px;'>");
                    ltr.AppendLine("<table>");
                    ltr.AppendLine("<thead><tr>");

                    foreach (DataColumn column in dt.Columns)
                    {
                        ltr.AppendLine("<th>");
                        ltr.AppendLine(column.ColumnName);
                        ltr.AppendLine("</th>");
                    }

                    ltr.AppendLine("</thead></tr><tbody>");

                    int countTotal = 0;
                    int countDone = 0;
                    int countNDone = 0;
                    int countWrong = 0;
                    int countInvalid = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        tempcount = 0;
                        ltr.AppendLine("<tr>");
                        foreach (DataColumn column in dt.Columns)
                        {
                            countTotal += 1;

                            //if (tempcount > 1)
                            //{
                            if (column.ColumnName.ToUpper() == "STATUS")
                            {
                                switch (row[column.ColumnName].ToString())
                                {
                                    case "DONE": countDone = countDone + 1; break;
                                    case "NOT DONE": countNDone = countNDone + 1; break;
                                    case "WRONG LOCATION": countWrong = countWrong + 1; break;
                                    case "INVALID RFID": countInvalid = countInvalid + 1; break;
                                }

                            }

                            ltr.AppendLine("<td>");
                            ltr.AppendLine(row[column.ColumnName].ToString());
                            ltr.AppendLine("</td>");
                            //}

                            //tempcount++;
                        }
                        ltr.AppendLine("</tr>");
                    }

                    ltr.AppendLine("<tbody></table>");
                    ltr.AppendLine("</div></div>");
                    ltr.AppendLine("</body></html>");

                    try
                    {
                        //string filename = "AuditReport" + dt.Rows[0]["Location"] + DateTime.Now.ToString("ddMMyyyyhhmmss");
                        string filename = "AuditReport" + pID + DateTime.Now.ToString("ddMMyyyyhhmmss");    //get the location base on location selected
                        string Path = AppDomain.CurrentDomain.BaseDirectory + "UploadedPath/StockTakeAudit/" + Convert.ToString(filename + ".xls");
                        //string Path = AppDomain.CurrentDomain.BaseDirectory + Convert.ToString(filename + ".xls");
                        Encoding objEncoding = Encoding.Default;

                        if (!File.Exists(Path))
                        {
                            using (StreamWriter sw = File.CreateText(Path))
                            {
                                sw.WriteLine(ltr);

                            }
                        }


                        string constr = ConfigurationManager.ConnectionStrings["SQLCon"].ConnectionString;
                        DataTable dt2 = new DataTable();
                        using (SqlConnection con = new SqlConnection(constr))
                        {
                            string query = "SELECT a.[email] " +
                                          "FROM[dbo].[mm_email]   a " +
                                        "WHERE a.record_typ <> '5'";
                            using (SqlCommand cmd = new SqlCommand(query))
                            {
                                cmd.Connection = con;
                                con.Open();
                                SqlDataReader sdr = cmd.ExecuteReader();
                                dt2.Load(sdr);
                            }

                        }

                        if (dt2.Rows.Count > 0)
                        {

                            string receiver = "";

                            for (Int16 i = 0; i < dt2.Rows.Count; i++)
                            {
                                if (receiver == "")
                                {
                                    receiver = dt2.Rows[i]["email"].ToString();
                                }
                                else
                                {
                                    receiver = receiver + "," + dt2.Rows[i]["email"].ToString();

                                }
                            }

                            SmtpClient SmtpServer = new SmtpClient("10.251.208.31", 25); //Old IP 10.252.130.24 (stop after 16/6/2022)
                            MailMessage mail = new MailMessage();
                            mail.IsBodyHtml = true;
                            mail.From = new MailAddress("appsnotification.tms.mb@mail.toray");
                            Int32 Prev_MaxServicePointIdleTime = 0;
                            mail.To.Add(receiver);

                            Prev_MaxServicePointIdleTime = System.Net.ServicePointManager.MaxServicePointIdleTime;
                            System.Net.ServicePointManager.MaxServicePointIdleTime = 1;

                            mail.Attachments.Add(new Attachment(Path));
                            mail.Subject = "Audit Scanning Report";
                            mail.Body = "please refer attachment";
                            SmtpServer.UseDefaultCredentials = true;
                            SmtpServer.Send(mail);
                            result = "EMAIL";
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        result = ex.ToString();
                        throw ex;
                    }
                }

            }
            else
            {
                command.ExecuteNonQuery();
            }

            CloseConnection();

            return result;
        }
        #endregion

        public string KeepSession() // Calling when we first hit controller
        {
            //ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
            //int userID = userobj.ID_ACL_USER;
            //userobj.ID_ACL_USER = userID;

            return "SUCCESS";
        }

    }

}
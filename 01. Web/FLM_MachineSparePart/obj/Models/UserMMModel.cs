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
using ImportExcel.Class;
using FILM_Sparepart_MVC.Helper_Code.Objects;

namespace UserMMModel
{ 

    public class SparePartModel
    {
        public int SPARE_PART_MODEL_ID { get; set; }
        [Required(ErrorMessage = "* Please Enter Model No.")]
        public string Model_No { get; set; }
        public string Remarks { get; set; }
        public string Record_Type { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string Created_Loc { get; set; }
        public string Updated_By { get; set; }
        public DateTime Updated_Date { get; set; }
        public string Updated_Loc { get; set; }
        public string SEARCH_VALUE { get; set; }

    }
    public class SparePartMachine
    {
        public int SPARE_PART_MACHINE_ID { get; set; }
        [Required(ErrorMessage = "* Please Enter Machine Model.")]
        public string Machine_Model { get; set; }
        public string Remarks { get; set; }
        public string Record_Type { get; set; }     
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string Created_Loc { get; set; }
        public string Updated_By { get; set; }
        public DateTime Updated_Date { get; set; }
        public string Updated_Loc { get; set; }
        public string SEARCH_VALUE { get; set; }
    }
    public class RfidType
    {
        public int RFID_TYPE_ID { get; set; }
        [Required(ErrorMessage = "* Please Enter RFID Type.")]
        public string Rfid_Type { get; set; }
        public string Remarks { get; set; }
        public string Record_Type { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string Created_Loc { get; set; }
        public string Updated_By { get; set; }
        public DateTime Updated_Date { get; set; }
        public string Updated_Loc { get; set; }
        public string SEARCH_VALUE { get; set; }
    }
    public class StoreLocation
    {
        public int STORAGE_ID { get; set; }        
        public string Storage_Code { get; set; }
        public string Storage_Desc_1 { get; set; }
        public string Storage_Desc_2 { get; set; }
        [Required(ErrorMessage = "* Please Select Storage Code.")]
        public int Reader_ID { get; set; }
        public string Reader_Name { get; set; }
        public string Remarks { get; set; }
        public string Record_Type { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string Created_Loc { get; set; }
        public string Updated_By { get; set; }
        public DateTime Updated_Date { get; set; }
        public string Updated_Loc { get; set; }
        public string SEARCH_VALUE { get; set; }

        public List<SelectListItem> DropdownStorageCode { get; set; }
        public List<int> SelectedMultiStorageCode { get; set; }
        public List<StorageCodeObj> SelectedStorageCodeLst { get; set; }
    }
    public class Manufacturer
    {
        public int Manufacturer_ID { get; set; }
        [Required(ErrorMessage = "* Please Enter Manufacturer Code.")]
        public string Manufacturer_Code { get; set; }
        public string Manufacturer_Desc { get; set; }
        public string Remarks { get; set; }
        public string Record_Type { get; set; }
        public string Record_Type_Desc { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string Created_Loc { get; set; }
        public string Updated_By { get; set; }
        public DateTime Updated_Date { get; set; }
        public string Updated_Loc { get; set; }
        public string SEARCH_VALUE { get; set; }
    }
    public class TransactionType
    {
        public int TRANS_TYPE_ID { get; set; }
        [Required(ErrorMessage = "* Please Enter Transaction Name.")]
        public string Trans_Name { get; set; }
        [Required(ErrorMessage = "* Please Select Transaction Description.")]
        public string Trans_Desc { get; set; }
        public string Trans_Desc1 { get; set; }
        public string Remarks { get; set; }
        public string Record_Type { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string Created_Loc { get; set; }
        public string Updated_By { get; set; }
        public DateTime Updated_Date { get; set; }
        public string Updated_Loc { get; set; }
        public string SEARCH_VALUE { get; set; }

        public List<SelectListItem> DropdownTransDesc { get; set; }
    }
    public class EmailList
    {
        public int EMAIL_LIST_ID { get; set; }
        [Required(ErrorMessage = "* Please Enter Email Group Name.")]
        public string Email_Group_Id { get; set; }        
        public string Email_Group_Cat { get; set; }
        [Required(ErrorMessage = "* Please Select Email Group Category.")]
        public int Email_Group_Cat_1 { get; set; }
        [Required(ErrorMessage = "* Please Enter Email List.")]
        public string Email_List { get; set; }
        public string Remarks { get; set; }
        public string Record_Type { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string Created_Loc { get; set; }
        public string Updated_By { get; set; }
        public DateTime Updated_Date { get; set; }
        public string Updated_Loc { get; set; }
        public string SEARCH_VALUE { get; set; }

        public List<SelectListItem> DropdownEmailGroupCat { get; set; }
    }
    public class SparePart
    {
        public int SPARE_PART_ID { get; set; }
        [Required(ErrorMessage = "* Please Select Model No.")]
        public int Spare_Part_Model_ID_1 { get; set; }
        //[Required(ErrorMessage = "* Please Select Machine No.")]
        public int Spare_Part_Machine_ID_1 {get; set;}
        //[Required(ErrorMessage = "* Please Select RFID Type.")]
        public int RFID_Type_ID_1 { get; set; }
        public int Manufacturer_ID_1 { get; set; }        
        public string Spare_Part_Model_ID { get; set; }
        [Required(ErrorMessage = "* Please Enter Series No.")]
        public string Series_No { get; set; }
        public string Part_No { get; set; }
        public string Spare_Part_Machine_ID { get; set; }        
        public string RFID_Type_ID { get; set; }

        [Required(ErrorMessage = "* Please Enter RFID Tag ID.")]
        public string RFID_Tag_ID { get; set; }

        [Required(ErrorMessage = "* Please Select Storage Code")]
        public int Reader_ID { get; set; }
        public string Storage_ID { get; set; }
        //[Required(ErrorMessage = "* Please Select Storage Description 1")]
        public string Storage_Description_1 { get; set; }
        //[Required(ErrorMessage = "* Please Select Storage Description 2")]
        public string Storage_Description_2 { get; set; }
        public string Manufacturer_ID { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string Record_Type { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string Created_Loc { get; set; }
        public string Updated_By { get; set; }
        public DateTime Updated_Date { get; set; }
        public string Updated_Loc { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string RFID_Tag_ID_2 { get; set; }
        public List<SelectListItem> DropdownModelNo { get; set; }
        public List<SelectListItem> DropdownMachineCode { get; set; }
        public List<SelectListItem> DropdownRFIDType { get; set; }
        public List<SelectListItem> DropdownStorageCode { get; set; }
        public List<SelectListItem> DropdownManufacCode { get; set; }

        //public List<SelectListItem> DropdownDesc1 { get; set; }
        //public List<SelectListItem> DropdownDesc2 { get; set; }

        //[Required]
        [Display(Name = "Model No")]
        public List<int> SelectedMultiModelNo { get; set; }
        public List<int> SelectedMultiMachine { get; set; }
        //[Required]
        [Display(Name = "RFID Type")]
        public List<int> SelectedMultiRFIDType { get; set; }
        //[Required]
        [Display(Name = "Storage Code")]
        public List<int> SelectedMultiStorageCode { get; set; }
        public List<int> SelectedMultiManufacCode { get; set; }

        //public List<int> SelectedMultiDesc1 { get; set; }
        //public List<int> SelectedMultiDesc2 { get; set; }
        public List<ModelObj> SelectedModelNoLst { get; set; }
        public List<ManufacturerObj> SelectedManufacCodeLst { get; set; }
        public List<RFIDTypeObj> SelectedRFIDTypeLst { get; set; }
        public List<StorageCodeObj> SelectedStorageCodeLst { get; set; }
        public List<MachineObj> SelectedMachineLst { get; set; }
        //public List<Desc1Obj> SelectedDesc1Lst { get; set; }
        //public List<Desc2Obj> SelectedDesc2Lst { get; set; }

    }
    public class Error
    {
        public int ERROR_TRANS_ID { get; set; }
        public int SPARE_PART_MODEL_ID { get; set; }
        public string RFID_TAG_ID { get; set; }
        public string Status { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string Created_Loc { get; set; }
        public string Updated_By { get; set; }
        public DateTime Updated_Date { get; set; }
        public string Updated_Loc {get; set;}

        public string Trans_Type { get; set; }
        public int Indicators { get; set; }

        public string Record_Type { get; set; }
        public string SEARCH_VALUE { get; set; }
    }
    public class StoreLocReader
    {
        public int READER_ID { get; set; }
        [Required(ErrorMessage = "* Please Enter Storage Code.")]
        public string Storage_Code { get; set; }
        [Required(ErrorMessage = "* Please Enter Reader Name.")]
        public string Reader_Name { get; set; }
        public string Remarks { get; set; }
        public string Record_Type { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string Created_Loc { get; set; }
        public string Updated_By { get; set; }
        public DateTime Updated_Date { get; set; }
        public string Updated_Loc { get; set; }
        public string SEARCH_VALUE { get; set; }
      
    }
    public class ImportFile
    {
        [Required(ErrorMessage = "Please select file")]
        [FileExt(Allow = ".xls,.xlsx", ErrorMessage = "Only excel file")]
        public HttpPostedFileBase file { get; set; }
        public int SPARE_PART_ID { get; set; }
        public int Spare_Part_Model_ID_1 { get; set; }
        public int Spare_Part_Machine_ID_1 { get; set; }
        public int RFID_Type_ID_1 { get; set; }
        public int Manufacturer_ID_1 { get; set; }
        public string Spare_Part_Model_ID { get; set; }
        public string Series_No { get; set; }
        public string Part_No { get; set; }
        public string Spare_Part_Machine_ID { get; set; }
        public string RFID_Type_ID { get; set; }
        public string RFID_Tag_ID { get; set; }
        public int Reader_ID { get; set; }
        public string Storage_ID { get; set; }
        public string Storage_Description_1 { get; set; }
        public string Storage_Description_2 { get; set; }
        public string Manufacturer_ID { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string Record_Type { get; set; }
        public string Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public string Created_Loc { get; set; }
        public string Updated_By { get; set; }
        public DateTime Updated_Date { get; set; }
        public string Updated_Loc { get; set; }
    }
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
        public DB()
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

        //Spare_Part_Model-------------------------------------------------------------------------------------------------
        public SparePartModel getSparePartModelData(string id)
        {
            OpenConnection();
            command.CommandText = "SP_FILM_SP_MODEL_SEL";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            SparePartModel SparePartModel = null;

            while (reader.Read())
            {
                SparePartModel = new SparePartModel();
                SparePartModel.SPARE_PART_MODEL_ID = Convert.ToInt32(reader["SPARE_PART_MODEL_ID"]);
                SparePartModel.Model_No = reader["MODEL_NO"].ToString();
                SparePartModel.Remarks = reader["REMARKS"].ToString();
                SparePartModel.Record_Type = reader["RECORD_TYPE"].ToString();
                SparePartModel.Created_By = reader["CREATED_BY"].ToString();
                SparePartModel.Created_Date = Convert.ToDateTime(reader["CREATED_DATE"]);
                SparePartModel.Created_Loc = reader["CREATED_LOC"].ToString();
                SparePartModel.Updated_By = reader["UPDATED_BY"].ToString();
                SparePartModel.Updated_Date = Convert.ToDateTime(reader["UPDATED_DATE"]);
                SparePartModel.Updated_Loc = reader["UPDATED_LOC"].ToString();
            }

            CloseReader();
            CloseConnection();

            return SparePartModel;

        }
        public string SparePartModelMaint(SparePartModel model, String record_typ, String loc, String pc)
        {
            string result = "";
            string return_value = "";
            string para3 = "";


            OpenConnection();
            try
            {

                command.CommandText = "SP_FILM_SP_MODEL_MAINT";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@para1", model.SPARE_PART_MODEL_ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para2", model.Model_No)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para3", para3)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@REMARKS", model.Remarks)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@PRECTYPE", record_typ)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@PCREATEDBY", pc)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@PCREATEDLOC", loc)).Direction = System.Data.ParameterDirection.Input;

                command.ExecuteScalar();
                return_value = "1";

                //return_value = command.ExecuteScalar().ToString();
                if (Convert.ToInt32(return_value) > 0)
                {
                    if (record_typ == "5")
                    {
                        result = "Data is successfully deleted.";
                    }
                    else
                    {
                        result = "Data is successfully saved.";
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

        //Spare_Part_Machine-------------------------------------------------------------------------------------------------
        public SparePartMachine getSparePartMachineData(string id)
        {
            OpenConnection();
            command.CommandText = "SP_FILM_SP_MACHINE_SEL";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            SparePartMachine SparePartMachine = null;

            while (reader.Read())
            {
                SparePartMachine = new SparePartMachine();
                SparePartMachine.SPARE_PART_MACHINE_ID = Convert.ToInt32(reader["SPARE_PART_MACHINE_ID"]);
                SparePartMachine.Machine_Model = reader["MACHINE_MODEL"].ToString();
                SparePartMachine.Remarks = reader["REMARKS"].ToString();
                SparePartMachine.Record_Type = reader["RECORD_TYPE"].ToString();
                SparePartMachine.Created_By = reader["CREATED_BY"].ToString();
                SparePartMachine.Created_Date = Convert.ToDateTime(reader["CREATED_DATE"]);
                SparePartMachine.Created_Loc = reader["CREATED_LOC"].ToString();
                SparePartMachine.Updated_By = reader["UPDATED_BY"].ToString();
                SparePartMachine.Updated_Date = Convert.ToDateTime(reader["UPDATED_DATE"]);
                SparePartMachine.Updated_Loc = reader["UPDATED_LOC"].ToString();
            }

            CloseReader();
            CloseConnection();

            return SparePartMachine;

        }
        public string SparePartMachineMaint(SparePartMachine model, String record_typ, String loc, String pc)
        {
            string result = "";
            string return_value = "";
            string para3 = "";

            OpenConnection();
            try
            {

                command.CommandText = "SP_FILM_SP_MACHINE_MAINT";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@para1", model.SPARE_PART_MACHINE_ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para2", model.Machine_Model)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para3", para3)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@REMARKS", model.Remarks)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@PRECTYPE", record_typ)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@PCREATEDBY", pc)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@PCREATEDLOC", loc)).Direction = System.Data.ParameterDirection.Input;

                command.ExecuteScalar();
                return_value = "1";
             
                //return_value = command.ExecuteScalar().ToString();
                if (Convert.ToInt32(return_value) > 0)
                {
                    if (record_typ == "5")
                    {
                        result = "Data is successfully deleted.";
                    }
                    else
                    {
                        result = "Data is successfully saved.";
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

        //MM_RFID_TYPE-------------------------------------------------------------------------------------------------
        public RfidType getRFIDTypeData(string id)
        {
            OpenConnection();
            command.CommandText = "SP_FILM_RFIDTYPE_SEL";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            RfidType RfidType = null;

            while (reader.Read())
            {
                RfidType = new RfidType();
                RfidType.RFID_TYPE_ID = Convert.ToInt32(reader["RFID_TYPE_ID"]);
                RfidType.Rfid_Type = reader["Rfid_Type"].ToString();
                RfidType.Remarks = reader["REMARKS"].ToString();
                RfidType.Record_Type = reader["RECORD_TYPE"].ToString();
                RfidType.Created_By = reader["CREATED_BY"].ToString();
                RfidType.Created_Date = Convert.ToDateTime(reader["CREATED_DATE"]);
                RfidType.Created_Loc = reader["CREATED_LOC"].ToString();
                RfidType.Updated_By = reader["UPDATED_BY"].ToString();
                RfidType.Updated_Date = Convert.ToDateTime(reader["UPDATED_DATE"]);
                RfidType.Updated_Loc = reader["UPDATED_LOC"].ToString();
            }

            CloseReader();
            CloseConnection();

            return RfidType;

        }
        public string RfidTypeMaint(RfidType model, String record_typ, String loc, String pc)
        {
            string result = "";
            string return_value = "0";
            string para3 = " ";

            OpenConnection();
            try
            {

                command.CommandText = "SP_FILM_RFID_TYPE_MAINT";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@para1", model.RFID_TYPE_ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para2", model.Rfid_Type)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para3", para3)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@REMARKS", model.Remarks)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pRecType", record_typ)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCreatedBy", pc)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCreatedLoc", loc)).Direction = System.Data.ParameterDirection.Input;


                command.ExecuteScalar();
                return_value = "1";

                //return_value = command.ExecuteScalar().ToString();
                if (Convert.ToInt32(return_value) > 0)
                {
                    if (record_typ == "5")
                    {
                        result = "Data is successfully deleted.";
                    }
                    else
                    {
                        result = "Data is successfully saved.";
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

        //MM_STORE_LOC-------------------------------------------------------------------------------------------------
        public StoreLocation getStoreLocData(string id)
        {
            OpenConnection();
            command.CommandText = "SP_FILM_STORAGE_SEL";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();
            DataTable dt = new DataTable();

            StoreLocation StoreLocation = null;

            while (reader.Read())
            {
                StoreLocation = new StoreLocation();
                StoreLocation.STORAGE_ID = Convert.ToInt32(reader["STORAGE_ID"]);
                StoreLocation.Storage_Code = reader["STORAGE_CODE"].ToString();
                StoreLocation.Storage_Desc_1 = reader["STORAGE_DESC_1"].ToString();
                StoreLocation.Storage_Desc_2 = reader["STORAGE_DESC_2"].ToString();
                StoreLocation.Reader_ID = Convert.ToInt32(reader["READER_ID"]);
                StoreLocation.Reader_Name = reader["READER_NAME"].ToString();
                StoreLocation.Remarks = reader["REMARKS"].ToString();
                StoreLocation.Record_Type = reader["RECORD_TYPE"].ToString();
                StoreLocation.Created_By = reader["CREATED_BY"].ToString();
                StoreLocation.Created_Date = Convert.ToDateTime(reader["CREATED_DATE"]);
                StoreLocation.Created_Loc = reader["CREATED_LOC"].ToString();
                StoreLocation.Updated_By = reader["UPDATED_BY"].ToString();
                StoreLocation.Updated_Date = Convert.ToDateTime(reader["UPDATED_DATE"]);
                StoreLocation.Updated_Loc = reader["UPDATED_LOC"].ToString();
            }
            CloseReader();
            CloseConnection();

            return StoreLocation;
        }       
        public string StoreLocationMaint(StoreLocation model, String record_typ, String loc, String pc)
        {
            string result = "";
            string return_value = "0";
            string para6 = " ";
            string para7 = " ";
            string para8 = " ";

            OpenConnection();
            try
            {

                command.CommandText = "SP_FILM_STORAGE_LOC_MAINT";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@para1", model.STORAGE_ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para2", model.Reader_ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para3", model.Storage_Desc_1)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para4", model.Storage_Desc_2)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para5", model.Reader_ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para6", para6)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para7", para7)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para8", para8)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@REMARKS", model.Remarks)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pRecType", record_typ)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCreatedBy", pc)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCreatedLoc", loc)).Direction = System.Data.ParameterDirection.Input;


                command.ExecuteScalar();
                return_value = "1";

                //return_value = command.ExecuteScalar().ToString();
                if (Convert.ToInt32(return_value) > 0)
                {
                    if (record_typ == "5")
                    {
                        result = "Data is successfully deleted.";
                    }
                    else
                    {
                        result = "Data is successfully saved.";
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

        //MM_MANUF-------------------------------------------------------------------------------------------------
        public Manufacturer getManufacturerData(string id)
        {
            OpenConnection();
            command.CommandText = "SP_FILM_MANU_SEL";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            Manufacturer Manufacturer = null;

            while (reader.Read())
            {
                Manufacturer = new Manufacturer();
                Manufacturer.Manufacturer_ID = Convert.ToInt32(reader["Manufacturer_ID"]);
                Manufacturer.Manufacturer_Code = reader["MANUFACTURER_CODE"].ToString();
                Manufacturer.Manufacturer_Desc = reader["MANUFACTURER_DESC"].ToString();
                Manufacturer.Remarks = reader["REMARKS"].ToString();
                Manufacturer.Record_Type = reader["RECORD_TYPE"].ToString();
                Manufacturer.Created_By = reader["CREATED_BY"].ToString();
                Manufacturer.Created_Date = Convert.ToDateTime(reader["CREATED_DATE"]);
                Manufacturer.Created_Loc = reader["CREATED_LOC"].ToString();
                Manufacturer.Updated_By = reader["UPDATED_BY"].ToString();
                Manufacturer.Updated_Date = Convert.ToDateTime(reader["UPDATED_DATE"]);
                Manufacturer.Updated_Loc = reader["UPDATED_LOC"].ToString();
            }

            CloseReader();
            CloseConnection();

            return Manufacturer;

        }
        public string ManufacturerMaint(Manufacturer model, String record_typ, String loc, String pc)
        {
            string result = "";
            string return_value = "0";
         
            OpenConnection();
            try
            {

                command.CommandText = "SP_FILM_MANU_MAINT";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@para1", model.Manufacturer_ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para2", model.Manufacturer_Code)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para3", model.Manufacturer_Desc)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@REMARKS", model.Remarks)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pRecType", record_typ)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCreatedBy", pc)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCreatedLoc", loc)).Direction = System.Data.ParameterDirection.Input;


                command.ExecuteScalar();
                return_value = "1";

                //return_value = command.ExecuteScalar().ToString();
                if (Convert.ToInt32(return_value) > 0)
                {
                    if (record_typ == "5")
                    {
                        result = "Data is successfully deleted.";
                    }
                    else
                    {
                        result = "Data is successfully saved.";
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

        //MM_TRAN_TYPE-------------------------------------------------------------------------------------------------
        public TransactionType getTransactionTypeData(string id)
        {
            OpenConnection();
            command.CommandText = "SP_FILM_TRANTYPE_SEL";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();
            TransactionType TransactionTypeModel = null;

            while (reader.Read())
            {
                TransactionTypeModel = new TransactionType();
                TransactionTypeModel.TRANS_TYPE_ID = Convert.ToInt32(reader["TRANS_TYPE_ID"]);
                TransactionTypeModel.Trans_Name = reader["TRANS_NAME"].ToString();
                TransactionTypeModel.Trans_Desc = reader["TRANS_DESC"].ToString();
                TransactionTypeModel.Trans_Desc1 = reader["TRANS_DESC1"].ToString();
                TransactionTypeModel.Remarks = reader["REMARKS"].ToString();
                TransactionTypeModel.Record_Type = reader["RECORD_TYPE"].ToString();
                TransactionTypeModel.Created_By = reader["CREATED_BY"].ToString();
                TransactionTypeModel.Created_Date = Convert.ToDateTime(reader["CREATED_DATE"]);
                TransactionTypeModel.Created_Loc = reader["CREATED_LOC"].ToString();
                TransactionTypeModel.Updated_By = reader["UPDATED_BY"].ToString();
                TransactionTypeModel.Updated_Date = Convert.ToDateTime(reader["UPDATED_DATE"]);
                TransactionTypeModel.Updated_Loc = reader["UPDATED_LOC"].ToString();
            }
            CloseReader();
            CloseConnection();

            return TransactionTypeModel;
        }
        public string TransactionTypeMaint(TransactionType model, String record_typ, String loc, String pc)
        {
            string result = "";
            string return_value = "0";

            OpenConnection();
            try
            {

                command.CommandText = "SP_FILM_TRANTYPE_MAINT";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@para1", model.TRANS_TYPE_ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para2", model.Trans_Name)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para3", model.Trans_Desc)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@REMARKS", model.Remarks)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pRecType", record_typ)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCreatedBy", pc)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCreatedLoc", loc)).Direction = System.Data.ParameterDirection.Input;


                command.ExecuteScalar();
                return_value = "1";

                //return_value = command.ExecuteScalar().ToString();
                if (Convert.ToInt32(return_value) > 0)
                {
                    if (record_typ == "5")
                    {
                        result = "Data is successfully deleted.";
                    }
                    else
                    {
                        result = "Data is successfully saved.";
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

        //Email-------------------------------------------------------------------------------------------------
        public EmailList getEmailListData(string id)
        {
            OpenConnection();
            command.CommandText = "SP_FILM_EMAIL_LIST_SEL";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            EmailList EmailList = null;

            while (reader.Read())
            {
                EmailList = new EmailList();
                EmailList.EMAIL_LIST_ID = Convert.ToInt32(reader["EMAIL_LIST_ID"]);
                EmailList.Email_Group_Id = reader["EMAIL_GROUP_ID"].ToString();
                EmailList.Email_Group_Cat = reader["EMAIL_GROUP_CAT"].ToString();
                EmailList.Email_Group_Cat_1 = Convert.ToInt32(reader["EMAIL_GROUP_CAT_1"]);
                EmailList.Email_List = reader["EMAIL_LIST"].ToString();
                EmailList.Remarks = reader["REMARKS"].ToString();
                EmailList.Record_Type = reader["RECORD_TYPE"].ToString();
                EmailList.Created_By = reader["CREATED_BY"].ToString();
                EmailList.Created_Date = Convert.ToDateTime(reader["CREATED_DATE"]);
                EmailList.Created_Loc = reader["CREATED_LOC"].ToString();
                EmailList.Updated_By = reader["UPDATED_BY"].ToString();
                EmailList.Updated_Date = Convert.ToDateTime(reader["UPDATED_DATE"]);
                EmailList.Updated_Loc = reader["UPDATED_LOC"].ToString();
            }

            CloseReader();
            CloseConnection();

            return EmailList;

        }
        public string EmailListMaint(EmailList model, String record_typ, String loc, String pc)
        {
            string result = "";
            string return_value = "0";
            string para5 = " ";
            string para6 = " ";
            string para7 = " ";
            string para8 = " ";

            OpenConnection();
            try
            {

                command.CommandText = "SP_FILM_EMAIL_LIST_MAINT";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@para1", model.EMAIL_LIST_ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para2", model.Email_Group_Id)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para3", model.Email_List)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para4", model.Email_Group_Cat_1)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para5", para5)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para6", para6)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para7", para7)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para8", para8)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@REMARKS", model.Remarks)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pRecType", record_typ)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCreatedBy", pc)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCreatedLoc", loc)).Direction = System.Data.ParameterDirection.Input;


                command.ExecuteScalar();
                return_value = "1";

                //return_value = command.ExecuteScalar().ToString();
                if (Convert.ToInt32(return_value) > 0)
                {
                    if (record_typ == "5")
                    {
                        result = "Data is successfully deleted.";
                    }
                    else
                    {
                        result = "Data is successfully saved.";
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

        //Storage Reader-------------------------------------------------------------------------------------------------
        public StoreLocReader getStorageReaderData(string id)
        {
            OpenConnection();
            command.CommandText = "SP_FILM_READER_SEL";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            StoreLocReader StoreLocReader = null;

            while (reader.Read())
            {
                StoreLocReader = new StoreLocReader();
                StoreLocReader.READER_ID = Convert.ToInt32(reader["READER_ID"]);
                StoreLocReader.Storage_Code = reader["STORAGE_CODE"].ToString();
                StoreLocReader.Reader_Name = reader["READER_NAME"].ToString();
                StoreLocReader.Remarks = reader["REMARKS"].ToString();
                StoreLocReader.Record_Type = reader["RECORD_TYPE"].ToString();
                StoreLocReader.Created_By = reader["CREATED_BY"].ToString();
                StoreLocReader.Created_Date = Convert.ToDateTime(reader["CREATED_DATE"]);
                StoreLocReader.Created_Loc = reader["CREATED_LOC"].ToString();
                StoreLocReader.Updated_By = reader["UPDATED_BY"].ToString();
                StoreLocReader.Updated_Date = Convert.ToDateTime(reader["UPDATED_DATE"]);
                StoreLocReader.Updated_Loc = reader["UPDATED_LOC"].ToString();
            }

            CloseReader();
            CloseConnection();

            return StoreLocReader;

        }        
        public string StoreLocReaderMaint(StoreLocReader model, String record_typ, String loc, String pc)
        {
            string result = "";
            string return_value = "0";

            OpenConnection();
            try
            {
                command.CommandText = "SP_FILM_READER_MAINT";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@para1", model.READER_ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para2", model.Storage_Code)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para3", model.Reader_Name)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@REMARKS", model.Remarks)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pRecType", record_typ)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCreatedBy", pc)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCreatedLoc", loc)).Direction = System.Data.ParameterDirection.Input;

                command.ExecuteScalar();
                return_value = "1";

                //return_value = command.ExecuteScalar().ToString();
                if (Convert.ToInt32(return_value) > 0)
                {
                    if (record_typ == "5")
                    {
                        result = "Data is successfully deleted.";
                    }
                    else
                    {
                        result = "Data is successfully saved.";
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

        //Spare Part-------------------------------------------------------------------------------------------------------
        public SparePart getSparePartData(string id)
        {
            OpenConnection();
            command.CommandText = "SP_FILM_SP_SEL";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pID", id)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            SparePart SparePart = null;

            while (reader.Read())
            {
                SparePart = new SparePart();
                SparePart.SPARE_PART_ID = Convert.ToInt32(reader["SPARE_PART_ID"]);
                SparePart.Spare_Part_Model_ID_1 = Convert.ToInt32(reader["SPARE_PART_MODEL_ID_1"]);

                int ? counts1 = string.IsNullOrEmpty(reader["SPARE_PART_MACHINE_ID_1"].ToString()) ? (int?)null : int.Parse(reader["SPARE_PART_MACHINE_ID_1"].ToString());
                if (counts1 == null)
                {
                    SparePart.Spare_Part_Machine_ID_1 = 0;
                }
                else
                {
                    SparePart.Spare_Part_Machine_ID_1 = Convert.ToInt32(counts1);
                }
             
                SparePart.RFID_Type_ID_1 = Convert.ToInt32(reader["RFID_TYPE_ID_1"]);
                SparePart.Manufacturer_ID_1 = Convert.ToInt32(reader["MANUFACTURER_ID_1"]);
                SparePart.Spare_Part_Model_ID = reader["SPARE_PART_MODEL_ID"].ToString();
                SparePart.Series_No = reader["SERIES_NO"].ToString();
                SparePart.Part_No = reader["PART_NO"].ToString();
                SparePart.Spare_Part_Machine_ID = reader["SPARE_PART_MACHINE_ID"].ToString();
                SparePart.RFID_Type_ID = reader["RFID_TYPE_ID"].ToString();
                SparePart.RFID_Tag_ID = reader["RFID_TAG_ID"].ToString();
                SparePart.RFID_Tag_ID_2 = reader["RFID_TAG_ID_2"].ToString();
                SparePart.Reader_ID = Convert.ToInt32(reader["READER_ID"]);
                SparePart.Storage_ID = reader["STORAGE_ID"].ToString();
                SparePart.Storage_Description_1 = reader["STORAGE_DESCRIPTION1"].ToString();
                SparePart.Storage_Description_2 = reader["STORAGE_DESCRIPTION2"].ToString();
                SparePart.Manufacturer_ID = reader["MANUFACTURER_ID"].ToString();
                SparePart.Status = reader["STATUS"].ToString();
                SparePart.Remarks = reader["REMARKS"].ToString();
                SparePart.Record_Type = reader["RECORD_TYPE"].ToString();
                SparePart.Created_By = reader["CREATED_BY"].ToString();
                SparePart.Created_Date = Convert.ToDateTime(reader["CREATED_DATE"]);
                SparePart.Created_Loc = reader["CREATED_LOC"].ToString();
                SparePart.Updated_By = reader["UPDATED_BY"].ToString();
                SparePart.Updated_Date = Convert.ToDateTime(reader["UPDATED_DATE"]);
                SparePart.Updated_Loc = reader["UPDATED_LOC"].ToString();
            }

            CloseReader();
            CloseConnection();

            return SparePart;

        }
        public string SparePartMaint(SparePart model, String record_typ, String loc, String pc)
        {
            string result = "";
            string return_value = "0";
            int mch = 0;
            string MACHINE_ID = "";

            if (model.Spare_Part_Machine_ID_1 == 0)
            {
                MACHINE_ID = null;
            }
            else
            {
                mch = model.Spare_Part_Machine_ID_1;
                MACHINE_ID = mch.ToString();
            }

            OpenConnection();
            try
            {
                command.CommandText = "SP_FILM_SP_MAINT";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@para1", model.SPARE_PART_ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para2", model.Series_No)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para3", model.Spare_Part_Model_ID_1)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para4", model.Part_No)).Direction = System.Data.ParameterDirection.Input;
                //command.Parameters.Add(new SqlParameter("@para5", model.Spare_Part_Machine_ID_1)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para5", MACHINE_ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para6", model.RFID_Type_ID_1)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para7", model.RFID_Tag_ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para12", model.RFID_Tag_ID_2)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para8", model.Reader_ID)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para9", model.Storage_Description_1)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para10", model.Storage_Description_2)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para11", model.Manufacturer_ID_1)).Direction = System.Data.ParameterDirection.Input;            
                command.Parameters.Add(new SqlParameter("@REMARKS", model.Remarks)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pRecType", record_typ)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCreatedBy", pc)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@pCreatedLoc", loc)).Direction = System.Data.ParameterDirection.Input;
                command.ExecuteScalar();
                return_value = "1";

                //return_value = command.ExecuteScalar().ToString();
                if (Convert.ToInt32(return_value) > 0)
                {
                    if (record_typ == "5")
                    {
                        result = "Data is successfully deleted.";
                    }
                    else
                    {
                        result = "Data is successfully saved.";
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
        public string SpartExcelImportFile(string para1, string modelno, string seriesno, string partno, string machine, string rfidtype, string rfidtag, string rfidtag2, string storagecode, string storagedesc1, string storagedesc2, string manucode, string remarks, string updatedby, string updatedloc, int prow)
        {
            string result = "";
            string return_value = "0";

            OpenConnection();
            try
            {
                command.CommandText = "SP_FILM_SP_MAINT_EXCEL";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@para1", para1)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para2", modelno)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para3", seriesno)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para4", partno)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para5", machine)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para6", rfidtype)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para7", rfidtag)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para12", rfidtag2)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para8", storagecode)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para9", storagedesc1)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para10", storagedesc2)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para11", manucode)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@REMARKS", remarks)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@PRECTYPE", 1)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@PCREATEDBY", updatedby)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@PCREATEDLOC", updatedloc)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@prow", prow)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@RETURN_VALUE", SqlDbType.NVarChar, 4000)).Direction = ParameterDirection.Output;

                command.ExecuteScalar();
                return_value = "1";

                //return_value = command.ExecuteScalar().ToString();
                if (Convert.ToInt32(return_value) > 0)
                {
                    result = "Data is successfully saved.";
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

        //Error List-------------------------------------------------------------------------------------------------------
        public string ErrorMaint(String id, String record_typ, String loc, String pc)
        {
            string result = "";
            string return_value = "0";

            OpenConnection();
            try
            {
                command.CommandText = "SP_FILM_ERROR_SS";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@para1", id)).Direction = System.Data.ParameterDirection.Input;
                command.Parameters.Add(new SqlParameter("@para2", pc)).Direction = System.Data.ParameterDirection.Input;                
                command.ExecuteScalar();
                return_value = "1";

                //return_value = command.ExecuteScalar().ToString();
                if (Convert.ToInt32(return_value) > 0)
                {
                    if (record_typ == "5")
                    {
                        result = "Data is successfully deleted.";
                    }
                    else
                    {
                        result = "Data is successfully saved.";
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

        public DataTable getModelNoLst(Int64 modelID)
        {
            OpenConnection();
            command.CommandText = "SELECT * from [dbo].[PVIEW_MM_SP_MODEL] WHERE RECORD_TYPE <> '5' AND SPARE_PART_MODEL_ID = @pID";
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pID", modelID)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();

            return dt;
        }
        public DataTable getManufacCodeLst(Int64 manuID)
        {
            OpenConnection();
            command.CommandText = "SELECT * from [dbo].[PVIEW_MM_MANUFACTURER] WHERE RECORD_TYPE <> '5' AND MANUFACTURER_ID = @pID";
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pID", manuID)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();

            return dt;
        }
        public DataTable getMachineLst(Int64 macID)
        {
            OpenConnection();
            command.CommandText = "SELECT * from [dbo].[pview_MM_SP_MACHINE] WHERE RECORD_TYPE <> '5' AND SPARE_PART_MACHINE_ID = @pID";
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pID", macID)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();

            return dt;
        }
        public DataTable getRFIDTypeLst(Int64 rfidID)
        {
            OpenConnection();
            command.CommandText = "SELECT * from [dbo].[PVIEW_MM_RFID_TYPE] WHERE RECORD_TYPE <> '5' AND RFID_TYPE_ID = @pID";
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pID", rfidID)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();

            return dt;
        }
        public DataTable getStorageCodeLst(Int64 storageID)
        {
            OpenConnection();
            command.CommandText = "SELECT * from [dbo].[PVIEW_MM_SPAREPART_RDR] WHERE RECORD_TYPE <> '5' AND READER_ID = @pID";
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pID", storageID)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();

            return dt;
        }
        public DataTable getReaderNameLst(Int64 id)
        {
            OpenConnection();
            command.CommandText = "SP_FILM_GET_READER_NAME";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@para1", id)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();
            
            return dt;

        }
        public DataTable getStorageDesc1Lst(String storagecode)
        {
            OpenConnection();
            command.CommandText = "SP_FILM_GET_STORAGE_DESCRIPTION_V1";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pSTORAGE_ID", storagecode)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();

            return dt;

        }

        public DataTable getStorageDesc2Lst(String storagecode, String desc1)
        {
            OpenConnection();
            command.CommandText = "SP_FILM_GET_STORAGE_DESCRIPTION_V2";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@pSTORAGE_ID", storagecode)).Direction = System.Data.ParameterDirection.Input;
            command.Parameters.Add(new SqlParameter("@pSTORAGE_DESC1", desc1)).Direction = System.Data.ParameterDirection.Input;
            reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            CloseReader();
            CloseConnection();

            return dt;

        }      
    }
}
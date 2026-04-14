using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FILM_Sparepart_MVC.Models
{
    public class SearchSource
    {
        //publich static string[] list name = {display name, db column name};
        public static string[] MM_SPARE_PART_LST = { "Model No,MODEL_NO", "Remarks,REMARKS" };
        public static string[] MM_RFID_TYPE_LST = { "RFID Type,RFID_TYPE", "Remarks,REMARKS" };
        public static string[] MM_SPARE_PART_M_LST = { "Machine Model,MACHINE_MODEL", "Remarks,REMARKS" };
        public static string[] MM_STORE_LOC_LST = { "Storage Code,STORAGE_CODE", "Storage Desc 1,STORAGE_DESC_1", "Storage Desc 2,STORAGE_DESC_2", "Reader Name,READER_NAME", "Remarks,REMARKS" };
        public static string[] MM_MANUF_LST = { "Manufacturer Code,MANUFACTURER_CODE", "Manufacturer Desc,MANUFACTURER_DESC", "Remarks,REMARKS" };
        public static string[] MM_TRAN_TYPE_LST = { "Trans Name,TRANS_NAME", "Trans Desc,TRANS_DESC1", "Remarks,REMARKS"};
        public static string[] MM_EMAIL_LIST_LST = { "Email Group,EMAIL_GROUP_ID", "Email Category,EMAIL_GROUP_CAT", "Email List,EMAIL_LIST", "Remarks,REMARKS" };
        public static string[] MM_SPART_LST = { "Model No,SPARE_PART_MODEL_ID", "Serial No,SERIES_NO", "Part No,PART_NO", "RFID Tag,RFID_TAG_ID", "Machine No,SPARE_PART_MACHINE_ID", "Storage,STORAGE_ID", "Manufacturer,MANUFACTURER_ID", "Status,STATUS", "Remarks,REMARKS" };
        public static string[] MM_ERROR_INVALID_SS = { "Error Days,INDICATORS", "Transaction Type,TRANS_TYPE", "RFID Tag,RFID_TAG_ID", "Status,STATUS", "Created Date,CREATED_DATE" };
        public static string[] MM_STORE_RDR_LST = { "Storage Code,STORAGE_CODE", "Reader Name,READER_NAME", "Remarks,REMARKS" };

        public static string[] MM_TRANS_REG_LST = { "Model No,MODEL_NO", "Series No,SERIES_NO", "Part No,PART_NO", "RFID Tag ID,RFID_TAG_ID", "Transaction Name,TRANS_NAME", "Reason,TRANS_REASON", "Status,STATUS" };
        public static string[] MM_TRANS_REG_PPMODEL_Search = { "Model No,MODEL_NO", "Remarks,REMARKS" };
        public static string[] MM_TRANS_REG_MODELNO_Search = { "Model No,MODEL_NO", "Remarks,REMARKS" };
        public static string[] MM_TRANS_REG_PPMODEL = { "Model No,MODEL_NO", "Remarks,REMARKS" };
    }
}
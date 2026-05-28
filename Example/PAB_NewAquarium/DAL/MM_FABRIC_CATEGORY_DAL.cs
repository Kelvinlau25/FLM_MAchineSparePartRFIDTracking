using PAB_NewAquarium.Helpers;
using PAB_NewAquarium.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;


namespace PAB_NewAquarium.DAL
{
    public class MM_FABRIC_CATEGORY_DAL
    {
        CommonFunction common = new CommonFunction();

        public async Task<MM_FABRIC_CATEGORY> PSP_FABRIC_CATEGORY_MAINT(MM_FABRIC_CATEGORY model)
        {
            List<SqlParameter> _pMssql = new List<SqlParameter>();
            _pMssql.Add(new SqlParameter("@pID", model.CATEGORY_H_ID));
            _pMssql.Add(new SqlParameter("@pDeleteID", model.CATEGORY_H_DELETE_ID));
            _pMssql.Add(new SqlParameter("@pCategoryName", model.CATEGORY_NAME));
            _pMssql.Add(new SqlParameter("@pCategoryDetail", model.CATEGORY_DETAIL));
            _pMssql.Add(new SqlParameter("@pRecordTyp", model.RECORD_TYP));
            _pMssql.Add(new SqlParameter("@pCreatedBy", model.CREATED_BY));
            _pMssql.Add(new SqlParameter("@pCreatedDate", model.CREATED_DATE));
            _pMssql.Add(new SqlParameter("@pCreatedLoc", model.CREATED_LOC));
            _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

            model.RETURN_ID = await common.PSP_COMMON_SQL("PSP_MM_CATEGORY_H_MAINT", CommandType.StoredProcedure, _pMssql, "", "", true);

            _pMssql = new List<SqlParameter>();
            _pMssql.Add(new SqlParameter("@pCATEGORY_H_ID", model.RETURN_ID));
            _pMssql.Add(new SqlParameter("@pPRODUCT_H_ID", model.PRODUCT_H_ID));
            _pMssql.Add(new SqlParameter("@pDeleteID", model.CATEGORY_H_DELETE_ID));
            _pMssql.Add(new SqlParameter("@pRecordTyp", model.RECORD_TYP));
            _pMssql.Add(new SqlParameter("@pCreatedBy", model.CREATED_BY));
            _pMssql.Add(new SqlParameter("@pCreatedDate", model.CREATED_DATE));
            _pMssql.Add(new SqlParameter("@pCreatedLoc", model.CREATED_LOC));
            _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

            model.RETURN_MESSAGE = await common.PSP_COMMON_SQL("PSP_MM_CATEGORY_D_MAINT", CommandType.StoredProcedure, _pMssql, "", "", true);

            return model;
        }
    }
}
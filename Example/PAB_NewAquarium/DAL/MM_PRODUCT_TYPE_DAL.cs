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
    public class MM_PRODUCT_TYPE_DAL
    {
        CommonFunction common = new CommonFunction();

        public async Task<MM_PRODUCT_TYPE> PSP_PRODUCT_TYPE_MAINT(MM_PRODUCT_TYPE model)
        {
            List<SqlParameter> _pMssql = new List<SqlParameter>();
            _pMssql.Add(new SqlParameter("@pID", model.PRODUCT_TYPE_ID));
            _pMssql.Add(new SqlParameter("@pDeleteID", model.PRODUCT_TYPE_DELETE_ID));
            _pMssql.Add(new SqlParameter("@pProductType", model.PRODUCT_TYPE));
            _pMssql.Add(new SqlParameter("@pProductDesc", model.PRODUCT_DESC));
            _pMssql.Add(new SqlParameter("@pRecordTyp", model.RECORD_TYP));
            _pMssql.Add(new SqlParameter("@pCreatedBy", model.CREATED_BY));
            _pMssql.Add(new SqlParameter("@pCreatedDate", model.CREATED_DATE));
            _pMssql.Add(new SqlParameter("@pCreatedLoc", model.CREATED_LOC));
            _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });

            model.RETURN_MESSAGE = await common.PSP_COMMON_SQL("PSP_MM_PRODUCT_TYPE_MAINT", CommandType.StoredProcedure, _pMssql, "", "", true);
            return model;
        }
    }
}
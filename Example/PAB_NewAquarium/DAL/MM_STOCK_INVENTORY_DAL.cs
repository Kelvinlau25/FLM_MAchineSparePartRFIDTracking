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
    public class MM_STOCK_INVENTORY_DAL
    {
        CommonFunction common = new CommonFunction();

        public async Task<MM_STOCK_INVENTORY> PSP_STOCK_INVENTORY_MAINT(MM_STOCK_INVENTORY model)
        {
            if (model.STOCK_TYPE == "In")
            {
                for (int i = 0; i < model.STOCK_IN_RFID.Count; i++)
                {
                    List<SqlParameter> _pMssql = new List<SqlParameter>();
                    _pMssql.Add(new SqlParameter("@pReferencNo", model.STOCK_IN_REFERENCE_NO[i]));
                    _pMssql.Add(new SqlParameter("@pChopNo", model.STOCK_IN_CHOP_NO[i]));
                    _pMssql.Add(new SqlParameter("@pColorWay", model.STOCK_IN_COLOR_WAY[i]));
                    _pMssql.Add(new SqlParameter("@pStoreLoc", model.STOCK_IN_STORE_LOC[i]));
                    _pMssql.Add(new SqlParameter("@pRFID", model.STOCK_IN_RFID[i]));
                    _pMssql.Add(new SqlParameter("@pCreatedBy", model.CREATED_BY));
                    _pMssql.Add(new SqlParameter("@pCreatedDate", model.CREATED_DATE));
                    _pMssql.Add(new SqlParameter("@pCreatedLoc", model.CREATED_LOC));
                    _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                    model.RETURN_MESSAGE = await common.PSP_COMMON_SQL("PSP_MM_PRODUCT_I_STOCK_IN", CommandType.StoredProcedure, _pMssql, "", "", true);
                }
            }
            else
            {
                for (int i = 0; i < model.STOCK_OUT_ID.Count; i++)
                {
                    List<SqlParameter> _pMssql = new List<SqlParameter>();
                    _pMssql.Add(new SqlParameter("@pInventoryID", model.STOCK_OUT_ID[i]));
                    _pMssql.Add(new SqlParameter("@pCreatedBy", model.CREATED_BY));
                    _pMssql.Add(new SqlParameter("@pCreatedDate", model.CREATED_DATE));
                    _pMssql.Add(new SqlParameter("@pCreatedLoc", model.CREATED_LOC));
                    _pMssql.Add(new SqlParameter("returnResult", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                    model.RETURN_MESSAGE = await common.PSP_COMMON_SQL("PSP_MM_PRODUCT_I_STOCK_OUT", CommandType.StoredProcedure, _pMssql, "", "", true);
                }
            }
            return model;
        }
    }
}
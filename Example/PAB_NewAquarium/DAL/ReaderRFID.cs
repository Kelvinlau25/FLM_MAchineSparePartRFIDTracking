using PAB_NewAquarium.Helpers;
using PAB_NewAquarium.Models;
using System.Data;
using System.Threading.Tasks;

namespace PAB_NewAquarium.DAL
{
    public class ReaderRFID
    {
        private readonly CommonFunction common = new CommonFunction();

        public async Task<RFIDModel> GetReader(string view)
        {
            object obj = new { RESOURCE_VIEW = view };
            RFIDModel model = await common.PSP_COMMON_DAPPER_SINGLE<RFIDModel>("PSP_GET_RFID_CONFIG", CommandType.StoredProcedure, obj);

            return model;
        }
    }
}
using ClosedXML.Excel;
using FILM_Sparepart_MVC.DAL;
using FILM_Sparepart_MVC.Enums;
using FILM_Sparepart_MVC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;

namespace FILM_Sparepart_MVC.Repository
{
    public class RfidAuditRepo
    {

        DateTime minDt = SqlDateTime.MinValue.Value;
        DateTime maxDt = SqlDateTime.MinValue.Value;

        RfidAuditContext _rfidAuditCtx;
        TMS_ITEquiptContext _TMS_ITEquiptContext;
        public RfidAuditRepo(RfidAuditContext rfidAuditCtx, TMS_ITEquiptContext TMS_ITEquiptContext)
        {
            _rfidAuditCtx = rfidAuditCtx;
            _TMS_ITEquiptContext = TMS_ITEquiptContext;
        }

        //public List<MM_SP_RFID_Audit> getAllRfids()
        //{
        //    return _rfidAuditCtx.MM_SP_RFID_Audit
        //        .ToList();
        //}

        public int saveRfid(MM_SP_RFID_Audit rfidAudit)
        {
            _rfidAuditCtx.MM_SP_RFID_Audits.Add(rfidAudit);
            try
            {
                return _rfidAuditCtx.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throwEntityException(e);
                return 0;
            }
        }

        public RfidResponseModel getInquiryTable(string seriesNo)
        {
            String queryString = getInquiryTableQueryString(seriesNo);
            RfidResponseModel inquiryResponseModel = new RfidResponseModel();
            List<object> sqlParams = getInquiryTableSqlParams(seriesNo);

            List<InquiryTable> inquiryTables =
            _rfidAuditCtx.Database.SqlQuery<InquiryTable>(
                queryString, sqlParams.ToArray()).ToList();

            inquiryResponseModel.inquiryTables = inquiryTables;

            return inquiryResponseModel;
        }

        public RfidResponseModel getInquiryTable()
        {
            return getInquiryTable(null);
        }
        public string getInquiryDetailQueryString(int id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT RA.RFID_AUDIT_ID AS rfidAuditId, RA.RFID_TAG_CODE as rfidTagCode, RA.RFID_FLAG as rfidFlag, SPMO.MODEL_NO as modelNo, SPM.MACHINE_MODEL as machineModel, ");
            sb.Append(" SP.SERIES_NO as seriesNo, SP.PART_NO as  partNo, SPM.MACHINE_MODEL as machineModel, SPR.STORAGE_CODE as storageCode, ");
            sb.Append(" SM.MANUFACTURER_CODE AS manufacturer,  SP.STATUS as status,  SPM.REMARKS as remarks, SP.RFID_TAG_ID as rfidTagId, RA.CREATED_DATE AS createdDate, RA.CREATED_BY AS createdBy, RA.UPDATED_DATE AS updatedDate, RA.UPDATED_BY AS updatedBy");
            sb.Append(" FROM ");
            sb.Append(" MM_SP_RFID_Audit AS  RA LEFT JOIN ");
            sb.Append(" MM_SPARE_PART  AS SP ");
            sb.Append(" ON RA.RFID_TAG_CODE = SP.RFID_TAG_ID ");
            sb.Append(" LEFT JOIN  MM_SPARE_PART_MACHINE AS SPM ");
            sb.Append(" ON SP.SPARE_PART_MACHINE_ID = SPM.SPARE_PART_MACHINE_ID ");
            sb.Append(" LEFT JOIN  MM_SP_STORAGE AS SS ");
            sb.Append(" ON SP.STORAGE_ID = SS.STORAGE_ID ");
            sb.Append(" LEFT JOIN  MM_SP_MANUFACTURER AS SM ");
            sb.Append(" ON SP.MANUFACTURER_ID = SM.MANUFACTURER_ID ");
            sb.Append(" LEFT JOIN MM_SPARE_PART_MODEL AS SPMO ");
            sb.Append(" ON SP.SPARE_PART_MODEL_ID = SPMO.SPARE_PART_MODEL_ID ");
            sb.Append(" LEFT JOIN MM_SPARE_PART_READER AS SPR ");
            sb.Append(" ON SPR.READER_ID = SS.READER_ID ");
            sb.Append(" WHERE RA.RECORD_TYPE <> 5 AND SP.RECORD_TYPE <> 5 ");
            sb.Append(" AND SPM.RECORD_TYPE <> 5 AND SS.RECORD_TYPE <> 5 ");
            sb.Append(" AND SM.RECORD_TYPE <> 5 AND SPMO.RECORD_TYPE <> 5 ");
            sb.Append(" AND SM.RECORD_TYPE <> 5 AND SPMO.RECORD_TYPE <> 5 ");
            sb.Append(" AND RA.RFID_AUDIT_ID = @id ");



            return sb.ToString();

        }
        public string getInquiryTableQueryString(string seriesNo)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT * FROM PVIEW_MM_SPARE_PART ");

            if (!String.IsNullOrEmpty(seriesNo))
            {
                sb.Append(" WHERE ");
                sb.Append(" SERIES_NO = @seriesNo");
            }
            return sb.ToString();

        }
        public InquiryTable getInquiryDetail(int id)
        {
            String queryString = getInquiryDetailQueryString(id);
            InquiryTable inquiryTable =
             _rfidAuditCtx.Database.SqlQuery<InquiryTable>(queryString,
                 new SqlParameter("@id", id)
                 ).First<InquiryTable>();


            return inquiryTable;
        }
        public List<MM_SP_RFID_Audit> getLastestRfids()
        {

            List<MM_SP_RFID_Audit> MM_SP_RFID_Audits = _rfidAuditCtx.MM_SP_RFID_Audits
               .OrderByDescending(rfid => rfid.RFID_AUDIT_ID)
               .ToList();
            return MM_SP_RFID_Audits;
        }


        public bool checkWhetherRfidAlreadyExists(string rfid)
        {
            int rfidCount = _rfidAuditCtx
                .MM_SPARE_PARTs
                .Where(obj => obj.RFID_TAG_ID.Equals(rfid)).Count();
            if (rfidCount > 0)
            {
                return true;
            }
            return false;

        }



        public RfidResponseModel getLocations()
        {
            Dictionary<int, string> locations = new Dictionary<int, string>();
            RfidResponseModel inquiryResponse = new RfidResponseModel();
            inquiryResponse.locations = new Dictionary<int, string>();
            foreach (var storage in _rfidAuditCtx.MM_SPARE_PART_READERs)
            {
                inquiryResponse.locations.Add(storage.READER_ID, storage.STORAGE_CODE);
            }
            return inquiryResponse;
        }
        public RfidResponseModel getReportTable(RfidReportRequestModel report)
        {
            String queryString = "";
            bool similarDate = false;
            if (((report.dateFrom < minDt || report.dateFrom > maxDt) || (report.dateTo < minDt || report.dateTo > maxDt)) && (report.dateFrom == report.dateTo))
            {
                similarDate = true;
            }
            else
            {
                similarDate = false;
            }

            queryString = getReportTableQueryString(report, similarDate);
            RfidResponseModel rfidResponseModel = new RfidResponseModel();



            List<object> sqlParams = getReportTableSqlParams(report);
            List<RfidReportTable> reportTables =
                 _rfidAuditCtx.Database.SqlQuery<RfidReportTable>(
                     queryString, sqlParams.ToArray()).ToList();
            if (similarDate)
            {
                reportTables =
                 reportTables.Where(x => x.createdDate == report.dateFrom.ToString("dd/MM/yyyy")).ToList();
            }
            else if (report.dateFilter && !similarDate)
            {
                var dateFrom = Convert.ToDateTime(report.dateFrom.ToString("dd/MM/yyyy"),
    System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                var dateTo = Convert.ToDateTime(report.dateTo.ToString("dd/MM/yyyy"),
    System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                reportTables =
                    reportTables.Where(x => Convert.ToDateTime(x.createdDate,
    System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) >= dateFrom && Convert.ToDateTime(x.createdDate,
    System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat) <= dateTo).ToList();
            }

            rfidResponseModel.rfidReportTables = reportTables;

            return rfidResponseModel;
        }
        public string getReportTableQueryString(RfidReportRequestModel report, Boolean similarDate)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT * FROM PVIEW_RFID_REPORT ");
            sb.Append(" WHERE 1 = 1");



            if (!String.IsNullOrEmpty(report.rfid))
            {
                sb.Append(" AND (rfidId = @rfid");

                sb.Append(" OR rfid_tag_code = @rfid )");
            }
            if (!String.IsNullOrEmpty(report.status))
            {
                sb.Append(" AND status = @status");
                if (report.status == "N" && String.IsNullOrEmpty(report.rfid))
                {
                    sb.Append(" OR status is null");
                }
            }
            if (!String.IsNullOrEmpty(report.location))
            {
                sb.Append(" AND (readerId = @location ");
                sb.Append(" OR rfidLocationId = @location )");
            }
            sb.Append(" ORDER BY RFID_AUDIT_ID DESC");


            return sb.ToString();

        }
        private void throwEntityException(DbEntityValidationException e)
        {
            foreach (var eve in e.EntityValidationErrors)
            {
                Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                foreach (var ve in eve.ValidationErrors)
                {
                    Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                        ve.PropertyName, ve.ErrorMessage);
                }
            }
        }
        private List<object> getInquiryTableSqlParams(string seriesNo)
        {
            List<object> sqlParams = new List<object>();
            if (seriesNo != null)
            {
                sqlParams.Add(new SqlParameter("@seriesNo", seriesNo));
            }
            return sqlParams;
        }
        private DateTime checkWhetherDateIsOverflow(DateTime dt)
        {
            if (dt < SqlDateTime.MinValue.Value)
            {
                return SqlDateTime.MinValue.Value;
            }
            else if (dt > SqlDateTime.MaxValue.Value)
            {
                return SqlDateTime.MaxValue.Value;
            }
            return dt;
        }

        private List<object> getReportTableSqlParams(RfidReportRequestModel report)
        {

            List<object> sqlParams = new List<object>();
            report.dateFrom = checkWhetherDateIsOverflow(report.dateFrom.Date);
            report.dateTo = checkWhetherDateIsOverflow(report.dateTo.Date);
            if (report.dateTo == SqlDateTime.MinValue.Value)
            {
                report.dateTo = SqlDateTime.MaxValue.Value;
            }

            sqlParams.Add(new SqlParameter("@dateFrom", report.dateFrom.ToString("yyyy/MM/dd")));
            sqlParams.Add(new SqlParameter("@dateTo", report.dateTo.ToString("yyyy/MM/dd")));

            if (!String.IsNullOrEmpty(report.location))
            {
                sqlParams.Add(new SqlParameter("@location", report.location));
            }
            if (!String.IsNullOrEmpty(report.rfid))
            {
                sqlParams.Add(new SqlParameter("@rfid", report.rfid));
            }
            if (!String.IsNullOrEmpty(report.status))
            {
                sqlParams.Add(new SqlParameter("@status", report.status));
            }

            // currently not using it
            //if (report.reportId != null)
            //{
            //    sqlParams.Add(new SqlParameter("@reportId", report.reportId));
            //}
            return sqlParams;
        }
        public byte[] RFIDReportDataToExcel(RfidReportRequestModel reportRequest)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("RfidAuditReport");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Rfid Id";
                worksheet.Cell(currentRow, 2).Value = "Series No";
                worksheet.Cell(currentRow, 3).Value = "Part No";
                worksheet.Cell(currentRow, 4).Value = "Model No";
                worksheet.Cell(currentRow, 5).Value = "Model";
                worksheet.Cell(currentRow, 6).Value = "StorageCode";
                worksheet.Cell(currentRow, 7).Value = "Manufacturer";
                worksheet.Cell(currentRow, 8).Value = "Audited Status";
                worksheet.Cell(currentRow, 9).Value = "Audited Date";
                worksheet.Cell(currentRow, 10).Value = "Audited By";

                RfidResponseModel reportResponse = getReportTable(reportRequest);
                foreach (var report in reportResponse.rfidReportTables)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = report.rfidId;
                    worksheet.Cell(currentRow, 2).Value = report.seriesNo;
                    worksheet.Cell(currentRow, 3).Value = report.partNo;
                    worksheet.Cell(currentRow, 4).Value = report.modelNo;
                    worksheet.Cell(currentRow, 5).Value = report.model;
                    worksheet.Cell(currentRow, 6).Value = report.storageCode;
                    worksheet.Cell(currentRow, 7).Value = report.manufacturer;
                    worksheet.Cell(currentRow, 8).Value = report.status;
                    worksheet.Cell(currentRow, 9).Value = report.createdDate;
                    worksheet.Cell(currentRow, 10).Value = report.createdBy;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return content;
                }

            }

        }


        public List<String> rfidCompares()
        {
            List<String> rfids =
                (from rfid in _TMS_ITEquiptContext.Registration_Asset_Managements
                 select rfid.RFID)
                 .Distinct()
                 .ToList();
            return rfids;
        }

        public string getInquiryListQueryString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("exec PSP_COMMON_LIST");
            sb.Append(" @Table,");
            sb.Append(" @TableID,");
            sb.Append(" @Search,");
            sb.Append(" @Value,");
            sb.Append(" @SortField,");
            sb.Append(" @Direction,");
            sb.Append(" @FrmRowno,");
            sb.Append(" @ToRowno,");
            sb.Append(" @Deleted");
            return sb.ToString();
        }

        public List<SqlParameter> getInquiryListParams(string Table, string TableID, string Search,
        string Value, string SortField, string Direction,
        string FrmRowno, string ToRowno, string Deleted)
        {


            List<SqlParameter> sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@Table", Table));
            sqlParams.Add(new SqlParameter("@TableID", TableID));
            sqlParams.Add(new SqlParameter("@Search", Search));
            sqlParams.Add(new SqlParameter("@Value", Value));
            sqlParams.Add(new SqlParameter("@SortField", SortField));
            sqlParams.Add(new SqlParameter("@Direction", Int32.Parse(Direction)));
            sqlParams.Add(new SqlParameter("@FrmRowno", Int32.Parse(FrmRowno)));
            sqlParams.Add(new SqlParameter("@ToRowno", Int32.Parse(ToRowno)));
            sqlParams.Add(new SqlParameter("@Deleted", Int32.Parse(Deleted)));


            return sqlParams;
        }
        public List<InquirySparePart> getInquiryList(string Table, string TableID, string Search,
        string Value, string SortField, string Direction,
        string FrmRowno, string ToRowno, string Deleted)
        {
            string queryString = getInquiryListQueryString();
            List<SqlParameter> param = getInquiryListParams(Table, TableID, Search, Value, SortField, Direction, FrmRowno, ToRowno, Deleted);

            List<InquirySparePart> result = _rfidAuditCtx.Database.SqlQuery<InquirySparePart>
                (queryString, param.ToArray())
                .ToList();

            return result;
        }
        public bool checkWhetherItemInSameLocation(string rfid, string location)
        {
            var locationId = getLocationId(location);
            var sameLocation = (from a in _rfidAuditCtx.MM_SPARE_PARTs
                                where a.RFID_TAG_ID == rfid
                                && a.STORAGE_ID == locationId
                                select 1).Count();
            if (sameLocation > 0)
            {
                return true;
            }
            return false;


        }

        public int getLocationIdByReader(string location)
        {
            var locationId =
             (from a in _rfidAuditCtx.MM_SPARE_PART_READERs
              where a.STORAGE_CODE == location
              select a.READER_ID
              ).FirstOrDefault();

            return locationId;
        }

        public int getLocationId(string location)
        {
            var locationId =
                (from a in _rfidAuditCtx.PVIEW_MM_SP_STORAGEs
                 where a.STORAGE_CODE == location
                 select a.STORAGE_ID
                 ).FirstOrDefault();

            return locationId;
        }


        public SparePart getSparePartData(string rfid)
        {
            var sparePartData =
                (from a in _rfidAuditCtx.PVIEW_MM_SPARE_PARTs
                 where a.RFID_TAG_ID == rfid
                 select new SparePart()
                 {
                     SPARE_PART_ID = a.SPARE_PART_ID,
                     SPARE_PART_MODEL_ID_1 = a.SPARE_PART_MODEL_ID_1,
                     SPARE_PART_MACHINE_ID_1 = a.SPARE_PART_MACHINE_ID_1,
                     RFID_TYPE_ID_1 = a.RFID_TYPE_ID_1,
                     MANUFACTURER_ID_1 = a.MANUFACTURER_ID_1,
                     SPARE_PART_MODEL_ID = a.SPARE_PART_MODEL_ID,
                     SERIES_NO = a.SERIES_NO,
                     PART_NO = a.PART_NO,
                     SPARE_PART_MACHINE_ID = a.SPARE_PART_MACHINE_ID,
                     RFID_TYPE_ID = a.RFID_TYPE_ID,
                     RFID_TAG_ID = a.RFID_TAG_ID,
                     READER_ID = a.READER_ID,
                     STORAGE_ID = a.STORAGE_ID,
                     STORAGE_DESCRIPTION1 = a.STORAGE_DESCRIPTION1,
                     STORAGE_DESCRIPTION2 = a.STORAGE_DESCRIPTION2,
                     MANUFACTURER_ID = a.MANUFACTURER_ID,
                     STATUS = a.STATUS,
                     REMARKS = a.REMARKS,
                     RECORD_TYPE = a.RECORD_TYPE,
                     CREATED_BY = a.CREATED_BY,
                     CREATED_DATE = a.CREATED_DATE,
                     CREATED_LOC = a.CREATED_LOC,
                     UPDATED_BY = a.UPDATED_BY,
                     UPDATED_DATE = a.UPDATED_DATE,
                     UPDATED_LOC = a.UPDATED_LOC,
                     REC_TYPE_DESC = a.REC_TYPE_DESC,
                     RFID_TAG_ID_2 = a.RFID_TAG_ID_2
                 }).FirstOrDefault();

            return sparePartData;

        }
    }

}
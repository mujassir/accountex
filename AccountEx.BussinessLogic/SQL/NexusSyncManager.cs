using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Nexus;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.BussinessLogic.SQL
{
    /// <summary>
    /// Responsible for all operations related to NEXUS-AMEX databases syncing
    /// </summary>
    public static class NexusSyncManager
    {

        /// <summary>
        /// Updates LastSyncRecordId in "AmexSyncTables" table in Nexus DB
        /// Method is used by Windows service
        /// </summary>
        /// <param name="taleName"></param>
        /// <param name="lastSyncRecordId"></param>
        public static void UpdateAmexSyncTableId(string taleName, int lastSyncRecordId)
        {
            string updateSPName = "UpdateAmexSyncTableId";

            SqlParameter[] parameters = 
            {
                new SqlParameter("@TableName", SqlDbType.VarChar, 50) { Value = taleName },
                new SqlParameter("@LastSyncRecordId", SqlDbType.VarChar, 50) { Value = lastSyncRecordId },
            };

            SqlHelper.ExecuteNonQuery(Connection.GetNexusConnectionString(), updateSPName, CommandType.StoredProcedure, parameters);
        }

        /// <summary>
        /// Executes "GetDataFromAmexSyncTables" SP on Nexus DB to get data for all tables to sync
        /// Method is used by Windows service
        /// </summary>
        /// <returns>DataSet containing "dbo.AmexSyncTables" on zero index all data of all other tables those were configured in "dbo.AmexSyncTables" </returns>
        public static DataSet GetSyncTablesAndData()
        {
            string syncSPName = ConfigurationReader.GetConfigKeyValue("SyncSPName", "dbo.GetDataFromAmexSyncTables");

            return SqlHelper.GetDataSetFromSP(Connection.GetNexusConnectionString(), syncSPName);
        }

        /// <summary>
        /// Converts json to its respective type and inserts into its specific table based on param: tableName in Amex DB
        /// Method is used by NexusSyncApiController
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="json"></param>
        public static void InsertRecords(string tableName, string json)
        {
            switch (tableName)
            {
                case "City":
                    AddCity(json);
                    break;
                case "Country":
                    AddCountry(json);
                    break;
                case "Case":
                    AddCase(json);
                    break;
                case "CaseDetail":
                    AddCaseDetail(json);
                    break;
                case "Consultant":
                    AddConsultant(json);
                    break;
                case "Patient":
                    AddPatient(json);
                    break;
                case "RateType":
                    AddRateType(json);
                    break;
                case "Reference":
                    AddReference(json);
                    break;
                case "TestDepartment":
                    AddTestDepartment(json);
                    break;
                case "TestGroup":
                    AddTestGroup(json);
                    break;
                case "TestNormalValues":
                    AddTestNormalValues(json);
                    break;
                case "Test":
                    AddTest(json);
                    break;
                case "TestDoctor":
                    AddTestDoctor(json);
                    break;
                case "TestSpecimen":
                    AddTestSpecimen(json);
                    break;
                default:
                     throw new OwnException("Specified table is not implemented.");

            }



        }

        #region Private methods
        
        static void AddCity(string json)
        {
            var records = JsonConvert.DeserializeObject<List<Nexus_City>>(json);
            var repo = new GenericRepository<Nexus_City>();

            var ids = records.Select(q => q.CityCode).ToList();
            var duplicateIds = repo.GetAll(p => ids.Contains(p.CityCode)).Select(p => p.CityCode).ToList();
            records = records.Where(p => !duplicateIds.Contains(p.CityCode)).ToList();

            repo.Add(records);
            repo.SaveChanges();
        }

        static void AddCountry(string json)
        {
            var records = JsonConvert.DeserializeObject<List<Nexus_Country>>(json);
            var repo = new GenericRepository<Nexus_Country>();

            var ids = records.Select(q => q.CountryCode).ToList();
            var duplicateIds = repo.GetAll(p => ids.Contains(p.CountryCode)).Select(p => p.CountryCode).ToList();
            records = records.Where(p => !duplicateIds.Contains(p.CountryCode)).ToList();

            repo.Add(records);
            repo.SaveChanges();
        }

        static void AddCase(string json)
        {
            var records = JsonConvert.DeserializeObject<List<Nexus_Case>>(json);
            var repo = new GenericRepository<Nexus_Case>();

            var ids = records.Select(p => p.ID).ToList();
            var duplicateIds = repo.GetAll(p => ids.Contains(p.ID)).Select(p => p.ID).ToList();
            records = records.Where(p => !duplicateIds.Contains(p.ID)).ToList();

            repo.Add(records);
            repo.SaveChanges();
        }
        static void AddCaseDetail(string json)
        {
            var records = JsonConvert.DeserializeObject<List<Nexus_CaseDetail>>(json);
            var repo = new GenericRepository<Nexus_CaseDetail>();

            var ids = records.Select(p => p.ID).ToList();
            var duplicateIds = repo.GetAll(p => ids.Contains(p.ID)).Select(p => p.ID).ToList();
            records = records.Where(p => !duplicateIds.Contains(p.ID)).ToList();

            repo.Add(records);
            repo.SaveChanges();
        }
        static void AddConsultant(string json)
        {
            var records = JsonConvert.DeserializeObject<List<Nexus_Consultant>>(json);
            var repo = new GenericRepository<Nexus_Consultant>();

            var ids = records.Select(p => p.ID).ToList();
            var duplicateIds = repo.GetAll(p => ids.Contains(p.ID)).Select(p => p.ID).ToList();
            records = records.Where(p => !duplicateIds.Contains(p.ID)).ToList();

            repo.Add(records);
            repo.SaveChanges();
        }
        static void AddPatient(string json)
        {
            var records = JsonConvert.DeserializeObject<List<Nexus_Patient>>(json);
            var repo = new GenericRepository<Nexus_Patient>();

            var ids = records.Select(p => p.ID).ToList();
            var duplicateIds = repo.GetAll(p => ids.Contains(p.ID)).Select(p => p.ID).ToList();
            records = records.Where(p => !duplicateIds.Contains(p.ID)).ToList();

            repo.Add(records);
            repo.SaveChanges();
        }
        static void AddRateType(string json)
        {
            var records = JsonConvert.DeserializeObject<List<Nexus_RateType>>(json);
            var repo = new GenericRepository<Nexus_RateType>();

            var ids = records.Select(p => p.ID).ToList();
            var duplicateIds = repo.GetAll(p => ids.Contains(p.ID)).Select(p => p.ID).ToList();
            records = records.Where(p => !duplicateIds.Contains(p.ID)).ToList();

            repo.Add(records);
            repo.SaveChanges();
        }
        static void AddReference(string json)
        {
            var records = JsonConvert.DeserializeObject<List<Nexus_Reference>>(json);
            var repo = new GenericRepository<Nexus_Reference>();

            var ids = records.Select(p => p.ID).ToList();
            var duplicateIds = repo.GetAll(p => ids.Contains(p.ID)).Select(p => p.ID).ToList();
            records = records.Where(p => !duplicateIds.Contains(p.ID)).ToList();

            repo.Add(records);
            repo.SaveChanges();
        }
        static void AddTestDepartment(string json)
        {
            var records = JsonConvert.DeserializeObject<List<Nexus_TestDepartment>>(json);
            var repo = new GenericRepository<Nexus_TestDepartment>();

            var ids = records.Select(p => p.ID).ToList();
            var duplicateIds = repo.GetAll(p => ids.Contains(p.ID)).Select(p => p.ID).ToList();
            records = records.Where(p => !duplicateIds.Contains(p.ID)).ToList();

            repo.Add(records);
            repo.SaveChanges();
        }
        static void AddTestGroup(string json)
        {
            var records = JsonConvert.DeserializeObject<List<Nexus_TestGroup>>(json);
            var repo = new GenericRepository<Nexus_TestGroup>();

            var ids = records.Select(p => p.ID).ToList();
            var duplicateIds = repo.GetAll(p => ids.Contains(p.ID)).Select(p => p.ID).ToList();
            records = records.Where(p => !duplicateIds.Contains(p.ID)).ToList();

            repo.Add(records);
            repo.SaveChanges();
        }
        static void AddTestNormalValues(string json)
        {
            var records = JsonConvert.DeserializeObject<List<Nexus_TestNormalValues>>(json);
            var repo = new GenericRepository<Nexus_TestNormalValues>();

            var ids = records.Select(p => p.ID).ToList();
            var duplicateIds = repo.GetAll(p => ids.Contains(p.ID)).Select(p => p.ID).ToList();
            records = records.Where(p => !duplicateIds.Contains(p.ID)).ToList();

            repo.Add(records);
            repo.SaveChanges();
        }
        static void AddTest(string json)
        {
            var records = JsonConvert.DeserializeObject<List<Nexus_Test>>(json);
            var repo = new GenericRepository<Nexus_Test>();

            var ids = records.Select(p => p.ID).ToList();
            var duplicateIds = repo.GetAll(p => ids.Contains(p.ID)).Select(p => p.ID).ToList();
            records = records.Where(p => !duplicateIds.Contains(p.ID)).ToList();

            repo.Add(records);
            repo.SaveChanges();
        }
        static void AddTestDoctor(string json)
        {
            var records = JsonConvert.DeserializeObject<List<Nexus_TestDoctor>>(json);
            var repo = new GenericRepository<Nexus_TestDoctor>();

            var ids = records.Select(p => p.ID).ToList();
            var duplicateIds = repo.GetAll(p => ids.Contains(p.ID)).Select(p => p.ID).ToList();
            records = records.Where(p => !duplicateIds.Contains(p.ID)).ToList();

            repo.Add(records);
            repo.SaveChanges();
        }
        static void AddTestSpecimen(string json)
        {
            var records = JsonConvert.DeserializeObject<List<Nexus_TestSpecimen>>(json);
            var repo = new GenericRepository<Nexus_TestSpecimen>();

            var ids = records.Select(p => p.ID).ToList();
            var duplicateIds = repo.GetAll(p => ids.Contains(p.ID)).Select(p => p.ID).ToList();
            records = records.Where(p => !duplicateIds.Contains(p.ID)).ToList();

            repo.Add(records);
            repo.SaveChanges();
        }

        #endregion
        
    }
}

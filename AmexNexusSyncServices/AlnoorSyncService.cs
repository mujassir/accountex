using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AccountEx.BussinessLogic.SQL;
using System.Net.Http;
using Newtonsoft.Json;
using AccountEx.BussinessLogic;

namespace AmexNexusSyncServices
{
    partial class AlnoorSyncService : ServiceBase
    {

        Timer dataSyncTimer;


        public AlnoorSyncService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Logger.ApplicationName = "NotificationService";

            Logger.Log("Service started");

            InitTimers();


        }



        private void InitTimers()
        {

            try
            {
                Logger.Log("Method enter: InitTimers");


                dataSyncTimer = new Timer(ConfigurationReader.GetConfigKeyValue("Interval", 5 * 1000));
                dataSyncTimer.Enabled = true;
                dataSyncTimer.Elapsed += dataSyncTimer_Elapsed;
                dataSyncTimer.Start();


                Logger.Log("Method exit: InitTimers");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                Logger.Log("Method exit: InitTimers");

            }


        }


        void dataSyncTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Logger.Log("Method enter: dataSyncTimer_Elapsed");

                dataSyncTimer.Stop();

                SyncData();

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            finally
            {
                dataSyncTimer.Start();

                Logger.Log("Method exit: dataSyncTimer_Elapsed");
            }
        }


        public void SyncData()
        {
            Logger.Log("Method enter: SyncData");

            DataSet ds = NexusSyncManager.GetSyncTablesAndData();

            var syncTableNames = ds.Tables[0];
            for (int index = 0; index < syncTableNames.Rows.Count; index++)
            {
                try
                {
                    var tableName = syncTableNames.Rows[index]["TableName"].ToString();
                    var pkFieldName = syncTableNames.Rows[index]["PKFieldName"].ToString();
                    var dt = ds.Tables[index + 1];
                    if (dt.Rows.Count == 0)
                    {
                        //Logger.Log("Skipping table " + tableName +", it doesn't have records to sync");
                        continue;
                    }
                    
                    //var json = UtilityFunctionManager.DataTableToJson(dt);
                    var json = JsonConvert.SerializeObject(dt);

                    var apiResponse = PostData(tableName, json);

                    if (apiResponse.Success)
                    {
                        int lastSyncId = Convert.ToInt32(dt.Rows[dt.Rows.Count - 1][pkFieldName]);
                        NexusSyncManager.UpdateAmexSyncTableId(tableName, lastSyncId);
                        Logger.Log("Table: " + tableName + " synced successfully till Id: " + lastSyncId);
                    }
                    else
                    {
                        Logger.Log("ApiResponse error: " + apiResponse.Error);

                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
            Logger.Log("Method exit: SyncData");
        }


        public ApiResponse PostData(string tableName, string json)
        {
            Logger.Log("Method enter: PostData");

            string apiUrl = ConfigurationReader.GetConfigKeyValue("NexusSyncApiUrl", "https://absorberp.com/api/NexusSyncApi");
            string username = ConfigurationReader.GetConfigKeyValue("Username", "kashifkr");
            string password = ConfigurationReader.GetConfigKeyValue("Password", "KashifKR!@#");

            List<KeyValuePair<string, string>> inputFilds = new List<KeyValuePair<string, string>>();
            inputFilds.Add(new KeyValuePair<string, string>("Username", username));
            inputFilds.Add(new KeyValuePair<string, string>("Password", password));
            inputFilds.Add(new KeyValuePair<string, string>("TableName", tableName));
            inputFilds.Add(new KeyValuePair<string, string>("Json", json));
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    HttpContent content = new FormUrlEncodedContent(inputFilds);

                    HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var data = response.Content.ReadAsStringAsync().Result;



                        ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(data);
                        if (apiResponse.Success)
                        {
                            Logger.Log("HttpRequest ApiResponse successful");
                        }
                        else
                        {
                            Logger.Log("HttpRequest ApiResponse failed: " + JsonConvert.SerializeObject(apiResponse));
                            Logger.Log("HttpRequest ApiResponse failed TableName: " + tableName);
                            Logger.Log("HttpRequest ApiResponse failed json: " + json);
                        }
                        Logger.Log("Method exit: PostData");

                        return apiResponse;
                    }

                    Logger.Log("HttpRequest failed TableName: " + tableName);
                    Logger.Log("HttpRequest failed json: " + json);
                    Logger.Log("HttpRequest failed: " + JsonConvert.SerializeObject(response));
                    Logger.Log("Method exit: PostData");
                    return null;

                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }
        }



        protected override void OnStop()
        {
            Logger.Log("Service stopped");
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }



    }
}

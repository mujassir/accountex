using AccountEx.BussinessLogic;
using AccountEx.BussinessLogic.SQL;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmexNexusSyncServices
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var service = new AlnoorSyncService();

            service.SyncData();

        }


        public ApiResponse PostData(string tableName, string json)
        {
         
            string apiUrl = ConfigurationReader.GetConfigKeyValue("NexusSyncApiUrl", "http://localhost:27099/api/NexusSyncApi");

            List<KeyValuePair<string, string>> inputFilds = new List<KeyValuePair<string, string>>();
            inputFilds.Add(new KeyValuePair<string, string>("TableName", tableName));
            inputFilds.Add(new KeyValuePair<string, string>("Json", "kashifkr"));

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpContent content = new FormUrlEncodedContent(inputFilds);
                
                HttpResponseMessage response = client.PostAsync(apiUrl,content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;

                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(data);
                    return apiResponse;
                }
                return null;


            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }
    }
}

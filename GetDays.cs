using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.VisualBasic;


namespace MCT.Functions
{
    public static class GetDays
    {
        [FunctionName("GetDays")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "days")] HttpRequest req,
            ILogger log)
        {
            try
            {
                string connectionString = Environment.GetEnvironmentVariable("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                await sqlConnection.OpenAsync();

                SqlCommand sqlCommand = new SqlCommand("SELECT DISTINCT DagVanDeWeek FROM VisitorData", sqlConnection);

                var reader = sqlCommand.ExecuteReader();
                List<string> days = new List<string>();
                while (reader.Read())
                {
                    var dag = reader["DagVanDeWeek"].ToString();
                    days.Add(dag);
                }

                await sqlConnection.CloseAsync();

                return new OkObjectResult(days);
            }
            catch (Exception ex)
            {
                // log.LogError(ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}

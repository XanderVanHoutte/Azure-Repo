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
using Howest.MCT.Functions.Models;

namespace Howest.MCT.Functions
{
    public static class GetVisiters
    {
        [FunctionName("GetVisiters")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Visitors/{day}")] HttpRequest req, string day,
            ILogger log)
        {
            try
            {
                string connectionString = Environment.GetEnvironmentVariable("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                await sqlConnection.OpenAsync();

                SqlCommand sqlCommand = new SqlCommand("SELECT * FROM VisitorData WHERE DagVanDeWeek = @day", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@day", day);

                SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync();

                List<Visit> visits = new List<Visit>();
                while (sqlDataReader.Read())
                {
                    int tijdstip = int.Parse(sqlDataReader["TijdstipDag"].ToString());
                    int aantalBezoekers = int.Parse(sqlDataReader["AantalBezoekers"].ToString());
                    string dagVanDeWeek = sqlDataReader["DagVanDeWeek"].ToString();
                    visits.Add(new Visit(tijdstip, aantalBezoekers, dagVanDeWeek));
                }


                return new OkObjectResult(visits);
            }
            catch (Exception ex)
            {
                // log.LogError(ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}

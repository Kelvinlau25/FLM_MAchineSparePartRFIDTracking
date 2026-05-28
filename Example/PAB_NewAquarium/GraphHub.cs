using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using PAB_NewAquarium.DAL;

namespace PAB_NewAquarium
{
    public class GraphHub : SignalRHub
    {
        //private readonly string connectionString = "Server=10.201.1.5,49818;Initial Catalog=pab_ACL;Persist Security Info=False;User ID=pabACL;Password=pabACL9#;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True;Connection Timeout=60;";
        public async System.Threading.Tasks.Task Send()
        {
            List<Graph> users = await GetUsersFromDatabase();
            // Call the addNewMessageToPage method to update clients.
            Clients.All.broadcastMessage("ReceiveUsers", users);
        }

        private async System.Threading.Tasks.Task<List<Graph>> GetUsersFromDatabase()
        {
            List<Graph> graphs = new List<Graph>();

            Database databaseConnection = new Database();
            string constr = await databaseConnection.GetConnectionStringAsync(false, "DEV_MVC");

            using (SqlConnection connection = new SqlConnection(constr))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("PSP_GRAPH_HUB1", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 0;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Graph user = new Graph
                            {
                                count = (string)reader["GRAPH_VAL"],
                                // Add other properties as needed
                            };
                            graphs.Add(user);
                        }
                    }
                }
            }
            return graphs;
        }

        public static void SendDBChange()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<GraphHub>();
            context.Clients.All.databaseChange();
        }
    }
}

public class Graph
{
    public string count { get; set; }
    // Add other properties as needed
}

public class Chart
{
    public DateTime axisX { get; set; }
    public string axisY { get; set; }
    // Add other properties as needed
}
using GridViewWebApp.Models;
using Microsoft.Data.SqlClient;
using System.Text;

namespace GridViewWebApp.Services
{
    public class StatusServices : IStatusServices
    {
        public string ConnString { get; set; }

        public IConfiguration _configuration;
        public SqlConnection con;

        public StatusServices(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnString = _configuration.GetConnectionString("DBConnection");
        }

        public List<ListOfStatus> GetStatuses()
        {
            List<ListOfStatus> mylist = new List<ListOfStatus>();

            using (con = new SqlConnection(ConnString))
            {
                con.Open();
                var cmd = new SqlCommand("select * from ListOfStatus", con);
                SqlDataReader sda = cmd.ExecuteReader();
                while (sda.Read())
                {
                    var status = new ListOfStatus();
                    status.StatusID = Convert.ToInt32(sda["StatusID"]);
                    status.StatusName = (string)sda["StatusName"];
                    mylist.Add(status);
                }
            }
            return mylist.ToList();
        }
    }
}

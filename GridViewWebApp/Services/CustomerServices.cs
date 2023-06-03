using GridViewWebApp.Models;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace GridViewWebApp.Services
{
    public class CustomerServices : ICustomerServices
    {
        public string ConnString { get; set; }

        public IConfiguration _configuration;
        public SqlConnection con;

        public CustomerServices(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnString = _configuration.GetConnectionString("DBConnection");
        }
        public int GetCustomersCount(string FName, string LName, int? StatusID)
        {
            int count = 0;
            StringBuilder stringBuilder = new StringBuilder("select Count(*) from [dbo].[Customers_View] where 1=1 ");
            if (!string.IsNullOrEmpty(FName))
            {
                stringBuilder.Append(" and FName like '%" + FName + "%' ");
            }
            if (!string.IsNullOrEmpty(LName))
            {
                stringBuilder.Append(" and LName like '%" + LName + "%' ");
            }
            if (StatusID != null && StatusID != 0)
            {
                stringBuilder.Append(" and StatusID=" + StatusID);
            }

            using (con = new SqlConnection(ConnString))
            {
                con.Open();
                var cmd = new SqlCommand(stringBuilder.ToString(), con);
                SqlDataReader sda = cmd.ExecuteReader();
                sda.Read();
                if (sda.HasRows)
                {
                    count = Convert.ToInt32(sda.GetValue(0));
                }
            }
            return count;
        }

        public List<Customers_View> GetCustomers(string FName, string LName, int? StatusID, int sortBy, bool Asc = true, int page = 1, int itemsInPage=20)
        {
            StringBuilder stringBuilder = new StringBuilder("select * from [dbo].[Customers_View] where 1=1 ");
            if (!string.IsNullOrEmpty(FName))
            {
                stringBuilder.Append(" and FName like '%" + FName + "%' ");
            }
            if (!string.IsNullOrEmpty(LName))
            {
                stringBuilder.Append(" and LName like '%" + LName + "%' ");
            }
            if (StatusID != null && StatusID!=0)
            {
                stringBuilder.Append(" and StatusID=" + StatusID);
            }

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    if (Asc)
                        stringBuilder.Append(" order by FName ");
                    else
                        stringBuilder.Append(" order by FName desc ");
                    break;
                case 2:
                    if (Asc)
                        stringBuilder.Append(" order by LName ");
                    else
                        stringBuilder.Append(" order by LName desc ");
                    break;
                case 3:
                    if (Asc)
                        stringBuilder.Append(" order by StatusName ");
                    else
                        stringBuilder.Append(" order by StatusName desc ");
                    break;
                default:
                    stringBuilder.Append(" order by CustomerID ");
                    break;
            }
            #endregion

            stringBuilder.Append(" OFFSET " + (page-1) * itemsInPage + " ROWS ");
            stringBuilder.Append(" FETCH NEXT " + itemsInPage + " ROWS ONLY; ");


            List<Customers_View> mylist = new List<Customers_View>();

            using (con = new SqlConnection(ConnString))
            {
                con.Open();
                var cmd = new SqlCommand(stringBuilder.ToString(), con);
                SqlDataReader sda = cmd.ExecuteReader();
                while (sda.Read())
                {
                    var customer = new Customers_View();
                    customer.CustomerID = Convert.ToInt32(sda["CustomerID"]);
                    customer.FName = (string)sda["FName"];
                    customer.LName = (string)sda["LName"];
                    customer.Picture = (string)sda["Picture"];
                    customer.StatusID = Convert.ToInt32(sda["StatusID"]);
                    customer.StatusName= (string)sda["StatusName"];

                    mylist.Add(customer);
                }
            }
            return mylist.ToList();
        }

        public void UpdateCustomer(Customer customer)
        {
            using(SqlConnection con = new SqlConnection(ConnString))
            {
                SqlCommand cmd = new SqlCommand("Update_Cutomer", con);
                cmd.CommandTimeout = 60;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("CustomerID", customer.CustomerID);
                cmd.Parameters.AddWithValue("@FName", customer.FName);
                cmd.Parameters.AddWithValue("@LName", customer.LName);
                cmd.Parameters.AddWithValue("@StatusID", customer.StatusID);
                cmd.Parameters.AddWithValue("@Picture", customer.Picture);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void InsertCustomer(Customer customer)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                SqlCommand cmd = new SqlCommand("InsertInto_Cutomers", con);
                cmd.CommandTimeout = 60;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@FName", customer.FName);
                cmd.Parameters.AddWithValue("@LName", customer.LName);
                cmd.Parameters.AddWithValue("@StatusID", customer.StatusID);
                cmd.Parameters.AddWithValue("@Picture", customer.Picture);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void DeleteCustomer(int customerID)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                SqlCommand cmd = new SqlCommand("Delete_Cutomer", con);
                cmd.CommandTimeout = 60;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("CustomerID", customerID);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

    }
}

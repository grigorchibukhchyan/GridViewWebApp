using GridViewWebApp.Models;

namespace GridViewWebApp.Services
{
    public interface ICustomerServices
    {
        int GetCustomersCount(string FName, string LName, int? StatusID);
        List<Customers_View> GetCustomers(string FName, string LName, int? StatusID, int sortBy, bool Asc = true, int page = 1, int itemsInPage = 20);

        void InsertCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        void DeleteCustomer(int customerID);
    }
}

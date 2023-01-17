using System.Collections.Generic;
using System.Threading.Tasks;
using Rabobank.TechnicalTest.GCOB.Domain;

namespace Rabobank.TechnicalTest.GCOB.Services
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerAsync(int id);
        Task<int> AddNewCustomer(Customer customer);
    }
}
using System.Threading.Tasks;
using Rabobank.TechnicalTest.GCOB.Domain;

namespace Rabobank.TechnicalTest.GCOB.Services
{
    public interface IAddressService
    {
        Task<Address> GetAddressAsync(int id);
        Task<int> AddNewAddress(Address address);
    }
}
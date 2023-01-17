using System.Threading.Tasks;
using Rabobank.TechnicalTest.GCOB.Domain;

namespace Rabobank.TechnicalTest.GCOB.Services
{
    public interface ICountryService
    {
        Task<Country> GetCountryAsync(int id);
        Task<int> GetCountryId(string name);
    }
}
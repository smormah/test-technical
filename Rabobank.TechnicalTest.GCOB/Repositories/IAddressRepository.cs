﻿using Rabobank.TechnicalTest.GCOB.Dtos;
using System.Threading.Tasks;

namespace Rabobank.TechnicalTest.GCOB.Repositories
{
    public interface IAddressRepository
    {
        Task<int> GenerateIdentityAsync();
        Task InsertAsync(AddressDto address);
        Task UpdateAsync(AddressDto address);
        Task<AddressDto> GetAsync(int addressId);
    }
}

using Sabio.Models.Domain.Addresses;
using Sabio.Models.Requests.Addresses;
using System.Collections.Generic;

namespace Sabio.Services
{
    public interface IAddressService
    {
        int Add(AddressAddRequest request);
        void Delete(int Id);
        Address Get(int id);
        List<Address> GetRandomAddresses();
        void Update(AddressUpdateRequest Model);
    }
}
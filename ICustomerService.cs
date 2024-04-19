using Sabio.Models;
using Sabio.Models.Domain.Customers;
using Sabio.Models.Requests.Customers;
using System.Collections.Generic;

namespace Sabio.Services
{
    public interface ICustomerService
    {
        int Add(CustomerAddRequest request);
        void Delete(int Id);
        List<Customer> GetAll();
        Customer GetById(int id);
        Paged<Customer> Pagination(int pageIndex, int pageSize);
        void Update(CustomerUpdateRequest request);
    }
}
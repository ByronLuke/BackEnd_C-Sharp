using Sabio.Models;
using Sabio.Models.Domain;
using System.Collections.Generic;

namespace Sabio.Services
{
    public interface ICustomersOrdersV3Service
    {
        List<CustomersOrdersV3> GetAll();
        CustomersOrdersV3 GetById(int id);
        Paged<CustomersOrdersV3> Pagination(int pageIndex, int pageSize);
    }
}
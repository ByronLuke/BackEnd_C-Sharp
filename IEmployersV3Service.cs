using Sabio.Models;
using Sabio.Models.Domain;
using System.Collections.Generic;

namespace Sabio.Services
{
    public interface IEmployersV3Service
    {
        List<EmployersV3> GetAll();
        EmployersV3 GetById(int id);
        Paged<EmployersV3> Pagination(int pageIndex, int pageSize);
    }
}
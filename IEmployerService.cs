using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Requests;
using System.Collections.Generic;

namespace Sabio.Services
{
    public interface IEmployerService
    {
        int Add(EmployerAddRequest request);
        void Delete(int Id);
        List<Employer> GetAll();
        Employer GetById(int id);
        Paged<Employer> Pagination(int pageIndex, int pageSize);
        void Update(EmployerUpdateRequest updateRequest);
    }
}
using Sabio.Models;
using Sabio.Models.CodingChallenge;
using Sabio.Models.Requests;

namespace Sabio.Services
{
    public interface ICourseService
    {
        int Add(CourseAddRequest request);

        Course GetById(int id);

        void DeleteStudent(int id);

        void Update(CourseUpdateRequest updateRequest);

        Paged<Course> Pagination(int pageIndex, int pageSize);
    }
}
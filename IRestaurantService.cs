using Sabio.Models.Requests;

namespace Sabio.Services
{
    public interface IRestaurantService
    {
        int Add(RestaurantAddRequest request);
    }
}
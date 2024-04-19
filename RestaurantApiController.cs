using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Sabio.Models.Requests;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/restaurants")]
    [ApiController]
    public class RestaurantApiController : BaseApiController
    {
       private IRestaurantService _restaurantService = null;
       private IAuthenticationService<int> _authService = null;

       public RestaurantApiController(IRestaurantService restaurantService, ILogger<RestaurantApiController> logger, IAuthenticationService<int> authService) : base(logger)
        {
            _restaurantService = restaurantService;
            _authService = authService;
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Add(RestaurantAddRequest model)
        {
            ObjectResult result = null;

            try
            {
                int id = _restaurantService.Add(model);
                ItemResponse<int> response = new ItemResponse<int>() { Item = id };

                result = Created201(response);
            }
            catch(Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }
            return result;
        }
    }
}

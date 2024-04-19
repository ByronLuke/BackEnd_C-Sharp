using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sabio.Services.Interfaces;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ConcertApiController : ControllerBase
    {
        private IConcertService _concertService=null;
        public ConcertApiController(IConcertService service)
        {
            _concertService = service;
        }

        // GET api/Converts
        [HttpGet("")]
        public ActionResult GetAll()
        {
            return Ok();
        }
    }
}
/*In the context of web development, particularly in MVC (Model-View-Controller) and similar patterns, a Controller is a component that handles user interaction, works with the model, and ultimately selects a view to render that displays UI. In essence, it acts as an interface between the Model and the View components to process all the business logic and incoming requests, manipulate data using the Model component and interact with the Views to render the final output.

In the context of ASP.NET, a Controller is a class that inherits from the `Controller` base class and contains methods that respond to HTTP requests. These methods are known as Action methods. 

1. `[Route("api/[controller]")]`: This is an attribute that specifies the route where this controller can be accessed. In this case, it's at "api/ConcertApi".

2. `[ApiController]`: This attribute indicates that this class is a controller that uses the ControllerBase class to provide HTTP responses.

3. `private IConcertService _concertService=null;`: This line declares a private field of type `IConcertService`. This field will hold a reference to the concert service, which is used to handle the logic related to concerts.

4. `public ConcertApiController(IConcertService service)`: This is the constructor of the `ConcertApiController` class. It takes an `IConcertService` as a parameter. This is an example of dependency injection, where the dependencies of a class are provided through its constructor.

5. `_concertService = service;`: This line assigns the provided `IConcertService` to the `_concertService` field */

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models.Domain.Customers;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System.Collections.Generic;
using System;
using Sabio.Models.Domain;
using Sabio.Models;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/v3/customersorders")]
    [ApiController]
    public class CustomersOrdersV3Controller : BaseApiController
    {
        private ICustomersOrdersV3Service _service = null;
        private IAuthenticationService<int> _authService = null;
    
    #region Linking to database
    public CustomersOrdersV3Controller(ICustomersOrdersV3Service service, ILogger<CustomerApiController> logger, IAuthenticationService<int> authService) : base(logger)
    {
        _service = service;
        _authService = authService;
    }
        #endregion

    #region GetAll
        //GET api/v3/customersorders Route Pattern
        [HttpGet("")]
        public ActionResult<ItemsResponse<CustomersOrdersV3>> GetAll()
        {
            int code = 200;
            BaseResponse response = null;

            List<CustomersOrdersV3> list = _service.GetAll();
            try
            {
                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found");
                }
                else
                {
                    response = new ItemsResponse<CustomersOrdersV3> { Items = list };
                }

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }
        #endregion

    #region GetById
        //GET api/v3/customersorders/{id:int} Route Pattern
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<CustomersOrdersV3>> GetById(int id)
        {

            int code = 200;
            BaseResponse response = null;

            try
            {
                CustomersOrdersV3 customer = _service.GetById(id);

                if (customer == null)
                {
                    code = 404;
                    response = new ErrorResponse("Customer not found");
                }
                else
                {
                    response = new ItemResponse<CustomersOrdersV3> { Item = customer };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }
        #endregion

    #region Paginate
        //GET api/v3/customersorders/paginate
        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<CustomersOrdersV3>>> Pagination(int pageIndex, int pageSize)
        {
            ActionResult result = null;
            try
            {
                Paged<CustomersOrdersV3> paged = _service.Pagination(pageIndex, pageSize);
                if (paged == null)
                {
                    result = NotFound404(new ErrorResponse("No records found"));
                }
                else
                {
                    ItemResponse<Paged<CustomersOrdersV3>> response = new ItemResponse<Paged<CustomersOrdersV3>>();
                    response.Item = paged;
                    result = Ok200(response);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.Message.ToString()));
            }
            return result;
        }
        #endregion

    }

}

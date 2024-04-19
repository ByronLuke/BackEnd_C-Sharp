using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.Extensions.Logging;
using Sabio.Models.Domain.Friends;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System.Collections.Generic;
using System;
using Sabio.Models.Domain.Customers;
using Sabio.Models.Requests.Friends;
using Sabio.Models;
using Sabio.Models.Requests.Customers;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomerApiController : BaseApiController
    {

        private ICustomerService _service = null;
        private IAuthenticationService<int> _authService = null;

        #region Linking to database
        public CustomerApiController(ICustomerService service, ILogger<CustomerApiController> logger, IAuthenticationService<int> authService) : base(logger)
        {
            _service = service;
            _authService = authService;
        }
        #endregion

        #region GetAll
        //GET api/customers Route Pattern
        [HttpGet("")]
        public ActionResult<ItemsResponse<Customer>> GetAll()
        {
            int code = 200;
            BaseResponse response = null;

            List<Customer> list = _service.GetAll();
            try
            {
                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found");
                }
                else
                {
                    response = new ItemsResponse<Customer> { Items = list };
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
        //GET api/customers/{id:int} Route Pattern
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<Customer>> GetById(int id)
        {

            int code = 200;
            BaseResponse response = null;

            try
            {
                Customer customer = _service.GetById(id);

                if (customer == null)
                {
                    code = 404;
                    response = new ErrorResponse("Customer not found");
                }
                else
                {
                    response = new ItemResponse<Customer> { Item = customer };
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

        #region Create
        //POST api/customers Route Pattern
        [HttpPost("")]
        public ActionResult<ItemResponse<int>> Add(CustomerAddRequest model)
        {
            ObjectResult result = null;
            int userId = _authService.GetCurrentUserId();

            try
            {
                int id = _service.Add(model);

                ItemResponse<int> response = new ItemResponse<int>() { Item = id };

                result = Created201(response);
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }
            return result;
        }
        #endregion

        #region Update
        //PUT api/customers/{id:int} Route Pattern
        [HttpPut("{id:int}")]
        public ActionResult<ItemResponse<int>> Update(CustomerUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;
            int userId = _authService.GetCurrentUserId();

            try
            {
                _service.Update(model);

                response = new SuccessResponse();
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
        
        #region Delete
        //DELETE api/customers/{id:int} Route Pattern
        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> Delete(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.Delete(id);

                response = new SuccessResponse();
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
        //GET api/customers/paginate
        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<Customer>>> Pagination(int pageIndex, int pageSize)
        {
            ActionResult result = null;
            try
            {
                Paged<Customer> paged = _service.Pagination(pageIndex, pageSize);
                if (paged == null)
                {
                    result = NotFound404(new ErrorResponse("No records found"));
                }
                else
                {
                    ItemResponse<Paged<Customer>> response = new ItemResponse<Paged<Customer>>();
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

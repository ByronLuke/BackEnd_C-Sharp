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
using Sabio.Models.Requests.Customers;
using Sabio.Models.Requests;
using Sabio.Models;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/employers")]
    [ApiController]
    public class EmployerApiController : BaseApiController
    {

        private IEmployerService _service = null;
        private IAuthenticationService<int> _authService = null;

        #region Linking to database
        public EmployerApiController(IEmployerService service, ILogger<EmployerApiController> logger, IAuthenticationService<int> authService) : base(logger)
        {
            _service = service;
            _authService = authService;
        }
        #endregion

        #region GetAll
        //GET api/employers Route Pattern
        [HttpGet("")]
        public ActionResult<ItemsResponse<Employer>> GetAll()
        {
            int code = 200;
            BaseResponse response = null;

            List<Employer> list = _service.GetAll();
            try
            {
                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found");
                }
                else
                {
                    response = new ItemsResponse<Employer> { Items = list };
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
        //GET api/employers/{id:int} Route Pattern
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<Employer>> GetById(int id)
        {

            int code = 200;
            BaseResponse response = null;

            try
            {
                Employer employer = _service.GetById(id);

                if (employer == null)
                {
                    code = 404;
                    response = new ErrorResponse("Employer not found");
                }
                else
                {
                    response = new ItemResponse<Employer> { Item = employer };
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
        //POST api/employers Route Pattern
        [HttpPost("")]
        public ActionResult<ItemResponse<int>> Add(EmployerAddRequest model)
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
        //PUT api/employers/{id:int} Route Pattern
        [HttpPut("{id:int}")]
        public ActionResult<ItemResponse<int>> Update(EmployerUpdateRequest model)
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
        //DELETE api/employers/{id:int} Route Pattern
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
        //GET api/employers/paginate
        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<Employer>>> Pagination(int pageIndex, int pageSize)
        {
            ActionResult result = null;
            try
            {
                Paged<Employer> paged = _service.Pagination(pageIndex, pageSize);
                if (paged == null)
                {
                    result = NotFound404(new ErrorResponse("No records found"));
                }
                else
                {
                    ItemResponse<Paged<Employer>> response = new ItemResponse<Paged<Employer>>();
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

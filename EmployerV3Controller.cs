using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Data.Providers;
using Sabio.Models.Domain;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System.Collections.Generic;
using System;
using Sabio.Models;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/v3/employers")]
    [ApiController]
    public class EmployerV3Controller : BaseApiController
    {
        private IEmployersV3Service _employersV3Services = null;
        private IAuthenticationService<int> _authenticationService = null;

        public EmployerV3Controller(IEmployersV3Service service, ILogger<EmployerV3Controller> logger, IAuthenticationService<int> authService) : base(logger)
        {
            _employersV3Services = service;
            _authenticationService = authService;
        }

        #region GetAll V3
        //GET api/v3/employers
        [HttpGet]
        public ActionResult<ItemsResponse<EmployersV3>> GetAll()
        {
            int code = 200;
            BaseResponse response = null;

            List<EmployersV3> list = _employersV3Services.GetAll();
            try
            {
                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found");
                }
                else
                {
                    response = new ItemsResponse<EmployersV3> { Items = list };
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

        #region GetById V3
        //GET api/v3/employers/{id:int}
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<EmployersV3>> GetById(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                EmployersV3 employersV3 = _employersV3Services.GetById(id);

                if (employersV3 == null)
                {
                    code = 404;
                    response = new ErrorResponse("Employer not found");
                }
                else
                {
                    response = new ItemResponse<EmployersV3> { Item = employersV3 };
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
        //GET api/v3/employers/paginate
        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<EmployersV3>>> Pagination(int pageIndex, int pageSize)
        {
            ActionResult result = null;
            try
            {
                Paged<EmployersV3> paged = _employersV3Services.Pagination(pageIndex, pageSize);
                if (paged == null)
                {
                    result = NotFound404(new ErrorResponse("No records found"));
                }
                else
                {
                    ItemResponse<Paged<EmployersV3>> response = new ItemResponse<Paged<EmployersV3>>();
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

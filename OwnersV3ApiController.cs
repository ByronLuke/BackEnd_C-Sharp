using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Services.Interfaces;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System.Collections.Generic;
using System;
using Sabio.Models.Domain;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/v3/owners")]
    [ApiController]
    public class OwnersV3ApiController : BaseApiController
    {
        private IOwnersV3Services _ownersV3Services = null;
        private IAuthenticationService<int> _authenticationService = null;

        public OwnersV3ApiController(IOwnersV3Services service, ILogger<OwnersV3ApiController> logger, IAuthenticationService<int> authService) : base(logger)
        {
            _ownersV3Services = service;
            _authenticationService = authService;
        }

        #region GetAll V3
        //GET api/v3/owners
        [HttpGet]
        public ActionResult<ItemsResponse<OwnersV3>> GetAll()
        {
            int code = 200;
            BaseResponse response = null;

            List<OwnersV3> list = _ownersV3Services.GetAll();
            try
            {
                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found");
                }
                else
                {
                    response = new ItemsResponse<OwnersV3> { Items = list };
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
        //GET api/v3/owners/{id:int}
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<OwnersV3>> GetById(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                OwnersV3 ownersV3 = _ownersV3Services.GetById(id);

                if (ownersV3 == null)
                {
                    code = 404;
                    response = new ErrorResponse("owner not found");
                }
                else
                {
                    response = new ItemResponse<OwnersV3> { Item = ownersV3 };
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

    }
}

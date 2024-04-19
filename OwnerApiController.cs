using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain.Addresses;
using Sabio.Models.Domain.Friends;
using Sabio.Models.Requests.Owner;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/owners")]
    [ApiController]
    public class OwnerApiController : BaseApiController
    {
        private IOwnerService ownerService = null;
        private IAuthenticationService<int> authService = null;

        public OwnerApiController(IOwnerService ownerService, ILogger<OwnerApiController> logger, IAuthenticationService<int> authService) : base(logger)
        {
            this.ownerService = ownerService;
            this.authService = authService;
        }

        #region GetAll
        [HttpGet("")]
        public ActionResult<ItemsResponse<Owner>> GetAll()
        {
            int code = 200;
            BaseResponse response = null;

            List<Owner> list = ownerService.GetAll();
            try
            {
                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found");
                }
                else
                {
                    response = new ItemsResponse<Owner> { Items = list };
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
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<Owner>> GetById(int id) {

            int code = 200;
            BaseResponse response = null;

            try
            {
                Owner owner = ownerService.GetById(id);

                if (owner == null)
                {
                    code = 404;
                    response = new ErrorResponse("Owner not found.");
                }
                else
                {
                    response = new ItemResponse<Owner> { Item = owner };
                }
            }
            catch(Exception ex) { 

                code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code,response);
        
        }
        #endregion

        #region Add
        [HttpPost]
        public ActionResult<ItemResponse<int>> Add(OwnerAddRequest request)
        {
            ObjectResult result = null;

            try
            {
                int id = ownerService.Add(request);

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
        [HttpPut("{id:int}")]
        public ActionResult<ItemResponse<int>> Update(OwnerUpdateRequest request)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {

                ownerService.Update(request);

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
        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> Delete(int id)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                ownerService.Delete(id);

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

        #region Pagination
        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<Owner>>> Pagination(int pageIndex, int pageSize)
        {
            ActionResult result = null;
            try
            {
                Paged<Owner > paged = ownerService.Pagination(pageIndex, pageSize);
                if (paged == null)
                {
                    result = NotFound404(new ErrorResponse("No records found"));
                }
                else
                {
                    ItemResponse<Paged<Owner>> response = new ItemResponse<Paged<Owner>>();
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
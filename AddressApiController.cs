using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain.Addresses;
using Sabio.Models.Requests.Addresses;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/addresses")]
    [ApiController]
    public class AddressApiController : BaseApiController
    {  
        private IAddressService _service = null;
        private IAuthenticationService<int> _authService = null;
        public AddressApiController(IAddressService service, ILogger<AddressApiController> logger, IAuthenticationService<int> authService) : base(logger) 
        { 
            
            _service = service;
            _authService = authService;
        }
        
        #region GetAll

        //GET api/addresses Route Pattern
        [HttpGet("")]
        public ActionResult<    ItemsResponse<Address>  > GetAll()
        {
            int iCode = 200;
            BaseResponse response = null;

            List <Address> list = _service.GetRandomAddresses();

            try
            {
                if (list == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("App resource not found");
                }
                else
                {
                    response = new ItemsResponse<Address> { Items = list };
                }
               
            }
            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse(ex.Message);
            } 


            return StatusCode(iCode, response);
        }

        #endregion

        #region GetById

        //GET api/addresses/{id:int} Route Pattern
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<Address>> GetById(int id)
        {
            int iCode = 200;
            BaseResponse response = null;
            try
            {
                Address address = _service.Get(id);

                if (address == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Address not found.");
                 }
                else
                {
                    response = new ItemResponse<Address> { Item = address };
                }

            }                                                                                     
            catch (SqlException sqlEx)                     
            //Extra clean up
            {
                iCode = 500;
                response = new ErrorResponse($"SqlException Error: ${sqlEx.Message}");
                base.Logger.LogError(sqlEx.ToString());
            }
            catch (ArgumentException argEx)
            {
                //Extra clean up
                iCode = 500;
                response = new ErrorResponse($"ArgumentException Error: ${argEx.Message}");
            }
            catch (Exception ex)
            {
                iCode = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: ${ex.Message}");
            }



            return StatusCode(iCode, response);
        }                                                                                                 

        #endregion

        #region Create
        //POST api/addresses Route Pattern
        [HttpPost ("")]
        public ActionResult<ItemResponse<int>> Add(AddressAddRequest model)
        {
            ObjectResult result = null;

            int userId = _authService.GetCurrentUserId();
            IUserAuthData user = _authService.GetCurrentUser();

            try
            {
                int id = _service.Add(model, userId);

                ItemResponse<int> response = new ItemResponse<int>() { Item = id};

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

        #region Delete
        //DELETE api/addresses/{id:int} Route Pattern
        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> Delete (int id)
        {
            int iCode = 200;
            BaseResponse response = null;

            try
            {
                _service.Delete(id);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(iCode, response);
        }


        #endregion

        #region Update
        //PUT api/addresses/{id:int} Route Pattern
        [HttpPut("{id:int}")]
        public ActionResult<ItemResponse<int>> Update(AddressUpdateRequest model)
        {
            int iCode = 200;
            BaseResponse response = null;
            try
            {

                _service.Update(model);

                response = new SuccessResponse(); 

            }
            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(iCode, response);
        }
        #endregion                                                    
    }
}         
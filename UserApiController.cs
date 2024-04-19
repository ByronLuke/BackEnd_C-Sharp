using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Sabio.Models.Domain.Users;
using Sabio.Models.Requests.NewFolder;
using Sabio.Models.Requests.Users;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserApiController : BaseApiController
    {

        private IUserServiceV1 _service = null;
        private IAuthenticationService<int> _authService = null;
        public UserApiController(IUserServiceV1 service, ILogger<UserApiController> logger,
            IAuthenticationService<int> authService) : base(logger)
        {
            _service = service;
            _authService = authService;
        }
    #region GetAll
    //GET api/users Rote Pattern
    [HttpGet("")]
    public ActionResult<ItemsResponse<User> > GetAll()
    {
        int code = 200;
        BaseResponse response = null;

        List <User> list = _service.GetAll();

        try
            {
                if(list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found");
                }
                else
                {
                    response = new ItemsResponse<User> { Items = list };
                }
            }
        catch(Exception ex)
           {
                code = 500;
                response = new ErrorResponse(ex.Message);
           }

            return StatusCode(code,response);

    }
        #endregion

    #region GetById
        //GET api/users/{id:int} Route Pattern
        [HttpGet("{id:int}")]
    public ActionResult<ItemResponse<User>> GetById(int id)
    {
        BaseResponse response = null;
        int code = 200;

        try
        {
            User user = _service.GetById(id);
            
                if(user == null)
                {
                    code = 404;
                    response = new ErrorResponse("User not found");
                }
                else
                {
                    response = new ItemResponse<User> { Item = user };
                }
        }
        catch (Exception ex) 
        { 
             code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: ${ex.Message}");
        }
         
        return StatusCode(code, response);
    }
        #endregion

    #region Delete
        //Delete api/users/{id:int} Route Pattern
        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> Delete (int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.Delete(id);

                response = new SuccessResponse();
            }
            catch(Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code,response);
        }
        #endregion

    #region Create
        //POST api/users Route Pattern
        [HttpPost("")]
        public ActionResult<ItemResponse<int>> Create(UserV1AddRequest model)
        {
            ObjectResult result = null;
            try
            {
                int id = _service.Add(model);

                ItemResponse<int> response = new ItemResponse<int> { Item = id };

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
        #endregion

    #region Update
        //PUT api/users/{id:int} Route Pattern
        [HttpPut("{id:int}")]
        public ActionResult<ItemResponse<int>> Update (UserV1UpdateRequest model)
        { 
            int code = 200;
            BaseResponse response = null;
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
    }



}

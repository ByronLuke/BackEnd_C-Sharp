using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain.Friends;
using Sabio.Models.Requests.Friends;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/friends")]
    [ApiController]
    public class FriendApiController : BaseApiController
    {
        private IFriendService _service = null;
        private IAuthenticationService<int> _authService = null;
        public FriendApiController(IFriendService service, ILogger<FriendApiController> logger, IAuthenticationService<int> authService) : base(logger)
        {
            _service = service;
            _authService = authService;
        }

        #region GetAll
        //GET api/friends Route Pattern
        [HttpGet("")]
        public ActionResult<ItemsResponse<Friend>> GetAll()
        {
            int code = 200;
            BaseResponse response = null;

            List<Friend> list = _service.GetAll();
            try
            {
                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found");
                }
                else
                {
                    response = new ItemsResponse<Friend> { Items = list };
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
        //GET api/friends/{id:int} Route Pattern
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<Friend>> GetById(int id)
        {

            int code = 200;
            BaseResponse response = null;

            try
            {
                Friend friend = _service.GetById(id);

                if (friend == null)
                {
                    code = 404;
                    response = new ErrorResponse("Friend not found");
                }
                else
                {
                    response = new ItemResponse<Friend> { Item = friend };
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
        //POST api/friends Route Pattern
        [HttpPost("")]
        public ActionResult<ItemResponse<int>> Add(FriendAddRequest model)
        {
            ObjectResult result = null;
            int userId = _authService.GetCurrentUserId();

            try
            {
                int id = _service.Add(model, userId);

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

        #region Delete
        //DELETE api/friends/{id:int} Route Pattern
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
            catch(Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code,response);
        }
        #endregion

        #region Update
        //PUT api/friends/{id:int} Route Pattern
        [HttpPut("{id:int}")]
        public ActionResult<ItemResponse<int>> Update(FriendUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;
            int userId = _authService.GetCurrentUserId();   
            
            try
            {
                _service.Update(model, userId);
                                                 
                response = new SuccessResponse();                          
            }
            catch(Exception ex)   
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());            
            }
            return StatusCode(code, response);  
        }
        #endregion

        //GET api/friend/paginate
        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<Friend>>> Pagination(int pageIndex, int pageSize) 
        {
            ActionResult result = null;
            try
            {
                Paged<Friend> paged = _service.Pagination(pageIndex, pageSize);
                if(paged == null)
                {
                    result = NotFound404(new ErrorResponse("No records found"));
                }
                else 
                {
                    ItemResponse<Paged<Friend>> response = new ItemResponse<Paged<Friend>>();
                    response.Item = paged;
                    result = Ok200(response);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse (ex.Message.ToString()));
            }
            return result;
        }
    }
}


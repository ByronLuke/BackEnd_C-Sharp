using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Sabio.Models.Domain;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/v3/friends")]
    [ApiController]
    public class FriendApiControllerV3 : BaseApiController
    {
        private IFriendV3Service _friendV3Service = null;
        private IAuthenticationService<int> _authenticationService = null;

        public FriendApiControllerV3(IFriendV3Service service, ILogger<FriendApiControllerV3> logger, IAuthenticationService<int> authService) : base(logger)
        {
            _friendV3Service = service;
            _authenticationService = authService;
        }               

        #region GetAll V3
        //GET api/v3/friends
        [HttpGet]
        public ActionResult<ItemsResponse<FriendV3>> GetAll()
        {
            int code = 200;
            BaseResponse response = null;

            List<FriendV3> list = _friendV3Service.GetAll();
            try
            {
                if(list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found");
                }
                else
                {
                    response = new ItemsResponse<FriendV3> { Items = list };
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
        //GET api/v3/friends/{id:int}
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<FriendV3>> GetById(int id)
        {
            int code = 200;
            BaseResponse response = null;
                     
            try
            {
                FriendV3 friendV3 = _friendV3Service.GetById(id);

                if(friendV3 == null)
                {
                    code = 404;
                    response = new ErrorResponse("Friend not found");
                }
                else
                {
                    response = new ItemResponse<FriendV3> { Item = friendV3 };
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

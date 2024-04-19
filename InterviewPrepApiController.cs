using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Services.Interfaces;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using Sabio.Models.Domain.InterviewPrep;
using System.Collections.Generic;
using System;
using Sabio.Models.Domain.Friends;
using SendGrid;
using Sabio.Models.Requests.Owner;
using Sabio.Models.Requests.InterviewPrep;
using Sabio.Models;

namespace Sabio.Web.Api.Controllers    
{
    [Route("api/interview_prep")]                                                 
    [ApiController]
    public class InterviewPrepApiController : BaseApiController
    {
        private IInterviewPrepService _service = null;
        private IAuthenticationService<int> _authService = null;

        public InterviewPrepApiController(IInterviewPrepService service, ILogger<InterviewPrepApiController> logger, IAuthenticationService<int> authService) : base(logger)     
        {
            _service = service;
            _authService = authService;
        }      

        #region GetAll
        [HttpGet]
        public ActionResult<ItemsResponse<InterviewPrep>> GetAll()
        {

            int code = 200;
            BaseResponse response = null;

            List<InterviewPrep> list = _service.GetAll();
            try
            {
                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("Not found");
                }
                else 
                {
                    response = new ItemsResponse<InterviewPrep> { Items = list };
                }
            }
            catch(Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
             
            return StatusCode(code, response);      
        }
        #endregion      

        #region GetByTopic
        [HttpGet("{topic}")]
        public ActionResult<ItemResponse<InterviewPrep>> GetByTopic(string topic)
        { 
        int code = 200;
        BaseResponse response = null;

        List<InterviewPrep> list = _service.getByTopic(topic);
            try
            {
                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("Not found");
    }
                else 
                {
                    response = new ItemsResponse<InterviewPrep> { Items = list
};
                }
            }
            catch(Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
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
              
        #region Update
        [HttpPut("{id:int}")]
        public ActionResult<ItemResponse<int>> Update(InterviewPrepUpdateRequest request)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {

                _service.Update(request);

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

        #region Add
        [HttpPost]
        public ActionResult<ItemResponse<int>> Add(InterviewPrepAddRequest request)
        {
            ObjectResult result = null;

            try
            {
                int id = _service.Add(request);  

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

        #region Pagination
        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<InterviewPrep>>> Pagination(int pageIndex, int pageSize)
        {
            ActionResult result = null;
            try
            {
                Paged<InterviewPrep> paged = _service.Pagination(pageIndex, pageSize);
                if (paged == null)
                {
                    result = NotFound404(new ErrorResponse("No records found"));
                }
                else
                {
                    ItemResponse<Paged<InterviewPrep>> response = new ItemResponse<Paged<InterviewPrep>>();
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

        #region SearchPagination
        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<InterviewPrep>>> SearchPaginationQuestion(int pageIndex, int pageSize, string searchQuery)   
        {
            ActionResult result = null;
            try
            {
                Paged<InterviewPrep> paged = _service.SearchQuestionPagination(pageIndex, pageSize, searchQuery);
                if (paged == null)
                {
                    result = NotFound404(new ErrorResponse("No records found"));
                }
                else
                {
                    ItemResponse<Paged<InterviewPrep>> response = new ItemResponse<Paged<InterviewPrep>>();
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

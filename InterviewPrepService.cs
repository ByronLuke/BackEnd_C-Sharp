using Sabio.Data;
using Sabio.Data.Extensions;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain.Friends;
using Sabio.Models.Domain.InterviewPrep;
using Sabio.Models.Requests.InterviewPrep;
using Sabio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class InterviewPrepService : IInterviewPrepService
    {
        #region DataProvider
        IDataProvider _data = null;

        public InterviewPrepService(IDataProvider data)
        {
            _data = data;
        }
        #endregion

        #region GetByTopic
        public List<InterviewPrep> getByTopic(string topic)
        {
            string procName = "[dbo].[InterviewPrep_SelectByTopic]";
            List<InterviewPrep> list = null;

            _data.ExecuteCmd(procName,
                delegate (SqlParameterCollection collection)
                {
                    collection.AddWithValue("@Topic", topic);

                }, singleRecordMapper: delegate (IDataReader reader, short set)
            {
                InterviewPrep aTopic = MapSpecificPrep(reader);

                if (list == null)
                {
                    list = new List<InterviewPrep>();
                }

                list.Add(aTopic);
            });
            return list;
        }
        #endregion

        #region GetById
        public InterviewPrep getById(int id)
        {
            string procName = "[dbo].[InterviewPrep_SelectById]";
            InterviewPrep interviewPrep = null;

            _data.ExecuteCmd(procName,
                delegate (SqlParameterCollection collection)
                {
                    collection.AddWithValue("@Id", id);

                }, delegate (IDataReader reader, short set)
                {
                    interviewPrep = MapSpecificPrep(reader);
                });
            return interviewPrep;
        }
        #endregion

        #region GetAll
        public List<InterviewPrep> GetAll()
        {
            string procName = "[dbo].[InterviewPrep_SelectAll]";
            List<InterviewPrep> list = null;

            _data.ExecuteCmd(procName,
                inputParamMapper: null, singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    InterviewPrep specificPrep = MapSpecificPrep(reader);

                    if (list == null)
                    {
                        list = new List<InterviewPrep>();
                    }

                    list.Add(specificPrep);
                });
            return list;
        }
        #endregion

        #region Add
        public int Add(InterviewPrepAddRequest request)
        {
            int id = 0;

            string procName = "[dbo].[InterviewPrep_Insert]";

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection collection)
                {
                    AddCommonParams(request, collection);
                    //collection.AddOutputParameter("@Id", collection.Id);

                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;

                    collection.Add(idOut);
                }, returnParameters: delegate (SqlParameterCollection returnCollection)
                {
                    object oId = returnCollection["@Id"].Value;
                    int.TryParse(oId.ToString(), out id);
                });
            return id;
        }
        #endregion

        #region Update
        public void Update(InterviewPrepUpdateRequest request)
        {
            string procName = "";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection collection)
                {
                    AddCommonParams(request, collection);
                    collection.AddWithValue("@Id", request.id);
                }, returnParameters: null);
        }
        #endregion

        #region Delete
        public void Delete(int id)
        {
            string procName = "";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection collection)
                {
                    collection.AddWithValue("@Id", id);
                }, returnParameters: null);
        }
        #endregion

        #region Paginate
        public Paged<InterviewPrep> Pagination(int pageIndex, int pageSize)
        {
            Paged<InterviewPrep> pagedList = null;
            List<InterviewPrep> list = null;
            int totalCount = 0;
            string procName = "[dbo].[InterviewPrep_Pagination]";

            _data.ExecuteCmd(procName,
                delegate (SqlParameterCollection collection)
                {
                    collection.AddWithValue("@PageIndex", pageIndex);
                    collection.AddWithValue("@PageSize", pageSize);

                }, delegate (IDataReader reader, short set)
                {
                    InterviewPrep interviewPrep = MapSpecificPrep(reader);
                    totalCount = reader.GetSafeInt32(3);

                    if (list == null)
                    {
                        list = new List<InterviewPrep>();
                    }

                    list.Add(interviewPrep);
                });
            if (list != null)
            {
                pagedList = new Paged<InterviewPrep>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }
        #endregion

        #region SearchQuestionPagination
        public Paged<InterviewPrep> SearchQuestionPagination(int pageIndex, int pageSize, string searchQuery)
        {
            Paged<InterviewPrep> pagedList = null;
            List<InterviewPrep> list = null;
            int totalCount = 0;
            string procName = "[dbo].[InterviewPrep_SearchQuestion_Pagination]";

            _data.ExecuteCmd(procName,
                delegate (SqlParameterCollection collection)
                {
                    collection.AddWithValue("@PageIndex", pageIndex);
                    collection.AddWithValue("@PageSize", pageSize);
                    collection.AddWithValue("@SearchQuery", searchQuery);
                }, delegate (IDataReader reader, short set)
                {
                    InterviewPrep interviewPrep = MapSpecificPrep(reader);
                    totalCount = reader.GetSafeInt32(3);

                    if (list == null)
                    {
                        list = new List<InterviewPrep>();
                    }

                    list.Add(interviewPrep);
                });

            if (list != null)
            {
                pagedList = new Paged<InterviewPrep>(list, pageIndex, pageSize, totalCount);
            }

            return pagedList;
        }
        #endregion

        #region SearchTopicPagination
        public Paged<InterviewPrep> SearchTopicPagination(int pageIndex, int pageSize, string searchQuery)
        {
            Paged<InterviewPrep> pagedList = null;
            List<InterviewPrep> list = null;
            int totalCount = 0;
            string procName = "[dbo].[InterviewPrep_SearchTopic_Pagination]";

            _data.ExecuteCmd(procName,
                delegate (SqlParameterCollection collection)
                {
                    collection.AddWithValue("@PageIndex", pageIndex);
                    collection.AddWithValue("@PageSize", pageSize);
                    collection.AddWithValue("@SearchQuery", searchQuery);
                }, delegate (IDataReader reader, short set)
                {
                    InterviewPrep interviewPrep = MapSpecificPrep(reader);
                    totalCount = reader.GetSafeInt32(3);

                    if (list == null)
                    {
                        list = new List<InterviewPrep>();
                    }

                    list.Add(interviewPrep);
                });

            if (list != null)
            {
                pagedList = new Paged<InterviewPrep>(list, pageIndex, pageSize, totalCount);
            }

            return pagedList;
        }
        #endregion

        #region CommonParams
        private static void AddCommonParams(InterviewPrepAddRequest request, SqlParameterCollection collection)
        {
            collection.AddWithValue("@Topic", request.Topic);
            collection.AddWithValue("@Question", request.Question);
            collection.AddWithValue("@Answer", request.Answer);
        }
        #endregion

        #region Mapper
        private static InterviewPrep MapSpecificPrep(IDataReader reader)
        {
            InterviewPrep specificInterviewPrep = new InterviewPrep();
            int startingIndex = 0;
            specificInterviewPrep.Id = reader.GetInt32(startingIndex++);
            specificInterviewPrep.Topic = reader.GetString(startingIndex++);
            specificInterviewPrep.Question = reader.GetString(startingIndex++);
            specificInterviewPrep.Answer = reader.GetString(startingIndex++);

            return specificInterviewPrep;
        }
        #endregion
    }
}

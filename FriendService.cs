using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain.Friends;
using Sabio.Models.Requests.Friends;
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
    public class FriendService : IFriendService
    {
        IDataProvider _data = null;

        public FriendService(IDataProvider data)
        {
            _data = data;
        }
        public Friend GetById(int id)
        {
            string procName = "[dbo].[Friends_SelectById]";

            Friend friend = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                friend = MapSingleFriend(reader);
            });
            return friend;
        }

        public List<Friend> GetAll()
        {
            string procName = "[dbo].[Friends_SelectAll]";
            List<Friend> list = null;

            _data.ExecuteCmd(procName, inputParamMapper: null, singleRecordMapper: delegate (IDataReader reader, short set)
            {
                Friend aFriend = MapSingleFriend(reader);

                if (list == null)
                {
                    list = new List<Friend>();
                }

                list.Add(aFriend);
            });
            return list;
        }

        public int Add(FriendAddRequest request, int UserId)        
        {
            int id = 0;

            string procName = "[dbo].[Friends_Insert]";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                AddCommonParams(request, collection);

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
          
        public void Update(FriendUpdateRequest updateRequest, int UserId)
        {
            string procName = "[dbo].[Friends_Update]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                AddCommonParams(updateRequest, collection);

                collection.AddWithValue("@Id", updateRequest.Id);

            }, returnParameters: null);
        }
        public void Delete(int Id)
        {
            string procName = "[dbo].[Friends_Delete]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@Id", Id);

            }, returnParameters: null);
        }
        public Paged<Friend> Pagination(int pageIndex, int pageSize)
        {
            Paged<Friend> pagedList = null;
            List<Friend> list = null;   
            int totalCount = 0;
            string procName = "[dbo].[Friends_Pagination]";

            _data.ExecuteCmd(procName,
                delegate (SqlParameterCollection collection)
                {
                    collection.AddWithValue("@PageIndex", pageIndex);
                    collection.AddWithValue("@PageSize", pageSize);
                }, delegate (IDataReader reader, short set)
                {
                    Friend friend = MapSingleFriend(reader);
                    totalCount = reader.GetSafeInt32(6);

                    if (list == null)
                    {
                        list = new List<Friend>();
                    }
                    list.Add(friend);
                });
            if(list!=null)
            {
                pagedList = new Paged<Friend>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;

        }
        private static void AddCommonParams(FriendAddRequest request, SqlParameterCollection collection)
        {
            collection.AddWithValue("@Title", request.Title);
            collection.AddWithValue("@Bio", request.Bio);
            collection.AddWithValue("@Summary", request.Summary);
            collection.AddWithValue("@Headline", request.Headline);
            collection.AddWithValue("@Slug", request.Slug);
            collection.AddWithValue("@StatusId", request.StatusId);
            collection.AddWithValue("@PrimaryImageUrl", request.PrimaryImageUrl);
            collection.AddWithValue("@UserId", request.UserId);
        }
                  
        private static Friend MapSingleFriend(IDataReader reader)
        {
            Friend aFriend = new Friend();
            int startingIndex = 0;
            aFriend.Id = reader.GetSafeInt32(startingIndex++);
            aFriend.Title = reader.GetString(startingIndex++);
            aFriend.Bio = reader.GetString(startingIndex++);
            aFriend.Summary = reader.GetString(startingIndex++);
            aFriend.Headline = reader.GetString(startingIndex++);
            aFriend.Slug = reader.GetString(startingIndex++);
            aFriend.StatusId = reader.GetSafeInt32(startingIndex++);
            aFriend.PrimaryImageUrl = reader.GetString(startingIndex++);
            aFriend.UserId = reader.GetSafeInt32(startingIndex++);
            aFriend.DateCreated = reader.GetSafeDateTime(startingIndex++);
            aFriend.DateModified = reader.GetSafeDateTime(startingIndex++);
            return aFriend;
        }
    }
           
}

using Sabio.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sabio.Data;
using System.Net;
using System.Reflection;
using Sabio.Models.Domain.Users;
using Sabio.Models.Requests.Users;
using Sabio.Models.Requests.NewFolder;
using Sabio.Services.Interfaces;

namespace Sabio.Services
{
    public class UserServiceV1 : IUserServiceV1
    {
        IDataProvider _data = null;

        public UserServiceV1(IDataProvider data)
        {
            _data = data;
        }

        public User GetById(int id)
        {
            string procName = "[dbo].[Users_SelectById]";

            User user = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                User aUser = MapSingleUser(reader);

                user = aUser;

            });
            return user;
        }
        public List<User> GetAll()
        {
            string procName = "[dbo].[Users_SelectAll]";
            List<User> list = null;

            _data.ExecuteCmd(procName, inputParamMapper: null, singleRecordMapper: delegate (IDataReader reader, short set)
            {
                User aUser = MapSingleUser(reader);

                if (list == null)
                {
                    list = new List<User>();
                }

                list.Add(aUser);
            });
            return list;
        }
        public int Add(UserV1AddRequest request)
        {
            int id = 0;

            string procName = "[dbo].[Users_Insert]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                //What we put in
                AddCommonParams(request, collection);

                //1 output
                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;

                collection.Add(idOut);

            }, returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                //What goes out
                object oId = returnCollection["@Id"].Value;
                int.TryParse(oId.ToString(), out id);

            });
            return id;
        }
        public void Update(UserV1UpdateRequest updateRequest)
        {
            string procName = "[dbo].[Users_Update]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                AddCommonParams(updateRequest, collection);

                collection.AddWithValue("@Id", updateRequest.Id);

            }, returnParameters: null);
        }
        public void Delete(int id)
        {
            string procName = "[dbo].[Users_Delete]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@Id", id);

            }, returnParameters: null);
        }
        private static void AddCommonParams(UserV1AddRequest request, SqlParameterCollection collection)
        {
            collection.AddWithValue("@FirstName", request.FirstName);
            collection.AddWithValue("@LastName", request.LastName);
            collection.AddWithValue("@Email", request.Email);
            collection.AddWithValue("@Password", request.Password);
            collection.AddWithValue("@AvatarUrl", request.AvatarUrl);
            collection.AddWithValue("@TenantId", request.TenantId);
        }
        private static User MapSingleUser(IDataReader reader)
        {
            int startingIndex = 0;

            User aUser = new User();
            aUser.Id = reader.GetSafeInt32(startingIndex++);
            aUser.FirstName = reader.GetSafeString(startingIndex++);
            aUser.LastName = reader.GetSafeString(startingIndex++);
            aUser.Email = reader.GetSafeString(startingIndex++);
            aUser.Password = reader.GetSafeString(startingIndex++);
            aUser.AvatarUrl = reader.GetSafeString(startingIndex++);
            aUser.TenantId = reader.GetSafeString(startingIndex++);
            aUser.DateCreated = reader.GetSafeDateTime(startingIndex++);
            aUser.DateModified = reader.GetSafeDateTime(startingIndex++);
            return aUser;
        }
    }
}

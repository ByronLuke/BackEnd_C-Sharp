using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain.Friends;
using Sabio.Models.Requests.Owner;
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
    public class OwnerService : IOwnerService
    {
        IDataProvider _data = null;

        public OwnerService(IDataProvider data)
        {
            _data = data;
        }

        public Owner GetById(int id)
        {
            string procName = "dbo.Owner_SelectById";

            Owner owner = null;

            _data.ExecuteCmd(procName,
                delegate (SqlParameterCollection collection)
                {
                    collection.AddWithValue("@Id", id);
                }, delegate (IDataReader reader, short set)
                {
                    owner = MapSingleOwner(reader);

                });
            return owner;
        }

        public List<Owner> GetAll()
        {
            string procName = "dbo.Owner_SelectAll";

            List<Owner> ownerList = null;

            _data.ExecuteCmd(procName, inputParamMapper: null
                , singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    Owner aOwner = MapSingleOwner(reader);

                    if (ownerList == null)
                    {
                        ownerList = new List<Owner>();
                    }

                    ownerList.Add(aOwner);
                });
            return ownerList;
        }

        public int Add(OwnerAddRequest request)
        {
            int id = 0;

            string procName = "[dbo].[Owner_Insert]";

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

        public void Update(OwnerUpdateRequest updateRequest)
        {
            string procName = "[dbo].[Owner_Update]";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                AddCommonParams(updateRequest, collection);
                collection.AddWithValue("@Id", updateRequest.Id);
            }, returnParameters: null);
        }

        public void Delete(int id)
        {
            string procName = "[dbo].[Owner_Delete]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@Id", id);

            }, returnParameters: null);
        }

        public Paged<Owner> Pagination(int pageIndex, int pageSize)
        {
            Paged<Owner> pagedList = null;
            List<Owner> list = null;
            int totalCount = 0;
            string procName = "[dbo].[Owner_Pagination]";

            _data.ExecuteCmd(procName,
                delegate (SqlParameterCollection collection)
                {
                    collection.AddWithValue("@PageIndex", pageIndex);
                    collection.AddWithValue("@PageSize", pageSize);
                }, delegate (IDataReader reader, short set)
                {
                    Owner aOwner = MapSingleOwner(reader);
                    totalCount = reader.GetSafeInt32(7);

                    if (list == null)
                    {
                        list = new List<Owner>();
                    }
                    list.Add(aOwner);
                });
            if (list != null)
            {
                pagedList = new Paged<Owner>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;

        }

        private static void AddCommonParams(OwnerAddRequest request, SqlParameterCollection collection)
        {
            collection.AddWithValue("@Age", request.Age);
            collection.AddWithValue("@FirstName", request.FirstName);
            collection.AddWithValue("@LastName", request.LastName);
            collection.AddWithValue("@HouseId", request.HouseId);
        }

        private static Owner MapSingleOwner(IDataReader reader)
        {
            Owner aOwner = new Owner();
            int startingIndex = 0;
            aOwner.Id = reader.GetInt32(startingIndex++);
            aOwner.Age = reader.GetInt32(startingIndex++);
            aOwner.FirstName = reader.GetString(startingIndex++);
            aOwner.LastName = reader.GetString(startingIndex++);
            aOwner.HouseId = reader.GetInt32(startingIndex++);
            aOwner.DateCreated = reader.GetDateTime(startingIndex++);
            aOwner.DateModified = reader.GetDateTime(startingIndex++);

            return aOwner;
        }
    }
}
 
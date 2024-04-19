using Sabio.Data.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Sabio.Data;
using Sabio.Models.Domain.Concerts;
using Sabio.Models.Requests.Concerts;
using Sabio.Services.Interfaces;

namespace Sabio.Services
{
    public class ConcertService : IConcertService
    {
        IDataProvider _data = null;

        public ConcertService(IDataProvider data)
        {
            _data = data;
        }
        public Concert GetById(int id)
        {
            string procName = "[dbo].[Concerts_SelectById]";

            Concert concert = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                concert = SingleConcertMapper(reader);        

            });
            return concert;
        }

        public List<Concert> GetAll()
        {
            string procName = "[dbo].[Concerts_SelectAll]";
            List<Concert> list = null;

            _data.ExecuteCmd(procName, inputParamMapper: null, singleRecordMapper: delegate (IDataReader reader, short set)
            {
                Concert aConcert = SingleConcertMapper(reader);

                if (list == null)
                {
                    list = new List<Concert>();
                }

                list.Add(aConcert);
            });
            return list;
        }

        public int Add(ConcertAddRequest request)
        {
            int id = 0;

            string procName = "[dbo].[Concerts_Insert]";

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

        public void Update(ConcertUpdateRequest updateRequest)
        {
            string procName = "[dbo].[Concerts_Update]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                AddCommonParams(updateRequest, collection);

                collection.AddWithValue("@Id", updateRequest.Id);

            }, returnParameters: null);
        }

        public void Delete(int Id)
        {
            string procName = "[dbo].[Concerts_Delete]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@Id", Id);

            }, returnParameters: null);
        }
        private static void AddCommonParams(ConcertAddRequest request, SqlParameterCollection collection)
        {
            collection.AddWithValue("@Name", request.Name);
            collection.AddWithValue("@Description", request.Description);
            collection.AddWithValue("@IsFree", request.IsFree);
            collection.AddWithValue("@Address", request.Address);
            collection.AddWithValue("@Cost", request.Cost);
            collection.AddWithValue("@DateOfEvent", request.DateOfEvent);
        }

        private static Concert SingleConcertMapper(IDataReader reader)
        {
            Concert aConcert = new Concert();
            int startingIndex = 0;

            aConcert.Id = reader.GetSafeInt32(startingIndex++);
            aConcert.Name = reader.GetString(startingIndex++);
            aConcert.Description = reader.GetString(startingIndex++);
            aConcert.IsFree = reader.GetBoolean(startingIndex++);
            aConcert.Address = reader.GetString(startingIndex++);
            aConcert.Cost = reader.GetSafeInt32(startingIndex++);
            aConcert.DateOfEvent = reader.GetDateTime(startingIndex++);

            return aConcert;
        }
    }
}

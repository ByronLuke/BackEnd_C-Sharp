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
using Sabio.Models.Requests.Addresses;
using Sabio.Models.Domain.Addresses;
using System.Reflection;
using Sabio.Services.Interfaces;

namespace Sabio.Services
{
    public class AddressService : IAddressService
    {
        IDataProvider _data = null;

        #region - Data
        public AddressService(IDataProvider data)
        {
            _data = data;
        }
        #endregion
        #region - Procs
        public Address Get(int id)
        {
            string procName = "[dbo].[Sabio_Addresses_SelectById]";

            Address address = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {

                // Takes data in one shape and prpoduces a second shape
                // I have a number and need to make a parameter that inside of it has the number int > param(int)

                paramCollection.AddWithValue("@Id", id);

            }, delegate (IDataReader reader, short set)
            {

                // for the single record mapper we have one shape and makes it into a different shape oneShape > secondShape
                // first shape is coming from the reader from the DB coming from the format Tabular DataStream ... reader from DB >>> Address
                // Where we are hydrating our model

                address = MapSingleAddress(reader);
            }
            );

            return address;
        }
        public void Delete(int Id)
        {
            string procName = "[dbo].[Sabio_Addresses_DeleteById]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                //What we send to SQL

                collection.AddWithValue("@Id", Id);


            }, returnParameters: null);
        }
        public void Update(AddressUpdateRequest Model)
        {
            string procName = "[dbo].[Sabio_Addresses_Update]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                //What we send to SQL
                AddCommonParams(Model, collection);

                collection.AddWithValue("@Id", Model.Id);


            }, returnParameters: null);
        }
        public int Add(AddressAddRequest request, int userId)
        {

            int id = 0;


            string procName = "[dbo].[Sabio_Addresses_Insert]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                //What goes in 
                AddCommonParams(request, collection);

                // and 1 output

                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;

                collection.Add(idOut);

            }, returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                //What goes out
                object oId = returnCollection["@Id"].Value;
                int.TryParse(oId.ToString(), out id);

                Console.WriteLine("");

            });
            return id;
        }
        public List<Address> GetRandomAddresses()
        {
            string procName = "[dbo].[Sabio_Addresses_SelectRandom50]";

            List<Address> list = null;

            _data.ExecuteCmd(procName, inputParamMapper: null, singleRecordMapper: delegate (IDataReader reader, short set)
            {
                // for the single record mapper we have one shape and makes it into a different shape oneShape > secondShape
                // first shape is coming from the reader from the DB coming from the format Tabular DataStream ... reader from DB >>> Address
                // Where we are hydrating our model

                Address aAddress = MapSingleAddress(reader);

                if (list == null)
                {
                    list = new List<Address>();
                }

                list.Add(aAddress);
            });
            return list;
        }
        #endregion
        #region - Mappers
        private static Address MapSingleAddress(IDataReader reader)
        {
            int startingIndex = 0;

            Address aAddress = new Address();
            aAddress.Id = reader.GetSafeInt32(startingIndex++);
            aAddress.LineOne = reader.GetSafeString(startingIndex++);
            aAddress.SuiteNumber = reader.GetSafeInt32(startingIndex++);
            aAddress.City = reader.GetSafeString(startingIndex++);
            aAddress.State = reader.GetSafeString(startingIndex++);
            aAddress.PostalCode = reader.GetSafeString(startingIndex++);
            aAddress.IsActive = reader.GetSafeBool(startingIndex++);
            aAddress.Lat = reader.GetSafeDouble(startingIndex++);
            aAddress.Long = reader.GetSafeDouble(startingIndex++);
            return aAddress;
        }
        private static void AddCommonParams(AddressAddRequest request, SqlParameterCollection collection)
        {
            collection.AddWithValue("@LineOne", request.LineOne);
            collection.AddWithValue("@SuiteNumber", request.SuiteNumber);
            collection.AddWithValue("@City", request.City);
            collection.AddWithValue("@State", request.State);
            collection.AddWithValue("@PostalCode", request.PostalCode);
            collection.AddWithValue("@IsActive", request.IsActive);
            collection.AddWithValue("@Lat", request.Lat);
            collection.AddWithValue("@Long", request.Long);
        }

        #endregion
    }
}

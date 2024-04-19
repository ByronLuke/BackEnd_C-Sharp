using Sabio.Data.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Data.SqlClient;
using System.Data;
using Sabio.Data;
using Sabio.Models.Requests.Properties;
using Sabio.Models.Domain.Properties;

namespace Sabio.Services
{
    public class PropertyService
    {
        IDataProvider _data = null;
       public PropertyService(IDataProvider data) { 
            _data = data;
        }

       public Property GetById(int id)
        {
            string procName = "[dbo].[Properties_SelectById]";
            Property property = null;

            _data.ExecuteCmd(procName,
                inputParamMapper: delegate (SqlParameterCollection collection)
                {
                    collection.AddWithValue("@Id", id);

                }, delegate (IDataReader reader, short set)
                {
                    property = MapSingleProperty(reader);

                });

            return property;
        }

        public List<Property> GetAll()
        {
            string procName = "[dbo].[Properties_SelectAll]";
            List<Property> list = null;

            _data.ExecuteCmd(procName, inputParamMapper: null, delegate (IDataReader reader, short set)
            {


                if (list  == null)
                {
                    list = new List<Property>();
                }
                list.Add(MapSingleProperty(reader));
            });
            return list;
        }

        public int Add(PropertyAddRequest request)
        {
            string procName = "[dbo].[Properties_Insert]";
            int id = 0;

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection collection)
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

        public void Update(PropertyUpdateRequest request)
        {
            string procName = "[dbo].[Properties_Update]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection collection)
                {
                    AddCommonParams(request, collection);
                    collection.AddWithValue("@Id", request.Id);

                }, returnParameters: null);
        }
       
        public void Delete(int Id)
        {
            string procName = "[dbo].[Properties_Delete]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection collection)
                {
                    collection.AddWithValue("@Id", Id);
                }, returnParameters: null);
        }
        private static void AddCommonParams(PropertyAddRequest request, SqlParameterCollection collection)
        {
            collection.AddWithValue("@Price", request.Price);
            collection.AddWithValue("@Address", request.Address);
            collection.AddWithValue("@Bedrooms", request.Bedrooms);
            collection.AddWithValue("@Bathrooms", request.Bathrooms);
            collection.AddWithValue("@Type", request.Type);
        }

        private static Property MapSingleProperty(IDataReader reader)
        {
            Property aProperty = new Property();
            int index = 0;
            aProperty.Id = reader.GetSafeInt32(index++);
            aProperty.Price = reader.GetSafeInt32(index++);
            aProperty.Address = reader.GetSafeString(index++);
            aProperty.Bedrooms = reader.GetSafeInt32(index++);
            aProperty.Bathrooms = reader.GetSafeInt32(index++);
            aProperty.Type = reader.GetSafeString(index++);

            return aProperty;
        }
    }
}

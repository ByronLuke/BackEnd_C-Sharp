using Sabio.Data.Extensions;
using Sabio.Data.Providers;
using Sabio.Models.Requests;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class RestaurantService : IRestaurantService
    {
        IDataProvider _data = null;

        public RestaurantService(IDataProvider data)
        {
            _data = data;

        }
        public int Add(RestaurantAddRequest request)
        {
            int id = 0;

            string procName = "dbo.RestaurantV2_Insert";

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection collection)
                {
                    collection.AddWithValue("@RestaurantName", request.RestaurantName);
                    collection.AddWithValue("@Region", request.Region);
                    collection.AddWithValue("@ChefName", request.ChefName);
                    //collection.AddOutputParameter()

                    SqlParameter idOut = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                    idOut.Direction = System.Data.ParameterDirection.Output;

                    collection.Add(idOut);
                }, returnParameters: delegate (SqlParameterCollection returnCollection)
                {
                    object oId = returnCollection["@Id"].Value;
                    int.TryParse(oId.ToString(), out id);
                });
            return id;
        }
    }
}

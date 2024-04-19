using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models.Domain;
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
    public class OwnersV3Services : IOwnersV3Services
    {
        IDataProvider _dataProvider = null;
        public OwnersV3Services(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public List<OwnersV3> GetAll()
        {
            List<OwnersV3> list = null;
            string procName = "[dbo].[Owner_SelectAllV3]";
            _dataProvider.ExecuteCmd(procName,
                inputParamMapper: null,
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    OwnersV3 ownerV3List = MapV3Owner(reader);

                    if (list == null)
                    {
                        list = new List<OwnersV3>();
                    }

                    list.Add(ownerV3List);

                });

            return list;
        }

        public OwnersV3 GetById(int id)
        {
            string procName = "[dbo].[Owner_SelectByIdV3]";

            OwnersV3 ownersV3 = null;

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                ownersV3 = MapV3Owner(reader);
            });
            return ownersV3;
        }

        private static OwnersV3 MapV3Owner(IDataReader reader)
        {
            OwnersV3 aOwnerV3 = new OwnersV3();
            aOwnerV3.House = new House();
            aOwnerV3.Pets = new List<Pets>();

            int startingIndex = 0;

            aOwnerV3.Id = reader.GetInt32(startingIndex++);
            aOwnerV3.Age = reader.GetInt32(startingIndex++);
            aOwnerV3.FirstName = reader.GetString(startingIndex++);
            aOwnerV3.LastName = reader.GetString(startingIndex++);

            aOwnerV3.House.Address = reader.GetString(startingIndex++);
            aOwnerV3.House.City = reader.GetString(startingIndex++);
            aOwnerV3.House.State = reader.GetString(startingIndex++);
            aOwnerV3.House.Bedrooms = reader.GetInt32(startingIndex++);
            aOwnerV3.House.Bathrooms = reader.GetInt32(startingIndex++);

            aOwnerV3.Pets = reader.DeserializeObject<List<Pets>>(startingIndex++);

            aOwnerV3.DateCreated = reader.GetDateTime(startingIndex++);
            aOwnerV3.DateModified = reader.GetDateTime(startingIndex++);

            return aOwnerV3;
        }
    }
}

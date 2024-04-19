using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Friends;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class CustomersOrdersV3Service : ICustomersOrdersV3Service
    {
        private IDataProvider _dataProvider = null;

        #region DataConnection
        public CustomersOrdersV3Service(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }
        #endregion

        #region GetAll
        public List<CustomersOrdersV3> GetAll()
        {
            List<CustomersOrdersV3> list = null;
            string procName = "dbo.CustomersOrders_SelectAllV3";

            _dataProvider.ExecuteCmd(procName,
                inputParamMapper: null,
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    CustomersOrdersV3 cOrdersV3 = MapV3CustomerOrder(reader);

                    if (list == null)
                    {
                        list = new List<CustomersOrdersV3>();
                    }

                    list.Add(cOrdersV3);

                });

            return list;
        }
        #endregion

        #region GetById
        public CustomersOrdersV3 GetById(int id)
        {
            string procName = "[dbo].[CustomersOrders_SelectByIdV3]";

            CustomersOrdersV3 cOrderV3 = null;

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                cOrderV3 = MapV3CustomerOrder(reader);
            });
            return cOrderV3;
        }
        #endregion

        #region Pagination

        public Paged<CustomersOrdersV3> Pagination(int pageIndex, int pageSize)
        {
            Paged<CustomersOrdersV3> pagedList = null;
            List<CustomersOrdersV3> list = null;
            int totalCount = 0;
            string procName = "dbo.CustomersOrders_PaginationV3";

            _dataProvider.ExecuteCmd(procName,
                delegate (SqlParameterCollection collection)
                {
                    collection.AddWithValue("@PageIndex", pageIndex);
                    collection.AddWithValue("@PageSize", pageSize);
                }, delegate (IDataReader reader, short set)
                {
                    CustomersOrdersV3 aCustomer = MapV3CustomerOrder(reader);
                    totalCount = reader.GetSafeInt32(8);

                    if (list == null)
                    {
                        list = new List<CustomersOrdersV3>();
                    }
                    list.Add(aCustomer);
                });
            if (list != null)
            {
                pagedList = new Paged<CustomersOrdersV3>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;

        }

        #endregion


        private static CustomersOrdersV3 MapV3CustomerOrder(IDataReader reader)
        {
            CustomersOrdersV3 aCustomer = new CustomersOrdersV3();
            aCustomer.Region = new Region();
            aCustomer.Order = new List<Order>();

            int startingIndex = 0;

            aCustomer.Id = reader.GetInt32(startingIndex++);
            aCustomer.Age = reader.GetInt32(startingIndex++);
            aCustomer.FirstName = reader.GetString(startingIndex++);
            aCustomer.LastName = reader.GetString(startingIndex++);
            aCustomer.LoyaltyMember = reader.GetBoolean(startingIndex++);

            aCustomer.Region.RegionName = reader.GetString(startingIndex++);
            aCustomer.Region.RegionalManager = reader.GetString(startingIndex++);

            aCustomer.Order = reader.DeserializeObject<List<Order>>(startingIndex++);

            return aCustomer;
        }
    }
}

using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain.Customers;
using Sabio.Models.Domain.Friends;
using Sabio.Models.Requests.Customers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class CustomerService : ICustomerService
    {
        IDataProvider _data = null;

        public CustomerService(IDataProvider data)
        {
            _data = data;
        }

        #region GetById
        public Customer GetById(int id)
        {
            string procName = "dbo.Customers_SelectById";
            Customer customer = null;
            

            _data.ExecuteCmd(procName,
            delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@Id", id);
            },
            delegate (IDataReader reader, short set)
            {
                customer = MapSingleCustomer(reader);
            });
            return customer;
        }
        #endregion

        #region GetAll
        public List<Customer> GetAll()
        {

            string procName = "dbo.Customers_SelectAll";
            List<Customer> list = null;
            _data.ExecuteCmd(procName,
                inputParamMapper: null, singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    Customer aCustomer = MapSingleCustomer(reader);

                    if (list == null)
                    {
                        list = new List<Customer>();
                    }

                    list.Add(aCustomer);
                });
            return list;
        }
        #endregion

        #region Add
        public int Add(CustomerAddRequest request)
        {
            int id = 0;

            string procName = "[dbo].[Customers_Insert]";

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
        #endregion

        #region Update
        public void Update(CustomerUpdateRequest request)
        {
            string procName = "[dbo].[Customers_Update]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection collection)
                {
                    AddCommonParams(request, collection);

                    collection.AddWithValue("@Id", request.Id);

                }, returnParameters: null);
        }
        #endregion

        #region Delete
        public void Delete(int Id)
        {
            string procName = "[dbo].[Customers_Delete]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@Id", Id);

            }, returnParameters: null);
        }
        #endregion

        #region Pagination
        public Paged<Customer> Pagination(int pageIndex, int pageSize)
        {
            Paged<Customer> pagedList = null;
            List<Customer> list = null;
            int totalCount = 0;
            string procName = "[dbo].[Customers_Pagination]";

            _data.ExecuteCmd(procName,
                delegate (SqlParameterCollection collection)
                {
                    collection.AddWithValue("@PageIndex", pageIndex);
                    collection.AddWithValue("@PageSize", pageSize);
                }, delegate (IDataReader reader, short set)
                {
                    Customer customer = MapSingleCustomer(reader);
                    totalCount = reader.GetSafeInt32(6);

                    if (list == null)
                    {
                        list = new List<Customer>();
                    }
                    list.Add(customer);
                });

            if (list != null)
            {
                pagedList = new Paged<Customer>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        #endregion

        private static void AddCommonParams(CustomerAddRequest request, SqlParameterCollection collection)
        {
            collection.AddWithValue("@Age", request.Age);
            collection.AddWithValue("@FirstName", request.FirstName);
            collection.AddWithValue("@LastName", request.LastName);
            collection.AddWithValue("@RegionId", request.RegionId);
            collection.AddWithValue("@LoyaltyMember", request.LoyaltyMember);
        }

        private static Customer MapSingleCustomer(IDataReader reader)
        {
            Customer aCustomer = new Customer();
            int startingIndex = 0;

            aCustomer.Id = reader.GetSafeInt32(startingIndex++);
            aCustomer.Age = reader.GetSafeInt32(startingIndex++);
            aCustomer.FirstName = reader.GetSafeString(startingIndex++);
            aCustomer.LastName = reader.GetSafeString(startingIndex++);
            aCustomer.RegionId = reader.GetSafeInt32(startingIndex++);
            aCustomer.LoyaltyMember = reader.GetBoolean(startingIndex++);

            return aCustomer;
        }
    }

}

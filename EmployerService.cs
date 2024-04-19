using Sabio.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sabio.Models.Domain;
using Sabio.Models.Requests;
using Sabio.Data;
using Sabio.Models;

namespace Sabio.Services
{
    public class EmployerService : IEmployerService
    {
        IDataProvider _data = null;

        #region Data
        public EmployerService(IDataProvider data)
        {
            _data = data;
        }
        #endregion

        #region GetById
        public Employer GetById(int id)
        {
            string procName = "[dbo].[Employers_SelectById]";

            Employer employer = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                employer = MapSingleEmployer(reader);

            });
            return employer;
        }
        #endregion

        #region GetAll
        public List<Employer> GetAll()
        {
            string procName = "[dbo].[Employers_SelectAll]";
            List<Employer> list = null;

            _data.ExecuteCmd(procName, inputParamMapper: null, singleRecordMapper: delegate (IDataReader reader, short set)
            {
                Employer aEmployer = MapSingleEmployer(reader);

                if (list == null)
                {
                    list = new List<Employer>();
                }

                list.Add(aEmployer);
            });
            return list;
        }
        #endregion

        #region Add
        public int Add(EmployerAddRequest request)
        {
            int id = 0;

            string procName = "[dbo].[Employers_Insert]";

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

        private static void AddCommonParams(EmployerAddRequest request, SqlParameterCollection collection)
        {
            collection.AddWithValue("@Age", request.Age);
            collection.AddWithValue("@FirstName", request.FirstName);
            collection.AddWithValue("@LastName", request.LastName);
            collection.AddWithValue("@CompanyId", request.CompanyId);
        }

        #endregion

        #region Update
        public void Update(EmployerUpdateRequest updateRequest)
        {
            string procName = "[dbo].[Employers_Update]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                AddCommonParams(updateRequest, collection);

                collection.AddWithValue("@Id", updateRequest.Id);

            }, returnParameters: null);
        }

        #endregion

        #region Delete
        public void Delete(int Id)
        {
            string procName = "[dbo].[Employers_Delete]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@Id", Id);

            }, returnParameters: null);
        }

        #endregion

        #region Pagination
        public Paged<Employer> Pagination(int pageIndex, int pageSize)
        {
            Paged<Employer> pagedList = null;
            List<Employer> list = null;
            int totalCount = 0;
            string procName = "[dbo].[Employer_Pagination]";

            _data.ExecuteCmd(procName,
                delegate (SqlParameterCollection collection)
                {
                    collection.AddWithValue("@PageIndex", pageIndex);
                    collection.AddWithValue("@PageSize", pageSize);
                }, delegate (IDataReader reader, short set)
                {
                    Employer employer = MapSingleEmployer(reader);
                    totalCount = reader.GetSafeInt32(5);

                    if (list == null)
                    {
                        list = new List<Employer>();
                    }
                    list.Add(employer);
                });
            if (list != null)
            {
                pagedList = new Paged<Employer>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;

        }

        #endregion

        private static Employer MapSingleEmployer(IDataReader reader)
        {
            Employer aEmployer = new Employer();
            int startingIndex = 0;

            aEmployer.Id = reader.GetInt32(startingIndex++);
            aEmployer.Age = reader.GetInt32(startingIndex++);
            aEmployer.FirstName = reader.GetString(startingIndex++);
            aEmployer.LastName = reader.GetString(startingIndex++);
            aEmployer.CompanyId = reader.GetInt32(startingIndex++);

            return aEmployer;
        }
    }
}

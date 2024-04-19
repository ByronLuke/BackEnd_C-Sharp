using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class EmployersV3Service : IEmployersV3Service
    {
        private IDataProvider _dataProvider = null;

        #region Data
        public EmployersV3Service(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        #endregion

        #region GetAll
        public List<EmployersV3> GetAll()
        {
            List<EmployersV3> list = null;
            string procName = "[dbo].[EmployersV3SelectAll]";
            _dataProvider.ExecuteCmd(procName,
                inputParamMapper: null,
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    EmployersV3 employersEmployees = MapV3Employer(reader);

                    if (list == null)
                    {
                        list = new List<EmployersV3>();
                    }

                    list.Add(employersEmployees);

                });

            return list;
        }
        #endregion

        #region GetById
        public EmployersV3 GetById(int id)
        {
            string procName = "[dbo].[EmployersV3SelectById]";

            EmployersV3 employerV3 = null;

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                employerV3 = MapV3Employer(reader);
            });
            return employerV3;
        }
        #endregion

        #region Pagination
        public Paged<EmployersV3> Pagination(int pageIndex, int pageSize)
        {
            Paged<EmployersV3> pagedList = null;
            List<EmployersV3> list = null;
            int totalCount = 0;
            string procName = "[dbo].[EmployersV3Pagination]";

            _dataProvider.ExecuteCmd(procName,
                delegate (SqlParameterCollection collection)
                {
                    collection.AddWithValue("@PageIndex", pageIndex);
                    collection.AddWithValue("@PageSize", pageSize);
                }, delegate (IDataReader reader, short set)
                {
                    EmployersV3 aEmployer = MapV3Employer(reader);
                    totalCount = reader.GetSafeInt32(7);

                    if (list == null)
                    {
                        list = new List<EmployersV3>();
                    }
                    list.Add(aEmployer);
                });
            if (list != null)
            {
                pagedList = new Paged<EmployersV3>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;

        }
        #endregion

        #region Mapper
        private static EmployersV3 MapV3Employer(IDataReader reader)
        {
            EmployersV3 employerV3 = new EmployersV3();
            employerV3.Company = new Company();
            employerV3.Employees = new List<Employee>();

            int startingIndex = 0;

            employerV3.Id = reader.GetInt32(startingIndex++);
            employerV3.Age = reader.GetInt32(startingIndex++);
            employerV3.FirstName = reader.GetString(startingIndex++);
            employerV3.LastName = reader.GetString(startingIndex++);

            employerV3.Company.CompanyName = reader.GetString(startingIndex++);
            employerV3.Company.YearEstablished = reader.GetInt32(startingIndex++);

            employerV3.Employees = reader.DeserializeObject<List<Employee>>(startingIndex++);

            return employerV3;
        }
        #endregion
    }
}

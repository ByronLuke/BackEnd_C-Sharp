using Sabio.Data.Providers;
using Sabio.Data;
using Sabio.Models.Domain.Friends;
using Sabio.Models.Requests.Friends;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sabio.Models.Domain.Books;
using Sabio.Models.Requests.Books;

namespace Sabio.Services
{
    public class BookService
    {
        IDataProvider _data = null;

        public BookService (IDataProvider data)
        {
            _data = data;
        }
        public Book GetById(int id){
        string procName = "[dbo].[Books_SelectById]";

        Book book = null;

            _data.ExecuteCmd(procName, 
                delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@Id", id);
                
            }, delegate (IDataReader reader, short set)
            {
                book = MapSingleBook(reader);

            });
            return book;
    }
        public List <Book> GetAll()
        {
            string procName = "[dbo].[Books_SelectAll]";

            List <Book> booklist = null;

            _data.ExecuteCmd(procName, inputParamMapper: null,
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    Book book = MapSingleBook(reader);

                    if (booklist == null)
                    {
                        booklist = new List<Book>();
                    }
                    booklist.Add(book);
                });
                return booklist;
        }

        public int Add(AddBookRequest request)
        {
            int id = 0;
            string procName = "[dbo].[Books_Insert]";

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection collection)
                {
                    CommonParams(request, collection);

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

        public void Update(UpdateBookRequest updateRequest)
        {
            string procName = "[dbo].[Books_Update]";

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection collection)
                {
                    CommonParams(updateRequest, collection);
                    collection.AddWithValue("@Id", updateRequest.Id);

                }, returnParameters: null);
        }
       
        public void Delete(int id)
        {
            string procName = "[dbo].[Books_Delete]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@Id", id);

            }, returnParameters: null);
        }
        private static void CommonParams(AddBookRequest request, SqlParameterCollection collection)
        {
            collection.AddWithValue("@Title", request.Title);
            collection.AddWithValue("@Author", request.Author);
            collection.AddWithValue("@Genre", request.Genre);
            collection.AddWithValue("@YearReleased", request.YearReleased);
        }

        private static Book MapSingleBook(IDataReader reader)
        {
            Book book = new Book();
            int index = 0;
            book.Id = reader.GetSafeInt32(index++);
            book.Title = reader.GetString(index++);
            book.Author = reader.GetString(index++);
            book.Genre = reader.GetString(index++);
            book.YearReleased = reader.GetSafeInt32(index++);
            return book;
        }
    }

}

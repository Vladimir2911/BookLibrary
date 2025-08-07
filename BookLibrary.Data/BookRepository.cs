using BookLibrary.Data.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security;
using System.Text;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace BookLibrary.Data
{
    public class BookRepository
    {
        private readonly string _connectionString;

        public BookRepository(string configuration)
        {
            _connectionString = configuration;
        }

        public List<Book> GetAllBooks()
        {
            var books = new List<Book>();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SelectBooks", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        books.Add(MapReader(reader));
                    }
                }
            }
            return books;
        }

        public Book GetBookById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SelectBookById", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    return reader.Read() ? MapReader(reader) : null;
                }
            }
        }

        public string ConvertXmlToHtml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return string.Empty;

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                var sb = new StringBuilder();

                foreach (XmlNode node in doc.DocumentElement.SelectNodes("Section"))
                {
                    sb.AppendFormat("<p>Раздел {0}: {1}</p>",
                        node.Attributes["number"]?.Value,
                        node.Attributes["title"]?.Value);
                }
                return sb.ToString();
            }
            catch
            {
                return xml;
            }
        }

        public void InsertBook(Book book)
        {
            string xmlContent = ConvertTextToStructuredXml(book.Contents);
            book.Contents = xmlContent;
            ExecuteNonQuery("InsertBook", book);
        }

        private string ConvertTextToStructuredXml(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "<Contents></Contents>";

            string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            var xmlBuilder = new StringBuilder();
            xmlBuilder.Append("<Contents>");

            int sectionNumber = 1;

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (!string.IsNullOrEmpty(trimmedLine))
                {
                    xmlBuilder.AppendFormat(
                        "<Section number=\"{0}\" title=\"{1}\"/>",
                        sectionNumber++,
                        SecurityElement.Escape(trimmedLine)
                    );
                }
            }

            xmlBuilder.Append("</Contents>");

            return xmlBuilder.ToString();
        }

        public void UpdateBook(Book book)
        {
            string xmlContent = ConvertTextToStructuredXml(book.Contents);
            book.Contents = xmlContent;
            ExecuteNonQuery("UpdateBook", book, includeId: true);
        }

        public void DeleteBook(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("DeleteBook", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        private void ExecuteNonQuery(string procedure, Book book, bool includeId = false)
        {
            using (var connenction = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(procedure, connenction))
            {
                command.CommandType = CommandType.StoredProcedure;

                if (includeId)
                    command.Parameters.AddWithValue("@Id", book.Id);

                command.Parameters.AddWithValue("@Title", book.Title);
                command.Parameters.AddWithValue("@Author", book.Author);
                command.Parameters.AddWithValue("@PublishYear", book.PublishYear);
                command.Parameters.AddWithValue("@Summary", book.Summary);
                command.Parameters.AddWithValue("@Contents", (object)book.Contents);

                connenction.Open();

                command.ExecuteNonQuery();
            }
        }

        private Book MapReader(SqlDataReader reader)
        {
            return new Book
            {
                Id = (int)reader["Id"],
                Title = reader["Title"] as string,
                Author = reader["Author"] as string,
                PublishYear = reader["PublishYear"] != DBNull.Value ? (int)reader["PublishYear"] : 0,
                Summary = reader["Summary"] as string,
                Contents = reader["Contents"] != DBNull.Value ? reader["Contents"].ToString() : null
            };
        }
    }
}
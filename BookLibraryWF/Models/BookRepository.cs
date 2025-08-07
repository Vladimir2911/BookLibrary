using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security;
using System.Text;
using System.Xml;

namespace BookLibraryWF.Models
{
    public class BookRepository
    {
        private readonly string _connectionString;

        public BookRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["HomeLibraryDb"].ConnectionString;
        }

        public DataRow SelectBookById(int id)
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("SelectBookById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                dt.Load(cmd.ExecuteReader());
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        public void SaveBook(int? id, string title, string author, int publishYear, string summary, string htmlContent)
        {
            string xmlContent = ConvertTextToStructuredXml(htmlContent);

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(id.HasValue ? "UpdateBook" : "InsertBook", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (id.HasValue)
                    cmd.Parameters.AddWithValue("@Id", id.Value);

                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Author", author);
                cmd.Parameters.AddWithValue("@PublishYear", publishYear);
                cmd.Parameters.AddWithValue("@Summary", summary);

                SqlParameter xmlParam = cmd.Parameters.Add("@Contents", SqlDbType.Xml);
                xmlParam.Value = new System.Data.SqlTypes.SqlXml(XmlReader.Create(new StringReader(xmlContent)));

                conn.Open();
                cmd.ExecuteNonQuery();
            }
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

        public DataRow GetBookById(int id)
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("GetBookById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count > 0)
                {                   
                    string xmlContent = dt.Rows[0]["Contents"].ToString();
                    dt.Rows[0]["Contents"] = ConvertXmlToHtml(xmlContent);
                }

                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }
      
        private string ConvertXmlToHtml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return string.Empty;
            
            return xml
                .Replace("<Content>", "<p>")
                .Replace("</Content>", "</p>")
                .Replace("&#160;", "&nbsp;")
                .Replace("&amp;", "&");
        }     

        public DataTable GetAllBooks()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SelectBooks", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                
                if (!dt.Columns.Contains("Id"))
                {
                    throw new Exception("В результате запроса отсутствует столбец Id");
                }
            }

            return dt;
        }
 
        public void DeleteBook(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("DeleteBook", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
      

    }
}




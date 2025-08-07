using BookLibraryWF.Models;
using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;

namespace BookLibraryWF.Pages
{
    public partial class ViewBook : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out int bookId))
                {
                    LoadBook(bookId);
                }
                else
                {
                    Response.Redirect("BooksList.aspx");
                }
            }
        }

        private void LoadBook(int id)
        {
            var repository = new BookRepository();
            DataRow book = repository.SelectBookById(id);

            if (book != null)
            {
                ltTitle.Text = book["Title"].ToString();
                ltAuthor.Text = book["Author"].ToString();
                ltPublishYear.Text = book["PublishYear"].ToString();
                ltSummary.Text = book["Summary"].ToString();

                string contents = book["Contents"].ToString();
                if (contents.StartsWith("<Contents>"))
                {
                    ltContents.Text = FormatXmlForDisplay(contents);
                }
                else
                {
                    ltContents.Text = contents;
                }
            }
        }

        private string FormatXmlForDisplay(string xml)
        {
            try
            {
                // Сначала очистим XML от лишнего экранирования
                string cleanXml = HttpUtility.HtmlDecode(xml);
                cleanXml = cleanXml.Replace("&lt;", "<").Replace("&gt;", ">")
                                 .Replace("&quot;", "\"").Replace("&amp;", "&");

                var doc = new System.Xml.XmlDocument();
                doc.LoadXml(cleanXml);

                var sections = doc.SelectNodes("//Section");
                if (sections == null || sections.Count == 0)
                {
                    return "Содержание отсутствует";
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("<ul class='contents-list'>");

                foreach (System.Xml.XmlNode section in sections)
                {
                    if (section.Attributes?["title"] != null)
                    {
                        string title = section.Attributes["title"].Value;
                        // Очищаем title от возможных HTML-тегов
                        title = System.Text.RegularExpressions.Regex.Replace(title, "<.*?>", string.Empty);
                        sb.AppendFormat("<li>{0}</li>", HttpUtility.HtmlEncode(title));
                    }
                }

                sb.Append("</ul>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                // Логируем ошибку для отладки
                System.Diagnostics.Debug.WriteLine($"Ошибка парсинга XML: {ex.Message}");
                return "Ошибка отображения содержания";
            }
        }

        //private string FormatXmlContentsForDisplay(string xml)
        //{
        //    try
        //    {
        //        var doc = new System.Xml.XmlDocument();
        //        doc.LoadXml(xml);
        //        var sections = doc.SelectNodes("//Section");
        //        StringBuilder sb = new StringBuilder();

        //        foreach (System.Xml.XmlNode section in sections)
        //        {
        //            string title = section.Attributes["title"].Value;
        //            sb.AppendFormat("<p>Глава {0}: {1}</p>",
        //                section.Attributes["number"].Value,
        //                HttpUtility.HtmlEncode(title));
        //        }

        //        return sb.ToString();
        //    }
        //    catch
        //    {
        //        return HttpUtility.HtmlEncode(xml);
        //    }
        //}

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("BooksList.aspx");
        }
    }
}
using BookLibraryWF.Models;
using System;
using System.Data;
using System.Text;
using System.Web.UI;

namespace BookLibraryWF.Pages
{
    public partial class EditBook : Page
    {


        private void LoadBook(int id)
        {
            var repository = new BookRepository();
            DataRow book = repository.SelectBookById(id);

            if (book != null)
            {
                hfId.Value = id.ToString();
                txtTitle.Text = book["Title"].ToString();
                txtAuthor.Text = book["Author"].ToString();
                txtPublishYear.Text = book["PublishYear"].ToString();
                txtSummary.Text = book["Summary"].ToString();
                string contents = book["Contents"].ToString();
                // Преобразуем XML в простой текст для редактора
                txtContents.Value = ConvertXmlToText(contents);
            }
            else
            {
                Response.Redirect("BooksList.aspx");
            }
        }

        private string ConvertXmlToText(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml) || !xml.StartsWith("<Contents>"))
                return xml;

            try
            {
                var doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);
                var sections = doc.SelectNodes("//Section");
                StringBuilder sb = new StringBuilder();

                foreach (System.Xml.XmlNode section in sections)
                {
                    if (section.Attributes?["title"] != null)
                    {
                        sb.AppendLine(section.Attributes["title"].Value);
                    }
                }

                return sb.ToString().Trim();
            }
            catch
            {
                return xml;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var repository = new BookRepository();

                int? id = string.IsNullOrEmpty(hfId.Value) ? null : (int?)Convert.ToInt32(hfId.Value);

                repository.SaveBook(
                    id,
                    txtTitle.Text.Trim(),
                    txtAuthor.Text.Trim(),
                    Convert.ToInt32(txtPublishYear.Text),
                    txtSummary.Text.Trim(),
                    txtContents.Value
                );

                Response.Redirect("BooksList.aspx");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out int bookId))
                {
                    LoadBook(bookId);
                    ltTitle.Text = "Редактировать книгу";
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("BooksList.aspx");
        }
    }
}
using BookLibraryWF.Models;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BookLibraryWF.Pages
{
    public partial class BooksList : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindBooks();
            }
        }

        private void BindBooks()
        {
            var repository = new BookRepository();
            gvBooks.DataSource = repository.GetAllBooks();
            gvBooks.DataBind();
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("EditBook.aspx");
        }

        protected void gvBooks_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument == null) return;

            int bookId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "View":
                    Response.Redirect($"ViewBook.aspx?id={bookId}");
                    break;

                case "Edit":
                    Response.Redirect($"EditBook.aspx?id={bookId}");
                    break;

                case "CustomDelete":
                    DeleteBook(bookId);
                    BindBooks(); // Обновляем список
                    break;
            }
        }

        private void DeleteBook(int bookId)
        {
            try
            {
                var repository = new BookRepository();
                repository.DeleteBook(bookId);

                // Уведомление об успехе
                ScriptManager.RegisterStartupScript(this, GetType(), "showToast",
                    "toastr.success('Книга успешно удалена');", true);
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                System.Diagnostics.Debug.WriteLine($"Ошибка удаления: {ex.Message}");

                // Уведомление об ошибке
                ScriptManager.RegisterStartupScript(this, GetType(), "showError",
                    $"toastr.error('Ошибка: {ex.Message}');", true);
            }
        }
    }
}
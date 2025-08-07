<%@ Page Title="Редактирование книги" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="EditBook.aspx.cs" 
    Inherits="BookLibraryWF.Pages.EditBook" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2><asp:Literal ID="ltTitle" runat="server" Text="Добавить книгу" /></h2>
        
        <asp:HiddenField ID="hfId" runat="server" />
        
        <div class="form-group">
            <label>Название:</label>
            <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" MaxLength="50" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTitle"
                ErrorMessage="Введите название" CssClass="text-danger" />
        </div>
        
        <div class="form-group">
            <label>Автор:</label>
            <asp:TextBox ID="txtAuthor" runat="server" CssClass="form-control" MaxLength="20" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAuthor"
                ErrorMessage="Введите автора" CssClass="text-danger" />
        </div>
        
        <div class="form-group">
            <label>Год издания:</label>
            <asp:TextBox ID="txtPublishYear" runat="server" CssClass="form-control" TextMode="Number" />
            <asp:RangeValidator runat="server" ControlToValidate="txtPublishYear" Type="Integer"
                MinimumValue="1000" MaximumValue="2100" ErrorMessage="Некорректный год" 
                CssClass="text-danger" />
        </div>
        
        <div class="form-group">
            <label>Краткое описание:</label>
            <asp:TextBox ID="txtSummary" runat="server" CssClass="form-control" 
                TextMode="MultiLine" Rows="3" />
        </div>
        
        <div class="form-group">
            <label>Содержание:</label>
            <textarea id="txtContents" runat="server" class="form-control" style="min-height:300px;"></textarea>
        </div>
        
        <div class="form-group">
            <asp:Button ID="btnSave" runat="server" Text="Сохранить" 
                OnClick="btnSave_Click" CssClass="btn btn-primary" />
            <asp:Button ID="btnCancel" runat="server" Text="Отмена" 
                OnClick="btnCancel_Click" CssClass="btn btn-secondary ml-2" CausesValidation="false" />
        </div>
    </div>

    <!-- TinyMCE Editor -->

    <script src="https://cdn.tiny.cloud/1/pbqiz8f8ciy50upitvq50eso05txanznsl0fhuuc1eu4vf5z/tinymce/6/tinymce.min.js" referrerpolicy="origin"></script>
    <<script>
         document.addEventListener('DOMContentLoaded', function () {
             tinymce.init({
                 selector: 'textarea.tinymce-editor',
                 plugins: 'lists',
                 toolbar: 'undo redo | bold italic | bullist numlist',
                 menubar: false,
                 statusbar: false,
                 branding: false,
                 forced_root_block: false,
                 setup: function (editor) {
                     editor.on('init', function () {
                         // Разблокируем редактор после инициализации
                         editor.setMode('design');
                         console.log('TinyMCE initialized successfully');
                     });
                 }
             });
         });
    </script>
</asp:Content>
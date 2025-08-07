<%@ Page Title="Просмотр книги" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="ViewBook.aspx.cs" Inherits="BookLibraryWF.Pages.ViewBook" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2>Просмотр книги</h2>
        
        <div class="card">
            <div class="card-body">
                <h4 class="card-title"><asp:Literal ID="ltTitle" runat="server" /></h4>
                <h6 class="card-subtitle mb-2 text-muted">
                    Автор: <asp:Literal ID="ltAuthor" runat="server" />
                </h6>
                <p class="card-text">
                    <strong>Год издания:</strong> <asp:Literal ID="ltPublishYear" runat="server" />
                </p>
                <div class="mb-3">
                    <h5>Описание:</h5>
                    <asp:Literal ID="ltSummary" runat="server" />
                </div>
                <div class="book-contents">
                    <h5>Содержание:</h5>
                    <asp:Literal ID="ltContents" runat="server" />
                </div>
                <asp:Button ID="btnBack" runat="server" Text="Назад к списку" 
                    OnClick="btnBack_Click" CssClass="btn btn-secondary mt-3" />
            </div>
        </div>
    </div>
</asp:Content>
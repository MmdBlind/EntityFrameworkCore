using EntityFrameworkCore.Models.ViewModels;

namespace EntityFrameworkCore.Models.Repository
{
    public interface IBooksRepository
    {
        List<TreeViewCategory> GetAllCategories();

        void BindSubCategories(TreeViewCategory category);

        List<BooksIndexViewModel> GetAllBooks(string title, string ISBN, string Language, string Publisher, string Author, string Translator, string Category);
    }
}

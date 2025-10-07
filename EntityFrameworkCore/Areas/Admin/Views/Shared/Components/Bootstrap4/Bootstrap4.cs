using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace EntityFrameworkCore.Areas.Admin.Views.Shared.Components.Bootstrap4;

public class Bootstrap4 : ViewComponent
{

    public async Task<IViewComponentResult> InvokeAsync(PagingList<EntityFrameworkCore.Models.ViewModels.BooksIndexViewModel> models)
    {
        await Task.CompletedTask;
        return View(models);
    }
}

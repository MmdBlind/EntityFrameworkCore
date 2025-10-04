﻿using EntityFrameworkCore.Models.ViewModels;
using NuGet.Protocol.Core.Types;

namespace EntityFrameworkCore.Models.Repository
{
    public class BooksRepository
    {
        private readonly BookShopContext _context;
        public BooksRepository(BookShopContext context)
        {
            _context = context;
        }

        public List<TreeViewCategory> GetAllCategories()
        {
            var Categories = (from c in _context.Categories
                              where (c.ParentCategoryID == null)
                              select new TreeViewCategory { CategoryID = c.CategoryID, CategoryName = c.CategoryName }).ToList();
            foreach (var item in Categories)
            {
                 BindSubCategories(item);
            }
            return Categories;
        }

        public void BindSubCategories(TreeViewCategory category)
        {
            var SubCategories = (from c in _context.Categories
                                 where (c.ParentCategoryID == category.CategoryID)
                                 select new TreeViewCategory { CategoryID = c.CategoryID, CategoryName = c.CategoryName }).ToList();
            foreach (var item in SubCategories)
            {
                BindSubCategories(item);
                category.SubCategory.Add(item);
            }

        }
    }
}

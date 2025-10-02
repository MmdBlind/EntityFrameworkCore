namespace EntityFrameworkCore.Models.ViewModels
{
    
    public class TreeViewCategory
    {
        public TreeViewCategory() 
        {
            SubCategory=new List<TreeViewCategory>();
        }

        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public  List<TreeViewCategory> SubCategory { get; set; }
    }
}

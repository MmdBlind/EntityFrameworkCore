namespace EntityFrameworkCore.Models.ViewModels
{
    public class ControllerViewModel
    {
        public string AreaName { get; set; }

        public IList<Attribute> ControllerAttributes { get; set; }

        public string ControllerId => $"{AreaName}:{ControllerName}";

        public string ControllerName { get; set; }

        public string ControllerDisplayName { get; set; }

        public IList<ActionViewModel> MvcActions { get; set; } = new List<ActionViewModel>();
    }
}

namespace EntityFrameworkCore.Models.ViewModels
{
    public class ActionViewModel
    {
        public IList<Attribute> ActionAttributes { get; set; }

        public string ActionDisplayName { get; set; }

        public string ActionId => $"{ControllerID}:{ActionName}";

        public string ActionName { get; set; }

        public string ControllerID { get; set; }

        public bool IsSecuredAction { get; set; }
    }
}

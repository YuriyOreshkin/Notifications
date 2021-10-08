namespace Notifications.WebUI.Models
{
    public class EmployeeTreeViewModel: EmployeeViewModel
    {
       public bool hasChildren { get; set; }
       public long[] parents { get; set; }
    }
}
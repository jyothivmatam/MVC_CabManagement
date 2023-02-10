namespace MVC_CabServices.Models
{
    public class CrudStatus
    {
        public bool Status { get; set; }
        public string? Message { get; set; }
        public int? id { get; set; }
        public int? checkAdminOrUser { get; set; }
    }
}

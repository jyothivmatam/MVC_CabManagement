namespace MVC_CabServices.Models
{
    public class TbCab
    {
        public int Cabid { get; set; }

        public string? RegistrationNun { get; set; }

        public int? CabTypeId { get; set; }

        public int? UserId { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int? Status { get; set; }

    }
}

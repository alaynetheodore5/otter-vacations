using System.ComponentModel.DataAnnotations;

namespace OtterVacations.Models
{
    public class Association
    {
        [Key]
        public int AssociationId { get; set; }
        public int OtterId { get; set; }
        public int VacationId { get; set; }
        public Otter Traveller { get; set; }
        public Vacation Trip { get; set; }
    }
}
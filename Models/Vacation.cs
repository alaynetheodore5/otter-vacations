using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OtterVacations.Models
{
    public class Vacation
    {
        [Key]
        public int VacationId { get; set; }

        [Required(ErrorMessage="Destination is required")]
        public string Destination { get; set; }

        [Required(ErrorMessage="Image Url is required")]
        public string ImgUrl { get; set; }

        [Required(ErrorMessage="Start Date is required")]
        [DataType(DataType.Date)]
        [FutureDate]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage="Duration is required")]
        [Range(1,90)]
        public int NumberDays { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int OtterId { get; set; }
        //this is the foreign key, must name what is primary key of the other model

        public Otter Planner { get; set; }
        // a vacation can have only one otter that plans it (not stored in DB)

        public List<Association> GroupMembers { get; set; }
        // list associating the otters
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OtterVacations.Models
{
    public class Otter
    {
        [Key]
        public int OtterId { get; set; }

        [Required(ErrorMessage="Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage="Birthday is required")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [Required(ErrorMessage="Type is required")]
        public string Type { get; set; }

        [Required(ErrorMessage="HasKids is required")]
        public bool HasKids { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<Vacation> PlannedVacations { get; set; }
        //navigational property - an otter can plan many vacations

        public List<Association> Tagalongs { get; set; }
        // list associating the vacations w. otters
    }
}
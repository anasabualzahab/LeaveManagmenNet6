﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace LeaveManagmen.Common.Models
{
    public class LeaveRequestCreateVM : IValidatableObject
    {

        [Required]
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        [Required]
        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        [Required]
        public int LeaveTypeId { get; set; }

        public SelectList? LeaveTypes { get; set; }

        [Display(Name = "Request Comment")]
        public string? RequestComments { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate > EndDate)
            {
                yield return new
                    ValidationResult("The Start Date must be Before End Date", new[] { nameof(StartDate), nameof(EndDate) });
            }
            if (RequestComments?.Length > 250)
            {
                yield return new
                    ValidationResult("Comment are too long", new[] { nameof(RequestComments) });
            }
        }
    }
}

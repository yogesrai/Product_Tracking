using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Product_Tracking.Models.ViewModel
{
    public class Deals : IValidatableObject
    {
        public int DealsId { get; set; }
        [Required(ErrorMessage = "DealsName Required!")]
        public string DealsName { get; set; }
        [Required(ErrorMessage = "DealsDescription Required!")]
        public string DealsDiscription { get; set; }
        [Required(ErrorMessage = "Discountpercent Required!")]
        public Nullable<int> DiscountPercent { get; set; }
        [Required(ErrorMessage = "StartDate Required!")]
        public Nullable<System.DateTime> StartDate { get; set; }
        [Required(ErrorMessage = "EndDate Required!")]
        public Nullable<System.DateTime> EndDate { get; set; }

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            if (EndDate < StartDate)
            {
                yield return new ValidationResult("EndDate must be greater than StartDate!");
            }
            if (StartDate > EndDate)
            {
                yield return new ValidationResult("StartDate must be less than EndDate!");
            }
        }
    }
}
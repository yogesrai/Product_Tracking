using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Product_Tracking.Models.ViewModel
{
    public class ProductViewModel : IValidatableObject
    {
        public int ProductId { get; set; }
        [Required(ErrorMessage = "ProductName Required!")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "ProducDescription Required!")]
        public string ProductDescription { get; set; }
        
        public string StoreName { get; set; }
        public string DealsName { get; set; }
        public string CategoryName { get; set; }
        public string StatusName { get; set; }
        [Required(ErrorMessage = "ProductPrackingDate Required!")]
        public Nullable<System.DateTime> ProductPackingDate { get; set; }
        [Required(ErrorMessage = "ProductExpireDate Required!")]
        public Nullable<System.DateTime> ProductExpireDate { get; set; }
        public Nullable<int> StoreId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<int> DealsId { get; set; }
        public Nullable<int> StatusId { get; set; }
        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            if (ProductExpireDate < ProductPackingDate)
            {
                yield return new ValidationResult("ProductPackingDate must be greater than ProductExpireDate!");
            }
            if (ProductPackingDate > ProductExpireDate)
            {
                yield return new ValidationResult("ProductPackingDate must be less than ProductExpireDate!");
            }
            //if(StoreId == 0)
            //{
            //    yield return new ValidationResult("Store name must be filled!");
            //}
            //if (DealsId == 0)
            //{
            //    yield return new ValidationResult("Store name must be filled!");
            //}
            //if (CategoryId == 0)
            //{
            //    yield return new ValidationResult("Store name must be filled!");
            //}
        }
    }
}
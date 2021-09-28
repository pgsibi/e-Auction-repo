using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EAuction.Seller.Product.Contracts.Models
{
    /// <summary>
    /// Product Model
    /// </summary>
    public class ProductModel : IValidatableObject
    {
        private List<string> _productCategories = new List<string> { "Painting", "Sculptor", "Ornament" };
        [Required]
        public string Category { get; set; }
        [Required, StringLength(maximumLength: 30, ErrorMessage = "Product Name is not null, min 5 and max 30 characters", MinimumLength = 5)]
        public string ProductName { get; set; }
        [Required]
        public string ShortDescription { get; set; }
        [Required]
        public string DetailedDescription { get; set; } 
        [RegularExpression("^[0-9]*$", ErrorMessage = "Starting price should be number.")]
        public string StartingPrice { get; set; }
        [Required]
        public DateTime BidEndDate { get; set; }

        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (!_productCategories.Contains(Category))
                results.Add(new ValidationResult("Invalid Category.", new[] { nameof(Category) }));

            if (BidEndDate > DateTime.Now)
                results.Add(new ValidationResult("Bid end date should be future date.", new[] { nameof(BidEndDate) }));

            return results;
        }
    }


}

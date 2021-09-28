using System;
using System.ComponentModel.DataAnnotations;

namespace EAuction.Buyer.Contracts.Models
{
    public class AddAuctionRequestModel
    {
        public Guid CorrelationId { get; set; }        
        [Required, StringLength(maximumLength: 30, ErrorMessage = "First Name is not null", MinimumLength = 5)]
        public string FirstName { get; set; }                
        [Required, StringLength(maximumLength: 25, ErrorMessage = "Last Name is not null", MinimumLength = 5)]
        public string LastName { get; set; }        
        public string Address { get; set; }        
        public string City { get; set; }        
        public string State { get; set; }
        public string Pin { get; set; }        
        [Phone]
        public string Phone { get; set; }        
        [EmailAddress]
        public string Email { get; set; }
        public string ProductId { get; set; }
        public double BidAmount { get; set; }
    }
}

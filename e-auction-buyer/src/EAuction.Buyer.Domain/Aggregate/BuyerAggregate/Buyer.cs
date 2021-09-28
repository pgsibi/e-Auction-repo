using System;
using System.Collections.Generic;

namespace EAuction.Buyer.Domain.Aggregate.BuyerAggregate
{
    public class Buyer : Entity, IAggregateRoot
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Address { get; private set; } 
        public string State { get; private set; }
        public string City { get; private set; }
        public string Pin { get; private set; }
        public string Phone { get; private set; }
        public string Email { get; private set; }
        public List<AuctionItem> Bids { get; private set; }

        public Buyer()
        {
            Bids = new List<AuctionItem>();
            Id = Guid.NewGuid().ToString();
        }

        public Buyer(string identity, string firstname, string lastname, string address, string state, string city, string pin, string phone, string email) : this(firstname, lastname, address, state, city, pin, phone, email)
        {
            Id = !string.IsNullOrWhiteSpace(identity) ? identity : throw new ArgumentNullException(nameof(identity));

        }

        public Buyer(string firstname, string lastname, string address,  string state, string city, string pin, string phone, string email) : this()
        {
            FirstName = !string.IsNullOrWhiteSpace(firstname) ? firstname : throw new ArgumentNullException(nameof(firstname));
            LastName = lastname;
            Address = address;
            City = city;
            State = state;
            Pin = pin;
            Phone = !string.IsNullOrWhiteSpace(phone) ? phone : throw new ArgumentNullException(nameof(phone));
            Email = !string.IsNullOrWhiteSpace(email) ? email : throw new ArgumentNullException(nameof(email));
        }


    }
}

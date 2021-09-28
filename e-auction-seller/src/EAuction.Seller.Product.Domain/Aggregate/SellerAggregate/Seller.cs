using System;
using System.Collections.Generic;

namespace EAuction.Seller.Product.Domain.Aggregate.SellerAggregate
{
    public class Seller : Entity, IAggregateRoot
    {
        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string Address { get; private set; }

        public string City { get; private set; }

        public string State { get; private set; }
        public string Pin { get; private set; }

        public string Phone { get; private set; }

        public string Email { get; private set; }

        public List<Product> Products { get; private set; }

        public Seller()
        {
            Products = new List<Product>();
            Id = Guid.NewGuid().ToString();
        }

        public Seller(string identity, string fname, string lname, string address, string city, string state, string pin, string phone, string email) : this(fname, lname, address, city, state, pin, phone, email)
        {
            Id = !string.IsNullOrWhiteSpace(identity) ? identity : throw new ArgumentNullException(nameof(identity));

        }

        public Seller(string fname, string lname, string address, string city, string state, string pin, string phone, string email) : this()
        {
            FirstName = !string.IsNullOrWhiteSpace(fname) ? fname : throw new ArgumentNullException(nameof(fname));
            LastName = lname;
            Address = address;
            City = city;
            State = state;
            Pin = pin;
            Phone = !string.IsNullOrWhiteSpace(phone) ? phone : throw new ArgumentNullException(nameof(phone));
            Email = !string.IsNullOrWhiteSpace(email) ? email : throw new ArgumentNullException(nameof(email));

        }


    }

}

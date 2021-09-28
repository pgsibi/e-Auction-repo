﻿using System;

namespace EAuction.Seller.Product.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public DomainException(string message)
            : base(message)
        { }


    }
}

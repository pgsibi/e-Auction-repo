using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Seller.Product.Contracts.Events
{
    /// <summary>
    /// ValueEntered message
    /// </summary>
    public interface IValueEntered
    {
        /// <summary>
        /// 
        /// </summary>
        string Value { get; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Auction.Domain.Aggregate
{
    public abstract class Entity
    {
        int? _requestedHashCode;
        string _id;
        public virtual string Id
        {
            get
            {
                return _id;
            }
            protected set
            {
                _id = value;
            }
        }

        public DateTime LastUpdatedTime { get; set; } = DateTime.Now;
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public EntityStatus Status { get; set; } = EntityStatus.Active;




        public bool IsTransient()
        {
            return this.Id == default(string);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            Entity item = (Entity)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id == this.Id;
        }

        public object GetId()
        {
            return this.Id;
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Id.GetHashCode() ^ 31;

                return _requestedHashCode.Value;
            }
            else
                return base.GetHashCode();

        }
        public static bool operator ==(Entity left, Entity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
    }
}

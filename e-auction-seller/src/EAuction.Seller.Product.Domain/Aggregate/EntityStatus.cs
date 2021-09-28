namespace EAuction.Seller.Product.Domain.Aggregate
{
    public class EntityStatus : Enumeration
    {
        public static EntityStatus Active = new EntityStatus(1, nameof(Active));
        public static EntityStatus Deleted = new EntityStatus(2, nameof(Deleted));
        public static EntityStatus Suspended = new EntityStatus(3, nameof(Suspended));

        public EntityStatus(int id, string name) : base(id, name)
        {
        }
    }
}

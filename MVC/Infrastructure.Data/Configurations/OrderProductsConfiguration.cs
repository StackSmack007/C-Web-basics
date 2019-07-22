namespace Infrastructure.Data.Configurations
{
    using Infrastructure.Models.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class OrderProductsConfiguration : IEntityTypeConfiguration<OrderProduct>
    {
        public void Configure(EntityTypeBuilder<OrderProduct> builder)
        {
                builder.HasKey(x => new { x.OrderID, x.ProductID });
                builder.HasOne(op => op.Order).WithMany(o => o.OrderProducts).HasForeignKey(op => op.OrderID);
                builder.HasOne(op => op.Product).WithMany(p => p.ProductOrders).HasForeignKey(op => op.ProductID);
        }
    }
}
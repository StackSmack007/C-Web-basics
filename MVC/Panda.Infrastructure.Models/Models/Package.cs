namespace Panda.Indfrastructure.Models.Models
{
    using Panda.Infrastructure.Models.Enums;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Package:BaseEntity<int>
    {
        public Package()
        {
            Status = Status.Pending;
        }

        [MaxLength(10240)]
        public string Description { get; set; }

        public decimal Weight { get; set; }

        [Required,MaxLength(512)]
        public string ShippingAddress { get; set; }

        public virtual Status Status { get; set; }

        public DateTime? EstimatedDeliveryDate { get; set; }

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public int? ReceiptId { get; set; }
        [ForeignKey(nameof(ReceiptId))]
        public virtual Receipt Receipt{get;set;}
    }
}
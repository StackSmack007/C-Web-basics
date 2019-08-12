namespace Panda.Indfrastructure.Models.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Receipt : BaseEntity<int>
    {
        public decimal Fee { get; set; }

        public DateTime IssuedOn { get; set; }

        public int RecipientId { get; set; }
        [ForeignKey(nameof(RecipientId))]
        public virtual User Recipient { get; set; }

        public int PackageId { get; set; }
        [ForeignKey(nameof(PackageId))]
        public virtual Package Package { get; set; }
    }
}
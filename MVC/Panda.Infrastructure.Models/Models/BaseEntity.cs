namespace Panda.Indfrastructure.Models.Models
{
using System.ComponentModel.DataAnnotations;
    public abstract class BaseEntity<T>
    {
        [Key]
        public T Id { get; set; }
    }
}
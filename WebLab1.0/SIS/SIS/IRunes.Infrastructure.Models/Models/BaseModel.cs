namespace IRunes.Infrastructure.Models.Models
{
using System.ComponentModel.DataAnnotations;
    public abstract  class BaseModel<T>
    {
        [Key]
        public T Id { get; set; }
    }
}
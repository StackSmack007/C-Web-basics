﻿namespace Infrastructure.Models.Models
{
  public abstract class BaseEntity<T>
    {
        public T Id { get; set; }
    }
}
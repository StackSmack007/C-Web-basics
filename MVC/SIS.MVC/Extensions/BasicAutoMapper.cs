namespace SIS.MVC.Extensions
{
    using System;
    using System.Linq;
    using System.Reflection;
    public static class BasicAutoMapper
    {
        public static T MapTo<T>(this object obj)
                       where T : new()
        {
            Type typeNeeded = typeof(T);
            PropertyInfo[] propertiesNeeded = typeNeeded.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            Type typeProvided = obj.GetType();
            PropertyInfo[] propertiesProvided = typeProvided.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            T result = (T)Activator.CreateInstance(typeNeeded);

            foreach (PropertyInfo property in propertiesProvided)
            {
                string sourseName = property.Name;
                Type sourseType = property.PropertyType;

                PropertyInfo destinationProperty = propertiesNeeded.FirstOrDefault(x => x.Name.ToLower() == sourseName.ToLower());

                if (destinationProperty is null)
                {
                    continue;
                }
                destinationProperty.SetValue(result, property.GetValue(obj));
            }

            return result;
        }
    }
}
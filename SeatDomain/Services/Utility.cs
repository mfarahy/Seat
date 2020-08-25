using Exir.Framework.Common;
using System;
using System.Collections.Generic;

namespace SeatDomain.Services
{
    public static class Utility
    {
        public static string RemoveZeroFromStartIfExist(this string mobileNo)
        {
            if (string.IsNullOrWhiteSpace(mobileNo)) return mobileNo;
            return mobileNo.TrimStart('0');
        }
        public static string AddZeroToStart(this string mobileNo)
        {
            if (string.IsNullOrWhiteSpace(mobileNo) || mobileNo.StartsWith("0")) return mobileNo;
            return "0" + mobileNo.Trim();
        }

        public static T Parse<T>(string n, string v, List<string> errors)
        {
            Type type = typeof(T);
            if (String.IsNullOrEmpty(v) && Nullable.GetUnderlyingType(type) != null)
                return Typing.ChangeType<T>(null);

            try
            {
                return Typing.ChangeType<T>(v);
            }
            catch (Exception ex)
            {
                errors.Add(n + ": " + ex.Message);
            }
            return default(T);
        }
    }

    public class PropertyCopier<TSource, TDestination> where TSource : class
                                            where TDestination : class
    {
        public static void Copy(TSource source, TDestination destination)
        {
            var sourceProperties = source.GetType().GetProperties();
            var destinationProperties = destination.GetType().GetProperties();

            foreach (var sourceProperty in sourceProperties)
            {
                foreach (var destinationProperty in destinationProperties)
                {
                    if (sourceProperty.Name == destinationProperty.Name && sourceProperty.PropertyType == destinationProperty.PropertyType && IsPrimitiveType(sourceProperty.PropertyType))
                    {
                        destinationProperty.SetValue(destination, sourceProperty.GetValue(source));
                        break;
                    }
                }
            }
        }

        private static bool IsPrimitiveType(Type propertyType)
        {
            return propertyType.IsPrimitive || propertyType.Namespace.Equals("System") || propertyType.Namespace == null;
        }
    }
}

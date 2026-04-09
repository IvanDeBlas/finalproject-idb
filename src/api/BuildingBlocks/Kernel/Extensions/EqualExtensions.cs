namespace WePlayRises.BuildingBlocks.Kernel.Extensions
{
    public static class EqualExtensions
    {
        public static bool IsEqualTo<T>(this T value1, T value2)
        {
            if (typeof(T) == typeof(string))
            {
                var str1 = value1 as string;
                var str2 = value2 as string;
                return string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2) || str1 == str2;
            }
            else if (value1 == null && value2 == null)
            {
                return true;
            }
            else if (value1 == null || value2 == null)
            {
                return false;
            }
            else
            {
                return EqualityComparer<T>.Default.Equals(value1, value2);
            }
        }
    }
}

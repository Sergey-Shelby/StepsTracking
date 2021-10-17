using StepsTracking.Model;

namespace StepsTracking.Common
{
    public static class Extensions
    {
        public static void Populate(this UserDay[] usersItem, int value)
        {
            foreach (var item in usersItem)
            {
                item.Day = value;
            }
        }
    }
}

namespace Shared.Kernel.Constants
{
    public static class CrmCustomEntityStateCodes
    {
        public const int Active = 0;
        public const int Inactive = 1;
        public static string GetDisplayName(int stateCode)
        {
            switch (stateCode)
            {
                case Active:
                    return "Active";
                case Inactive:
                    return "Inactive";
                default:
                    return "Unknown";
            }
        }
    }
}

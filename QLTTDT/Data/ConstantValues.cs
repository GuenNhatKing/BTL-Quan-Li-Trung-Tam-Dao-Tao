namespace QLTTDT.Data
{
    public class ConstantValues
    {
        public static readonly int COOKIE_EXPIRY_DAYS = 3;
        public static readonly int SALT_SIZE = 32;
        public static readonly int RANDOM_SIZE = 32;
        public static readonly DateTime DAYS_USE_FOR_RANDOM = new DateTime(2024, 1, 1);
    }
}

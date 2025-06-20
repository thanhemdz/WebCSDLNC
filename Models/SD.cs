namespace BuiTanThanh_2280602928_W3.Models
{
    public static class SD
    {
        // Roles
        public const string Role_Admin = "admin";
        public const string Role_Staff = "staff";
        public const string Role_Customer = "customer";

        // Product status
        public const string Product_Active = "active";
        public const string Product_Inactive = "inactive";

        // Table status
        public const string Table_Available = "available";
        public const string Table_Occupied = "occupied";
        public const string Table_Reserved = "reserved";

        // Order status
        public const string Order_Pending = "pending";
        public const string Order_Preparing = "preparing";
        public const string Order_Done = "done";
        public const string Order_Cancelled = "cancelled";

        // Payment status
        public const string Payment_Unpaid = "unpaid";
        public const string Payment_Paid = "paid";

        // Payment methods
        public const string PaymentMethod_Cash = "cash";
        public const string PaymentMethod_Card = "card";
        public const string PaymentMethod_Momo = "momo";
        public const string PaymentMethod_Banking = "banking";
    }
}

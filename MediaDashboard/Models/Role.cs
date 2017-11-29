namespace MediaDashboard.Models
{
    public static class Role
    {
        public const string Administrator = "Administrator";
        public const string Operator = "Operator";

        // helper strings to use with authorize attribute
        public const string OperatorOrHigher = Operator + "," + Administrator;
    }
}

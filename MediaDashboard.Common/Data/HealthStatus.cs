namespace MediaDashboard.Common.Data
{
    public enum HealthStatus
    {
        Ignore = 0, // Gray
        Healthy = 1, // Green
        Warning = 2, //Orange/Yellow
        Critical = 3, // Red
        None = 4, // Black
    }
}

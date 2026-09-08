namespace WorkspaceHub.Application.DTOs.Dashboard.SuperAdmin
{
    public class RevenueStatisticsDto
    {
        public decimal Total { get; set; }
        public decimal Today { get; set; }
        public decimal ThisMonth { get; set; }
        public decimal TotalOverpayment { get; set; }
        public decimal ThisMonthOverpayment { get; set; }
    }
}

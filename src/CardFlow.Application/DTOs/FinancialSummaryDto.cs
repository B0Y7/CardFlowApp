namespace CardFlow.Application.DTOs;

// DTO chứa số liệu tài chính tổng hợp toàn bộ các đợt thẻ trong hệ thống
public record FinancialSummaryDto(
    int TotalBatches,
    int TotalCards,
    decimal TotalBlankCost,
    decimal TotalProcessingCost,
    decimal TotalCost,
    decimal TotalRevenue,
    decimal NetProfit,
    decimal ProfitMarginPercentage
);

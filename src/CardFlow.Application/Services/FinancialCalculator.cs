using CardFlow.Application.DTOs;
using CardFlow.Application.Interfaces;
using CardFlow.Domain.Entities;

namespace CardFlow.Application.Services;

// Hiện thực việc tính toán tài chính.
// Tách riêng logic tính tiền ra khỏi giao diện để dễ bảo trì và tái sử dụng.
public class FinancialCalculator : IFinancialCalculator
{
    public FinancialSummaryDto CalculateSummary(IEnumerable<CardBatch> batches)
    {
        var batchList = batches?.ToList() ?? new List<CardBatch>();

        int totalBatches = batchList.Count;
        int totalCards = batchList.Sum(b => b.TotalQuantity);

        decimal totalBlankCost = batchList.Sum(b => b.TotalBlankCost);
        decimal totalProcessingCost = batchList.Sum(b => b.TotalProcessingCost);
        decimal totalCost = totalBlankCost + totalProcessingCost;
        decimal totalRevenue = batchList.Sum(b => b.TotalRevenue);
        decimal netProfit = totalRevenue - totalCost;

        decimal margin = totalRevenue > 0
            ? Math.Round((netProfit / totalRevenue) * 100, 2)
            : 0m;

        return new FinancialSummaryDto(
            totalBatches,
            totalCards,
            totalBlankCost,
            totalProcessingCost,
            totalCost,
            totalRevenue,
            netProfit,
            margin
        );
    }

    public FinancialSummaryDto CalculateBatchSummary(CardBatch batch)
    {
        ArgumentNullException.ThrowIfNull(batch);
        return CalculateSummary(new[] { batch });
    }
}

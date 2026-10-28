using CardFlow.Application.DTOs;
using CardFlow.Domain.Entities;

namespace CardFlow.Application.Interfaces;

// Bộ tính toán tài chính độc lập, chỉ làm nhiệm vụ tổng hợp Thu - Chi - Lãi
public interface IFinancialCalculator
{
    FinancialSummaryDto CalculateSummary(IEnumerable<CardBatch> batches);
    FinancialSummaryDto CalculateBatchSummary(CardBatch batch);
}

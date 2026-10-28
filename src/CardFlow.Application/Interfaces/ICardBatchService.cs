using CardFlow.Domain.Entities;

namespace CardFlow.Application.Interfaces;

// Request DTO cho việc tạo đợt nhập thẻ mới
public record CreateBatchRequest(
    string BatchCode,
    string BatchName,
    string CustomerName,
    string CardTypeKey,
    int Quantity,
    string CodePrefix,
    string HolderNamePattern,
    decimal? CustomBlankCost = null,
    decimal? CustomSellingPrice = null,
    string Notes = ""
);

// Service điều phối các nghiệp vụ liên quan đến đợt thẻ
public interface ICardBatchService
{
    Task<IReadOnlyList<CardBatch>> GetAllBatchesAsync();
    Task<CardBatch?> GetBatchByIdAsync(Guid id);
    Task<CardBatch> CreateBatchAsync(CreateBatchRequest request);
    Task DeleteBatchAsync(Guid batchId);
}

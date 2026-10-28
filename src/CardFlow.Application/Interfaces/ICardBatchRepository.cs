using CardFlow.Domain.Entities;

namespace CardFlow.Application.Interfaces;

// Interface lưu trữ các đợt thẻ.
// Tầng Application chỉ phụ thuộc vào interface này, tầng Infrastructure sẽ hiện thực cụ thể (InMemory, DB...).
public interface ICardBatchRepository
{
    Task<IReadOnlyList<CardBatch>> GetAllAsync();
    Task<CardBatch?> GetByIdAsync(Guid id);
    Task<CardBatch?> GetByCodeAsync(string batchCode);
    Task AddAsync(CardBatch batch);
    Task UpdateAsync(CardBatch batch);
    Task DeleteAsync(Guid id);
}

using CardFlow.Application.Interfaces;
using CardFlow.Domain.Entities;

namespace CardFlow.Application.Services;

// Xử lý logic tạo đợt, sinh danh sách thẻ theo loại và lưu trữ.
// Phụ thuộc vào interface ICardBatchRepository và ICardTypeRegistry (DIP).
public class CardBatchService : ICardBatchService
{
    private readonly ICardBatchRepository _repository;
    private readonly ICardTypeRegistry _cardTypeRegistry;

    public CardBatchService(
        ICardBatchRepository repository,
        ICardTypeRegistry cardTypeRegistry)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _cardTypeRegistry = cardTypeRegistry ?? throw new ArgumentNullException(nameof(cardTypeRegistry));
    }

    public async Task<IReadOnlyList<CardBatch>> GetAllBatchesAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<CardBatch?> GetBatchByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<CardBatch> CreateBatchAsync(CreateBatchRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Quantity <= 0)
            throw new ArgumentException("Số lượng thẻ nhập phải lớn hơn 0.", nameof(request.Quantity));

        var existing = await _repository.GetByCodeAsync(request.BatchCode);
        if (existing != null)
            throw new InvalidOperationException($"Mã đợt '{request.BatchCode}' đã tồn tại trong hệ thống.");

        // Lấy định nghĩa loại thẻ từ Registry, không cần switch-case loại thẻ
        var cardDef = _cardTypeRegistry.GetByKey(request.CardTypeKey);

        var batch = new CardBatch(
            request.BatchCode,
            request.BatchName,
            request.CustomerName,
            request.Notes
        );

        // Sinh danh sách thẻ cho đợt theo số lượng yêu cầu
        for (int i = 1; i <= request.Quantity; i++)
        {
            string code = $"{request.CodePrefix}{i:D4}";
            string holder = string.IsNullOrWhiteSpace(request.HolderNamePattern)
                ? $"Chủ thẻ #{i:D3}"
                : $"{request.HolderNamePattern} #{i:D3}";

            var card = cardDef.CreateCard(
                code,
                holder,
                request.CustomBlankCost,
                request.CustomSellingPrice
            );

            batch.AddCard(card);
        }

        await _repository.AddAsync(batch);
        return batch;
    }

    public async Task DeleteBatchAsync(Guid batchId)
    {
        await _repository.DeleteAsync(batchId);
    }
}

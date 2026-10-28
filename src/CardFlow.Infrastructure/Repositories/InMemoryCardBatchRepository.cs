using System.Collections.Concurrent;
using CardFlow.Application.Interfaces;
using CardFlow.Domain.Entities;

namespace CardFlow.Infrastructure.Repositories;

// Lưu trữ các đợt thẻ tạm thời trong bộ nhớ Ram (an toàn cho đa luồng).
// Khi khởi động, nạp sẵn 3 đợt thẻ mẫu với số liệu thực tế để xem báo cáo được ngay.
public class InMemoryCardBatchRepository : ICardBatchRepository
{
    private readonly ConcurrentDictionary<Guid, CardBatch> _batches = new();

    public InMemoryCardBatchRepository(ICardTypeRegistry registry)
    {
        SeedSampleData(registry);
    }

    private void SeedSampleData(ICardTypeRegistry registry)
    {
        // Đợt 1: Thẻ sinh viên Đại học Bách Khoa (100 thẻ PVC thường)
        var pvcDef = registry.GetByKey("PVC_STANDARD");
        var batch1 = new CardBatch("BATCH-2026-001", "Đợt 1 - Thẻ Sinh Viên Bách Khoa", "Đại học Bách Khoa", "In thẻ sinh viên đầu khóa K68");
        for (int i = 1; i <= 100; i++)
        {
            var card = pvcDef.CreateCard($"BK26-{i:D4}", $"Sinh Viên BK #{i:D3}", 3500m, 15000m);
            batch1.AddCard(card);
        }
        _batches[batch1.Id] = batch1;

        // Đợt 2: Thẻ RFID kiểm soát ra vào Tòa nhà Landmark (50 thẻ RFID 125kHz)
        var rfidDef = registry.GetByKey("RFID_125K");
        var batch2 = new CardBatch("BATCH-2026-002", "Đợt 2 - Thẻ Kiểm Soát Tòa Nhà Landmark", "BQL Landmark 81", "Thẻ cư dân & kiểm soát thang máy");
        for (int i = 1; i <= 50; i++)
        {
            var card = rfidDef.CreateCard($"LM-RFID-{i:D3}", $"Cư Dân Landmark #{i:D3}", 7000m, 28000m);
            batch2.AddCard(card);
        }
        _batches[batch2.Id] = batch2;

        // Đợt 3: Thẻ VIP Gold Khách Hàng Thân Thiết (30 thẻ VIP Smart Chip)
        var vipDef = registry.GetByKey("VIP_CHIP");
        var batch3 = new CardBatch("BATCH-2026-003", "Đợt 3 - Thẻ VIP Gold Khách Hàng", "Tập đoàn VinGroup", "In màng Re-transfer cán Hologram bảo mật");
        for (int i = 1; i <= 30; i++)
        {
            var card = vipDef.CreateCard($"VIP-GOLD-{i:D3}", $"Hội Viên VIP #{i:D3}", 25000m, 85000m);
            batch3.AddCard(card);
        }
        _batches[batch3.Id] = batch3;
    }

    public Task<IReadOnlyList<CardBatch>> GetAllAsync()
    {
        IReadOnlyList<CardBatch> list = _batches.Values
            .OrderByDescending(b => b.CreatedAt)
            .ToList();
        return Task.FromResult(list);
    }

    public Task<CardBatch?> GetByIdAsync(Guid id)
    {
        _batches.TryGetValue(id, out var batch);
        return Task.FromResult(batch);
    }

    public Task<CardBatch?> GetByCodeAsync(string batchCode)
    {
        var batch = _batches.Values.FirstOrDefault(b => 
            string.Equals(b.BatchCode, batchCode, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(batch);
    }

    public Task AddAsync(CardBatch batch)
    {
        ArgumentNullException.ThrowIfNull(batch);
        _batches[batch.Id] = batch;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(CardBatch batch)
    {
        ArgumentNullException.ThrowIfNull(batch);
        _batches[batch.Id] = batch;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _batches.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}

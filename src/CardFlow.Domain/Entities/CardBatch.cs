namespace CardFlow.Domain.Entities;

// Đại diện cho một đợt/lô nhập thẻ của khách hàng.
// Đóng gói toàn bộ danh sách thẻ và tự động tổng hợp số liệu Thu - Chi - Lợi Nhuận của đợt.
public class CardBatch
{
    private readonly List<BaseCard> _cards = new();

    public Guid Id { get; }
    public string BatchCode { get; private set; }
    public string BatchName { get; private set; }
    public string CustomerName { get; private set; }
    public DateTime CreatedAt { get; }
    public string Notes { get; private set; }

    // Chỉ cho bên ngoài đọc danh sách thẻ, muốn thêm thẻ phải gọi qua phương thức của Batch
    public IReadOnlyList<BaseCard> Cards => _cards.AsReadOnly();
    public int TotalQuantity => _cards.Count;

    // --- HẠCH TOÁN TÀI CHÍNH CỦA ĐỢT ---

    // 1. Tổng tiền mua phôi thẻ của cả đợt (Chi phí đầu vào 1)
    public decimal TotalBlankCost => _cards.Sum(c => c.UnitBlankCost);

    // 2. Tổng chi phí gia công kỹ thuật của cả đợt (Chi phí đầu vào 2 - mỗi thẻ tự tính theo loại của nó)
    public decimal TotalProcessingCost => _cards.Sum(c => c.CalculateUnitProcessingCost());

    // 3. TỔNG CHI = Tiền phôi + Tiền gia công
    public decimal TotalCost => TotalBlankCost + TotalProcessingCost;

    // 4. TỔNG THU = Doanh thu bán ra của toàn bộ số thẻ trong đợt
    public decimal TotalRevenue => _cards.Sum(c => c.UnitSellingPrice);

    // 5. LỢI NHUẬN RÒNG = Tổng Thu - Tổng Chi
    public decimal NetProfit => TotalRevenue - TotalCost;

    // 6. TỶ SUẤT LỢI NHUẬN ROI (%) = (Lợi Nhuận / Tổng Thu) * 100
    public decimal ProfitMargin => TotalRevenue > 0 
        ? Math.Round((NetProfit / TotalRevenue) * 100, 2) 
        : 0;

    public CardBatch(string batchCode, string batchName, string customerName, string notes = "")
    {
        if (string.IsNullOrWhiteSpace(batchCode))
            throw new ArgumentException("Mã đợt không được để trống.", nameof(batchCode));
        if (string.IsNullOrWhiteSpace(batchName))
            throw new ArgumentException("Tên đợt không được để trống.", nameof(batchName));

        Id = Guid.NewGuid();
        BatchCode = batchCode.Trim().ToUpperInvariant();
        BatchName = batchName.Trim();
        CustomerName = customerName?.Trim() ?? "Khách hàng vãng lai";
        Notes = notes?.Trim() ?? "";
        CreatedAt = DateTime.UtcNow;
    }

    public void AddCard(BaseCard card)
    {
        ArgumentNullException.ThrowIfNull(card);
        _cards.Add(card);
    }

    public void AddCards(IEnumerable<BaseCard> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);
        _cards.AddRange(cards);
    }
}

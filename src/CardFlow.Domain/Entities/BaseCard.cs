using CardFlow.Domain.Interfaces;

namespace CardFlow.Domain.Entities;

// Lớp cơ sở cho mọi loại thẻ trong hệ thống.
// Đóng gói các thông tin định danh và đơn giá cơ sở (giá phôi, giá bán).
public abstract class BaseCard : IFinancialEvaluator
{
    public Guid Id { get; }
    public string CardCode { get; protected set; }
    public string HolderName { get; protected set; }
    public DateTime CreatedAt { get; }

    // Chi phí phôi thẻ nhập về (đầu vào)
    public decimal UnitBlankCost { get; protected set; }

    // Đơn giá bán ra cho khách hàng (đầu ra)
    public decimal UnitSellingPrice { get; protected set; }

    protected BaseCard(string cardCode, string holderName, decimal unitBlankCost, decimal unitSellingPrice)
    {
        // Chặn dữ liệu không hợp lệ ngay từ lúc tạo đối tượng
        if (string.IsNullOrWhiteSpace(cardCode))
            throw new ArgumentException("Mã thẻ không được để trống.", nameof(cardCode));
        if (string.IsNullOrWhiteSpace(holderName))
            throw new ArgumentException("Tên chủ thẻ không được để trống.", nameof(holderName));
        if (unitBlankCost < 0)
            throw new ArgumentOutOfRangeException(nameof(unitBlankCost), "Đơn giá phôi không được âm.");
        if (unitSellingPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(unitSellingPrice), "Giá bán không được âm.");

        Id = Guid.NewGuid();
        CardCode = cardCode.Trim();
        HolderName = holderName.Trim();
        UnitBlankCost = unitBlankCost;
        UnitSellingPrice = unitSellingPrice;
        CreatedAt = DateTime.UtcNow;
    }

    // Mỗi loại thẻ con sẽ có chi phí gia công/kỹ thuật riêng (in mực, nạp chip, dán tem...)
    public abstract decimal CalculateUnitProcessingCost();

    // Tên hiển thị của loại thẻ trên giao diện báo cáo
    public abstract string GetCardTypeDisplayName();

    // Cập nhật lại giá khi có thay đổi chính sách
    public virtual void UpdatePricing(decimal newBlankCost, decimal newSellingPrice)
    {
        if (newBlankCost < 0) throw new ArgumentOutOfRangeException(nameof(newBlankCost));
        if (newSellingPrice < 0) throw new ArgumentOutOfRangeException(nameof(newSellingPrice));

        UnitBlankCost = newBlankCost;
        UnitSellingPrice = newSellingPrice;
    }
}

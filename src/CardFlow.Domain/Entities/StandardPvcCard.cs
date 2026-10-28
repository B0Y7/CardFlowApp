namespace CardFlow.Domain.Entities;

// Thẻ nhựa PVC thông thường (thẻ nhân viên phổ thông, thẻ tích điểm).
// Chi phí gia công chủ yếu là tiền mực in màu Ribbon.
public class StandardPvcCard : BaseCard
{
    public decimal RibbonCostPerCard { get; private set; }

    public StandardPvcCard(
        string cardCode, 
        string holderName, 
        decimal unitBlankCost = 3500m, 
        decimal unitSellingPrice = 15000m, 
        decimal ribbonCost = 2500m)
        : base(cardCode, holderName, unitBlankCost, unitSellingPrice)
    {
        RibbonCostPerCard = ribbonCost >= 0 ? ribbonCost : throw new ArgumentOutOfRangeException(nameof(ribbonCost));
    }

    public override decimal CalculateUnitProcessingCost() => RibbonCostPerCard;

    public override string GetCardTypeDisplayName() => "Thẻ Nhựa PVC Tiêu Chuẩn";
}

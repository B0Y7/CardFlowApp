namespace CardFlow.Domain.Entities;

// Thẻ Kim Cương Cao Cấp (Diamond Card).
// Thể hiện tính MỞ (Open for extension): Kế thừa BaseCard và tự định nghĩa chi phí gia công riêng.
public class DiamondCard : BaseCard
{
    // Chi phí gia công nạm đá quý / khắc kim cương / viền mạ vàng
    public decimal DiamondEngravingCost { get; private set; }

    public DiamondCard(
        string cardCode,
        string holderName,
        decimal unitBlankCost = 50000m,
        decimal unitSellingPrice = 200000m,
        decimal engravingCost = 35000m)
        : base(cardCode, holderName, unitBlankCost, unitSellingPrice)
    {
        DiamondEngravingCost = engravingCost >= 0 
            ? engravingCost 
            : throw new ArgumentOutOfRangeException(nameof(engravingCost), "Chi phí gia công không được âm.");
    }

    // Chi phí gia công đặc thù của thẻ kim cương
    public override decimal CalculateUnitProcessingCost() => DiamondEngravingCost;

    public override string GetCardTypeDisplayName() => "Thẻ Kim Cương Cao Cấp (Diamond)";
}

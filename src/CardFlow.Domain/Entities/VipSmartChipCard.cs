namespace CardFlow.Domain.Entities;

// Thẻ VIP gắn chip thông minh (thẻ ngân hàng, cư dân cao cấp).
// Chi phí kỹ thuật gồm: màng phim Re-transfer bảo vệ thẻ, cán màng Hologram chống làm giả và phí mã hóa chip EMV.
public class VipSmartChipCard : BaseCard
{
    public decimal RetransferFilmCost { get; private set; }
    public decimal HologramSecurityCost { get; private set; }
    public decimal ChipEncodingFee { get; private set; }
    public string ChipSerial { get; private set; }

    public VipSmartChipCard(
        string cardCode,
        string holderName,
        string chipSerial,
        decimal unitBlankCost = 25000m,
        decimal unitSellingPrice = 85000m,
        decimal retransferFilmCost = 6500m,
        decimal hologramSecurityCost = 4000m,
        decimal chipEncodingFee = 5000m)
        : base(cardCode, holderName, unitBlankCost, unitSellingPrice)
    {
        if (string.IsNullOrWhiteSpace(chipSerial))
            throw new ArgumentException("Số serial chip không được để trống.", nameof(chipSerial));

        ChipSerial = chipSerial;
        RetransferFilmCost = retransferFilmCost;
        HologramSecurityCost = hologramSecurityCost;
        ChipEncodingFee = chipEncodingFee;
    }

    public override decimal CalculateUnitProcessingCost() => RetransferFilmCost + HologramSecurityCost + ChipEncodingFee;

    public override string GetCardTypeDisplayName() => "Thẻ VIP Smart Chip (Hologram)";
}

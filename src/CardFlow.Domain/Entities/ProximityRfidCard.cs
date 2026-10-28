namespace CardFlow.Domain.Entities;

// Thẻ RFID tần số 125kHz dùng chấm công, mở cửa thang máy.
// Chi phí phát sinh thêm: tiền mực in và công nạp/đọc kiểm tra mã UID chip.
public class ProximityRfidCard : BaseCard
{
    public decimal RibbonCost { get; private set; }
    public decimal RfidEncodingFee { get; private set; }
    public string RfidUid { get; private set; }

    public ProximityRfidCard(
        string cardCode, 
        string holderName, 
        string rfidUid,
        decimal unitBlankCost = 7000m, 
        decimal unitSellingPrice = 28000m, 
        decimal ribbonCost = 2500m,
        decimal rfidEncodingFee = 3000m)
        : base(cardCode, holderName, unitBlankCost, unitSellingPrice)
    {
        if (string.IsNullOrWhiteSpace(rfidUid))
            throw new ArgumentException("Mã UID RFID không được rỗng.", nameof(rfidUid));

        RfidUid = rfidUid;
        RibbonCost = ribbonCost;
        RfidEncodingFee = rfidEncodingFee;
    }

    public override decimal CalculateUnitProcessingCost() => RibbonCost + RfidEncodingFee;

    public override string GetCardTypeDisplayName() => "Thẻ RFID Proximity 125kHz";
}

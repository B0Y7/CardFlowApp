using CardFlow.Application.Interfaces;
using CardFlow.Domain.Entities;

namespace CardFlow.Infrastructure.Definitions;

// 1. Thẻ PVC tiêu chuẩn
public class StandardPvcCardDefinition : ICardTypeDefinition
{
    public string TypeKey => "PVC_STANDARD";
    public string DisplayName => "Thẻ Nhựa PVC Tiêu Chuẩn";
    public string Description => "Thẻ nhân viên phổ thông, thẻ tích điểm (chỉ chịu phí mực in Ribbon).";
    public decimal DefaultBlankCost => 3500m;
    public decimal DefaultSellingPrice => 15000m;

    public BaseCard CreateCard(string cardCode, string holderName, decimal? customBlankCost = null, decimal? customSellingPrice = null)
    {
        return new StandardPvcCard(
            cardCode,
            holderName,
            customBlankCost ?? DefaultBlankCost,
            customSellingPrice ?? DefaultSellingPrice
        );
    }
}

// 2. Thẻ RFID Proximity 125kHz
public class ProximityRfidCardDefinition : ICardTypeDefinition
{
    public string TypeKey => "RFID_125K";
    public string DisplayName => "Thẻ RFID Proximity 125kHz";
    public string Description => "Dùng kiểm soát thang máy, chấm công (có phụ phí đọc ghi mã chip UID).";
    public decimal DefaultBlankCost => 7000m;
    public decimal DefaultSellingPrice => 28000m;

    public BaseCard CreateCard(string cardCode, string holderName, decimal? customBlankCost = null, decimal? customSellingPrice = null)
    {
        string uid = $"UID-{Random.Shared.Next(100000, 999999)}";
        return new ProximityRfidCard(
            cardCode,
            holderName,
            uid,
            customBlankCost ?? DefaultBlankCost,
            customSellingPrice ?? DefaultSellingPrice
        );
    }
}

// 3. Thẻ VIP Smart Chip
public class VipSmartChipCardDefinition : ICardTypeDefinition
{
    public string TypeKey => "VIP_CHIP";
    public string DisplayName => "Thẻ VIP Smart Chip (Hologram)";
    public string Description => "Thẻ cao cấp: cán màng Hologram bảo mật, màng phim Re-transfer và nạp chip EMV.";
    public decimal DefaultBlankCost => 25000m;
    public decimal DefaultSellingPrice => 85000m;

    public BaseCard CreateCard(string cardCode, string holderName, decimal? customBlankCost = null, decimal? customSellingPrice = null)
    {
        string chipSerial = $"CHIP-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        return new VipSmartChipCard(
            cardCode,
            holderName,
            chipSerial,
            customBlankCost ?? DefaultBlankCost,
            customSellingPrice ?? DefaultSellingPrice
        );
    }
}

// 4. Thẻ NFC 13.56MHz
public class NfcAccessCardDefinition : ICardTypeDefinition
{
    public string TypeKey => "NFC_1356M";
    public string DisplayName => "Thẻ NFC 13.56MHz";
    public string Description => "Dùng cho kiểm soát cửa thông minh, bãi xe thông minh bảo mật cao.";
    public decimal DefaultBlankCost => 12000m;
    public decimal DefaultSellingPrice => 45000m;

    public BaseCard CreateCard(string cardCode, string holderName, decimal? customBlankCost = null, decimal? customSellingPrice = null)
    {
        string secKey = $"SEC-KEY-{Random.Shared.Next(1000, 9999)}";
        return new NfcAccessCard(
            cardCode,
            holderName,
            secKey,
            customBlankCost ?? DefaultBlankCost,
            customSellingPrice ?? DefaultSellingPrice
        );
    }
}

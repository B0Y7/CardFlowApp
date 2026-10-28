namespace CardFlow.Domain.Entities;

// Thẻ NFC tần số 13.56MHz (chuẩn Mifare / Desfire) dùng kiểm soát truy cập độ bảo mật cao.
public class NfcAccessCard : BaseCard
{
    public decimal AntennaTuningCost { get; private set; }
    public decimal EncryptionKeyFee { get; private set; }
    public string NfcKey { get; private set; }

    public NfcAccessCard(
        string cardCode,
        string holderName,
        string nfcKey,
        decimal unitBlankCost = 12000m,
        decimal unitSellingPrice = 45000m,
        decimal antennaTuningCost = 3500m,
        decimal encryptionKeyFee = 2500m)
        : base(cardCode, holderName, unitBlankCost, unitSellingPrice)
    {
        if (string.IsNullOrWhiteSpace(nfcKey))
            throw new ArgumentException("Mã khóa NFC không được để trống.", nameof(nfcKey));

        NfcKey = nfcKey;
        AntennaTuningCost = antennaTuningCost;
        EncryptionKeyFee = encryptionKeyFee;
    }

    public override decimal CalculateUnitProcessingCost() => AntennaTuningCost + EncryptionKeyFee;

    public override string GetCardTypeDisplayName() => "Thẻ NFC 13.56MHz";
}

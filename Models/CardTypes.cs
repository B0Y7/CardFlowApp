namespace CardFlowApp.Models;

// Thẻ PVC thông thường: chỉ phát sinh chi phí mực in nhiệt cơ bản
public class ThePvcThuong : BaseCard
{
    public decimal PhiMucIn { get; }

    public ThePvcThuong(string maThe, string tenChuThe, decimal giaPhoi = 3500m, decimal giaBan = 15000m, decimal phiMucIn = 2500m)
        : base(maThe, tenChuThe, giaPhoi, giaBan)
    {
        PhiMucIn = phiMucIn;
    }

    public override decimal TinhChiPhiGiaCong() => PhiMucIn;
    public override string TenLoaiThe => "Thẻ PVC Thường";
}

// Thẻ Proximity RFID 125kHz: gồm chi phí mực in và phí nạp mã UID vào chip
public class TheRfid : BaseCard
{
    public decimal PhiMucIn { get; }
    public decimal PhiNapMaChip { get; }

    public TheRfid(string maThe, string tenChuThe, decimal giaPhoi = 7000m, decimal giaBan = 28000m, decimal phiMucIn = 2500m, decimal phiNapMa = 3000m)
        : base(maThe, tenChuThe, giaPhoi, giaBan)
    {
        PhiMucIn = phiMucIn;
        PhiNapMaChip = phiNapMa;
    }

    public override decimal TinhChiPhiGiaCong() => PhiMucIn + PhiNapMaChip;
    public override string TenLoaiThe => "Thẻ Chip RFID 125kHz";
}

// Thẻ VIP Smart Chip: dùng màng in re-transfer, dán hologram chống giả và mã hóa chip
public class TheVipChip : BaseCard
{
    public decimal PhiMangRetransfer { get; }
    public decimal PhiHologram { get; }
    public decimal PhiMaHoaChip { get; }

    public TheVipChip(string maThe, string tenChuThe, decimal giaPhoi = 25000m, decimal giaBan = 85000m)
        : base(maThe, tenChuThe, giaPhoi, giaBan)
    {
        PhiMangRetransfer = 6500m;
        PhiHologram = 4000m;
        PhiMaHoaChip = 5000m;
    }

    public override decimal TinhChiPhiGiaCong() => PhiMangRetransfer + PhiHologram + PhiMaHoaChip;
    public override string TenLoaiThe => "Thẻ VIP Smart Chip";
}

// Thẻ NFC 13.56MHz: minh họa cho việc mở rộng loại thẻ mới.
// Chỉ cần tạo thêm class này mà không cần chạm vào logic tính toán ở CardBatch hay Service.
public class TheNfc : BaseCard
{
    public decimal PhiCanChinhAnten { get; }

    public TheNfc(string maThe, string tenChuThe, decimal giaPhoi = 12000m, decimal giaBan = 45000m, decimal phiAnten = 5000m)
        : base(maThe, tenChuThe, giaPhoi, giaBan)
    {
        PhiCanChinhAnten = phiAnten;
    }

    public override decimal TinhChiPhiGiaCong() => PhiCanChinhAnten;
    public override string TenLoaiThe => "Thẻ NFC 13.56MHz";
}

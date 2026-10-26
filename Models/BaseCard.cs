namespace CardFlowApp.Models;

// Giao diện hạch toán chi phí và lợi nhuận cho từng thẻ
public interface IHachToan
{
    decimal GiaPhoi { get; }
    decimal TinhChiPhiGiaCong();
    decimal GiaBan { get; }
    decimal TongChiPhi => GiaPhoi + TinhChiPhiGiaCong();
    decimal LoiNhuan => GiaBan - TongChiPhi;
}

// Lớp cơ sở dùng chung cho mọi loại thẻ trong hệ thống.
// Đóng gói các thuộc tính cơ bản và kiểm soát tính hợp lệ (không để mã trống, không nhận giá âm).
public abstract class BaseCard : IHachToan
{
    public string MaThe { get; private set; }
    public string TenChuThe { get; private set; }
    public decimal GiaPhoi { get; private set; }
    public decimal GiaBan { get; private set; }

    protected BaseCard(string maThe, string tenChuThe, decimal giaPhoi, decimal giaBan)
    {
        if (string.IsNullOrWhiteSpace(maThe)) 
            throw new ArgumentException("Mã thẻ không được để trống!");
        if (giaPhoi < 0 || giaBan < 0) 
            throw new ArgumentException("Giá phôi và giá bán không được âm!");

        MaThe = maThe;
        TenChuThe = tenChuThe;
        GiaPhoi = giaPhoi;
        GiaBan = giaBan;
    }

    // Mỗi loại thẻ con sẽ tự cài đặt cách tính phí gia công/in ấn riêng (mực, chip, hologram...)
    public abstract decimal TinhChiPhiGiaCong();

    public abstract string TenLoaiThe { get; }
}

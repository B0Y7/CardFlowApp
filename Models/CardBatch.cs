namespace CardFlowApp.Models;

// Đại diện cho một đợt/lô thẻ nhập về.
// Quản lý danh sách thẻ và tự động tổng hợp chi phí, doanh thu, lợi nhuận của cả đợt.
public class CardBatch
{
    private readonly List<BaseCard> _danhSachThe = new();

    public string MaDot { get; private set; }
    public string TenDot { get; private set; }
    public string KhachHang { get; private set; }
    public DateTime NgayTao { get; private set; }

    public IReadOnlyList<BaseCard> DanhSachThe => _danhSachThe.AsReadOnly();
    public int SoLuong => _danhSachThe.Count;

    // Chi phí vốn đầu vào
    public decimal TongTienPhoi => _danhSachThe.Sum(c => c.GiaPhoi);
    public decimal TongTienGiaCong => _danhSachThe.Sum(c => c.TinhChiPhiGiaCong());
    public decimal TongChi => TongTienPhoi + TongTienGiaCong;

    // Doanh thu bán ra
    public decimal TongThu => _danhSachThe.Sum(c => c.GiaBan);

    // Lợi nhuận và tỷ suất sinh lời
    public decimal LoiNhuanRong => TongThu - TongChi;
    public decimal TySuatRoi => TongThu > 0 ? Math.Round((LoiNhuanRong / TongThu) * 100, 1) : 0;

    public CardBatch(string maDot, string tenDot, string khachHang)
    {
        if (string.IsNullOrWhiteSpace(maDot)) 
            throw new ArgumentException("Mã đợt không được để trống!");

        MaDot = maDot;
        TenDot = tenDot;
        KhachHang = khachHang;
        NgayTao = DateTime.Now;
    }

    public void ThemThe(BaseCard the)
    {
        if (the != null) _danhSachThe.Add(the);
    }
}

using CardFlowApp.Models;

namespace CardFlowApp.Services;

// Interface định nghĩa các nghiệp vụ lưu trữ và tính toán tổng hợp các đợt thẻ
public interface IHachToanService
{
    List<CardBatch> LayTatCaDot();
    void ThemDot(CardBatch dot);
    void XoaDot(string maDot);

    // Tính tổng hợp toàn bộ chi phí, doanh thu và lợi nhuận hệ thống
    (decimal TongChi, decimal TongThu, decimal TongLoiNhuan, decimal RoiTrungBinh) TinhTongKet();
}

// Xử lý lưu trữ danh sách các đợt thẻ và tính toán các chỉ số tài chính tổng hợp
public class HachToanService : IHachToanService
{
    private readonly List<CardBatch> _danhSachDot = new();

    public HachToanService()
    {
        NapDuLieuBanDau();
    }

    // Nạp sẵn một số đợt mẫu thực tế để tiện theo dõi ngay khi mở ứng dụng
    private void NapDuLieuBanDau()
    {
        // Đợt 1: Thẻ sinh viên Đại học Bách Khoa
        var dot1 = new CardBatch("DOT-001", "Đợt 1 - Thẻ SV Đại Học Bách Khoa", "ĐH Bách Khoa HN");
        for (int i = 1; i <= 50; i++)
            dot1.ThemThe(new ThePvcThuong($"BK-{i:D3}", $"Sinh viên {i}", giaPhoi: 3500, giaBan: 15000));
        _danhSachDot.Add(dot1);

        // Đợt 2: Thẻ chấm công & thang máy tòa nhà Landmark
        var dot2 = new CardBatch("DOT-002", "Đợt 2 - Thẻ Thang Máy Landmark", "BQL Tòa Nhà Landmark");
        for (int i = 1; i <= 30; i++)
            dot2.ThemThe(new TheRfid($"LM-{i:D3}", $"Cư dân {i}", giaPhoi: 7000, giaBan: 28000));
        _danhSachDot.Add(dot2);

        // Đợt 3: Thẻ VIP dành cho hội viên thân thiết
        var dot3 = new CardBatch("DOT-003", "Đợt 3 - Thẻ VIP Gold Khách Hàng", "VinGroup VIP");
        for (int i = 1; i <= 20; i++)
            dot3.ThemThe(new TheVipChip($"VIP-{i:D3}", $"Khách VIP {i}", giaPhoi: 25000, giaBan: 85000));
        _danhSachDot.Add(dot3);
    }

    public List<CardBatch> LayTatCaDot() => _danhSachDot;

    public void ThemDot(CardBatch dot)
    {
        if (dot != null) _danhSachDot.Insert(0, dot);
    }

    public void XoaDot(string maDot)
    {
        _danhSachDot.RemoveAll(d => d.MaDot == maDot);
    }

    public (decimal TongChi, decimal TongThu, decimal TongLoiNhuan, decimal RoiTrungBinh) TinhTongKet()
    {
        decimal tongChi = _danhSachDot.Sum(d => d.TongChi);
        decimal tongThu = _danhSachDot.Sum(d => d.TongThu);
        decimal tongLoiNhuan = tongThu - tongChi;
        decimal roi = tongThu > 0 ? Math.Round((tongLoiNhuan / tongThu) * 100, 1) : 0;
        return (tongChi, tongThu, tongLoiNhuan, roi);
    }
}

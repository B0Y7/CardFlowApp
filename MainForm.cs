using CardFlowApp.Models;
using CardFlowApp.Services;

namespace CardFlowApp;

public class MainForm : Form
{
    private readonly IHachToanService _service;

    // Các nhãn hiển thị số liệu thống kê tổng quan
    private Label _lblTongChi = null!;
    private Label _lblTongThu = null!;
    private Label _lblLoiNhuan = null!;
    private Label _lblTongDot = null!;

    // Bảng danh sách đợt và bảng chi tiết thẻ trong đợt
    private DataGridView _dgvDotThe = null!;
    private DataGridView _dgvChiTietThe = null!;

    public MainForm(IHachToanService service)
    {
        _service = service;
        KhoiTaoGiaoDien();
        TaiDuLieu();
    }

    private void KhoiTaoGiaoDien()
    {
        Text = "Quản lý nhập thẻ & hạch toán thu chi lợi nhuận từng đợt";
        Width = 1100;
        Height = 720;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9.5f);
        BackColor = Color.FromArgb(245, 247, 250);

        // Thanh tiêu đề phía trên
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.FromArgb(30, 41, 59),
            Padding = new Padding(15, 10, 15, 10)
        };
        var lblTitle = new Label
        {
            Text = "HỆ THỐNG QUẢN LÝ NHẬP THẺ & HẠCH TOÁN THU CHI TỪNG ĐỢT",
            Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(15, 12)
        };
        pnlHeader.Controls.Add(lblTitle);
        Controls.Add(pnlHeader);

        // 4 thẻ tóm tắt số liệu tổng quan (Quy mô, Chi phí, Doanh thu, Lợi nhuận)
        var pnlKpis = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 78,
            ColumnCount = 4,
            RowCount = 1,
            Padding = new Padding(10, 6, 10, 6),
            BackColor = Color.FromArgb(241, 245, 249)
        };
        pnlKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        pnlKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        pnlKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        pnlKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

        pnlKpis.Controls.Add(TaoHopThongKe("TỔNG ĐỢT & SẢN LƯỢNG", Color.FromArgb(59, 130, 246), out _lblTongDot));
        pnlKpis.Controls.Add(TaoHopThongKe("TỔNG CHI PHÍ VỐN", Color.FromArgb(239, 68, 68), out _lblTongChi));
        pnlKpis.Controls.Add(TaoHopThongKe("TỔNG DOANH THU", Color.FromArgb(16, 185, 129), out _lblTongThu));
        pnlKpis.Controls.Add(TaoHopThongKe("LỢI NHUẬN RÒNG", Color.FromArgb(168, 85, 247), out _lblLoiNhuan));
        Controls.Add(pnlKpis);

        // Thanh thao tác (Thêm đợt mới, Xóa đợt)
        var pnlActions = new Panel
        {
            Dock = DockStyle.Top,
            Height = 44,
            BackColor = Color.White,
            Padding = new Padding(12, 6, 12, 6)
        };

        var btnThem = TaoNut("➕ Nhập Đợt Thẻ Mới", Color.FromArgb(37, 99, 235), Color.White);
        btnThem.Click += OnThemDotClicked;

        var btnXoa = TaoNut("🗑️ Xóa Đợt Thẻ", Color.FromArgb(239, 68, 68), Color.White);
        btnXoa.Click += OnXoaDotClicked;

        btnThem.Location = new Point(12, 6);
        btnXoa.Location = new Point(190, 6);
        pnlActions.Controls.AddRange([btnThem, btnXoa]);
        Controls.Add(pnlActions);

        // Chia khung hiển thị: Nửa trên là danh sách đợt, nửa dưới là chi tiết từng thẻ
        var splitMain = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Horizontal,
            SplitterDistance = 280,
            SplitterWidth = 6
        };

        // Bảng danh sách các đợt thẻ
        var pnlDot = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        var lblDot = new Label
        {
            Text = "Danh sách các đợt nhập thẻ và hạch toán tài chính:",
            Dock = DockStyle.Top,
            Height = 24,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59)
        };
        _dgvDotThe = TaoBang();
        _dgvDotThe.Columns.Add("MaDot", "Mã Đợt");
        _dgvDotThe.Columns.Add("TenDot", "Tên Đợt Thẻ");
        _dgvDotThe.Columns.Add("KhachHang", "Khách Hàng");
        _dgvDotThe.Columns.Add("SoLuong", "Số Lượng");
        _dgvDotThe.Columns.Add("TienPhoi", "Tiền Phôi (đ)");
        _dgvDotThe.Columns.Add("TienGiaCong", "Phí Gia Công (đ)");
        _dgvDotThe.Columns.Add("TongChi", "Tổng Chi (đ)");
        _dgvDotThe.Columns.Add("TongThu", "Tổng Thu (đ)");
        _dgvDotThe.Columns.Add("LoiNhuan", "Lãi Ròng (đ)");
        _dgvDotThe.Columns.Add("ROI", "Tỷ Suất ROI");

        _dgvDotThe.SelectionChanged += OnChonDotThe;
        pnlDot.Controls.AddRange([_dgvDotThe, lblDot]);
        splitMain.Panel1.Controls.Add(pnlDot);

        // Bảng chi tiết các thẻ thuộc đợt được chọn
        var pnlThe = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        var lblThe = new Label
        {
            Text = "Chi tiết từng thẻ trong đợt được chọn:",
            Dock = DockStyle.Top,
            Height = 24,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59)
        };
        _dgvChiTietThe = TaoBang();
        _dgvChiTietThe.Columns.Add("MaThe", "Mã Thẻ");
        _dgvChiTietThe.Columns.Add("ChuThe", "Chủ Thẻ");
        _dgvChiTietThe.Columns.Add("LoaiThe", "Loại Thẻ");
        _dgvChiTietThe.Columns.Add("GiaPhoi", "Giá Phôi (đ)");
        _dgvChiTietThe.Columns.Add("PhiGiaCong", "Phí Gia Công (đ)");
        _dgvChiTietThe.Columns.Add("GiaBan", "Giá Bán (đ)");
        _dgvChiTietThe.Columns.Add("LoiNhuan", "Lãi / Thẻ (đ)");
        pnlThe.Controls.AddRange([_dgvChiTietThe, lblThe]);
        splitMain.Panel2.Controls.Add(pnlThe);

        Controls.Add(splitMain);

        // Sắp xếp thứ tự hiển thị các panel
        pnlHeader.BringToFront();
        pnlKpis.BringToFront();
        pnlActions.BringToFront();
        splitMain.BringToFront();
    }

    // Tải dữ liệu các đợt lên bảng và cập nhật các chỉ số tài chính
    private void TaiDuLieu()
    {
        var danhSach = _service.LayTatCaDot();

        var tk = _service.TinhTongKet();
        _lblTongDot.Text = $"{danhSach.Count} Đợt ({danhSach.Sum(d => d.SoLuong)} thẻ)";
        _lblTongChi.Text = $"{tk.TongChi:N0} đ";
        _lblTongThu.Text = $"{tk.TongThu:N0} đ";
        _lblLoiNhuan.Text = $"{tk.TongLoiNhuan:N0} đ (ROI: {tk.RoiTrungBinh}%)";

        _dgvDotThe.Rows.Clear();
        foreach (var d in danhSach)
        {
            _dgvDotThe.Rows.Add(
                d.MaDot,
                d.TenDot,
                d.KhachHang,
                d.SoLuong,
                d.TongTienPhoi.ToString("N0"),
                d.TongTienGiaCong.ToString("N0"),
                d.TongChi.ToString("N0"),
                d.TongThu.ToString("N0"),
                d.LoiNhuanRong.ToString("N0"),
                $"{d.TySuatRoi}%"
            );
        }

        if (_dgvDotThe.Rows.Count > 0 && _dgvDotThe.CurrentRow == null)
            _dgvDotThe.Rows[0].Selected = true;
    }

    // Khi người dùng chọn 1 dòng đợt thẻ, hiển thị danh sách chi tiết các thẻ bên dưới
    private void OnChonDotThe(object? sender, EventArgs e)
    {
        if (_dgvDotThe.CurrentRow == null)
        {
            _dgvChiTietThe.Rows.Clear();
            return;
        }

        string? maDot = _dgvDotThe.CurrentRow.Cells["MaDot"].Value?.ToString();
        var dot = _service.LayTatCaDot().FirstOrDefault(d => d.MaDot == maDot);

        _dgvChiTietThe.Rows.Clear();
        if (dot != null)
        {
            foreach (var the in dot.DanhSachThe)
            {
                decimal phiGiaCong = the.TinhChiPhiGiaCong();
                decimal laiTungThe = the.GiaBan - (the.GiaPhoi + phiGiaCong);

                _dgvChiTietThe.Rows.Add(
                    the.MaThe,
                    the.TenChuThe,
                    the.TenLoaiThe,
                    the.GiaPhoi.ToString("N0"),
                    phiGiaCong.ToString("N0"),
                    the.GiaBan.ToString("N0"),
                    laiTungThe.ToString("N0")
                );
            }
        }
    }

    // Hộp thoại nhập thông tin đợt thẻ mới
    private void OnThemDotClicked(object? sender, EventArgs e)
    {
        using var form = new Form
        {
            Text = "Nhập Đợt Thẻ Mới",
            Width = 380,
            Height = 320,
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };

        var lbl1 = new Label { Text = "Mã đợt:", Location = new Point(20, 20), AutoSize = true };
        var txtMa = new TextBox { Text = $"DOT-{DateTime.Now:yyyyMMdd}-{Random.Shared.Next(10, 99)}", Location = new Point(120, 18), Width = 220 };

        var lbl2 = new Label { Text = "Tên đợt:", Location = new Point(20, 60), AutoSize = true };
        var txtTen = new TextBox { Text = "Đợt in thẻ cư dân mới", Location = new Point(120, 58), Width = 220 };

        var lbl3 = new Label { Text = "Khách hàng:", Location = new Point(20, 100), AutoSize = true };
        var txtKhach = new TextBox { Text = "Chung cư Sunshine", Location = new Point(120, 98), Width = 220 };

        var lbl4 = new Label { Text = "Loại thẻ:", Location = new Point(20, 140), AutoSize = true };
        var cboLoai = new ComboBox { Location = new Point(120, 138), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
        cboLoai.Items.AddRange(["Thẻ PVC Thường", "Thẻ Chip RFID 125k", "Thẻ VIP Smart Chip", "Thẻ NFC 13.56MHz"]);
        cboLoai.SelectedIndex = 0;

        var lbl5 = new Label { Text = "Số lượng:", Location = new Point(20, 180), AutoSize = true };
        var numQty = new NumericUpDown { Minimum = 1, Maximum = 1000, Value = 25, Location = new Point(120, 178), Width = 100 };

        var btnLuu = new Button { Text = "Tạo Đợt", DialogResult = DialogResult.OK, Location = new Point(120, 230), Width = 100, Height = 32, BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White };
        var btnHuy = new Button { Text = "Hủy", DialogResult = DialogResult.Cancel, Location = new Point(230, 230), Width = 80, Height = 32 };

        form.Controls.AddRange([lbl1, txtMa, lbl2, txtTen, lbl3, txtKhach, lbl4, cboLoai, lbl5, numQty, btnLuu, btnHuy]);

        if (form.ShowDialog(this) == DialogResult.OK)
        {
            var dotMoi = new CardBatch(txtMa.Text.Trim(), txtTen.Text.Trim(), txtKhach.Text.Trim());
            int sl = (int)numQty.Value;

            for (int i = 1; i <= sl; i++)
            {
                BaseCard the = cboLoai.SelectedIndex switch
                {
                    1 => new TheRfid($"RFID-{i:D3}", $"Cư dân #{i:D3}"),
                    2 => new TheVipChip($"VIP-{i:D3}", $"Hội viên VIP #{i:D3}"),
                    3 => new TheNfc($"NFC-{i:D3}", $"Khách NFC #{i:D3}"),
                    _ => new ThePvcThuong($"PVC-{i:D3}", $"Người dùng #{i:D3}")
                };
                dotMoi.ThemThe(the);
            }

            _service.ThemDot(dotMoi);
            TaiDuLieu();
            MessageBox.Show($"Đã tạo thành công đợt thẻ: {dotMoi.TenDot}!\nTổng Thu: {dotMoi.TongThu:N0}đ | Tổng Chi: {dotMoi.TongChi:N0}đ | Lãi: {dotMoi.LoiNhuanRong:N0}đ", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    // Xóa đợt thẻ đang chọn
    private void OnXoaDotClicked(object? sender, EventArgs e)
    {
        if (_dgvDotThe.CurrentRow == null) return;
        string? maDot = _dgvDotThe.CurrentRow.Cells["MaDot"].Value?.ToString();
        if (string.IsNullOrEmpty(maDot)) return;

        if (MessageBox.Show($"Bạn có chắc chắn muốn xóa đợt {maDot}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            _service.XoaDot(maDot);
            TaiDuLieu();
        }
    }

    private Control TaoHopThongKe(string tieuDe, Color mauVien, out Label lblGiaTri)
    {
        var pnl = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(4), Padding = new Padding(8) };
        var vien = new Panel { Dock = DockStyle.Top, Height = 3, BackColor = mauVien };
        var lblTitle = new Label { Text = tieuDe, Dock = DockStyle.Top, Height = 18, ForeColor = Color.FromArgb(100, 116, 139), Font = new Font("Segoe UI", 8f, FontStyle.Bold) };
        lblGiaTri = new Label { Text = "0", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), TextAlign = ContentAlignment.MiddleLeft };
        pnl.Controls.AddRange([lblGiaTri, lblTitle, vien]);
        return pnl;
    }

    private Button TaoNut(string text, Color mauNen, Color mauChu)
    {
        var btn = new Button
        {
            Text = text,
            BackColor = mauNen,
            ForeColor = mauChu,
            FlatStyle = FlatStyle.Flat,
            Height = 32,
            AutoSize = true,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btn.FlatAppearance.BorderSize = 0;
        return btn;
    }

    private DataGridView TaoBang()
    {
        var dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
            RowHeadersVisible = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            GridColor = Color.FromArgb(241, 245, 249),
            Font = new Font("Segoe UI", 9f)
        };
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(71, 85, 105);
        dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
        dgv.ColumnHeadersHeight = 32;
        dgv.EnableHeadersVisualStyles = false;
        dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 231, 255);
        dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 41, 59);
        dgv.RowTemplate.Height = 28;
        return dgv;
    }
}

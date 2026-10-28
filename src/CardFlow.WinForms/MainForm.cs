using CardFlow.Application.DTOs;
using CardFlow.Application.Interfaces;
using CardFlow.Domain.Entities;
using CardFlow.WinForms.Dialogs;

namespace CardFlow.WinForms;

// Màn hình chính của hệ thống quản lý đợt nhập thẻ và hạch toán thu chi lợi nhuận.
public class MainForm : Form
{
    private readonly ICardBatchService _batchService;
    private readonly IFinancialCalculator _financialCalculator;
    private readonly ICardTypeRegistry _cardTypeRegistry;

    // Các thẻ hiển thị chỉ số tài chính tổng quan (KPI)
    private Label _lblKpiBatches = null!;
    private Label _lblKpiExpenses = null!;
    private Label _lblKpiRevenue = null!;
    private Label _lblKpiProfit = null!;

    // Các bảng dữ liệu
    private DataGridView _dgvBatches = null!;
    private DataGridView _dgvBatchCards = null!;
    private DataGridView _dgvCardTypeSummary = null!;

    // Các nút chức năng
    private Button _btnNewBatch = null!;
    private Button _btnDeleteBatch = null!;
    private Button _btnRefresh = null!;
    private Label _lblSelectedBatchInfo = null!;

    private CardBatch? _selectedBatch;

    public MainForm(
        ICardBatchService batchService,
        IFinancialCalculator financialCalculator,
        ICardTypeRegistry cardTypeRegistry)
    {
        _batchService = batchService ?? throw new ArgumentNullException(nameof(batchService));
        _financialCalculator = financialCalculator ?? throw new ArgumentNullException(nameof(financialCalculator));
        _cardTypeRegistry = cardTypeRegistry ?? throw new ArgumentNullException(nameof(cardTypeRegistry));

        InitializeComponent();
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        await ReloadAllDataAsync();
    }

    private void InitializeComponent()
    {
        Text = "CardFlow - Quản Lý Nhập Thẻ & Hạch Toán Thu Chi Lợi Nhuận Từng Đợt";
        Width = 1320;
        Height = 840;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        BackColor = Color.FromArgb(241, 245, 249);

        // Header phía trên cùng
        var pnlHeader = CreateAppHeader();
        Controls.Add(pnlHeader);

        // Hàng thẻ KPI tài chính
        var pnlKpis = CreateKpiPanel();
        Controls.Add(pnlKpis);

        // Thanh công cụ thao tác
        var pnlToolbar = CreateToolbar();
        Controls.Add(pnlToolbar);

        // Khung hiển thị chính: Nửa trên là bảng các đợt thẻ, nửa dưới là các tab chi tiết
        var splitMain = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Horizontal,
            SplitterDistance = 300,
            SplitterWidth = 6
        };

        // Bảng danh sách các đợt thẻ
        var pnlTop = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12, 6, 12, 6) };
        var lblBatches = new Label
        {
            Text = "DANH SÁCH CÁC ĐỢT NHẬP THẺ & HẠCH TOÁN THU - CHI - LỢI NHUẬN TỪNG ĐỢT:",
            Dock = DockStyle.Top,
            Height = 26,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59)
        };

        _dgvBatches = CreateStyledDataGridView();
        _dgvBatches.Columns.Add("Code", "Mã Đợt");
        _dgvBatches.Columns.Add("Name", "Tên Đợt Thẻ");
        _dgvBatches.Columns.Add("Customer", "Khách Hàng / Đơn Vị");
        _dgvBatches.Columns.Add("Quantity", "Số Lượng");
        _dgvBatches.Columns.Add("BlankCost", "Tiền Phôi (đ)");
        _dgvBatches.Columns.Add("ProcessingCost", "Phí Gia Công (đ)");
        _dgvBatches.Columns.Add("TotalCost", "TỔNG CHI (đ)");
        _dgvBatches.Columns.Add("Revenue", "TỔNG THU (đ)");
        _dgvBatches.Columns.Add("NetProfit", "LỢI NHUẬN RÒNG (đ)");
        _dgvBatches.Columns.Add("Margin", "Tỷ Suất ROI (%)");
        _dgvBatches.Columns.Add("CreatedAt", "Ngày Tạo");

        _dgvBatches.SelectionChanged += OnBatchSelectionChanged;

        pnlTop.Controls.Add(_dgvBatches);
        pnlTop.Controls.Add(lblBatches);
        splitMain.Panel1.Controls.Add(pnlTop);

        // Nửa dưới: Hệ thống các Tab chi tiết
        var tabDetails = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Padding = new Point(16, 6)
        };

        // Tab 1: Chi tiết từng thẻ trong đợt đã chọn
        var tabCards = new TabPage("🔍 CHI TIẾT CÁC THẺ TRONG ĐỢT") { BackColor = Color.White };
        InitializeTabCards(tabCards);

        // Tab 2: So sánh hiệu quả kinh tế theo từng loại thẻ
        var tabTypeComparison = new TabPage("📊 SO SÁNH HIỆU QUẢ THEO LOẠI THẺ") { BackColor = Color.White };
        InitializeTabTypeComparison(tabTypeComparison);

        tabDetails.TabPages.AddRange([tabCards, tabTypeComparison]);
        splitMain.Panel2.Controls.Add(tabDetails);

        Controls.Add(splitMain);

        pnlHeader.BringToFront();
        pnlKpis.BringToFront();
        pnlToolbar.BringToFront();
        splitMain.BringToFront();
    }

    private Panel CreateAppHeader()
    {
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 65,
            BackColor = Color.FromArgb(15, 23, 42),
            Padding = new Padding(20, 8, 20, 8)
        };

        var lblBrand = new Label
        {
            Text = "CARDFLOW - QUẢN LÝ NHẬP THẺ & HẠCH TOÁN THU CHI LỢI NHUẬN",
            ForeColor = Color.FromArgb(56, 189, 248),
            Font = new Font("Segoe UI", 12.5f, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(18, 10)
        };

        var lblSub = new Label
        {
            Text = "Hệ thống quản lý theo từng đợt • Thiết kế chuẩn OOP & SOLID • Tự động tính Thu, Chi, Lợi nhuận và ROI",
            ForeColor = Color.FromArgb(203, 213, 225),
            Font = new Font("Segoe UI", 8.5f),
            AutoSize = true,
            Location = new Point(20, 36)
        };

        pnlHeader.Controls.AddRange([lblBrand, lblSub]);
        return pnlHeader;
    }

    private Panel CreateKpiPanel()
    {
        var pnlKpis = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 90,
            ColumnCount = 4,
            RowCount = 1,
            Padding = new Padding(12, 8, 12, 4),
            BackColor = Color.FromArgb(248, 250, 252)
        };
        pnlKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        pnlKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        pnlKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        pnlKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

        var card1 = CreateKpiCard("📦 QUY MÔ ĐỢT & THẺ", "0 Đợt (0 Thẻ)", Color.FromArgb(59, 130, 246), out _lblKpiBatches);
        var card2 = CreateKpiCard("💸 TỔNG CHI PHÍ ĐẦU VÀO", "0 đ", Color.FromArgb(239, 68, 68), out _lblKpiExpenses);
        var card3 = CreateKpiCard("💰 TỔNG DOANH THU THU VỀ", "0 đ", Color.FromArgb(16, 185, 129), out _lblKpiRevenue);
        var card4 = CreateKpiCard("💎 LỢI NHUẬN RÒNG & TỶ SUẤT", "0 đ (0%)", Color.FromArgb(168, 85, 247), out _lblKpiProfit);

        pnlKpis.Controls.AddRange([card1, card2, card3, card4]);
        return pnlKpis;
    }

    private Panel CreateKpiCard(string title, string initialValue, Color accentColor, out Label valueLabel)
    {
        var card = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(10, 8, 10, 8),
            Margin = new Padding(4)
        };

        var borderTop = new Panel
        {
            Dock = DockStyle.Top,
            Height = 3,
            BackColor = accentColor
        };

        var lblTitle = new Label
        {
            Text = title,
            ForeColor = Color.FromArgb(100, 116, 139),
            Font = new Font("Segoe UI", 8f, FontStyle.Bold),
            Dock = DockStyle.Top,
            Height = 20
        };

        valueLabel = new Label
        {
            Text = initialValue,
            ForeColor = Color.FromArgb(15, 23, 42),
            Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        card.Controls.AddRange([valueLabel, lblTitle, borderTop]);
        return card;
    }

    private Panel CreateToolbar()
    {
        var pnl = new Panel
        {
            Dock = DockStyle.Top,
            Height = 48,
            BackColor = Color.FromArgb(248, 250, 252),
            Padding = new Padding(12, 6, 12, 6)
        };

        _btnNewBatch = CreateButton("➕ Nhập Đợt Thẻ Mới", Color.FromArgb(37, 99, 235), Color.White, 160);
        _btnNewBatch.Click += OnNewBatchClicked;

        _btnRefresh = CreateButton("🔄 Làm Mới Dữ Liệu", Color.FromArgb(226, 232, 240), Color.FromArgb(51, 65, 85), 140);
        _btnRefresh.Click += async (s, e) => await ReloadAllDataAsync();

        _btnDeleteBatch = CreateButton("🗑️ Xóa Đợt Thẻ", Color.FromArgb(254, 226, 226), Color.FromArgb(185, 28, 28), 120);
        _btnDeleteBatch.Click += OnDeleteBatchClicked;

        int x = 12;
        foreach (var btn in new[] { _btnNewBatch, _btnRefresh, _btnDeleteBatch })
        {
            btn.Location = new Point(x, 6);
            pnl.Controls.Add(btn);
            x += btn.Width + 8;
        }

        return pnl;
    }

    private void InitializeTabCards(TabPage page)
    {
        var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

        _lblSelectedBatchInfo = new Label
        {
            Text = "Chọn một đợt thẻ ở bảng trên để xem chi tiết hạch toán từng thẻ:",
            Dock = DockStyle.Top,
            Height = 25,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59)
        };

        _dgvBatchCards = CreateStyledDataGridView();
        _dgvBatchCards.Columns.Add("CardCode", "Mã Thẻ");
        _dgvBatchCards.Columns.Add("HolderName", "Chủ Thẻ");
        _dgvBatchCards.Columns.Add("CardType", "Loại Thẻ");
        _dgvBatchCards.Columns.Add("BlankCost", "Giá Phôi (đ)");
        _dgvBatchCards.Columns.Add("ProcessingCost", "Phí Gia Công (đ)");
        _dgvBatchCards.Columns.Add("TotalCost", "Tổng Vốn 1 Thẻ (đ)");
        _dgvBatchCards.Columns.Add("SellingPrice", "Đơn Giá Bán (đ)");
        _dgvBatchCards.Columns.Add("Profit", "Lãi / Thẻ (đ)");
        _dgvBatchCards.Columns.Add("Margin", "Tỷ Suất Lãi (%)");

        pnl.Controls.Add(_dgvBatchCards);
        pnl.Controls.Add(_lblSelectedBatchInfo);
        page.Controls.Add(pnl);
    }

    private void InitializeTabTypeComparison(TabPage page)
    {
        var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

        var lbl = new Label
        {
            Text = "TỔNG HỢP & SO SÁNH HIỆU QUẢ KINH TẾ THEO TỪNG LOẠI THẺ TRONG HỆ THỐNG:",
            Dock = DockStyle.Top,
            Height = 25,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59)
        };

        _dgvCardTypeSummary = CreateStyledDataGridView();
        _dgvCardTypeSummary.Columns.Add("TypeKey", "Mã Loại Thẻ");
        _dgvCardTypeSummary.Columns.Add("TypeName", "Tên Loại Thẻ");
        _dgvCardTypeSummary.Columns.Add("Quantity", "Tổng Số Thẻ");
        _dgvCardTypeSummary.Columns.Add("BlankCost", "Tổng Tiền Phôi (đ)");
        _dgvCardTypeSummary.Columns.Add("ProcessingCost", "Tổng Phí Gia Công (đ)");
        _dgvCardTypeSummary.Columns.Add("TotalCost", "TỔNG CHI (đ)");
        _dgvCardTypeSummary.Columns.Add("Revenue", "TỔNG THU (đ)");
        _dgvCardTypeSummary.Columns.Add("Profit", "LỢI NHUẬN RÒNG (đ)");
        _dgvCardTypeSummary.Columns.Add("Margin", "Biên Lợi Nhuận (%)");

        pnl.Controls.Add(_dgvCardTypeSummary);
        pnl.Controls.Add(lbl);
        page.Controls.Add(pnl);
    }

    #region Xử lý dữ liệu & sự kiện giao diện

    private async Task ReloadAllDataAsync()
    {
        var batches = await _batchService.GetAllBatchesAsync();
        var summary = _financialCalculator.CalculateSummary(batches);

        // Cập nhật 4 thẻ KPI tài chính trên cùng
        _lblKpiBatches.Text = $"{summary.TotalBatches} Đợt ({summary.TotalCards:N0} Thẻ)";
        _lblKpiExpenses.Text = $"{summary.TotalCost:N0} đ\n(Phôi: {summary.TotalBlankCost:N0}đ | Gia công: {summary.TotalProcessingCost:N0}đ)";
        _lblKpiRevenue.Text = $"{summary.TotalRevenue:N0} đ";
        _lblKpiProfit.Text = $"{summary.NetProfit:N0} đ  (ROI: {summary.ProfitMarginPercentage:F1}%)";

        // Cập nhật bảng danh sách các đợt thẻ
        _dgvBatches.Rows.Clear();
        foreach (var b in batches)
        {
            _dgvBatches.Rows.Add(
                b.BatchCode,
                b.BatchName,
                b.CustomerName,
                b.TotalQuantity,
                b.TotalBlankCost.ToString("N0"),
                b.TotalProcessingCost.ToString("N0"),
                b.TotalCost.ToString("N0"),
                b.TotalRevenue.ToString("N0"),
                b.NetProfit.ToString("N0"),
                $"{b.ProfitMargin:F1}%",
                b.CreatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm")
            );
        }

        if (_dgvBatches.Rows.Count > 0 && _dgvBatches.CurrentRow == null)
        {
            _dgvBatches.Rows[0].Selected = true;
        }

        // Cập nhật bảng so sánh hiệu quả kinh tế theo từng loại thẻ
        UpdateCardTypeEconomics(batches);
    }

    private void UpdateCardTypeEconomics(IReadOnlyList<CardBatch> batches)
    {
        var allCards = batches.SelectMany(b => b.Cards).ToList();
        var types = _cardTypeRegistry.GetAll();

        _dgvCardTypeSummary.Rows.Clear();
        foreach (var t in types)
        {
            var cardsOfType = allCards.Where(c => c.GetCardTypeDisplayName().Contains(t.DisplayName[..Math.Min(10, t.DisplayName.Length)])).ToList();

            int count = cardsOfType.Count;
            decimal blankCost = cardsOfType.Sum(c => c.UnitBlankCost);
            decimal procCost = cardsOfType.Sum(c => c.CalculateUnitProcessingCost());
            decimal totalCost = blankCost + procCost;
            decimal revenue = cardsOfType.Sum(c => c.UnitSellingPrice);
            decimal profit = revenue - totalCost;
            decimal margin = revenue > 0 ? Math.Round((profit / revenue) * 100, 2) : 0m;

            _dgvCardTypeSummary.Rows.Add(
                t.TypeKey,
                t.DisplayName,
                count,
                blankCost.ToString("N0"),
                procCost.ToString("N0"),
                totalCost.ToString("N0"),
                revenue.ToString("N0"),
                profit.ToString("N0"),
                $"{margin:F1}%"
            );
        }
    }

    private async void OnBatchSelectionChanged(object? sender, EventArgs e)
    {
        if (_dgvBatches.CurrentRow == null)
        {
            _selectedBatch = null;
            _dgvBatchCards.Rows.Clear();
            _lblSelectedBatchInfo.Text = "Chọn một đợt thẻ ở bảng trên để xem chi tiết hạch toán từng thẻ:";
            return;
        }

        string? batchCode = _dgvBatches.CurrentRow.Cells["Code"].Value?.ToString();
        if (string.IsNullOrEmpty(batchCode)) return;

        var batches = await _batchService.GetAllBatchesAsync();
        _selectedBatch = batches.FirstOrDefault(b => b.BatchCode == batchCode);

        _dgvBatchCards.Rows.Clear();
        if (_selectedBatch != null)
        {
            _lblSelectedBatchInfo.Text = $"ĐỢT ĐANG CHỌN: {_selectedBatch.BatchCode} - {_selectedBatch.BatchName} | TỔNG CHI: {_selectedBatch.TotalCost:N0}đ | TỔNG THU: {_selectedBatch.TotalRevenue:N0}đ | LÃI: {_selectedBatch.NetProfit:N0}đ ({_selectedBatch.ProfitMargin:F1}%)";

            foreach (var card in _selectedBatch.Cards)
            {
                decimal procCost = card.CalculateUnitProcessingCost();
                decimal totalCost = card.UnitBlankCost + procCost;
                decimal unitProfit = card.UnitSellingPrice - totalCost;
                decimal margin = card.UnitSellingPrice > 0 ? Math.Round((unitProfit / card.UnitSellingPrice) * 100, 2) : 0;

                _dgvBatchCards.Rows.Add(
                    card.CardCode,
                    card.HolderName,
                    card.GetCardTypeDisplayName(),
                    card.UnitBlankCost.ToString("N0"),
                    procCost.ToString("N0"),
                    totalCost.ToString("N0"),
                    card.UnitSellingPrice.ToString("N0"),
                    unitProfit.ToString("N0"),
                    $"{margin:F1}%"
                );
            }
        }
    }

    private async void OnNewBatchClicked(object? sender, EventArgs e)
    {
        using var dialog = new NewBatchDialog(_cardTypeRegistry);
        if (dialog.ShowDialog(this) == DialogResult.OK && dialog.ResultRequest != null)
        {
            try
            {
                var newBatch = await _batchService.CreateBatchAsync(dialog.ResultRequest);
                MessageBox.Show(
                    $"Đã tạo thành công đợt thẻ '{newBatch.BatchName}'!\n" +
                    $"• Số lượng: {newBatch.TotalQuantity} thẻ\n" +
                    $"• Tổng vốn đầu vào (Chi): {newBatch.TotalCost:N0} đ\n" +
                    $"• Doanh thu dự kiến (Thu): {newBatch.TotalRevenue:N0} đ\n" +
                    $"• Lợi nhuận ròng: {newBatch.NetProfit:N0} đ (ROI: {newBatch.ProfitMargin:F1}%)",
                    "Hạch Toán Thành Công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                await ReloadAllDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo đợt thẻ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void OnDeleteBatchClicked(object? sender, EventArgs e)
    {
        if (_selectedBatch == null)
        {
            MessageBox.Show("Vui lòng chọn đợt thẻ cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            $"Bạn có chắc chắn muốn xóa đợt thẻ '{_selectedBatch.BatchName}' ({_selectedBatch.BatchCode})?",
            "Xác nhận xóa",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm == DialogResult.Yes)
        {
            await _batchService.DeleteBatchAsync(_selectedBatch.Id);
            await ReloadAllDataAsync();
        }
    }
    #endregion

    #region Hàm dựng giao diện phụ trợ

    private static Button CreateButton(string text, Color backColor, Color foreColor, int width)
    {
        var btn = new Button
        {
            Text = text,
            Width = width,
            Height = 35,
            BackColor = backColor,
            ForeColor = foreColor,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btn.FlatAppearance.BorderSize = 0;
        return btn;
    }

    private static DataGridView CreateStyledDataGridView()
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
        dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(6);
        dgv.ColumnHeadersHeight = 36;
        dgv.EnableHeadersVisualStyles = false;

        dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 231, 255);
        dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 41, 59);
        dgv.DefaultCellStyle.Padding = new Padding(4);
        dgv.RowTemplate.Height = 32;

        return dgv;
    }

    #endregion
}

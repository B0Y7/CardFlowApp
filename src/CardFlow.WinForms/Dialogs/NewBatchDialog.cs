using CardFlow.Application.Interfaces;

namespace CardFlow.WinForms.Dialogs;

// Hộp thoại tạo đợt nhập thẻ mới
public class NewBatchDialog : Form
{
    private readonly ICardTypeRegistry _registry;

    private TextBox _txtBatchCode = null!;
    private TextBox _txtBatchName = null!;
    private TextBox _txtCustomerName = null!;
    private ComboBox _cboCardType = null!;
    private NumericUpDown _numQuantity = null!;
    private TextBox _txtCodePrefix = null!;
    private TextBox _txtHolderPattern = null!;
    private NumericUpDown _numBlankCost = null!;
    private NumericUpDown _numSellingPrice = null!;
    private TextBox _txtNotes = null!;
    private Label _lblTypeDesc = null!;
    private Button _btnSave = null!;
    private Button _btnCancel = null!;

    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public CreateBatchRequest? ResultRequest { get; private set; }

    public NewBatchDialog(ICardTypeRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        InitializeComponent();
        PopulateCardTypes();
    }

    private void InitializeComponent()
    {
        Text = "Nhập Đợt Thẻ Mới (Tạo Lô Thẻ)";
        Width = 560;
        Height = 620;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.FromArgb(248, 249, 250);
        Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

        // Header
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 65,
            BackColor = Color.FromArgb(30, 41, 59),
            Padding = new Padding(20, 12, 20, 10)
        };

        var lblTitle = new Label
        {
            Text = "TẠO ĐỢT NHẬP THẺ & THIẾT LẬP GIÁ",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(18, 10)
        };

        var lblSub = new Label
        {
            Text = "Thiết lập thông tin lô thẻ, số lượng, giá phôi và đơn giá bán ra",
            ForeColor = Color.FromArgb(203, 213, 225),
            Font = new Font("Segoe UI", 8.5f),
            AutoSize = true,
            Location = new Point(19, 36)
        };

        pnlHeader.Controls.AddRange([lblTitle, lblSub]);

        // Body form controls
        var pnlBody = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(25, 15, 25, 15)
        };

        int y = 15;
        int lblWidth = 140;
        int inputWidth = 340;
        int rowHeight = 40;

        void AddRow(string labelText, Control inputControl)
        {
            var lbl = new Label
            {
                Text = labelText,
                Location = new Point(15, y + 4),
                Width = lblWidth,
                ForeColor = Color.FromArgb(51, 65, 85),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            inputControl.Location = new Point(165, y);
            inputControl.Width = inputWidth;

            pnlBody.Controls.Add(lbl);
            pnlBody.Controls.Add(inputControl);

            y += rowHeight;
        }

        _txtBatchCode = new TextBox { Text = $"BATCH-{DateTime.Now:yyyyMMdd}-{Random.Shared.Next(100, 999)}" };
        AddRow("Mã đợt thẻ:", _txtBatchCode);

        _txtBatchName = new TextBox { Text = "Đợt in thẻ cư dân Sunshine" };
        AddRow("Tên đợt thẻ:", _txtBatchName);

        _txtCustomerName = new TextBox { Text = "BQL Chung Cư Sunshine" };
        AddRow("Khách hàng:", _txtCustomerName);

        _cboCardType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        _cboCardType.SelectedIndexChanged += OnCardTypeSelected;
        AddRow("Loại thẻ:", _cboCardType);

        _lblTypeDesc = new Label
        {
            Text = "Mô tả loại thẻ...",
            Location = new Point(165, y - 5),
            Width = inputWidth,
            Height = 30,
            ForeColor = Color.FromArgb(100, 116, 139),
            Font = new Font("Segoe UI", 8.5f, FontStyle.Italic)
        };
        pnlBody.Controls.Add(_lblTypeDesc);
        y += 28;

        _numQuantity = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 10000,
            Value = 30
        };
        AddRow("Số lượng thẻ:", _numQuantity);

        _txtCodePrefix = new TextBox { Text = "CARD-" };
        AddRow("Tiền tố mã thẻ:", _txtCodePrefix);

        _txtHolderPattern = new TextBox { Text = "Cư Dân" };
        AddRow("Tên chủ thẻ mẫu:", _txtHolderPattern);

        _numBlankCost = new NumericUpDown
        {
            Minimum = 0,
            Maximum = 10000000,
            Increment = 500,
            ThousandsSeparator = true
        };
        AddRow("Giá phôi nhập (đ):", _numBlankCost);

        _numSellingPrice = new NumericUpDown
        {
            Minimum = 0,
            Maximum = 10000000,
            Increment = 1000,
            ThousandsSeparator = true
        };
        AddRow("Giá bán ra (đ):", _numSellingPrice);

        _txtNotes = new TextBox { Text = "Giao hàng đúng tiến độ hợp đồng" };
        AddRow("Ghi chú đợt:", _txtNotes);

        // Footer buttons
        var pnlFooter = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 60,
            BackColor = Color.FromArgb(241, 245, 249),
            Padding = new Padding(15)
        };

        _btnSave = new Button
        {
            Text = "✓ Tạo Đợt Thẻ",
            Width = 140,
            Height = 36,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
            Location = new Point(230, 12)
        };
        _btnSave.FlatAppearance.BorderSize = 0;
        _btnSave.Click += OnSaveClicked;

        _btnCancel = new Button
        {
            Text = "Hủy Bỏ",
            Width = 100,
            Height = 36,
            BackColor = Color.FromArgb(226, 232, 240),
            ForeColor = Color.FromArgb(71, 85, 105),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
            Cursor = Cursors.Hand,
            Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
            Location = new Point(380, 12),
            DialogResult = DialogResult.Cancel
        };
        _btnCancel.FlatAppearance.BorderSize = 0;

        pnlFooter.Controls.AddRange([_btnSave, _btnCancel]);

        Controls.AddRange([pnlBody, pnlHeader, pnlFooter]);
    }

    private void PopulateCardTypes()
    {
        var types = _registry.GetAll();
        _cboCardType.Items.Clear();

        foreach (var def in types)
        {
            _cboCardType.Items.Add(new CardTypeComboItem(def));
        }

        if (_cboCardType.Items.Count > 0)
        {
            _cboCardType.SelectedIndex = 0;
        }
    }

    private void OnCardTypeSelected(object? sender, EventArgs e)
    {
        if (_cboCardType.SelectedItem is CardTypeComboItem item)
        {
            var def = item.Definition;
            _lblTypeDesc.Text = def.Description;
            _numBlankCost.Value = def.DefaultBlankCost;
            _numSellingPrice.Value = def.DefaultSellingPrice;
        }
    }

    private void OnSaveClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_txtBatchCode.Text))
        {
            MessageBox.Show("Vui lòng nhập Mã đợt thẻ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtBatchCode.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(_txtBatchName.Text))
        {
            MessageBox.Show("Vui lòng nhập Tên đợt thẻ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtBatchName.Focus();
            return;
        }

        if (_cboCardType.SelectedItem is not CardTypeComboItem selectedItem)
        {
            MessageBox.Show("Vui lòng chọn loại thẻ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        ResultRequest = new CreateBatchRequest(
            _txtBatchCode.Text.Trim(),
            _txtBatchName.Text.Trim(),
            _txtCustomerName.Text.Trim(),
            selectedItem.Definition.TypeKey,
            (int)_numQuantity.Value,
            _txtCodePrefix.Text.Trim(),
            _txtHolderPattern.Text.Trim(),
            _numBlankCost.Value,
            _numSellingPrice.Value,
            _txtNotes.Text.Trim()
        );

        DialogResult = DialogResult.OK;
        Close();
    }

    private class CardTypeComboItem
    {
        public ICardTypeDefinition Definition { get; }

        public CardTypeComboItem(ICardTypeDefinition definition)
        {
            Definition = definition;
        }

        public override string ToString() => Definition.DisplayName;
    }
}

using QuanLyThuVien.BusinessLogic;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Forms
{
    public class FormSach : Form
    {
        private DataGridView dgvSach = null!;
        private TextBox txtTimKiem = null!;
        private ComboBox cboTheLoai = null!;
        private TextBox txtTacGia = null!;
        private Button btnTimKiem = null!;
        private Button btnThem = null!;
        private Button btnSua = null!;
        private Button btnXoa = null!;
        private Button btnLamMoi = null!;

        // Input fields
        private TextBox txtTenSach = null!;
        private TextBox txtTacGiaInput = null!;
        private ComboBox cboTheLoaiInput = null!;
        private TextBox txtNhaXuatBan = null!;
        private NumericUpDown nudNamXB = null!;
        private NumericUpDown nudSoLuong = null!;
        private TextBox txtMoTa = null!;

        private readonly SachBLL _bll = new SachBLL();
        private readonly TheLoaiBLL _theLoaiBLL = new TheLoaiBLL();

        // Colors
        private static readonly Color PrimaryDark = Color.FromArgb(25, 42, 86);
        private static readonly Color PrimaryAccent = Color.FromArgb(52, 152, 219);
        private static readonly Color SuccessColor = Color.FromArgb(46, 204, 113);
        private static readonly Color DangerColor = Color.FromArgb(231, 76, 60);
        private static readonly Color GrayColor = Color.FromArgb(149, 165, 166);
        private static readonly Color BgColor = Color.FromArgb(240, 243, 247);
        private static readonly Color CardBg = Color.White;

        public FormSach()
        {
            InitializeComponent();
            LoadTheLoai();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Quan ly Sach";
            this.Size = new Size(1200, 780);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9.5F);
            this.BackColor = BgColor;
            this.MinimumSize = new Size(900, 600);

            // === HEADER BAR ===
            var panelHeader = new Panel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 60;
            panelHeader.BackColor = PrimaryDark;
            panelHeader.Padding = new Padding(20, 0, 20, 0);

            var lblIcon = new Label();
            lblIcon.Text = "\U0001F4DA";
            lblIcon.Font = new Font("Segoe UI Emoji", 20F);
            lblIcon.ForeColor = Color.White;
            lblIcon.Dock = DockStyle.Left;
            lblIcon.Width = 50;
            lblIcon.TextAlign = ContentAlignment.MiddleCenter;
            panelHeader.Controls.Add(lblIcon);

            var lblTitle = new Label();
            lblTitle.Text = "QUAN LY SACH";
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Dock = DockStyle.Left;
            lblTitle.Width = 250;
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblTitle.Padding = new Padding(5, 0, 0, 0);
            panelHeader.Controls.Add(lblTitle);

            // Subtitle in header
            var lblSubtitle = new Label();
            lblSubtitle.Text = "Them, sua, xoa va tim kiem sach trong thu vien";
            lblSubtitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
            lblSubtitle.ForeColor = Color.FromArgb(180, 200, 230);
            lblSubtitle.Dock = DockStyle.Fill;
            lblSubtitle.TextAlign = ContentAlignment.MiddleLeft;
            panelHeader.Controls.Add(lblSubtitle);

            lblSubtitle.BringToFront();
            lblTitle.BringToFront();
            lblIcon.BringToFront();

            this.Controls.Add(panelHeader);

            // === MAIN CONTENT ===
            var panelMain = new Panel();
            panelMain.Dock = DockStyle.Fill;
            panelMain.Padding = new Padding(15, 10, 15, 10);
            panelMain.BackColor = BgColor;

            // Use two panels instead of SplitContainer (avoids SplitterDistance bugs)
            // Bottom: Input form (fixed height)
            var panelBottom = new Panel();
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Height = 240;
            panelBottom.BackColor = BgColor;

            // Top: Search + Grid (fills remaining)
            var panelTop = new Panel();
            panelTop.Dock = DockStyle.Fill;
            panelTop.BackColor = BgColor;

            // =====================
            // TOP PANEL: Search + Grid
            // =====================

            // -- Search Bar Card --
            var panelSearchCard = new Panel();
            panelSearchCard.Dock = DockStyle.Top;
            panelSearchCard.Height = 55;
            panelSearchCard.BackColor = CardBg;
            panelSearchCard.Padding = new Padding(12, 10, 12, 10);
            panelSearchCard.Margin = new Padding(0, 0, 0, 8);

            // Search row using FlowLayout for better auto-sizing
            var lblTK = new Label { Text = "\U0001F50D Tim kiem:", AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = PrimaryDark, Anchor = AnchorStyles.Left, Padding = new Padding(0, 5, 0, 0) };
            txtTimKiem = new TextBox { Width = 175, Height = 28, Anchor = AnchorStyles.Left };
            txtTimKiem.PlaceholderText = "Ten sach...";

            var lblTL = new Label { Text = "The loai:", AutoSize = true, Anchor = AnchorStyles.Left, Padding = new Padding(10, 5, 0, 0) };
            cboTheLoai = new ComboBox { Width = 140, DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Left };

            var lblTG = new Label { Text = "Tac gia:", AutoSize = true, Anchor = AnchorStyles.Left, Padding = new Padding(10, 5, 0, 0) };
            txtTacGia = new TextBox { Width = 140, Anchor = AnchorStyles.Left };
            txtTacGia.PlaceholderText = "Tac gia...";

            btnTimKiem = CreateStyledButton("Tim kiem", PrimaryAccent, 90);
            btnTimKiem.Click += BtnTimKiem_Click;

            btnLamMoi = CreateStyledButton("Lam moi", GrayColor, 85);
            btnLamMoi.Click += (s, e) => { txtTimKiem.Clear(); txtTacGia.Clear(); if (cboTheLoai.Items.Count > 0) cboTheLoai.SelectedIndex = 0; LoadData(); };

            // Position search controls
            int sx = 12;
            lblTK.Location = new Point(sx, 12); sx += lblTK.PreferredWidth + 5;
            txtTimKiem.Location = new Point(sx, 10); sx += txtTimKiem.Width + 10;
            lblTL.Location = new Point(sx, 12); sx += lblTL.PreferredWidth + 5;
            cboTheLoai.Location = new Point(sx, 10); sx += cboTheLoai.Width + 10;
            lblTG.Location = new Point(sx, 12); sx += lblTG.PreferredWidth + 5;
            txtTacGia.Location = new Point(sx, 10); sx += txtTacGia.Width + 15;
            btnTimKiem.Location = new Point(sx, 8); sx += btnTimKiem.Width + 5;
            btnLamMoi.Location = new Point(sx, 8);

            panelSearchCard.Controls.AddRange(new Control[] { lblTK, txtTimKiem, lblTL, cboTheLoai, lblTG, txtTacGia, btnTimKiem, btnLamMoi });

            // -- DataGridView Card --
            var panelGridCard = new Panel();
            panelGridCard.Dock = DockStyle.Fill;
            panelGridCard.BackColor = CardBg;
            panelGridCard.Padding = new Padding(2);

            dgvSach = new DataGridView();
            dgvSach.Dock = DockStyle.Fill;
            dgvSach.BackgroundColor = CardBg;
            dgvSach.BorderStyle = BorderStyle.None;
            dgvSach.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSach.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSach.MultiSelect = false;
            dgvSach.ReadOnly = true;
            dgvSach.AllowUserToAddRows = false;
            dgvSach.RowHeadersVisible = false;
            dgvSach.Font = new Font("Segoe UI", 9.5F);
            dgvSach.GridColor = Color.FromArgb(230, 235, 240);
            dgvSach.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvSach.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219, 80);
            dgvSach.DefaultCellStyle.SelectionForeColor = PrimaryDark;
            dgvSach.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
            dgvSach.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvSach.ColumnHeadersDefaultCellStyle.BackColor = PrimaryDark;
            dgvSach.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSach.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvSach.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSach.ColumnHeadersDefaultCellStyle.Padding = new Padding(4);
            dgvSach.EnableHeadersVisualStyles = false;
            dgvSach.ColumnHeadersHeight = 40;
            dgvSach.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvSach.RowTemplate.Height = 34;
            dgvSach.CellClick += DgvSach_CellClick;

            panelGridCard.Controls.Add(dgvSach);

            // Add the grid separator
            var gridSpacer = new Panel();
            gridSpacer.Dock = DockStyle.Top;
            gridSpacer.Height = 8;
            gridSpacer.BackColor = BgColor;

            panelTop.Controls.Add(panelGridCard);
            panelTop.Controls.Add(gridSpacer);
            panelTop.Controls.Add(panelSearchCard);

            // =====================
            // BOTTOM PANEL: Input Form
            // =====================
            var panelInputCard = new Panel();
            panelInputCard.Dock = DockStyle.Fill;
            panelInputCard.BackColor = CardBg;
            panelInputCard.Padding = new Padding(15, 10, 15, 10);

            // Section title with accent bar
            var panelAccent = new Panel();
            panelAccent.Dock = DockStyle.Top;
            panelAccent.Height = 4;
            panelAccent.BackColor = PrimaryAccent;
            panelInputCard.Controls.Add(panelAccent);

            var lblInputTitle = new Label();
            lblInputTitle.Text = "\u270F\uFE0F  THONG TIN SACH";
            lblInputTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblInputTitle.ForeColor = PrimaryDark;
            lblInputTitle.Dock = DockStyle.Top;
            lblInputTitle.Height = 35;
            lblInputTitle.Padding = new Padding(5, 8, 0, 0);
            panelInputCard.Controls.Add(lblInputTitle);

            // Inner input area
            var panelInputInner = new Panel();
            panelInputInner.Dock = DockStyle.Fill;

            int y = 5;
            int labelWidth = 85;
            int col2X = 480;

            // Row 1: Ten sach + Tac gia
            panelInputInner.Controls.Add(CreateInputLabel("Ten sach:", 15, y));
            txtTenSach = CreateInputTextBox(15 + labelWidth, y, 360);
            panelInputInner.Controls.Add(txtTenSach);

            panelInputInner.Controls.Add(CreateInputLabel("Tac gia:", col2X, y));
            txtTacGiaInput = CreateInputTextBox(col2X + labelWidth, y, 280);
            panelInputInner.Controls.Add(txtTacGiaInput);

            y += 36;
            // Row 2: The loai + NXB + Nam XB
            panelInputInner.Controls.Add(CreateInputLabel("The loai:", 15, y));
            cboTheLoaiInput = new ComboBox { Location = new Point(15 + labelWidth, y), Width = 190, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) };
            panelInputInner.Controls.Add(cboTheLoaiInput);

            panelInputInner.Controls.Add(CreateInputLabel("NXB:", 310, y));
            txtNhaXuatBan = CreateInputTextBox(310 + 45, y, 190);
            panelInputInner.Controls.Add(txtNhaXuatBan);

            panelInputInner.Controls.Add(CreateInputLabel("Nam XB:", col2X + 85, y));
            nudNamXB = new NumericUpDown { Location = new Point(col2X + 85 + 70, y), Width = 100, Minimum = 1900, Maximum = 2100, Value = 2024, Font = new Font("Segoe UI", 9.5F) };
            panelInputInner.Controls.Add(nudNamXB);

            y += 36;
            // Row 3: So luong + Mo ta
            panelInputInner.Controls.Add(CreateInputLabel("So luong:", 15, y));
            nudSoLuong = new NumericUpDown { Location = new Point(15 + labelWidth, y), Width = 100, Minimum = 1, Maximum = 9999, Value = 1, Font = new Font("Segoe UI", 9.5F) };
            panelInputInner.Controls.Add(nudSoLuong);

            panelInputInner.Controls.Add(CreateInputLabel("Mo ta:", 220, y));
            txtMoTa = CreateInputTextBox(220 + 50, y, 575);
            panelInputInner.Controls.Add(txtMoTa);

            y += 42;
            // Button row
            int bx = 15;
            btnThem = CreateActionButton("\u2795 Them moi", SuccessColor, new Point(bx, y));
            btnThem.Click += BtnThem_Click;
            panelInputInner.Controls.Add(btnThem);

            bx += btnThem.Width + 8;
            btnSua = CreateActionButton("\u270F Cap nhat", PrimaryAccent, new Point(bx, y));
            btnSua.Click += BtnSua_Click;
            panelInputInner.Controls.Add(btnSua);

            bx += btnSua.Width + 8;
            btnXoa = CreateActionButton("\U0001F5D1 Xoa", DangerColor, new Point(bx, y));
            btnXoa.Click += BtnXoa_Click;
            panelInputInner.Controls.Add(btnXoa);

            bx += btnXoa.Width + 8;
            var btnClear = CreateActionButton("\U0001F504 Xoa form", GrayColor, new Point(bx, y));
            btnClear.Click += (s, e) => ClearInput();
            panelInputInner.Controls.Add(btnClear);

            panelInputCard.Controls.Add(panelInputInner);
            panelInputInner.BringToFront();

            panelBottom.Controls.Add(panelInputCard);

            panelMain.Controls.Add(panelTop);
            panelMain.Controls.Add(panelBottom);
            panelTop.BringToFront();
            this.Controls.Add(panelMain);

            // Bring to front order
            panelMain.BringToFront();
        }

        private Label CreateInputLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Location = new Point(x, y + 4),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 95, 120)
            };
        }

        private TextBox CreateInputTextBox(int x, int y, int width)
        {
            var txt = new TextBox();
            txt.Location = new Point(x, y);
            txt.Width = width;
            txt.Font = new Font("Segoe UI", 9.5F);
            txt.BorderStyle = BorderStyle.FixedSingle;
            return txt;
        }

        private Button CreateStyledButton(string text, Color color, int width)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Size = new Size(width, 30);
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        private Button CreateActionButton(string text, Color color, Point location)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Location = location;
            btn.Size = new Size(120, 36);
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;

            // Hover effects
            btn.MouseEnter += (s, e) => { btn.BackColor = ControlPaint.Light(color, 0.15f); };
            btn.MouseLeave += (s, e) => { btn.BackColor = color; };
            return btn;
        }

        private void LoadTheLoai()
        {
            var list = _theLoaiBLL.GetAll();

            // For search combo
            cboTheLoai.Items.Clear();
            cboTheLoai.Items.Add("-- Tat ca --");
            foreach (var tl in list)
                cboTheLoai.Items.Add(tl);
            cboTheLoai.DisplayMember = "TenTheLoai";
            cboTheLoai.SelectedIndex = 0;

            // For input combo
            cboTheLoaiInput.Items.Clear();
            cboTheLoaiInput.Items.Add("-- Chon the loai --");
            foreach (var tl in list)
                cboTheLoaiInput.Items.Add(tl);
            cboTheLoaiInput.DisplayMember = "TenTheLoai";
            cboTheLoaiInput.SelectedIndex = 0;
        }

        private void LoadData()
        {
            var list = _bll.GetAll();
            DisplayData(list);
        }

        private void DisplayData(List<Sach> list)
        {
            dgvSach.DataSource = null;
            dgvSach.Columns.Clear();

            dgvSach.DataSource = list;

            if (dgvSach.Columns.Count > 0)
            {
                dgvSach.Columns["MaSach"].HeaderText = "Ma";
                dgvSach.Columns["MaSach"].MinimumWidth = 50;
                dgvSach.Columns["MaSach"].FillWeight = 30;
                dgvSach.Columns["TenSach"].HeaderText = "Ten sach";
                dgvSach.Columns["TenSach"].FillWeight = 130;
                dgvSach.Columns["TacGia"].HeaderText = "Tac gia";
                dgvSach.Columns["TacGia"].FillWeight = 100;
                dgvSach.Columns["TenTheLoai"].HeaderText = "The loai";
                dgvSach.Columns["TenTheLoai"].FillWeight = 80;
                dgvSach.Columns["NhaXuatBan"].HeaderText = "NXB";
                dgvSach.Columns["NhaXuatBan"].FillWeight = 80;
                dgvSach.Columns["NamXuatBan"].HeaderText = "Nam XB";
                dgvSach.Columns["NamXuatBan"].MinimumWidth = 70;
                dgvSach.Columns["NamXuatBan"].FillWeight = 50;
                dgvSach.Columns["NamXuatBan"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvSach.Columns["SoLuongTong"].HeaderText = "Tong";
                dgvSach.Columns["SoLuongTong"].MinimumWidth = 55;
                dgvSach.Columns["SoLuongTong"].FillWeight = 35;
                dgvSach.Columns["SoLuongTong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvSach.Columns["SoLuongCon"].HeaderText = "Con";
                dgvSach.Columns["SoLuongCon"].MinimumWidth = 55;
                dgvSach.Columns["SoLuongCon"].FillWeight = 35;
                dgvSach.Columns["SoLuongCon"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvSach.Columns["TrangThai"].HeaderText = "Trang thai";
                dgvSach.Columns["TrangThai"].MinimumWidth = 75;
                dgvSach.Columns["TrangThai"].FillWeight = 50;
                dgvSach.Columns["TrangThai"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // Hide unnecessary columns
                dgvSach.Columns["MaTheLoai"].Visible = false;
                dgvSach.Columns["MoTa"].Visible = false;
                dgvSach.Columns["NgayTao"].Visible = false;
                dgvSach.Columns["NgayCapNhat"].Visible = false;
                dgvSach.Columns["SoLuongDangMuon"].Visible = false;
                dgvSach.Columns["ConSach"].Visible = false;
            }
        }

        private void BtnTimKiem_Click(object? sender, EventArgs e)
        {
            int? maTheLoai = null;
            if (cboTheLoai.SelectedIndex > 0 && cboTheLoai.SelectedItem is TheLoai selectedTL)
                maTheLoai = selectedTL.MaTheLoai;

            var list = _bll.Search(txtTimKiem.Text, maTheLoai, txtTacGia.Text);
            DisplayData(list);
        }

        private void DgvSach_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvSach.Rows[e.RowIndex];
            txtTenSach.Text = row.Cells["TenSach"].Value?.ToString();
            txtTacGiaInput.Text = row.Cells["TacGia"].Value?.ToString();
            txtNhaXuatBan.Text = row.Cells["NhaXuatBan"].Value?.ToString();
            txtMoTa.Text = row.Cells["MoTa"].Value?.ToString();

            if (row.Cells["NamXuatBan"].Value != null && int.TryParse(row.Cells["NamXuatBan"].Value.ToString(), out int nam))
                nudNamXB.Value = nam;

            nudSoLuong.Value = Convert.ToInt32(row.Cells["SoLuongTong"].Value);

            // Select the loai in combo
            int maTheLoai = Convert.ToInt32(row.Cells["MaTheLoai"].Value);
            for (int i = 1; i < cboTheLoaiInput.Items.Count; i++)
            {
                if (cboTheLoaiInput.Items[i] is TheLoai tl && tl.MaTheLoai == maTheLoai)
                {
                    cboTheLoaiInput.SelectedIndex = i;
                    break;
                }
            }
        }

        private void BtnThem_Click(object? sender, EventArgs e)
        {
            var sach = GetSachFromInput();
            if (sach == null) return;

            var (success, message) = _bll.Insert(sach);
            if (success)
            {
                MessageBox.Show(message, "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInput();
                LoadData();
            }
            else
            {
                MessageBox.Show(message, "Loi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnSua_Click(object? sender, EventArgs e)
        {
            if (dgvSach.CurrentRow == null) { MessageBox.Show("Vui long chon sach can sua!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var sach = GetSachFromInput();
            if (sach == null) return;
            sach.MaSach = Convert.ToInt32(dgvSach.CurrentRow.Cells["MaSach"].Value);
            sach.SoLuongCon = Convert.ToInt32(dgvSach.CurrentRow.Cells["SoLuongCon"].Value);

            // Dieu chinh so luong con neu thay doi so luong tong
            int oldTong = Convert.ToInt32(dgvSach.CurrentRow.Cells["SoLuongTong"].Value);
            if (sach.SoLuongTong != oldTong)
            {
                sach.SoLuongCon = sach.SoLuongCon + (sach.SoLuongTong - oldTong);
                if (sach.SoLuongCon < 0) sach.SoLuongCon = 0;
            }

            var (success, message) = _bll.Update(sach);
            if (success)
            {
                MessageBox.Show(message, "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            else
            {
                MessageBox.Show(message, "Loi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnXoa_Click(object? sender, EventArgs e)
        {
            if (dgvSach.CurrentRow == null) { MessageBox.Show("Vui long chon sach can xoa!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            int maSach = Convert.ToInt32(dgvSach.CurrentRow.Cells["MaSach"].Value);
            string tenSach = dgvSach.CurrentRow.Cells["TenSach"].Value?.ToString() ?? "";

            if (MessageBox.Show($"Ban co chac chan muon xoa sach '{tenSach}'?", "Xac nhan xoa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var (success, message) = _bll.Delete(maSach);
                if (success)
                {
                    MessageBox.Show(message, "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInput();
                    LoadData();
                }
                else
                {
                    MessageBox.Show(message, "Loi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private Sach? GetSachFromInput()
        {
            if (cboTheLoaiInput.SelectedIndex <= 0 || cboTheLoaiInput.SelectedItem is not TheLoai theLoai)
            {
                MessageBox.Show("Vui long chon the loai!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            return new Sach
            {
                TenSach = txtTenSach.Text.Trim(),
                TacGia = txtTacGiaInput.Text.Trim(),
                MaTheLoai = theLoai.MaTheLoai,
                NhaXuatBan = txtNhaXuatBan.Text.Trim(),
                NamXuatBan = (int)nudNamXB.Value,
                SoLuongTong = (int)nudSoLuong.Value,
                SoLuongCon = (int)nudSoLuong.Value,
                MoTa = txtMoTa.Text.Trim(),
                TrangThai = true
            };
        }

        private void ClearInput()
        {
            txtTenSach.Clear();
            txtTacGiaInput.Clear();
            cboTheLoaiInput.SelectedIndex = 0;
            txtNhaXuatBan.Clear();
            nudNamXB.Value = 2024;
            nudSoLuong.Value = 1;
            txtMoTa.Clear();
        }
    }
}

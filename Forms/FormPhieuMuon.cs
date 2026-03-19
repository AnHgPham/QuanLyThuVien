using QuanLyThuVien.BusinessLogic;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Forms
{
    public class FormPhieuMuon : Form
    {
        // Danh sach phieu muon
        private DataGridView dgvPhieuMuon = null!;
        private TextBox txtTimKiem = null!;
        private ComboBox cboTrangThai = null!;
        private Button btnTimKiem = null!;

        // Tab control
        private TabControl tabControl = null!;
        private TabPage tabLapPhieu = null!;
        private TabPage tabChiTiet = null!;

        // Tab Lap phieu
        private ComboBox cboBanDoc = null!;
        private DateTimePicker dtpNgayMuon = null!;
        private NumericUpDown nudSoNgay = null!;
        private TextBox txtGhiChu = null!;
        private ComboBox cboSach = null!;
        private Button btnThemSach = null!;
        private ListBox lstSachMuon = null!;
        private Button btnXoaSach = null!;
        private Button btnLapPhieu = null!;
        private Label lblSachDangMuon = null!;

        // Tab Chi tiet
        private DataGridView dgvChiTiet = null!;
        private Button btnTraSach = null!;
        private Button btnXoaPhieu = null!;

        private readonly PhieuMuonBLL _bll = new PhieuMuonBLL();
        private readonly BanDocBLL _banDocBLL = new BanDocBLL();
        private readonly SachBLL _sachBLL = new SachBLL();

        private List<Sach> _sachDaChon = new List<Sach>();

        public FormPhieuMuon()
        {
            InitializeComponent();
            LoadComboData();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Phieu Muon / Tra Sach";
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9.5F);
            this.BackColor = Color.FromArgb(240, 243, 247);
            this.MinimumSize = new Size(950, 600);

            // === HEADER BAR ===
            var panelHeader = new Panel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 55;
            panelHeader.BackColor = Color.FromArgb(25, 42, 86);
            panelHeader.Padding = new Padding(20, 0, 20, 0);

            var lblTitle = new Label();
            lblTitle.Text = "MUON / TRA SACH";
            lblTitle.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            panelHeader.Controls.Add(lblTitle);

            // === SEARCH PANEL ===
            var panelSearch = new Panel();
            panelSearch.Dock = DockStyle.Top;
            panelSearch.Height = 50;
            panelSearch.BackColor = Color.White;
            panelSearch.Padding = new Padding(15, 8, 15, 8);

            var lblTK = new Label { Text = "Tim kiem:", AutoSize = true, Location = new Point(15, 15) };
            txtTimKiem = new TextBox { Location = new Point(85, 12), Width = 200, PlaceholderText = "Ma phieu, ten ban doc..." };

            var lblTT = new Label { Text = "Trang thai:", AutoSize = true, Location = new Point(300, 15) };
            cboTrangThai = new ComboBox { Location = new Point(380, 12), Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };
            cboTrangThai.Items.AddRange(new object[] { "Tat ca", "Dang muon", "Da tra", "Qua han" });
            cboTrangThai.SelectedIndex = 0;

            btnTimKiem = new Button { Text = "Tim", Location = new Point(525, 10), Width = 70, Height = 30 };
            btnTimKiem.BackColor = Color.FromArgb(52, 152, 219);
            btnTimKiem.ForeColor = Color.White;
            btnTimKiem.FlatStyle = FlatStyle.Flat;
            btnTimKiem.FlatAppearance.BorderSize = 0;
            btnTimKiem.Click += (s, e) => { var list = _bll.Search(txtTimKiem.Text, cboTrangThai.SelectedItem?.ToString()); DisplayData(list); };

            var btnLamMoi = new Button { Text = "Lam moi", Location = new Point(605, 10), Width = 80, Height = 30 };
            btnLamMoi.BackColor = Color.FromArgb(149, 165, 166);
            btnLamMoi.ForeColor = Color.White;
            btnLamMoi.FlatStyle = FlatStyle.Flat;
            btnLamMoi.FlatAppearance.BorderSize = 0;
            btnLamMoi.Click += (s, e) => { txtTimKiem.Clear(); cboTrangThai.SelectedIndex = 0; LoadData(); };

            panelSearch.Controls.AddRange(new Control[] { lblTK, txtTimKiem, lblTT, cboTrangThai, btnTimKiem, btnLamMoi });

            // === MAIN AREA: Left (DataGrid) + Right (Tabs) ===
            // Using two panels instead of SplitContainer to avoid MinSize/SplitterDistance bugs

            // RIGHT PANEL: Tabs (docked right, fixed width)
            var panelRight = new Panel();
            panelRight.Dock = DockStyle.Right;
            panelRight.Width = 460;
            panelRight.BackColor = Color.White;
            panelRight.Padding = new Padding(5, 0, 0, 0);

            // LEFT PANEL: DataGridView (fills remaining space)
            var panelLeft = new Panel();
            panelLeft.Dock = DockStyle.Fill;
            panelLeft.BackColor = Color.White;

            // DataGridView
            dgvPhieuMuon = new DataGridView();
            dgvPhieuMuon.Dock = DockStyle.Fill;
            dgvPhieuMuon.BackgroundColor = Color.White;
            dgvPhieuMuon.BorderStyle = BorderStyle.None;
            dgvPhieuMuon.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPhieuMuon.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPhieuMuon.MultiSelect = false;
            dgvPhieuMuon.ReadOnly = true;
            dgvPhieuMuon.AllowUserToAddRows = false;
            dgvPhieuMuon.RowHeadersVisible = false;
            dgvPhieuMuon.Font = new Font("Segoe UI", 9F);
            dgvPhieuMuon.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
            dgvPhieuMuon.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(25, 42, 86);
            dgvPhieuMuon.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPhieuMuon.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvPhieuMuon.EnableHeadersVisualStyles = false;
            dgvPhieuMuon.ColumnHeadersHeight = 35;
            dgvPhieuMuon.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvPhieuMuon.RowTemplate.Height = 28;
            dgvPhieuMuon.CellClick += DgvPhieuMuon_CellClick;
            panelLeft.Controls.Add(dgvPhieuMuon);

            // Tab Control
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 10F);

            // --- Tab 1: Lap phieu muon ---
            tabLapPhieu = new TabPage("Lap phieu muon");
            tabLapPhieu.BackColor = Color.White;
            tabLapPhieu.Padding = new Padding(10);
            tabLapPhieu.AutoScroll = true;

            int y = 10;
            int inputW = 310;

            tabLapPhieu.Controls.Add(new Label { Text = "Ban doc:", Location = new Point(10, y + 3), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) });
            cboBanDoc = new ComboBox { Location = new Point(90, y), Width = inputW, DropDownStyle = ComboBoxStyle.DropDownList };
            cboBanDoc.SelectedIndexChanged += CboBanDoc_SelectedIndexChanged;
            tabLapPhieu.Controls.Add(cboBanDoc);

            y += 28;
            lblSachDangMuon = new Label { Text = "Sach dang muon: 0 / 5", Location = new Point(90, y), AutoSize = true, ForeColor = Color.Gray, Font = new Font("Segoe UI", 8.5F) };
            tabLapPhieu.Controls.Add(lblSachDangMuon);

            y += 22;
            tabLapPhieu.Controls.Add(new Label { Text = "Ngay muon:", Location = new Point(10, y + 3), AutoSize = true });
            dtpNgayMuon = new DateTimePicker { Location = new Point(90, y), Width = 145, Format = DateTimePickerFormat.Short };
            tabLapPhieu.Controls.Add(dtpNgayMuon);

            tabLapPhieu.Controls.Add(new Label { Text = "So ngay:", Location = new Point(245, y + 3), AutoSize = true });
            nudSoNgay = new NumericUpDown { Location = new Point(310, y), Width = 90, Minimum = 1, Maximum = 90, Value = 14 };
            tabLapPhieu.Controls.Add(nudSoNgay);

            y += 30;
            tabLapPhieu.Controls.Add(new Label { Text = "Ghi chu:", Location = new Point(10, y + 3), AutoSize = true });
            txtGhiChu = new TextBox { Location = new Point(90, y), Width = inputW };
            tabLapPhieu.Controls.Add(txtGhiChu);

            y += 32;
            var lblSachTitle = new Label { Text = "CHON SACH MUON:", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Location = new Point(10, y), AutoSize = true, ForeColor = Color.FromArgb(25, 42, 86) };
            tabLapPhieu.Controls.Add(lblSachTitle);

            y += 22;
            cboSach = new ComboBox { Location = new Point(10, y), Width = inputW, DropDownStyle = ComboBoxStyle.DropDownList };
            tabLapPhieu.Controls.Add(cboSach);

            btnThemSach = new Button { Text = "+", Location = new Point(inputW + 15, y - 2), Width = 35, Height = 28 };
            btnThemSach.BackColor = Color.FromArgb(46, 204, 113);
            btnThemSach.ForeColor = Color.White;
            btnThemSach.FlatStyle = FlatStyle.Flat;
            btnThemSach.FlatAppearance.BorderSize = 0;
            btnThemSach.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnThemSach.Click += BtnThemSach_Click;
            tabLapPhieu.Controls.Add(btnThemSach);

            btnXoaSach = new Button { Text = "-", Location = new Point(inputW + 55, y - 2), Width = 35, Height = 28 };
            btnXoaSach.BackColor = Color.FromArgb(231, 76, 60);
            btnXoaSach.ForeColor = Color.White;
            btnXoaSach.FlatStyle = FlatStyle.Flat;
            btnXoaSach.FlatAppearance.BorderSize = 0;
            btnXoaSach.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnXoaSach.Click += BtnXoaSach_Click;
            tabLapPhieu.Controls.Add(btnXoaSach);

            y += 30;
            lstSachMuon = new ListBox { Location = new Point(10, y), Size = new Size(inputW + 80, 100) };
            tabLapPhieu.Controls.Add(lstSachMuon);

            y += 108;
            btnLapPhieu = new Button { Text = "LAP PHIEU MUON", Location = new Point(10, y), Size = new Size(inputW + 80, 38) };
            btnLapPhieu.BackColor = Color.FromArgb(25, 42, 86);
            btnLapPhieu.ForeColor = Color.White;
            btnLapPhieu.FlatStyle = FlatStyle.Flat;
            btnLapPhieu.FlatAppearance.BorderSize = 0;
            btnLapPhieu.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnLapPhieu.Cursor = Cursors.Hand;
            btnLapPhieu.Click += BtnLapPhieu_Click;
            tabLapPhieu.Controls.Add(btnLapPhieu);

            tabControl.TabPages.Add(tabLapPhieu);

            // --- Tab 2: Chi tiet phieu ---
            tabChiTiet = new TabPage("Chi tiet phieu");
            tabChiTiet.BackColor = Color.White;
            tabChiTiet.Padding = new Padding(10);

            dgvChiTiet = new DataGridView();
            dgvChiTiet.Dock = DockStyle.Fill;
            dgvChiTiet.BackgroundColor = Color.White;
            dgvChiTiet.BorderStyle = BorderStyle.None;
            dgvChiTiet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvChiTiet.ReadOnly = true;
            dgvChiTiet.AllowUserToAddRows = false;
            dgvChiTiet.RowHeadersVisible = false;
            dgvChiTiet.Font = new Font("Segoe UI", 9F);
            dgvChiTiet.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(25, 42, 86);
            dgvChiTiet.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvChiTiet.EnableHeadersVisualStyles = false;
            dgvChiTiet.ColumnHeadersHeight = 32;
            dgvChiTiet.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            var panelChiTietBtn = new Panel();
            panelChiTietBtn.Dock = DockStyle.Bottom;
            panelChiTietBtn.Height = 50;

            btnTraSach = new Button { Text = "TRA SACH", Location = new Point(10, 8), Size = new Size(180, 35) };
            btnTraSach.BackColor = Color.FromArgb(46, 204, 113);
            btnTraSach.ForeColor = Color.White;
            btnTraSach.FlatStyle = FlatStyle.Flat;
            btnTraSach.FlatAppearance.BorderSize = 0;
            btnTraSach.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnTraSach.Click += BtnTraSach_Click;
            panelChiTietBtn.Controls.Add(btnTraSach);

            btnXoaPhieu = new Button { Text = "Xoa phieu", Location = new Point(200, 8), Size = new Size(120, 35) };
            btnXoaPhieu.BackColor = Color.FromArgb(231, 76, 60);
            btnXoaPhieu.ForeColor = Color.White;
            btnXoaPhieu.FlatStyle = FlatStyle.Flat;
            btnXoaPhieu.FlatAppearance.BorderSize = 0;
            btnXoaPhieu.Click += BtnXoaPhieu_Click;
            panelChiTietBtn.Controls.Add(btnXoaPhieu);

            tabChiTiet.Controls.Add(dgvChiTiet);
            tabChiTiet.Controls.Add(panelChiTietBtn);
            tabControl.TabPages.Add(tabChiTiet);

            panelRight.Controls.Add(tabControl);

            // === ADD TO FORM IN CORRECT ORDER ===
            // Fill control first, then Right, then Top controls last
            this.Controls.Add(panelLeft);
            this.Controls.Add(panelRight);
            this.Controls.Add(panelSearch);
            this.Controls.Add(panelHeader);

            // Ensure correct z-order
            panelLeft.BringToFront();
        }

        private void LoadComboData()
        {
            cboBanDoc.Items.Clear();
            cboBanDoc.Items.Add("-- Chon ban doc --");
            foreach (var bd in _banDocBLL.GetAll())
            {
                cboBanDoc.Items.Add(bd);
            }
            cboBanDoc.DisplayMember = "HoTen";
            cboBanDoc.SelectedIndex = 0;

            LoadSachCombo();
        }

        private void LoadSachCombo()
        {
            cboSach.Items.Clear();
            cboSach.Items.Add("-- Chon sach --");
            foreach (var s in _sachBLL.GetAll())
            {
                if (s.SoLuongCon > 0)
                    cboSach.Items.Add(s);
            }
            cboSach.DisplayMember = "TenSach";
            cboSach.SelectedIndex = 0;
        }

        private void CboBanDoc_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cboBanDoc.SelectedIndex > 0 && cboBanDoc.SelectedItem is BanDoc bd)
            {
                int sachDangMuon = _banDocBLL.DemSachDangMuon(bd.MaBanDoc);
                lblSachDangMuon.Text = $"Sach dang muon: {sachDangMuon} / {PhieuMuonBLL.MAX_SACH_MUON}";
                lblSachDangMuon.ForeColor = sachDangMuon >= PhieuMuonBLL.MAX_SACH_MUON ? Color.Red : Color.Gray;
            }
            else
            {
                lblSachDangMuon.Text = "Sach dang muon: 0 / 5";
                lblSachDangMuon.ForeColor = Color.Gray;
            }
        }

        private void LoadData()
        {
            _bll.CapNhatQuaHan();
            var list = _bll.GetAll();
            DisplayData(list);
        }

        private void DisplayData(List<PhieuMuon> list)
        {
            dgvPhieuMuon.DataSource = null;
            dgvPhieuMuon.DataSource = list;

            if (dgvPhieuMuon.Columns.Count > 0)
            {
                dgvPhieuMuon.Columns["MaPhieuMuon"].HeaderText = "Ma";
                dgvPhieuMuon.Columns["MaPhieuMuon"].MinimumWidth = 45;
                dgvPhieuMuon.Columns["MaPhieuMuon"].FillWeight = 30;
                dgvPhieuMuon.Columns["TenBanDoc"].HeaderText = "Ban doc";
                dgvPhieuMuon.Columns["NgayMuon"].HeaderText = "Ngay muon";
                dgvPhieuMuon.Columns["NgayMuon"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvPhieuMuon.Columns["NgayHenTra"].HeaderText = "Hen tra";
                dgvPhieuMuon.Columns["NgayHenTra"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvPhieuMuon.Columns["NgayTraThucTe"].HeaderText = "Ngay tra";
                dgvPhieuMuon.Columns["NgayTraThucTe"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvPhieuMuon.Columns["TrangThai"].HeaderText = "Trang thai";
                dgvPhieuMuon.Columns["TrangThai"].MinimumWidth = 80;
                dgvPhieuMuon.Columns["TrangThai"].FillWeight = 50;

                dgvPhieuMuon.Columns["MaBanDoc"].Visible = false;
                dgvPhieuMuon.Columns["GhiChu"].Visible = false;
                dgvPhieuMuon.Columns["IsQuaHan"].Visible = false;
                dgvPhieuMuon.Columns["SoNgayMuon"].Visible = false;
            }

            // Color status rows
            foreach (DataGridViewRow row in dgvPhieuMuon.Rows)
            {
                string trangThai = row.Cells["TrangThai"].Value?.ToString() ?? "";
                if (trangThai == "Qua han")
                    row.DefaultCellStyle.ForeColor = Color.Red;
                else if (trangThai == "Dang muon")
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(52, 152, 219);
                else if (trangThai == "Da tra")
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(46, 204, 113);
            }
        }

        private void DgvPhieuMuon_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int maPhieu = Convert.ToInt32(dgvPhieuMuon.Rows[e.RowIndex].Cells["MaPhieuMuon"].Value);
            LoadChiTiet(maPhieu);
            tabControl.SelectedTab = tabChiTiet;
        }

        private void LoadChiTiet(int maPhieuMuon)
        {
            var list = _bll.GetChiTiet(maPhieuMuon);
            dgvChiTiet.DataSource = null;
            dgvChiTiet.DataSource = list;

            if (dgvChiTiet.Columns.Count > 0)
            {
                dgvChiTiet.Columns["MaChiTiet"].HeaderText = "STT";
                dgvChiTiet.Columns["MaChiTiet"].MinimumWidth = 50;
                dgvChiTiet.Columns["MaChiTiet"].FillWeight = 30;
                dgvChiTiet.Columns["MaSach"].HeaderText = "Ma sach";
                dgvChiTiet.Columns["MaSach"].MinimumWidth = 70;
                dgvChiTiet.Columns["MaSach"].FillWeight = 40;
                dgvChiTiet.Columns["TenSach"].HeaderText = "Ten sach";
                dgvChiTiet.Columns["DaTra"].HeaderText = "Da tra";
                dgvChiTiet.Columns["DaTra"].MinimumWidth = 60;
                dgvChiTiet.Columns["DaTra"].FillWeight = 35;
                dgvChiTiet.Columns["MaPhieuMuon"].Visible = false;
            }
        }

        private void BtnThemSach_Click(object? sender, EventArgs e)
        {
            if (cboSach.SelectedIndex <= 0 || cboSach.SelectedItem is not Sach sach) return;

            if (_sachDaChon.Any(s => s.MaSach == sach.MaSach))
            {
                MessageBox.Show("Sach nay da duoc chon!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _sachDaChon.Add(sach);
            lstSachMuon.Items.Add($"{sach.TenSach} - {sach.TacGia}");
            cboSach.SelectedIndex = 0;
        }

        private void BtnXoaSach_Click(object? sender, EventArgs e)
        {
            if (lstSachMuon.SelectedIndex >= 0)
            {
                _sachDaChon.RemoveAt(lstSachMuon.SelectedIndex);
                lstSachMuon.Items.RemoveAt(lstSachMuon.SelectedIndex);
            }
        }

        private void BtnLapPhieu_Click(object? sender, EventArgs e)
        {
            if (cboBanDoc.SelectedIndex <= 0 || cboBanDoc.SelectedItem is not BanDoc banDoc)
            {
                MessageBox.Show("Vui long chon ban doc!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_sachDaChon.Count == 0)
            {
                MessageBox.Show("Vui long chon it nhat 1 cuon sach!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var phieuMuon = new PhieuMuon
            {
                MaBanDoc = banDoc.MaBanDoc,
                NgayMuon = dtpNgayMuon.Value.Date,
                NgayHenTra = dtpNgayMuon.Value.Date.AddDays((int)nudSoNgay.Value),
                TrangThai = "Dang muon",
                GhiChu = txtGhiChu.Text.Trim()
            };

            var chiTietList = _sachDaChon.Select(s => new ChiTietMuon
            {
                MaSach = s.MaSach,
                TenSach = s.TenSach,
                DaTra = false
            }).ToList();

            var (success, message) = _bll.TaoPhieuMuon(phieuMuon, chiTietList);
            if (success)
            {
                MessageBox.Show(message, "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _sachDaChon.Clear();
                lstSachMuon.Items.Clear();
                txtGhiChu.Clear();
                cboBanDoc.SelectedIndex = 0;
                LoadData();
                LoadSachCombo();
            }
            else
            {
                MessageBox.Show(message, "Loi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnTraSach_Click(object? sender, EventArgs e)
        {
            if (dgvPhieuMuon.CurrentRow == null)
            {
                MessageBox.Show("Vui long chon phieu muon can tra!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvChiTiet.CurrentRow == null)
            {
                MessageBox.Show("Vui long chon cuon sach can tra trong danh sach chi tiet!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int maPhieu = Convert.ToInt32(dgvPhieuMuon.CurrentRow.Cells["MaPhieuMuon"].Value);
            string tenBanDoc = dgvPhieuMuon.CurrentRow.Cells["TenBanDoc"].Value?.ToString() ?? "";

            int maChiTiet = Convert.ToInt32(dgvChiTiet.CurrentRow.Cells["MaChiTiet"].Value);
            string tenSach = dgvChiTiet.CurrentRow.Cells["TenSach"].Value?.ToString() ?? "";

            bool daTra = dgvChiTiet.CurrentRow.Cells["DaTra"].Value is bool v && v;
            if (daTra)
            {
                MessageBox.Show("Cuon sach nay da duoc tra truoc do!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show($"Xac nhan tra sach '{tenSach}' cho ban doc '{tenBanDoc}'?\nMa phieu: {maPhieu}", "Xac nhan tra sach", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var (success, message) = _bll.TraSachChiTiet(maChiTiet);
                if (success)
                {
                    MessageBox.Show(message, "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Reload both ticket list and detail of current ticket
                    LoadData();
                    LoadSachCombo();
                    LoadChiTiet(maPhieu);
                }
                else
                {
                    MessageBox.Show(message, "Loi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void BtnXoaPhieu_Click(object? sender, EventArgs e)
        {
            if (dgvPhieuMuon.CurrentRow == null)
            {
                MessageBox.Show("Vui long chon phieu muon can xoa!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int maPhieu = Convert.ToInt32(dgvPhieuMuon.CurrentRow.Cells["MaPhieuMuon"].Value);
            if (MessageBox.Show($"Ban co chac chan xoa phieu muon #{maPhieu}?", "Xac nhan", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var (success, message) = _bll.Delete(maPhieu);
                if (success)
                {
                    MessageBox.Show(message, "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    dgvChiTiet.DataSource = null;
                }
                else
                {
                    MessageBox.Show(message, "Loi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}

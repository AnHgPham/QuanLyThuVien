using QuanLyThuVien.BusinessLogic;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Forms
{
    public class FormBanDoc : Form
    {
        private DataGridView dgvBanDoc = null!;
        private TextBox txtTimKiem = null!;
        private Button btnTimKiem = null!;
        private Button btnThem = null!;
        private Button btnSua = null!;
        private Button btnXoa = null!;

        // Input fields
        private TextBox txtHoTen = null!;
        private DateTimePicker dtpNgaySinh = null!;
        private ComboBox cboGioiTinh = null!;
        private TextBox txtSoDienThoai = null!;
        private TextBox txtEmail = null!;
        private TextBox txtDiaChi = null!;

        private readonly BanDocBLL _bll = new BanDocBLL();

        public FormBanDoc()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Quan ly Ban doc";
            this.Size = new Size(1050, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9.5F);
            this.BackColor = Color.FromArgb(245, 245, 250);

            // === TITLE ===
            var lblTitle = new Label();
            lblTitle.Text = "QUAN LY BAN DOC";
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(25, 42, 86);
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 45;
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblTitle.Padding = new Padding(15, 0, 0, 0);
            this.Controls.Add(lblTitle);

            // === SEARCH PANEL ===
            var panelSearch = new Panel();
            panelSearch.Dock = DockStyle.Top;
            panelSearch.Height = 50;
            panelSearch.BackColor = Color.White;
            panelSearch.Padding = new Padding(15, 8, 15, 8);

            var lblTK = new Label { Text = "Tim kiem:", AutoSize = true, Location = new Point(15, 15) };
            txtTimKiem = new TextBox { Location = new Point(85, 12), Width = 300 };
            txtTimKiem.PlaceholderText = "Ten, SDT, Email...";

            btnTimKiem = new Button { Text = "Tim", Location = new Point(400, 10), Width = 70, Height = 30 };
            btnTimKiem.BackColor = Color.FromArgb(52, 152, 219);
            btnTimKiem.ForeColor = Color.White;
            btnTimKiem.FlatStyle = FlatStyle.Flat;
            btnTimKiem.Click += (s, e) => { var list = _bll.Search(txtTimKiem.Text); DisplayData(list); };

            var btnLamMoi = new Button { Text = "Lam moi", Location = new Point(480, 10), Width = 80, Height = 30 };
            btnLamMoi.BackColor = Color.FromArgb(149, 165, 166);
            btnLamMoi.ForeColor = Color.White;
            btnLamMoi.FlatStyle = FlatStyle.Flat;
            btnLamMoi.Click += (s, e) => { txtTimKiem.Clear(); LoadData(); };

            panelSearch.Controls.AddRange(new Control[] { lblTK, txtTimKiem, btnTimKiem, btnLamMoi });
            this.Controls.Add(panelSearch);

            // === INPUT PANEL ===
            var panelInput = new Panel();
            panelInput.Dock = DockStyle.Bottom;
            panelInput.Height = 200;
            panelInput.BackColor = Color.White;
            panelInput.Padding = new Padding(15);

            var lblInputTitle = new Label { Text = "THONG TIN BAN DOC", Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(25, 42, 86), Dock = DockStyle.Top, Height = 30 };
            panelInput.Controls.Add(lblInputTitle);

            int y = 40;
            panelInput.Controls.Add(new Label { Text = "Ho ten:", Location = new Point(15, y), AutoSize = true });
            txtHoTen = new TextBox { Location = new Point(115, y - 3), Width = 250 };
            panelInput.Controls.Add(txtHoTen);

            panelInput.Controls.Add(new Label { Text = "Gioi tinh:", Location = new Point(390, y), AutoSize = true });
            cboGioiTinh = new ComboBox { Location = new Point(470, y - 3), Width = 100, DropDownStyle = ComboBoxStyle.DropDownList };
            cboGioiTinh.Items.AddRange(new object[] { "Nam", "Nu" });
            cboGioiTinh.SelectedIndex = 0;
            panelInput.Controls.Add(cboGioiTinh);

            panelInput.Controls.Add(new Label { Text = "Ngay sinh:", Location = new Point(590, y), AutoSize = true });
            dtpNgaySinh = new DateTimePicker { Location = new Point(680, y - 3), Width = 160, Format = DateTimePickerFormat.Short };
            panelInput.Controls.Add(dtpNgaySinh);

            y += 35;
            panelInput.Controls.Add(new Label { Text = "SDT:", Location = new Point(15, y), AutoSize = true });
            txtSoDienThoai = new TextBox { Location = new Point(115, y - 3), Width = 200 };
            panelInput.Controls.Add(txtSoDienThoai);

            panelInput.Controls.Add(new Label { Text = "Email:", Location = new Point(340, y), AutoSize = true });
            txtEmail = new TextBox { Location = new Point(400, y - 3), Width = 250 };
            panelInput.Controls.Add(txtEmail);

            y += 35;
            panelInput.Controls.Add(new Label { Text = "Dia chi:", Location = new Point(15, y), AutoSize = true });
            txtDiaChi = new TextBox { Location = new Point(115, y - 3), Width = 535 };
            panelInput.Controls.Add(txtDiaChi);

            y += 40;
            btnThem = CreateActionButton("Them moi", Color.FromArgb(46, 204, 113), new Point(15, y));
            btnThem.Click += BtnThem_Click;
            panelInput.Controls.Add(btnThem);

            btnSua = CreateActionButton("Cap nhat", Color.FromArgb(52, 152, 219), new Point(130, y));
            btnSua.Click += BtnSua_Click;
            panelInput.Controls.Add(btnSua);

            btnXoa = CreateActionButton("Xoa", Color.FromArgb(231, 76, 60), new Point(245, y));
            btnXoa.Click += BtnXoa_Click;
            panelInput.Controls.Add(btnXoa);

            var btnClear = CreateActionButton("Xoa form", Color.FromArgb(149, 165, 166), new Point(360, y));
            btnClear.Click += (s, e) => ClearInput();
            panelInput.Controls.Add(btnClear);

            this.Controls.Add(panelInput);

            // === DATAGRIDVIEW ===
            dgvBanDoc = new DataGridView();
            dgvBanDoc.Dock = DockStyle.Fill;
            dgvBanDoc.BackgroundColor = Color.White;
            dgvBanDoc.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBanDoc.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBanDoc.MultiSelect = false;
            dgvBanDoc.ReadOnly = true;
            dgvBanDoc.AllowUserToAddRows = false;
            dgvBanDoc.RowHeadersVisible = false;
            dgvBanDoc.Font = new Font("Segoe UI", 9F);
            dgvBanDoc.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
            dgvBanDoc.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(25, 42, 86);
            dgvBanDoc.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvBanDoc.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvBanDoc.EnableHeadersVisualStyles = false;
            dgvBanDoc.ColumnHeadersHeight = 35;
            dgvBanDoc.RowTemplate.Height = 30;
            dgvBanDoc.CellClick += DgvBanDoc_CellClick;
            this.Controls.Add(dgvBanDoc);

            dgvBanDoc.BringToFront();
        }

        private Button CreateActionButton(string text, Color color, Point location)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Location = location;
            btn.Size = new Size(105, 35);
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 10F);
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        private void LoadData()
        {
            DisplayData(_bll.GetAll());
        }

        private void DisplayData(List<BanDoc> list)
        {
            dgvBanDoc.DataSource = null;
            dgvBanDoc.DataSource = list;

            if (dgvBanDoc.Columns.Count > 0)
            {
                dgvBanDoc.Columns["MaBanDoc"].HeaderText = "Ma";
                dgvBanDoc.Columns["MaBanDoc"].MinimumWidth = 50;
                dgvBanDoc.Columns["MaBanDoc"].FillWeight = 30;
                dgvBanDoc.Columns["HoTen"].HeaderText = "Ho ten";
                dgvBanDoc.Columns["NgaySinh"].HeaderText = "Ngay sinh";
                dgvBanDoc.Columns["NgaySinh"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvBanDoc.Columns["GioiTinh"].HeaderText = "Gioi tinh";
                dgvBanDoc.Columns["GioiTinh"].MinimumWidth = 75;
                dgvBanDoc.Columns["GioiTinh"].FillWeight = 50;
                dgvBanDoc.Columns["SoDienThoai"].HeaderText = "SDT";
                dgvBanDoc.Columns["Email"].HeaderText = "Email";
                dgvBanDoc.Columns["DiaChi"].HeaderText = "Dia chi";
                dgvBanDoc.Columns["NgayLapThe"].HeaderText = "Ngay lap the";
                dgvBanDoc.Columns["NgayLapThe"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvBanDoc.Columns["TrangThai"].HeaderText = "Trang thai";
                dgvBanDoc.Columns["TrangThai"].MinimumWidth = 75;
                dgvBanDoc.Columns["TrangThai"].FillWeight = 50;
            }
        }

        private void DgvBanDoc_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvBanDoc.Rows[e.RowIndex];
            txtHoTen.Text = row.Cells["HoTen"].Value?.ToString();
            txtSoDienThoai.Text = row.Cells["SoDienThoai"].Value?.ToString();
            txtEmail.Text = row.Cells["Email"].Value?.ToString();
            txtDiaChi.Text = row.Cells["DiaChi"].Value?.ToString();

            if (row.Cells["NgaySinh"].Value != null && row.Cells["NgaySinh"].Value != DBNull.Value)
                dtpNgaySinh.Value = Convert.ToDateTime(row.Cells["NgaySinh"].Value);

            string gioiTinh = row.Cells["GioiTinh"].Value?.ToString() ?? "Nam";
            cboGioiTinh.SelectedItem = gioiTinh;
        }

        private void BtnThem_Click(object? sender, EventArgs e)
        {
            var bd = GetBanDocFromInput();
            var (success, message) = _bll.Insert(bd);
            ShowResult(success, message);
        }

        private void BtnSua_Click(object? sender, EventArgs e)
        {
            if (dgvBanDoc.CurrentRow == null) { MessageBox.Show("Vui long chon ban doc!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            var bd = GetBanDocFromInput();
            bd.MaBanDoc = Convert.ToInt32(dgvBanDoc.CurrentRow.Cells["MaBanDoc"].Value);
            var (success, message) = _bll.Update(bd);
            ShowResult(success, message);
        }

        private void BtnXoa_Click(object? sender, EventArgs e)
        {
            if (dgvBanDoc.CurrentRow == null) { MessageBox.Show("Vui long chon ban doc!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            int maBanDoc = Convert.ToInt32(dgvBanDoc.CurrentRow.Cells["MaBanDoc"].Value);
            string hoTen = dgvBanDoc.CurrentRow.Cells["HoTen"].Value?.ToString() ?? "";
            if (MessageBox.Show($"Ban co chac chan muon xoa ban doc '{hoTen}'?", "Xac nhan", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var (success, message) = _bll.Delete(maBanDoc);
                ShowResult(success, message);
            }
        }

        private void ShowResult(bool success, string message)
        {
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

        private BanDoc GetBanDocFromInput()
        {
            return new BanDoc
            {
                HoTen = txtHoTen.Text.Trim(),
                NgaySinh = dtpNgaySinh.Value.Date,
                GioiTinh = cboGioiTinh.SelectedItem?.ToString(),
                SoDienThoai = txtSoDienThoai.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                DiaChi = txtDiaChi.Text.Trim(),
                TrangThai = true
            };
        }

        private void ClearInput()
        {
            txtHoTen.Clear();
            dtpNgaySinh.Value = DateTime.Now;
            cboGioiTinh.SelectedIndex = 0;
            txtSoDienThoai.Clear();
            txtEmail.Clear();
            txtDiaChi.Clear();
        }
    }
}

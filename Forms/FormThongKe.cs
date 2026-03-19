using QuanLyThuVien.BusinessLogic;
using QuanLyThuVien.Models;
using QuanLyThuVien.Reports;

namespace QuanLyThuVien.Forms
{
    public class FormThongKe : Form
    {
        private ComboBox cboNam = null!;
        private ComboBox cboThang = null!;
        private Button btnXem = null!;
        private Button btnExportCSV = null!;
        private DataGridView dgvCacThang = null!;
        private DataGridView dgvSachMuonNhieu = null!;

        // Thong ke thang
        private Label lblTongPhieu = null!;
        private Label lblDaTra = null!;
        private Label lblDangMuon = null!;
        private Label lblQuaHan = null!;
        private Label lblTongSach = null!;

        private readonly ThongKeBLL _bll = new ThongKeBLL();

        public FormThongKe()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Thong ke Muon/Tra sach";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9.5F);
            this.BackColor = Color.FromArgb(245, 245, 250);

            // === TITLE ===
            var lblTitle = new Label();
            lblTitle.Text = "THONG KE MUON / TRA SACH";
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(25, 42, 86);
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 45;
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblTitle.Padding = new Padding(15, 0, 0, 0);
            this.Controls.Add(lblTitle);

            // === FILTER PANEL ===
            var panelFilter = new Panel();
            panelFilter.Dock = DockStyle.Top;
            panelFilter.Height = 50;
            panelFilter.BackColor = Color.White;

            panelFilter.Controls.Add(new Label { Text = "Nam:", AutoSize = true, Location = new Point(15, 15) });
            cboNam = new ComboBox { Location = new Point(55, 12), Width = 80, DropDownStyle = ComboBoxStyle.DropDownList };
            for (int i = DateTime.Now.Year; i >= 2020; i--)
                cboNam.Items.Add(i);
            cboNam.SelectedIndex = 0;
            panelFilter.Controls.Add(cboNam);

            panelFilter.Controls.Add(new Label { Text = "Thang:", AutoSize = true, Location = new Point(150, 15) });
            cboThang = new ComboBox { Location = new Point(200, 12), Width = 70, DropDownStyle = ComboBoxStyle.DropDownList };
            for (int i = 1; i <= 12; i++)
                cboThang.Items.Add(i);
            cboThang.SelectedIndex = DateTime.Now.Month - 1;
            panelFilter.Controls.Add(cboThang);

            btnXem = new Button { Text = "Xem thong ke", Location = new Point(290, 10), Width = 120, Height = 30 };
            btnXem.BackColor = Color.FromArgb(52, 152, 219);
            btnXem.ForeColor = Color.White;
            btnXem.FlatStyle = FlatStyle.Flat;
            btnXem.Click += (s, e) => LoadData();
            panelFilter.Controls.Add(btnXem);

            btnExportCSV = new Button { Text = "Xuat CSV", Location = new Point(425, 10), Width = 120, Height = 30 };
            btnExportCSV.BackColor = Color.FromArgb(46, 204, 113);
            btnExportCSV.ForeColor = Color.White;
            btnExportCSV.FlatStyle = FlatStyle.Flat;
            btnExportCSV.Click += BtnExportCSV_Click;
            panelFilter.Controls.Add(btnExportCSV);

            this.Controls.Add(panelFilter);

            // === SUMMARY CARDS ===
            var panelSummary = new FlowLayoutPanel();
            panelSummary.Dock = DockStyle.Top;
            panelSummary.Height = 100;
            panelSummary.WrapContents = true;
            panelSummary.Padding = new Padding(10, 5, 10, 5);

            lblTongPhieu = CreateStatLabel("Tong phieu muon", "0", Color.FromArgb(52, 152, 219));
            lblDaTra = CreateStatLabel("Da tra", "0", Color.FromArgb(46, 204, 113));
            lblDangMuon = CreateStatLabel("Dang muon", "0", Color.FromArgb(241, 196, 15));
            lblQuaHan = CreateStatLabel("Qua han", "0", Color.FromArgb(231, 76, 60));
            lblTongSach = CreateStatLabel("Tong sach muon", "0", Color.FromArgb(155, 89, 182));

            panelSummary.Controls.AddRange(new Control[] { lblTongPhieu.Parent!, lblDaTra.Parent!, lblDangMuon.Parent!, lblQuaHan.Parent!, lblTongSach.Parent! });
            this.Controls.Add(panelSummary);

            // === Two panels instead of SplitContainer ===
            // Top: Thong ke theo thang
            var panelTopGrid = new Panel();
            panelTopGrid.Dock = DockStyle.Fill;

            var lblCacThang = new Label { Text = "THONG KE THEO THANG", Font = new Font("Segoe UI", 11F, FontStyle.Bold), Dock = DockStyle.Top, Height = 30, ForeColor = Color.FromArgb(25, 42, 86) };
            dgvCacThang = CreateDataGridView();
            panelTopGrid.Controls.Add(dgvCacThang);
            panelTopGrid.Controls.Add(lblCacThang);

            // Bottom: Sach muon nhieu
            var panelBottomGrid = new Panel();
            panelBottomGrid.Dock = DockStyle.Bottom;
            panelBottomGrid.Height = 250;

            var lblSachNhieu = new Label { Text = "SACH MUON NHIEU NHAT", Font = new Font("Segoe UI", 11F, FontStyle.Bold), Dock = DockStyle.Top, Height = 30, ForeColor = Color.FromArgb(25, 42, 86) };
            dgvSachMuonNhieu = CreateDataGridView();
            panelBottomGrid.Controls.Add(dgvSachMuonNhieu);
            panelBottomGrid.Controls.Add(lblSachNhieu);

            this.Controls.Add(panelTopGrid);
            this.Controls.Add(panelBottomGrid);
            panelTopGrid.BringToFront();
        }

        private DataGridView CreateDataGridView()
        {
            var dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.RowHeadersVisible = false;
            dgv.Font = new Font("Segoe UI", 9F);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(25, 42, 86);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 32;
            dgv.RowTemplate.Height = 28;
            return dgv;
        }

        private Label CreateStatLabel(string title, string value, Color color)
        {
            var panel = new Panel { Size = new Size(190, 80), BackColor = Color.White, Margin = new Padding(5) };

            var bar = new Panel { Dock = DockStyle.Top, Height = 4, BackColor = color };
            panel.Controls.Add(bar);

            var lblVal = new Label { Text = value, Font = new Font("Segoe UI", 18F, FontStyle.Bold), ForeColor = color, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            panel.Controls.Add(lblVal);

            var lblTit = new Label { Text = title, Font = new Font("Segoe UI", 8.5F), ForeColor = Color.Gray, Dock = DockStyle.Bottom, Height = 22, TextAlign = ContentAlignment.MiddleCenter };
            panel.Controls.Add(lblTit);

            return lblVal;
        }

        private void LoadData()
        {
            int nam = (int)cboNam.SelectedItem!;
            int thang = (int)cboThang.SelectedItem!;

            try
            {
                // Thong ke thang hien tai
                var thongKe = _bll.GetThongKeThang(nam, thang);
                if (thongKe != null)
                {
                    lblTongPhieu.Text = thongKe.TongPhieuMuon.ToString();
                    lblDaTra.Text = thongKe.SoPhieuDaTra.ToString();
                    lblDangMuon.Text = thongKe.SoPhieuDangMuon.ToString();
                    lblQuaHan.Text = thongKe.SoPhieuQuaHan.ToString();
                    lblTongSach.Text = thongKe.TongSoSachMuon.ToString();
                }
                else
                {
                    lblTongPhieu.Text = lblDaTra.Text = lblDangMuon.Text = lblQuaHan.Text = lblTongSach.Text = "0";
                }

                // Cac thang trong nam
                var cacThang = _bll.GetThongKeCacThang(nam);
                dgvCacThang.DataSource = null;
                dgvCacThang.DataSource = cacThang;
                if (dgvCacThang.Columns.Count > 0)
                {
                    dgvCacThang.Columns["Nam"].Visible = false;
                    dgvCacThang.Columns["TongSoSachMuon"].Visible = false;
                    dgvCacThang.Columns["Thang"].HeaderText = "Thang";
                    dgvCacThang.Columns["TongPhieuMuon"].HeaderText = "Tong phieu";
                    dgvCacThang.Columns["SoPhieuDaTra"].HeaderText = "Da tra";
                    dgvCacThang.Columns["SoPhieuDangMuon"].HeaderText = "Dang muon";
                    dgvCacThang.Columns["SoPhieuQuaHan"].HeaderText = "Qua han";
                }

                // Sach muon nhieu
                var sachNhieu = _bll.GetSachMuonNhieu(nam, thang);
                dgvSachMuonNhieu.DataSource = null;
                dgvSachMuonNhieu.DataSource = sachNhieu;
                if (dgvSachMuonNhieu.Columns.Count > 0)
                {
                    dgvSachMuonNhieu.Columns["MaSach"].HeaderText = "Ma";
                    dgvSachMuonNhieu.Columns["MaSach"].MinimumWidth = 50;
                    dgvSachMuonNhieu.Columns["MaSach"].FillWeight = 30;
                    dgvSachMuonNhieu.Columns["TenSach"].HeaderText = "Ten sach";
                    dgvSachMuonNhieu.Columns["TacGia"].HeaderText = "Tac gia";
                    dgvSachMuonNhieu.Columns["TenTheLoai"].HeaderText = "The loai";
                    dgvSachMuonNhieu.Columns["SoLanMuon"].HeaderText = "So lan muon";
                    dgvSachMuonNhieu.Columns["SoLanMuon"].MinimumWidth = 100;
                    dgvSachMuonNhieu.Columns["SoLanMuon"].FillWeight = 60;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loi tai du lieu thong ke:\n{ex.Message}", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExportCSV_Click(object? sender, EventArgs e)
        {
            int nam = (int)cboNam.SelectedItem!;
            int thang = (int)cboThang.SelectedItem!;

            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV files (*.csv)|*.csv";
            saveDialog.FileName = $"BaoCao_MuonTra_{thang}_{nam}.csv";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var thongKe = _bll.GetThongKeThang(nam, thang);
                    var cacThang = _bll.GetThongKeCacThang(nam);
                    var sachNhieu = _bll.GetSachMuonNhieu(nam, thang);

                    CsvExporter.ExportBaoCaoMuonTra(saveDialog.FileName, nam, thang, thongKe, cacThang, sachNhieu);

                    MessageBox.Show($"Xuat CSV thanh cong!\n{saveDialog.FileName}", "Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Loi xuat CSV:\n{ex.Message}", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

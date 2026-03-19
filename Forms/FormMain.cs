using System.Data;
using QuanLyThuVien.BusinessLogic;
using QuanLyThuVien.DataAccess;

namespace QuanLyThuVien.Forms
{
    public class FormMain : Form
    {
        private Panel panelSidebar = null!;
        private Panel panelContent = null!;
        private StatusStrip statusStrip = null!;
        private ToolStripStatusLabel toolStripStatus = null!;

        // Colors
        private static readonly Color SidebarBg = Color.FromArgb(30, 39, 73);
        private static readonly Color AccentCyan = Color.FromArgb(72, 219, 251);
        private static readonly Color BgColor = Color.FromArgb(240, 243, 247);
        private static readonly Color CardBg = Color.White;

        public FormMain()
        {
            InitializeComponent();
            try { new PhieuMuonBLL().CapNhatQuaHan(); } catch { }
            LoadDashboard();
        }

        private void InitializeComponent()
        {
            this.Text = "Quan Ly Thu Vien - Library Manager";
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 600);
            this.BackColor = BgColor;
            this.Font = new Font("Segoe UI", 9.5F);

            // ============================
            // Build from OUTSIDE to INSIDE
            // ============================

            // 1. STATUS STRIP at bottom
            statusStrip = new StatusStrip();
            statusStrip.BackColor = Color.FromArgb(22, 29, 58);
            statusStrip.SizingGrip = false;
            toolStripStatus = new ToolStripStatusLabel("San sang");
            toolStripStatus.ForeColor = Color.FromArgb(180, 200, 230);
            statusStrip.Items.Add(toolStripStatus);
            this.Controls.Add(statusStrip);

            // 2. SIDEBAR at left (no logo, just buttons)
            panelSidebar = new Panel();
            panelSidebar.Dock = DockStyle.Left;
            panelSidebar.Width = 200;
            panelSidebar.BackColor = SidebarBg;

            // Sidebar title
            var lblNav = new Label();
            lblNav.Text = "MENU";
            lblNav.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNav.ForeColor = Color.FromArgb(100, 120, 160);
            lblNav.Dock = DockStyle.Top;
            lblNav.Height = 35;
            lblNav.TextAlign = ContentAlignment.BottomLeft;
            lblNav.Padding = new Padding(20, 0, 0, 5);

            // Accent line
            var accentLine = new Panel();
            accentLine.Dock = DockStyle.Top;
            accentLine.Height = 2;
            accentLine.BackColor = AccentCyan;

            // Navigation buttons - add in reverse order (LIFO for Dock=Top)
            var btnThongKe = CreateSidebarButton("  Thong ke bao cao");
            btnThongKe.Click += (s, e) => OpenForm(new FormThongKe());

            var btnPhieuMuon = CreateSidebarButton("  Muon / Tra sach");
            btnPhieuMuon.Click += (s, e) => OpenForm(new FormPhieuMuon());

            var btnBanDoc = CreateSidebarButton("  Quan ly ban doc");
            btnBanDoc.Click += (s, e) => OpenForm(new FormBanDoc());

            var btnSach = CreateSidebarButton("  Quan ly sach");
            btnSach.Click += (s, e) => OpenForm(new FormSach());

            var btnDashboard = CreateSidebarButton("  Trang chu");
            btnDashboard.Click += (s, e) => LoadDashboard();

            // Add buttons in reverse visual order (last added = topmost)
            panelSidebar.Controls.Add(btnThongKe);
            panelSidebar.Controls.Add(btnPhieuMuon);
            panelSidebar.Controls.Add(btnBanDoc);
            panelSidebar.Controls.Add(btnSach);
            panelSidebar.Controls.Add(btnDashboard);
            panelSidebar.Controls.Add(accentLine);
            panelSidebar.Controls.Add(lblNav);

            this.Controls.Add(panelSidebar);

            // 3. CONTENT PANEL fills remaining space
            panelContent = new Panel();
            panelContent.Dock = DockStyle.Fill;
            panelContent.BackColor = BgColor;
            panelContent.Padding = new Padding(30, 25, 30, 15);
            this.Controls.Add(panelContent);

            // KEY: Dock=Fill must be in FRONT (highest z-order)
            // so it fills AFTER sidebar/statusbar take their space
            panelContent.BringToFront();
        }

        private Button CreateSidebarButton(string text)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Dock = DockStyle.Top;
            btn.Height = 45;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 55, 95);
            btn.BackColor = SidebarBg;
            btn.ForeColor = Color.FromArgb(200, 210, 230);
            btn.Font = new Font("Segoe UI", 10.5F);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(15, 0, 0, 0);
            btn.Cursor = Cursors.Hand;
            btn.MouseEnter += (s, e) => { btn.ForeColor = Color.White; };
            btn.MouseLeave += (s, e) => { btn.ForeColor = Color.FromArgb(200, 210, 230); };
            return btn;
        }

        private void LoadDashboard()
        {
            panelContent.Controls.Clear();

            // Use a single scrollable container
            var container = new Panel();
            container.Dock = DockStyle.Fill;
            container.AutoScroll = true;
            container.BackColor = BgColor;

            // === Title (absolute position for reliability) ===
            var lblTitle = new Label();
            lblTitle.Text = "TONG QUAN THU VIEN";
            lblTitle.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(30, 39, 73);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(0, 0);
            container.Controls.Add(lblTitle);

            var lblDesc = new Label();
            lblDesc.Text = "Tong hop tinh trang hoat dong cua thu vien";
            lblDesc.Font = new Font("Segoe UI", 10F);
            lblDesc.ForeColor = Color.FromArgb(130, 140, 160);
            lblDesc.AutoSize = true;
            lblDesc.Location = new Point(2, 42);
            container.Controls.Add(lblDesc);

            // === Stats Cards (FlowLayout with absolute position) ===
            var panelCards = new FlowLayoutPanel();
            panelCards.Location = new Point(0, 80);
            panelCards.Size = new Size(900, 300);
            panelCards.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelCards.WrapContents = true;
            panelCards.AutoSize = false;
            panelCards.BackColor = Color.Transparent;

            try
            {
                var thongKeBLL = new ThongKeBLL();
                DataTable dt = thongKeBLL.GetTongQuan();
                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    panelCards.Controls.Add(CreateDashboardCard("Tong so sach", row["TongSach"].ToString()!, Color.FromArgb(52, 152, 219), "Trong kho"));
                    panelCards.Controls.Add(CreateDashboardCard("Tong ban doc", row["TongBanDoc"].ToString()!, Color.FromArgb(155, 89, 182), "Da dang ky"));
                    panelCards.Controls.Add(CreateDashboardCard("Dang muon", row["DangMuon"].ToString()!, Color.FromArgb(46, 204, 113), "Phieu muon"));
                    panelCards.Controls.Add(CreateDashboardCard("Qua han", row["QuaHan"].ToString()!, Color.FromArgb(231, 76, 60), "Can xu ly"));
                    panelCards.Controls.Add(CreateDashboardCard("Phieu thang nay", row["PhieuThangNay"].ToString()!, Color.FromArgb(241, 196, 15), $"Thang {DateTime.Now.Month}"));
                    panelCards.Controls.Add(CreateDashboardCard("Tra thang nay", row["TraThangNay"].ToString()!, Color.FromArgb(26, 188, 156), $"Thang {DateTime.Now.Month}"));
                }
            }
            catch (Exception ex)
            {
                var lblError = new Label();
                lblError.Text = $"Khong the ket noi database!\n\nVui long kiem tra:\n1. SQL Server da khoi dong\n2. Database QuanLyThuVienDB da duoc tao\n3. Connection string trong App.config\n\nLoi: {ex.Message}";
                lblError.Font = new Font("Segoe UI", 11F);
                lblError.ForeColor = Color.FromArgb(180, 60, 50);
                lblError.AutoSize = true;
                lblError.MaximumSize = new Size(600, 0);
                lblError.Location = new Point(0, 80);
                container.Controls.Add(lblError);
            }

            container.Controls.Add(panelCards);
            panelContent.Controls.Add(container);

            toolStripStatus.Text = "Trang chu - Dashboard";
        }

        private Panel CreateDashboardCard(string title, string value, Color color, string subtitle)
        {
            var card = new Panel();
            card.Size = new Size(210, 110);
            card.BackColor = CardBg;
            card.Margin = new Padding(0, 0, 15, 15);

            // Top accent bar
            var colorBar = new Panel();
            colorBar.Dock = DockStyle.Top;
            colorBar.Height = 4;
            colorBar.BackColor = color;
            card.Controls.Add(colorBar);

            var lblCardTitle = new Label();
            lblCardTitle.Text = title;
            lblCardTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCardTitle.ForeColor = Color.FromArgb(100, 110, 130);
            lblCardTitle.Location = new Point(15, 14);
            lblCardTitle.AutoSize = true;
            card.Controls.Add(lblCardTitle);

            var lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblValue.ForeColor = color;
            lblValue.Location = new Point(15, 36);
            lblValue.AutoSize = true;
            card.Controls.Add(lblValue);

            var lblSub = new Label();
            lblSub.Text = subtitle;
            lblSub.Font = new Font("Segoe UI", 8.5F);
            lblSub.ForeColor = Color.FromArgb(160, 170, 185);
            lblSub.Location = new Point(15, 85);
            lblSub.AutoSize = true;
            card.Controls.Add(lblSub);

            // Hover
            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(245, 248, 255);
            card.MouseLeave += (s, e) => card.BackColor = CardBg;

            return card;
        }

        private void OpenForm(Form form)
        {
            form.ShowDialog();
            LoadDashboard();
        }

        private void ShowConnectionConfig()
        {
            string currentConn = DatabaseHelper.ConnectionString;
            string? input = Microsoft.VisualBasic.Interaction.InputBox(
                "Nhap Connection String:", "Cau hinh ket noi", currentConn);
            if (!string.IsNullOrWhiteSpace(input))
            {
                DatabaseHelper.ConnectionString = input;
                MessageBox.Show("Da cap nhat Connection String!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDashboard();
            }
        }

        private void TestConnection()
        {
            if (DatabaseHelper.TestConnection())
                MessageBox.Show("Ket noi SQL Server thanh cong!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Ket noi SQL Server that bai!\nVui long kiem tra lai Connection String.", "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowAbout()
        {
            MessageBox.Show(
                "Ung dung Quan Ly Thu Vien\n" +
                "Phien ban: 1.0\n\n" +
                "Chuc nang:\n" +
                "- Quan ly sach (CRUD, tim kiem, loc)\n" +
                "- Quan ly ban doc\n" +
                "- Lap phieu muon / tra sach\n" +
                "- Rang buoc: toi da 5 sach, danh dau qua han\n" +
                "- Xuat bao cao muon theo thang (CSV)\n" +
                "- Thong ke sach muon nhieu\n\n" +
                "Cong nghe: C# Windows Forms + SQL Server\n" +
                "Kien truc: MVC + DAO (3 lop)",
                "Gioi thieu", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

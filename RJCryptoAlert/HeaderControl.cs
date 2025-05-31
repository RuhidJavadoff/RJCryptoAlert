using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace RJCryptoAlert
{
    public partial class HeaderControl : UserControl
    {
        private Panel? pnlLogoArea;
        private PictureBox? pbLogo;
        private Panel? pnlControlsArea;
        private Button? btnThemeToggleStyled;
        private Button? btnMenu;

        private Image? logoImageLight;
        private Image? logoImageNight;

        public event EventHandler? ThemeToggleButtonClicked;
        public event EventHandler? MenuButtonClicked;

        private bool _isNightMode = false;
        public bool IsNightMode
        {
            get { return _isNightMode; }
            set
            {
                _isNightMode = value;
                ApplyHeaderTheme();
                UpdateToggleButtonAppearance();
            }
        }

        public HeaderControl()
        {
            this.DoubleBuffered = true;
            InitializeOwnControls();
            LoadAllThemeLogos();
            this.Dock = DockStyle.Top;
            this.Height = 60;
            this.Padding = new Padding(15, 0, 15, 0);

            ApplyHeaderTheme();
            UpdateToggleButtonAppearance();
        }

        private void InitializeOwnControls()
        {
            pnlLogoArea = new Panel { Dock = DockStyle.Left, Width = 160 };
            this.Controls.Add(pnlLogoArea);

            pbLogo = new PictureBox
            {
                Size = new Size(130, 40),
                SizeMode = PictureBoxSizeMode.Zoom,
            };
            pnlLogoArea.Controls.Add(pbLogo);
            pnlLogoArea.SizeChanged += (s, e) => PositionLogo();

            pnlControlsArea = new Panel { Dock = DockStyle.Right, Width = 130 };
            this.Controls.Add(pnlControlsArea);

            int buttonHeight = 34;
            int buttonWidth = 45;
            int toggleWidth = 65;

            btnMenu = new Button { Text = "☰", Font = new Font("Segoe UI Symbol", 15F, FontStyle.Bold), Size = new Size(buttonWidth, buttonHeight), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnMenu.FlatAppearance.BorderSize = 0;
            btnMenu.Click += (sender, e) => MenuButtonClicked?.Invoke(this, EventArgs.Empty);
            pnlControlsArea.Controls.Add(btnMenu);

            btnThemeToggleStyled = new Button { Size = new Size(toggleWidth, buttonHeight), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnThemeToggleStyled.FlatAppearance.BorderSize = 0;
            btnThemeToggleStyled.Paint += BtnThemeToggleStyled_Paint;
            btnThemeToggleStyled.Click += BtnThemeToggle_UC_Click;
            pnlControlsArea.Controls.Add(btnThemeToggleStyled);

            PositionLogo();
            PositionControlsInHeader();
            this.SizeChanged += (s, e) => { PositionControlsInHeader(); PositionLogo(); };
        }

        private void LoadAllThemeLogos()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceNameLight = "RJCryptoAlert.Resources.logol.png";
            try { using (Stream? stream = assembly.GetManifestResourceStream(resourceNameLight)) { if (stream != null) logoImageLight = Image.FromStream(stream); else Console.WriteLine($"XƏTA (Light Logo): Resurs '{resourceNameLight}' tapılmadı."); } }
            catch (Exception ex) { Console.WriteLine($"Açıq tema logosu ('{resourceNameLight}') yüklənərkən xəta: {ex.Message}"); }

            string resourceNameNight = "RJCryptoAlert.Resources.logon.png";
            try { using (Stream? stream = assembly.GetManifestResourceStream(resourceNameNight)) { if (stream != null) logoImageNight = Image.FromStream(stream); else Console.WriteLine($"XƏTA (Night Logo): Resurs '{resourceNameNight}' tapılmadı."); } }
            catch (Exception ex) { Console.WriteLine($"Tünd tema logosu ('{resourceNameNight}') yüklənərkən xəta: {ex.Message}"); }

            if (logoImageLight == null && pbLogo != null) pbLogo.BackColor = Color.LightGray;
            if (logoImageNight == null && pbLogo != null && _isNightMode) pbLogo.BackColor = Color.DarkGray;
        }

        private void PositionLogo() { if (pbLogo != null && pnlLogoArea != null) { pbLogo.Location = new Point(0, (pnlLogoArea.ClientSize.Height - pbLogo.Height) / 2); } }
        private void PositionControlsInHeader() { if (pnlControlsArea == null) return; int currentX = pnlControlsArea.Width; int controlPadding = 10; if (btnMenu != null) { currentX -= btnMenu.Width; btnMenu.Location = new Point(currentX, (pnlControlsArea.Height - btnMenu.Height) / 2); currentX -= controlPadding; } if (btnThemeToggleStyled != null) { currentX -= btnThemeToggleStyled.Width; btnThemeToggleStyled.Location = new Point(currentX, (pnlControlsArea.Height - btnThemeToggleStyled.Height) / 2); } }
        protected override void OnSizeChanged(EventArgs e) { base.OnSizeChanged(e); PositionControlsInHeader(); PositionLogo(); }

        private void BtnThemeToggleStyled_Paint(object? sender, PaintEventArgs e) // sender nullable
        {
            Button? btn = sender as Button; if (btn == null) return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = new GraphicsPath()) { path.AddArc(new Rectangle(0, 0, btn.Height, btn.Height), 90, 180); path.AddArc(new Rectangle(btn.Width - btn.Height, 0, btn.Height, btn.Height), -90, 180); path.CloseFigure(); using (SolidBrush backgroundBrush = new SolidBrush(IsNightMode ? Color.FromArgb(70, 70, 75) : Color.FromArgb(220, 220, 225))) { e.Graphics.FillPath(backgroundBrush, path); } }
            int knobDiameter = btn.Height - 6; Point knobLocation = IsNightMode ? new Point(btn.Width - knobDiameter - 3, 3) : new Point(3, 3);
            using (SolidBrush knobBrush = new SolidBrush(IsNightMode ? Color.FromArgb(180, 180, 180) : Color.DodgerBlue)) { e.Graphics.FillEllipse(knobBrush, new Rectangle(knobLocation, new Size(knobDiameter, knobDiameter))); }
            string iconToDraw; Rectangle iconRect; Font iconFont = new Font("Segoe UI Emoji", 10F); int iconSize = knobDiameter - 4; int iconY = (btn.Height - iconSize) / 2;
            if (IsNightMode) { iconToDraw = "☀️"; iconRect = new Rectangle(3 + (knobDiameter - iconSize) / 2, iconY, iconSize, iconSize); }
            else { iconToDraw = "🌙"; iconRect = new Rectangle(btn.Width - knobDiameter - 3 + (knobDiameter - iconSize) / 2, iconY, iconSize, iconSize); }
            TextRenderer.DrawText(e.Graphics, iconToDraw, iconFont, iconRect, btn.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private void BtnThemeToggle_UC_Click(object? sender, EventArgs e) { ThemeToggleButtonClicked?.Invoke(this, EventArgs.Empty); } // sender nullable
        private void UpdateToggleButtonAppearance() { if (btnThemeToggleStyled != null) btnThemeToggleStyled.Invalidate(); }

        public void ApplyHeaderTheme()
        {
            Color headerBackColor, logoAreaBackColor, controlsPanelBackColor, buttonForeColor;
            if (IsNightMode) { headerBackColor = Color.FromArgb(28, 28, 30); logoAreaBackColor = headerBackColor; controlsPanelBackColor = headerBackColor; buttonForeColor = Color.FromArgb(240, 240, 240); if (pbLogo != null) pbLogo.Image = logoImageNight ?? logoImageLight; }
            else { headerBackColor = Color.FromArgb(250, 250, 250); logoAreaBackColor = headerBackColor; controlsPanelBackColor = headerBackColor; buttonForeColor = Color.FromArgb(51, 51, 51); if (pbLogo != null) pbLogo.Image = logoImageLight ?? logoImageNight; }
            this.BackColor = headerBackColor;
            if (pnlLogoArea != null) pnlLogoArea.BackColor = logoAreaBackColor;
            if (pbLogo != null) { pbLogo.BackColor = Color.Transparent; if (pbLogo.Image == null) pbLogo.BackColor = IsNightMode ? Color.FromArgb(50, 50, 50) : Color.LightGray; pbLogo.Invalidate(); }
            if (pnlControlsArea != null) pnlControlsArea.BackColor = controlsPanelBackColor;
            if (btnThemeToggleStyled != null) btnThemeToggleStyled.ForeColor = buttonForeColor;
            if (btnMenu != null) { btnMenu.ForeColor = buttonForeColor; btnMenu.BackColor = controlsPanelBackColor; btnMenu.Invalidate(); }
            UpdateToggleButtonAppearance();
        }
    }
}
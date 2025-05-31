using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;

namespace RJCryptoAlert
{
    public partial class CoinCardControl : UserControl
    {
        private Label? lblSymbol; 
        private Label? lblPrice;   
        private Label? lblChangePercent; 
        private Panel? pnlTopBorder;     

        private string _symbol = "SYM";
        private string _price = "0.00 USD";
        private string _changePercent = "0.00%";
        private decimal _rawValueChangePercent = 0;
        private bool _isNightMode = false;
        private bool _isActiveCard = false;

        public event EventHandler? CardClicked; 

        public string SymbolText
        {
            get { return _symbol; }
            set { _symbol = value; if (lblSymbol != null) lblSymbol.Text = _symbol; }
        }

        public string PriceText
        {
            get { return _price; }
            set { _price = value; if (lblPrice != null) lblPrice.Text = _price; }
        }

        public string ChangePercentText
        {
            get { return _changePercent; }
            set { _changePercent = value; if (lblChangePercent != null) lblChangePercent.Text = _changePercent; }
        }

        public decimal RawValueChangePercent
        {
            get { return _rawValueChangePercent; }
            set { _rawValueChangePercent = value; UpdateChangePercentColor(); }
        }

        public bool IsNightMode
        {
            get { return _isNightMode; }
            set { _isNightMode = value; ApplyCardTheme(); }
        }

        public bool IsActiveCard
        {
            get { return _isActiveCard; }
            set { _isActiveCard = value; UpdateActiveState(); }
        }

        public string? CoinSlug { get; set; } // Null

        public CoinCardControl()
        {
            this.DoubleBuffered = true;
            this.Size = new Size(130, 90);
            this.Padding = new Padding(8);
            this.Margin = new Padding(5);
            this.Cursor = Cursors.Hand;

            InitializeCardControls();
            ApplyCardTheme();

            this.Click += (sender, e) => OnCardClicked(sender, e); 
            foreach (Control control in this.Controls)
            {
                control.Click += (sender, e) => OnCardClicked(sender, e); 
            }
        }

        private void InitializeCardControls()
        {
            pnlTopBorder = new Panel { Height = 3, Dock = DockStyle.Top, BackColor = Color.Transparent };
            this.Controls.Add(pnlTopBorder);

            lblSymbol = new Label { Font = new Font("Segoe UI", 11F, FontStyle.Bold), AutoSize = false, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 25, Padding = new Padding(0, 3, 0, 0) };
            this.Controls.Add(lblSymbol);
            lblSymbol.BringToFront();

            lblPrice = new Label { Font = new Font("Segoe UI", 10F, FontStyle.Regular), AutoSize = false, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
            this.Controls.Add(lblPrice);
            lblPrice.BringToFront();

            lblChangePercent = new Label { Font = new Font("Segoe UI", 9F, FontStyle.Regular), AutoSize = false, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Bottom, Height = 20 };
            this.Controls.Add(lblChangePercent);
            lblChangePercent.BringToFront();

            lblSymbol.Text = _symbol;
            lblPrice.Text = _price;
            lblChangePercent.Text = _changePercent;
        }

        private void OnCardClicked(object? sender, EventArgs e)
        {
            CardClicked?.Invoke(this, EventArgs.Empty); 
        }

        private void UpdateActiveState()
        {
            if (pnlTopBorder == null) return;
            pnlTopBorder.BackColor = _isActiveCard ? (IsNightMode ? Color.LightSkyBlue : Color.DodgerBlue) : Color.Transparent;
        }

        private void UpdateChangePercentColor()
        {
            if (lblChangePercent == null) return;
            if (_rawValueChangePercent > 0) lblChangePercent.ForeColor = IsNightMode ? Color.FromArgb(144, 238, 144) : Color.Green; // gunduz gece 
            else if (_rawValueChangePercent < 0) lblChangePercent.ForeColor = IsNightMode ? Color.FromArgb(250, 128, 114) : Color.Red; // gunduz gece 
            else lblChangePercent.ForeColor = IsNightMode ? Color.FromArgb(220, 220, 220) : SystemColors.ControlText;
        }

        public void ApplyCardTheme()
        {
            Color cardBackColor, textColor;
            if (IsNightMode) { cardBackColor = Color.FromArgb(45, 45, 48); textColor = Color.FromArgb(220, 220, 220); }
            else { cardBackColor = Color.White; textColor = Color.FromArgb(47, 54, 64); }
            this.BackColor = cardBackColor;
            if (lblSymbol != null) lblSymbol.ForeColor = textColor;
            if (lblPrice != null) lblPrice.ForeColor = textColor;
            UpdateChangePercentColor();
            UpdateActiveState();
            this.BorderStyle = BorderStyle.FixedSingle;
        }
    }
}
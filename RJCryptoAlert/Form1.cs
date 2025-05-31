using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Globalization;
using System.Media;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Reflection;
using SCI = System.StringComparison;

namespace RJCryptoAlert
{
    public class CoinInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; 
        public string Symbol { get; set; } = string.Empty; // İlkin dyer
        public string? Slug { get; set; } // Null
        public override string ToString() => $"{Name} ({Symbol})";
    }

    public class ApiSettingsPlaceholder
    {
        public string ApiKey { get; set; } = string.Empty; 
    }

    public partial class Form1 : Form
    {
        private Panel? pnlMainContent;
        private HeaderControl? myHeaderControl;
        private Panel? pnlApiSettings;
        private Panel? pnlTopControls;
        private FlowLayoutPanel? flpCoinCards;
        private Panel? pnlChartArea;
        private Chart? nativeChart;
        private Label? lblApiSourcePrompt;
        private ComboBox? cmbApiSource;
        private Label? lblApiKeyPrompt;
        private TextBox? txtApiKeyInput;
        private Button? btnSaveApiKey;
        private Label? lblCoinSelectPrompt;
        private ComboBox? cmbCoinSelect;
        private Button? btnAddCoin;
        private Label? lblAlertPercentagePrompt;
        private NumericUpDown? nudAlertPercentage;
        private Label? lblRefreshIntervalPrompt;
        private NumericUpDown? nudRefreshInterval;
        private Button? btnToggleAutoRefresh;
        private NotifyIcon? trayIcon;
        private ContextMenuStrip? trayMenu;
        //bu hissede sexsi apini qoyun bu pulsuz apidir limiti var
        private string currentApiKey = "af248d4f-a06f-4ddf-a639-d2e80e49a7ee";
        private static readonly HttpClient client = new HttpClient();

        private Dictionary<string, JToken> latestCoinQuotes = new Dictionary<string, JToken>();
        private Dictionary<string, CoinCardControl> trackedCoinItems = new Dictionary<string, CoinCardControl>();
        private System.Windows.Forms.Timer? refreshTimer; // Null

        private readonly string settingsFolderPath;
        private readonly string trackedCoinsFilePath;
        private readonly string themeSettingsFilePath;
        private readonly string apiSettingsFilePath;

        private List<CoinInfo> availableCoins = new List<CoinInfo>();
        private bool isLoadingCoins = false;
        private bool isNightModeActive = false;
        private CoinCardControl? _activeCoinCard = null; 

        public Form1()
        {
            settingsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RJCryptoAlert");
            if (!Directory.Exists(settingsFolderPath)) Directory.CreateDirectory(settingsFolderPath);
            trackedCoinsFilePath = Path.Combine(settingsFolderPath, "tracked_coins.json");
            themeSettingsFilePath = Path.Combine(settingsFolderPath, "theme.settings");
            apiSettingsFilePath = Path.Combine(settingsFolderPath, "api_settings.json");

            this.DoubleBuffered = true;
            LoadApiSettings();
            SetupMainFormProperties();
            CreateLayoutPanelsAndHeader();
            CreateMainContentControls();
            InitializeTimer();
            InitializeTrayIconAndMenu();
            LoadThemePreference();
            if (myHeaderControl != null) myHeaderControl.IsNightMode = this.isNightModeActive;
            ApplyThemeToMainForm();
            LoadFormIcon();
            this.Text = "RJ Crypto Alert";
            this.Load += new EventHandler(Form1_Load);
            this.Resize += new EventHandler(Form1_Resize);
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);              
        }

        private void LoadFormIcon()
        {
            try
            {
                string resourceName = "RJCryptoAlert.Resources.appicon.ico";
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream? stream = assembly.GetManifestResourceStream(resourceName)) 
                {
                    if (stream != null) this.Icon = new System.Drawing.Icon(stream);
                    else Console.WriteLine($"Form icon resource not found: '{resourceName}'.");
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error loading form icon: {ex.Message}"); }
        }

        private void SetupMainFormProperties() { this.Size = new Size(620, 780); this.MinimumSize = new Size(580, 550); this.StartPosition = FormStartPosition.CenterScreen; }
        private void CreateLayoutPanelsAndHeader() { myHeaderControl = new HeaderControl { Name = "myHeaderControlInstance", Dock = DockStyle.Top }; myHeaderControl.ThemeToggleButtonClicked += MyHeaderControl_ThemeToggleButtonClicked; this.Controls.Add(myHeaderControl); pnlMainContent = new Panel { Name = "pnlMainContentInstance", Dock = DockStyle.Fill, Padding = new Padding(10) }; this.Controls.Add(pnlMainContent); pnlMainContent.BringToFront(); }

        private void CreateMainContentControls()
        {
            int controlHeight = 23; int padding = 5; int leftMargin = 0;
            pnlApiSettings = new Panel { Name = "pnlApiSettings", Height = 35, Dock = DockStyle.Top, Padding = new Padding(0, 0, 0, padding) };
            lblApiSourcePrompt = new Label { Name = "lblApiSourcePrompt", Text = "API Mənbəyi:", Location = new Point(leftMargin, padding + 3), AutoSize = true }; pnlApiSettings.Controls.Add(lblApiSourcePrompt);
            cmbApiSource = new ComboBox { Name = "cmbApiSource", Location = new Point(lblApiSourcePrompt.Right + padding, padding), Size = new Size(120, controlHeight), DropDownStyle = ComboBoxStyle.DropDownList }; cmbApiSource.Items.Add("CoinMarketCap"); cmbApiSource.SelectedIndex = 0; pnlApiSettings.Controls.Add(cmbApiSource);
            lblApiKeyPrompt = new Label { Name = "lblApiKeyPrompt", Text = "API Açarı:", Location = new Point(cmbApiSource.Right + padding + 10, padding + 3), AutoSize = true }; pnlApiSettings.Controls.Add(lblApiKeyPrompt);
            txtApiKeyInput = new TextBox { Name = "txtApiKeyInput", Location = new Point(lblApiKeyPrompt.Right + padding, padding), Size = new Size(180, controlHeight), PasswordChar = '*', Anchor = AnchorStyles.Top | AnchorStyles.Left }; txtApiKeyInput.Text = currentApiKey; pnlApiSettings.Controls.Add(txtApiKeyInput);
            btnSaveApiKey = new Button { Name = "btnSaveApiKey", Text = "Yadda Saxla", Location = new Point(txtApiKeyInput.Right + padding, padding - 2), Size = new Size(90, controlHeight + 2), Anchor = AnchorStyles.Top | AnchorStyles.Left }; btnSaveApiKey.Click += BtnSaveApiKey_Click; pnlApiSettings.Controls.Add(btnSaveApiKey);
            pnlTopControls = new Panel { Name = "pnlTopControlsInstance", Height = 70, Dock = DockStyle.Top, Padding = new Padding(0, 0, 0, padding) };
            int currentY_topPanel = padding;
            lblCoinSelectPrompt = new Label { Name = "lblCoinSelectPromptInstance", Text = "Kripto Seçin:", Location = new Point(leftMargin, currentY_topPanel + 3), AutoSize = true }; pnlTopControls.Controls.Add(lblCoinSelectPrompt);
            cmbCoinSelect = new ComboBox { Name = "cmbCoinSelectInstance", Location = new Point(lblCoinSelectPrompt.Right + padding, currentY_topPanel), Size = new Size(180, controlHeight), DropDownStyle = ComboBoxStyle.DropDown, AutoCompleteMode = AutoCompleteMode.SuggestAppend, AutoCompleteSource = AutoCompleteSource.ListItems }; pnlTopControls.Controls.Add(cmbCoinSelect);
            btnAddCoin = new Button { Name = "btnAddCoinInstance", Text = "Əlavə Et", Location = new Point(cmbCoinSelect.Right + padding, currentY_topPanel - 2), Size = new Size(75, controlHeight + 2) }; btnAddCoin.Click += new EventHandler(btnAddCoin_Click); pnlTopControls.Controls.Add(btnAddCoin);
            lblAlertPercentagePrompt = new Label { Name = "lblAlertPercentagePromptInstance", Text = "Siqnal %:", Location = new Point(btnAddCoin.Right + padding + 10, currentY_topPanel + 3), AutoSize = true }; pnlTopControls.Controls.Add(lblAlertPercentagePrompt);
            nudAlertPercentage = new NumericUpDown { Name = "nudAlertPercentageInstance", Location = new Point(lblAlertPercentagePrompt.Right + padding, currentY_topPanel), Size = new Size(55, controlHeight), Minimum = 0.1m, Maximum = 100, Value = 5, DecimalPlaces = 1, Increment = 0.1m }; pnlTopControls.Controls.Add(nudAlertPercentage);
            currentY_topPanel += controlHeight + padding + 5;
            lblRefreshIntervalPrompt = new Label { Name = "lblRefreshIntervalPromptInstance", Text = "Yeniləmə (dəq):", Location = new Point(leftMargin, currentY_topPanel + 3), AutoSize = true }; pnlTopControls.Controls.Add(lblRefreshIntervalPrompt);
            nudRefreshInterval = new NumericUpDown { Name = "nudRefreshIntervalInstance", Location = new Point(lblRefreshIntervalPrompt.Right + padding, currentY_topPanel), Size = new Size(50, controlHeight), Minimum = 1, Maximum = 60, Value = 1 }; nudRefreshInterval.ValueChanged += new EventHandler(nudRefreshInterval_ValueChanged); pnlTopControls.Controls.Add(nudRefreshInterval);
            btnToggleAutoRefresh = new Button { Name = "btnToggleAutoRefreshInstance", Text = "Yeniləməni Dayandır", Location = new Point(nudRefreshInterval.Right + padding, currentY_topPanel - 2), Size = new Size(160, controlHeight + 2) }; btnToggleAutoRefresh.Click += BtnToggleAutoRefresh_Click; pnlTopControls.Controls.Add(btnToggleAutoRefresh);
            pnlChartArea = new Panel { Name = "pnlChartAreaInstance", Dock = DockStyle.Bottom, Height = 300, Padding = new Padding(0, padding, 0, 0) };
            nativeChart = new Chart { Name = "nativeChartInstance", Dock = DockStyle.Fill };
            ChartArea chartArea1 = new ChartArea("MainChartArea"); nativeChart.ChartAreas.Add(chartArea1);
            Series priceSeries = new Series("PriceSeries") { ChartType = SeriesChartType.Line, BorderWidth = 2, XValueType = ChartValueType.String, YValueType = ChartValueType.Double }; nativeChart.Series.Add(priceSeries);
            pnlChartArea.Controls.Add(nativeChart);
            flpCoinCards = new FlowLayoutPanel { Name = "flpCoinCardsInstance", Dock = DockStyle.Fill, AutoScroll = true, FlowDirection = FlowDirection.LeftToRight, WrapContents = true, Padding = new Padding(0, 0, 0, padding) };
            pnlMainContent.Controls.Add(flpCoinCards); pnlMainContent.Controls.Add(pnlChartArea); pnlMainContent.Controls.Add(pnlTopControls); pnlMainContent.Controls.Add(pnlApiSettings);
        }

        private void MyHeaderControl_ThemeToggleButtonClicked(object? sender, EventArgs e) { isNightModeActive = !isNightModeActive; if (myHeaderControl != null) myHeaderControl.IsNightMode = isNightModeActive; ApplyThemeToMainForm(); UpdateNativeChart(_activeCoinCard?.Name, latestCoinQuotes.TryGetValue(_activeCoinCard?.Name ?? "", out JToken? quote) ? quote : null); } 

        private void ApplyThemeToMainForm() { Color formBackColor, textColor, mainContentBackColor, apiSettingsBackColor, topControlsBackColor, chartAreaBackColor, buttonBackColor, buttonForeColor, comboBoxBackColor, comboBoxForeColor, nudBackColor, nudForeColor, flowPanelBackColor; if (isNightModeActive) { formBackColor = Color.FromArgb(30, 30, 30); textColor = Color.FromArgb(220, 220, 220); mainContentBackColor = Color.FromArgb(30, 30, 30); apiSettingsBackColor = Color.FromArgb(45, 45, 48); topControlsBackColor = Color.FromArgb(37, 37, 42); chartAreaBackColor = Color.FromArgb(25, 25, 27); buttonBackColor = Color.FromArgb(63, 63, 70); buttonForeColor = Color.FromArgb(220, 220, 220); comboBoxBackColor = Color.FromArgb(51, 51, 55); comboBoxForeColor = Color.FromArgb(220, 220, 220); nudBackColor = comboBoxBackColor; nudForeColor = comboBoxForeColor; flowPanelBackColor = mainContentBackColor; } else { formBackColor = SystemColors.Control; textColor = SystemColors.ControlText; mainContentBackColor = SystemColors.Control; apiSettingsBackColor = SystemColors.ControlLight; topControlsBackColor = SystemColors.ControlLight; chartAreaBackColor = SystemColors.ControlDark; buttonBackColor = SystemColors.Control; buttonForeColor = SystemColors.ControlText; comboBoxBackColor = SystemColors.Window; comboBoxForeColor = SystemColors.WindowText; nudBackColor = comboBoxBackColor; nudForeColor = comboBoxForeColor; flowPanelBackColor = mainContentBackColor; } this.BackColor = formBackColor; if (pnlMainContent != null) pnlMainContent.BackColor = mainContentBackColor; if (pnlApiSettings != null) pnlApiSettings.BackColor = apiSettingsBackColor; if (pnlTopControls != null) pnlTopControls.BackColor = topControlsBackColor; if (pnlChartArea != null) pnlChartArea.BackColor = chartAreaBackColor; if (flpCoinCards != null) flpCoinCards.BackColor = flowPanelBackColor; ApplyChartTheme(); Action<Control.ControlCollection>? setThemeRecursive = null; setThemeRecursive = (controls) => { foreach (Control ctrl in controls) { ctrl.ForeColor = textColor; if (ctrl is Button) { ctrl.BackColor = buttonBackColor; ctrl.ForeColor = buttonForeColor; } else if (ctrl is ComboBox) { ctrl.BackColor = comboBoxBackColor; ctrl.ForeColor = comboBoxForeColor; } else if (ctrl is TextBox) { ctrl.BackColor = comboBoxBackColor; ctrl.ForeColor = comboBoxForeColor; } else if (ctrl is NumericUpDown) { ctrl.BackColor = nudBackColor; ctrl.ForeColor = nudForeColor; } else if (ctrl is Label) { ctrl.BackColor = Color.Transparent; } if (ctrl.HasChildren && !(ctrl is UserControl) && !(ctrl is Chart) && setThemeRecursive != null) setThemeRecursive(ctrl.Controls); } }; if (pnlApiSettings != null && setThemeRecursive != null) setThemeRecursive(pnlApiSettings.Controls); if (pnlTopControls != null && setThemeRecursive != null) setThemeRecursive(pnlTopControls.Controls); foreach (CoinCardControl card in trackedCoinItems.Values) { if (card != null) card.IsNightMode = this.isNightModeActive; } }
        private void SaveThemePreference() { try { if (!Directory.Exists(settingsFolderPath)) Directory.CreateDirectory(settingsFolderPath); File.WriteAllText(themeSettingsFilePath, isNightModeActive.ToString()); } catch (Exception ex) { Console.WriteLine($"Error saving theme: {ex.Message}"); } }
        private void LoadThemePreference() { try { if (File.Exists(themeSettingsFilePath)) { if (bool.TryParse(File.ReadAllText(themeSettingsFilePath), out bool saved)) isNightModeActive = saved; } else isNightModeActive = false; } catch (Exception ex) { Console.WriteLine($"Error loading theme: {ex.Message}"); isNightModeActive = false; } }
        private async void Form1_Load(object? sender, EventArgs e) { await LoadAvailableCoinsToList(); await LoadTrackedCoinsAsync(); ApplyThemeToMainForm(); if (_activeCoinCard != null) ActivateCard(_activeCoinCard); else if (trackedCoinItems.Any()) ActivateCard(trackedCoinItems.Values.First()); else UpdateNativeChart(null, null); } 
        private void InitializeTimer() { refreshTimer = new System.Windows.Forms.Timer(); if (nudRefreshInterval != null) refreshTimer.Interval = (int)nudRefreshInterval.Value * 60 * 1000; else refreshTimer.Interval = 60000; refreshTimer.Tick += new EventHandler(refreshTimer_Tick); refreshTimer.Start(); }
        private void InitializeTrayIconAndMenu() { trayMenu = new ContextMenuStrip(); ToolStripMenuItem o = new ToolStripMenuItem("Proqramı Aç"); o.Click += OpenMenuItem_Click; trayMenu.Items.Add(o); trayMenu.Items.Add(new ToolStripSeparator()); ToolStripMenuItem ex = new ToolStripMenuItem("Çıxış"); ex.Click += ExitMenuItem_Click; trayMenu.Items.Add(ex); trayIcon = new NotifyIcon { Icon = SystemIcons.Information, Text = "RJ Crypto Alert v1.0 Beta", ContextMenuStrip = trayMenu, Visible = false }; trayIcon.DoubleClick += TrayIcon_DoubleClick; }
        private void Form1_Resize(object? sender, EventArgs e) { if (this.WindowState == FormWindowState.Minimized) { this.Hide(); if (trayIcon != null) trayIcon.Visible = true; } } // sender nullable, trayIcon null check
        private void TrayIcon_DoubleClick(object? sender, EventArgs e) { ShowForm(); } 
        private void OpenMenuItem_Click(object? sender, EventArgs e) { ShowForm(); } 
        private void ShowForm() { this.Show(); this.WindowState = FormWindowState.Normal; this.Activate(); if (trayIcon != null) trayIcon.Visible = false; }
        private void ExitMenuItem_Click(object? sender, EventArgs e) { this.Close(); } 
        private void nudRefreshInterval_ValueChanged(object? sender, EventArgs e) { if (refreshTimer != null && nudRefreshInterval != null) { bool w = refreshTimer.Enabled; refreshTimer.Stop(); refreshTimer.Interval = Math.Max(1000, (int)nudRefreshInterval.Value * 60 * 1000); if (w) refreshTimer.Start(); } } 
        private void BtnToggleAutoRefresh_Click(object? sender, EventArgs e) { if (refreshTimer != null && btnToggleAutoRefresh != null) { if (refreshTimer.Enabled) { refreshTimer.Stop(); btnToggleAutoRefresh.Text = "Yeniləməni Başlat"; } else { refreshTimer.Start(); btnToggleAutoRefresh.Text = "Yeniləməni Dayandır"; refreshTimer_Tick(null, EventArgs.Empty); } } } 

        private async Task LoadAvailableCoinsToList()
        {
            if (isLoadingCoins) return; isLoadingCoins = true;
            if (cmbCoinSelect != null) cmbCoinSelect.Enabled = false;
            string rUrl = $"https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest?limit=200&convert=USD";
            try
            {
                HttpClient tempClient = new HttpClient();
                if (!string.IsNullOrWhiteSpace(currentApiKey)) tempClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", currentApiKey);
                tempClient.DefaultRequestHeaders.Accept.Clear(); tempClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage resp = await tempClient.GetAsync(rUrl); string rBody = await resp.Content.ReadAsStringAsync();
                tempClient.Dispose();
                if (!resp.IsSuccessStatusCode) { Console.WriteLine($"Coin list err:{resp.StatusCode}"); isLoadingCoins = false; if (cmbCoinSelect != null && cmbCoinSelect.IsHandleCreated) cmbCoinSelect.Invoke((System.Windows.Forms.MethodInvoker)delegate { if (cmbCoinSelect != null) cmbCoinSelect.Enabled = true; }); return; }
                JObject jR = JObject.Parse(rBody); var cD = jR["data"];
                if (cD != null && cD.Type == JTokenType.Array)
                {
                    availableCoins.Clear();
                    foreach (var cT in cD) { availableCoins.Add(new CoinInfo { Id = cT["id"]?.Value<int>() ?? 0, Name = cT["name"]?.Value<string>() ?? "N/A", Symbol = cT["symbol"]?.Value<string>() ?? "N/A", Slug = cT["slug"]?.Value<string>() }); } // Slug nulldu
                    if (this.IsHandleCreated && cmbCoinSelect != null && cmbCoinSelect.IsHandleCreated) this.Invoke((System.Windows.Forms.MethodInvoker)delegate {
                        if (cmbCoinSelect != null) { cmbCoinSelect.Items.Clear(); foreach (var c in availableCoins.OrderBy(c => c.Name)) { cmbCoinSelect.Items.Add(c); } cmbCoinSelect.SelectedIndex = -1; }
                    });
                }
            }
            catch (Exception ex) { Console.WriteLine($"Coin list exc:{ex.Message}"); }
            finally { isLoadingCoins = false; if (cmbCoinSelect != null && cmbCoinSelect.IsHandleCreated) this.Invoke((System.Windows.Forms.MethodInvoker)delegate { if (cmbCoinSelect != null) cmbCoinSelect.Enabled = true; }); }
        }

        private async void btnAddCoin_Click(object? sender, EventArgs e) // nkll
        {
            string? coinSymbol = null; CoinInfo? selectedCoinInfo = null;
            if (cmbCoinSelect?.SelectedItem is CoinInfo si) { coinSymbol = si.Symbol; selectedCoinInfo = si; }
            else if (cmbCoinSelect != null && !string.IsNullOrWhiteSpace(cmbCoinSelect.Text)) { string inputText = cmbCoinSelect.Text.Trim().ToUpper(); var fBS = availableCoins.FirstOrDefault(c => c.Symbol.Equals(inputText, SCI.OrdinalIgnoreCase)); if (fBS != null) { selectedCoinInfo = fBS; coinSymbol = fBS.Symbol; } else { var fBaS = availableCoins.FirstOrDefault(c => c.ToString().Equals(cmbCoinSelect.Text, SCI.OrdinalIgnoreCase)); if (fBaS != null) { selectedCoinInfo = fBaS; coinSymbol = fBaS.Symbol; } else { int oP = inputText.LastIndexOf('('); int cP = inputText.LastIndexOf(')'); if (oP != -1 && cP > oP) coinSymbol = inputText.Substring(oP + 1, cP - oP - 1); else coinSymbol = inputText; selectedCoinInfo = availableCoins.FirstOrDefault(c => c.Symbol.Equals(coinSymbol, SCI.OrdinalIgnoreCase)); } } }
            if (string.IsNullOrWhiteSpace(coinSymbol)) { MessageBox.Show("Kriptovalyuta seçin/daxil edin.", "Xəta"); if (cmbCoinSelect != null) cmbCoinSelect.Focus(); return; }
            if (trackedCoinItems.ContainsKey(coinSymbol)) { MessageBox.Show($"'{coinSymbol}' artıq izlənilir.", "Məlumat"); if (cmbCoinSelect != null) cmbCoinSelect.Focus(); return; }
            if (string.IsNullOrWhiteSpace(currentApiKey) || currentApiKey.StartsWith("YOUR_API_KEY") || currentApiKey.Length < 20) { MessageBox.Show("Zəhmət olmasa, yuxarıdakı API Açarı xanasına düzgün açar daxil edib 'Yadda Saxla' düyməsini basın.", "API Açarı Xətası"); return; }
            string nameForCard = selectedCoinInfo?.Name ?? coinSymbol; string? slugForCard = selectedCoinInfo?.Slug ?? (nameForCard.ToLowerInvariant().Replace(" ", "-"));
            await FetchAndProcessCoinPrice(coinSymbol, nameForCard, slugForCard, true);
            if (cmbCoinSelect != null) { cmbCoinSelect.Text = ""; cmbCoinSelect.Focus(); }
        }

        private async void refreshTimer_Tick(object? sender, EventArgs e) // sil
        {
            if (refreshTimer == null || !refreshTimer.Enabled || trackedCoinItems.Count == 0) return;
            foreach (KeyValuePair<string, CoinCardControl> entry in trackedCoinItems.ToList())
            {
                await FetchAndProcessCoinPrice(entry.Key, entry.Value.SymbolText, entry.Value.CoinSlug, false);
                await Task.Delay(300);
            }
        }

        private async Task FetchAndProcessCoinPrice(string symbol, string cardName, string? cardSlug, bool isNewAddition) // cardSlug bos
        {
            if (string.IsNullOrWhiteSpace(symbol)) return; this.Cursor = Cursors.WaitCursor;
            JToken? quoteUSD = null; 
            try
            {
                HttpClient requestClient = new HttpClient();
                if (!string.IsNullOrWhiteSpace(currentApiKey)) requestClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", currentApiKey);
                requestClient.DefaultRequestHeaders.Accept.Clear(); requestClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string rUrl = $"https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest?symbol={symbol}&convert=USD";
                HttpResponseMessage resp = await requestClient.GetAsync(rUrl); string rBody = await resp.Content.ReadAsStringAsync();
                requestClient.Dispose();
                if (!resp.IsSuccessStatusCode) { JObject? ej = null; string sm = resp.ReasonPhrase ?? "Unknown error"; try { ej = JObject.Parse(rBody); } catch { } if (ej?["status"]?["error_message"] != null) { sm = ej["status"]!["error_message"]!.ToString(); } Console.WriteLine($"API Err {symbol}({resp.StatusCode}):{sm}"); if (trackedCoinItems.TryGetValue(symbol, out CoinCardControl? cce)) HandleCardError(cce, "API Xətası"); this.Cursor = Cursors.Default; return; }
                JObject jR = JObject.Parse(rBody); var cD = jR["data"]?[symbol];
                quoteUSD = cD?["quote"]?["USD"]; var pT = quoteUSD?["price"];
                if (pT != null && pT.Type != JTokenType.Null)
                {
                    decimal cPD; try { cPD = pT.Value<decimal>(); } catch (FormatException) { if (!decimal.TryParse(pT.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out cPD)) { Console.WriteLine($"CRITICAL {symbol} FAILED parse '{pT}'"); HandleCardError(trackedCoinItems.TryGetValue(symbol, out var cce1) ? cce1 : null, "Qiymət Err"); this.Cursor = Cursors.Default; return; } }
                    if (quoteUSD != null) latestCoinQuotes[symbol] = quoteUSD;
                    CoinCardControl? card; // Nullable
                    if (isNewAddition)
                    {
                        card = new CoinCardControl { SymbolText = cardName, Name = symbol, CoinSlug = cardSlug, PriceText = cPD.ToString("F4", CultureInfo.InvariantCulture) + " USD", RawValueChangePercent = 0, ChangePercentText = "0.00%", IsNightMode = this.isNightModeActive };
                        card.CardClicked += CoinCard_Clicked;
                        Action addAction = () => { if (flpCoinCards != null) { flpCoinCards.Controls.Add(card); flpCoinCards.PerformLayout(); } };
                        if (flpCoinCards != null && flpCoinCards.IsHandleCreated) { if (flpCoinCards.InvokeRequired) flpCoinCards.Invoke(addAction); else addAction(); }
                        else if (this.IsHandleCreated) { this.BeginInvoke(addAction); }
                        else { Console.WriteLine($"ERROR: No handle for Form/FLP to add card {symbol}."); }
                        trackedCoinItems[symbol] = card; card.Tag = cPD;
                        if (_activeCoinCard == null) ActivateCard(card);
                    }
                    else if (trackedCoinItems.TryGetValue(symbol, out card) && card != null) { decimal pPD = (card.Tag is decimal) ? (decimal)card.Tag : 0; card.PriceText = cPD.ToString("F4", CultureInfo.InvariantCulture) + " USD"; decimal pc = 0; if (pPD != 0) pc = ((cPD - pPD) / pPD) * 100; card.ChangePercentText = $"{pc:F2}%"; card.RawValueChangePercent = pc; decimal at = nudAlertPercentage?.Value ?? 5m; bool sc = false; if (pPD != 0) { if (pc >= at) { ShowAlert(symbol, cPD, pPD, pc, true); sc = true; } else if (pc <= -at) { ShowAlert(symbol, cPD, pPD, pc, false); sc = true; } } if (sc) { if (pc >= at) card.BackColor = isNightModeActive ? Color.FromArgb(30, 80, 30) : Color.LightGreen; else if (pc <= -at) card.BackColor = isNightModeActive ? Color.FromArgb(100, 30, 30) : Color.LightCoral; } else { card.IsActiveCard = card.IsActiveCard; card.ApplyCardTheme(); } card.Tag = cPD; if (_activeCoinCard != null && _activeCoinCard.Name == symbol) UpdateNativeChart(symbol, quoteUSD); }
                }
                else { HandleCardError(trackedCoinItems.TryGetValue(symbol, out var cce2) ? cce2 : null, "Tapılmadı"); }
            }
            catch (Exception ex) { HandleCardError(trackedCoinItems.TryGetValue(symbol, out var cce3) ? cce3 : null, "Xəta!"); Console.WriteLine($"Gen Exc {symbol}:{ex.Message}\n{ex.StackTrace}"); }
            finally { this.Cursor = Cursors.Default; }
        }

        private void CoinCard_Clicked(object? sender, EventArgs e) { if (sender is CoinCardControl c) ActivateCard(c); } 
        private void ActivateCard(CoinCardControl? cardToActivate) { if (cardToActivate == null) { UpdateNativeChart(null, null); return; } if (_activeCoinCard != null && _activeCoinCard != cardToActivate) _activeCoinCard.IsActiveCard = false; _activeCoinCard = cardToActivate; _activeCoinCard.IsActiveCard = true; if (latestCoinQuotes.TryGetValue(_activeCoinCard.Name, out JToken? currentQuote)) UpdateNativeChart(_activeCoinCard.Name, currentQuote); else UpdateNativeChart(_activeCoinCard.Name, null); } // JToken? nullable

        private void UpdateNativeChart(string? coinSymbol, JToken? quoteData) 
        {
            if (nativeChart == null || !nativeChart.IsHandleCreated || nativeChart.Series.Count == 0 || nativeChart.ChartAreas.Count == 0) { return; }
            var series = nativeChart.Series["PriceSeries"];
            if (series == null) { return; }
            series.Points.Clear(); nativeChart.Titles.Clear();
            if (string.IsNullOrWhiteSpace(coinSymbol) || quoteData == null) { nativeChart.Titles.Add(string.IsNullOrWhiteSpace(coinSymbol) ? "Koin Seçin" : $"{coinSymbol} - Məlumat Yoxdur/Gözlənilir"); ChartArea ma = nativeChart.ChartAreas["MainChartArea"]; ma.AxisX.Title = ""; ma.AxisY.Title = ""; ma.AxisX.CustomLabels.Clear(); ma.AxisY.CustomLabels.Clear(); ApplyChartTheme(); nativeChart.Invalidate(); return; }
            try { decimal cP = quoteData["price"]?.Value<decimal>() ?? 0; var dP = new List<Tuple<string, decimal>>(); dP.Add(Tuple.Create("İndi", cP)); Func<decimal, double?, decimal?> calcPast = (curr, pct) => pct.HasValue ? curr / (1 + (decimal)(pct.Value / 100.0)) : (decimal?)null; var p24h = calcPast(cP, quoteData["percent_change_24h"]?.Value<double?>()); if (p24h.HasValue) dP.Add(Tuple.Create("-24s", p24h.Value)); var p7d = calcPast(cP, quoteData["percent_change_7d"]?.Value<double?>()); if (p7d.HasValue) dP.Add(Tuple.Create("-7g", p7d.Value)); var p30d = calcPast(cP, quoteData["percent_change_30d"]?.Value<double?>()); if (p30d.HasValue) dP.Add(Tuple.Create("-30g", p30d.Value)); var oP = new List<Tuple<string, decimal>>(); if (p30d.HasValue) oP.Add(dP.First(dp => dp.Item1 == "-30g")); if (p7d.HasValue) oP.Add(dP.First(dp => dp.Item1 == "-7g")); if (p24h.HasValue) oP.Add(dP.First(dp => dp.Item1 == "-24s")); oP.Add(dP.First(dp => dp.Item1 == "İndi")); if (oP.Count < 2 && oP.Any()) { oP.Insert(0, Tuple.Create("Əvvəl", oP[0].Item2)); } else if (!oP.Any()) { oP.Add(Tuple.Create("Məlumat Yoxdur", 0m)); } foreach (var p in oP) series.Points.AddXY(p.Item1, (double)p.Item2); nativeChart.Titles.Add($"{coinSymbol} - Trend (CMC Faiz Dəyişiklikləri)"); ChartArea maN = nativeChart.ChartAreas["MainChartArea"]; maN.AxisX.Title = "Zaman Aralığı"; maN.AxisY.Title = "Qiymət (USD)"; maN.AxisX.LabelStyle.Angle = 0; maN.AxisX.Interval = 1; maN.AxisY.LabelStyle.Format = "N2"; }
            catch (Exception ex) { Console.WriteLine($"Chart data err {coinSymbol}:{ex.Message}"); nativeChart.Titles.Add($"{coinSymbol} - Qrafik Xətası"); }
            ApplyChartTheme(); nativeChart.Invalidate();
        }

        private void ApplyChartTheme() { if (nativeChart == null || !nativeChart.IsHandleCreated || nativeChart.ChartAreas.Count == 0) return; Color cB, cF, cG, cA, sC; if (isNightModeActive) { cB = Color.FromArgb(30, 30, 30); cF = Color.FromArgb(220, 220, 220); cG = Color.FromArgb(60, 60, 60); cA = Color.FromArgb(150, 150, 150); sC = Color.SkyBlue; } else { cB = SystemColors.Window; cF = SystemColors.ControlText; cG = Color.LightGray; cA = Color.DarkGray; sC = Color.DodgerBlue; } nativeChart.BackColor = cB; if (nativeChart.Titles.Any()) nativeChart.Titles[0].ForeColor = cF; ChartArea mA = nativeChart.ChartAreas["MainChartArea"]; mA.BackColor = cB; mA.AxisX.TitleForeColor = cF; mA.AxisY.TitleForeColor = cF; mA.AxisX.LabelStyle.ForeColor = cF; mA.AxisY.LabelStyle.ForeColor = cF; mA.AxisX.MajorTickMark.LineColor = cA; mA.AxisY.MajorTickMark.LineColor = cA; mA.AxisX.MajorGrid.LineColor = cG; mA.AxisY.MajorGrid.LineColor = cG; mA.AxisX.LineColor = cA; mA.AxisY.LineColor = cA; if (nativeChart.Series.Count > 0 && nativeChart.Series["PriceSeries"] != null) nativeChart.Series["PriceSeries"].Color = sC; nativeChart.Invalidate(); }
        private void HandleCardError(CoinCardControl? card, string message = "Xəta!") { if (card == null) return; card.PriceText = message; card.ChangePercentText = ""; card.RawValueChangePercent = 0; card.ApplyCardTheme(); card.BackColor = isNightModeActive ? Color.FromArgb(70, 70, 70) : Color.LightGray; } // card bos
        private void ShowAlert(string s, decimal cP, decimal pP, decimal pc, bool iR) { string d = iR ? "YÜKSƏLDİ" : "DÜŞDÜ"; string a = iR ? "AL təklif!" : "SAT təklif!"; string t = $"RJ Alert:{s} {Math.Abs(pc):F2}% {d}!"; string msg = $"Yeni Qiymət:{cP:F4}\nƏvvəlki:{pP:F4}\n\n{a}"; SystemSounds.Exclamation.Play(); if (trayIcon != null && trayIcon.Visible) { ToolTipIcon ti = iR ? ToolTipIcon.Info : ToolTipIcon.Warning; trayIcon.ShowBalloonTip(5000, t, msg, ti); } MessageBox.Show(msg, t, MessageBoxButtons.OK, iR ? MessageBoxIcon.Information : MessageBoxIcon.Warning); }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e) // bos
        {
            SaveTrackedCoins(); SaveThemePreference(); SaveApiSettings();
            if (refreshTimer != null) { refreshTimer.Stop(); refreshTimer.Dispose(); refreshTimer = null; }
            if (trayIcon != null) { trayIcon.Visible = false; trayIcon.Dispose(); trayIcon = null; }
        }

        private void SaveTrackedCoins() { try { if (!Directory.Exists(settingsFolderPath)) Directory.CreateDirectory(settingsFolderPath); List<string> sym = trackedCoinItems.Keys.ToList(); string j = JsonConvert.SerializeObject(sym, Formatting.Indented); File.WriteAllText(trackedCoinsFilePath, j); } catch (Exception ex) { Console.WriteLine($"Err save coins:{ex.Message}"); } }

        private async Task LoadTrackedCoinsAsync()
        {
            try { if (File.Exists(trackedCoinsFilePath)) { string j = await File.ReadAllTextAsync(trackedCoinsFilePath); List<string>? syms = JsonConvert.DeserializeObject<List<string>>(j); if (syms != null && syms.Any()) { while (!this.IsHandleCreated || (flpCoinCards != null && !flpCoinCards.IsHandleCreated) || (myHeaderControl != null && !myHeaderControl.IsHandleCreated)) { await Task.Delay(100); } CoinCardControl? fC = null; foreach (string sym in syms) { if (!trackedCoinItems.ContainsKey(sym)) { CoinInfo? cD = availableCoins.FirstOrDefault(c => c.Symbol.Equals(sym, SCI.OrdinalIgnoreCase)); string cn = cD?.Name ?? sym; string? cs = cD?.Slug ?? (cn.ToLowerInvariant().Replace(" ", "-")); await FetchAndProcessCoinPrice(sym, cn, cs, true); if (fC == null && trackedCoinItems.TryGetValue(sym, out var curC)) fC = curC; await Task.Delay(350); } } if (fC != null) ActivateCard(fC); else if (trackedCoinItems.Any()) ActivateCard(trackedCoinItems.Values.First()); } } else { UpdateNativeChart(null, null); } } catch (Exception ex) { Console.WriteLine($"Err load coins:{ex.Message}"); UpdateNativeChart("Yükləmə Xətası", null); }
        }

        private void BtnSaveApiKey_Click(object? sender, EventArgs e) 
        {
            if (txtApiKeyInput != null)
            {
                currentApiKey = txtApiKeyInput.Text.Trim();
                client.DefaultRequestHeaders.Remove("X-CMC_PRO_API_KEY");
                if (!string.IsNullOrWhiteSpace(currentApiKey)) client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", currentApiKey);
                SaveApiSettings();
                MessageBox.Show("API açarı yadda saxlanıldı.", "Məlumat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SaveApiSettings() { try { var settings = new { ApiKey = currentApiKey }; string json = JsonConvert.SerializeObject(settings, Formatting.Indented); File.WriteAllText(apiSettingsFilePath, json); Console.WriteLine("API settings saved."); } catch (Exception ex) { Console.WriteLine($"Error saving API settings: {ex.Message}"); } }
        private void LoadApiSettings() { try { if (File.Exists(apiSettingsFilePath)) { string json = File.ReadAllText(apiSettingsFilePath); var settings = JsonConvert.DeserializeObject<ApiSettingsPlaceholder>(json); if (settings != null && !string.IsNullOrWhiteSpace(settings.ApiKey)) { currentApiKey = settings.ApiKey; if (txtApiKeyInput != null) txtApiKeyInput.Text = currentApiKey; Console.WriteLine("API settings loaded."); } } } catch (Exception ex) { Console.WriteLine($"Error loading API settings: {ex.Message}"); } }
    }
}
# RJ Crypto Alert v1.0 Beta

**Welcome!** RJ Crypto Alert is a Windows desktop application designed to track cryptocurrency price changes and provide alerts for significant movements.

**Note:** This application is currently in **Beta**. This means you might encounter some bugs or features might not work exactly as expected. Your feedback is highly valuable for improving the application!

## Current Features (v1.0 Beta)

* **Cryptocurrency Selection:**
    * Select from a list of ~200 popular cryptocurrencies (loaded from CoinMarketCap API) via a `ComboBox`.
    * Option to manually enter a symbol for coins not in the pre-loaded list.
* **Multi-Coin Tracking:** Track multiple cryptocurrencies simultaneously, displayed in a card-based view showing symbol, current price, last update time, previous price, and percentage change.
* **Automatic Price Refresh:**
    * Prices for all tracked cryptocurrencies are automatically refreshed from the CoinMarketCap API at regular intervals.
    * The refresh interval is user-configurable (1-60 minutes).
    * A button to toggle automatic refresh on/off.
* **Alert System:**
    * Ability to set a percentage threshold for price change alerts (default 5%).
    * Notifications when a coin's price changes by more than the set percentage (both for increases and decreases):
        * Sound notification.
        * Windows system tray "balloon tip" pop-up notification.
        * On-screen `MessageBox` with detailed information and "BUY/SELL" suggestions.
        * The corresponding coin card's background color changes to indicate the alert.
* **Interface and User Experience:**
    * User interface created entirely with C# code (no visual designer).
    * **Day/Night Theme Mode:** Switch between two color schemes with persistence of the last selected theme.
    * **Header Panel:** Includes a theme toggle button and a placeholder for a future menu.
    * **API Settings Section:** UI to select API source (currently CoinMarketCap only) and to input/save the API key.
    * **Minimize to System Tray:** The application window can be minimized to the system tray.
        * The window can be restored by double-clicking the tray icon or using the "Open Program" option from its right-click context menu.
        * The context menu also includes an "Exit" option.
* **Data Persistence:**
    * The list of tracked cryptocurrencies is saved when the program closes and reloaded on startup.
    * The selected theme mode is saved.
    * The entered API key is saved.
* **Chart Area:**
    * An internal chart area to display price information for the selected cryptocurrency.
    * Currently, the chart shows a simple trend line based on available percentage change data from the CoinMarketCap API (e.g., 24h, 7d, 30d changes).
* **Custom Application Icon:** For the application window and the `.exe` file.

## Planned Features & Improvements (For Future Versions)

* **UI/UX Enhancements:**
    * More modern and customizable design elements.
    * Improved appearance for coin cards (e.g., dynamic loading of coin logos, rounded corners, shadow effects).
    * Adding functionality to the "Menu" placeholder in the header (e.g., "Settings", "Help", "About" sections).
    * Full customization of the window's title bar and standard buttons (minimize, maximize, close), if desired.
* **Language Support:** Multi-language interface (e.g., Azerbaijani, English, Russian).
* **Advanced Theme Options:** More color schemes or user-customizable theme options.
* **Comprehensive Charting:**
    * Fetching **real, detailed historical price data** (OHLCV) from APIs (CoinMarketCap if possible, or alternatives like CoinGecko) to display more informative and interactive charts (e.g., candlestick charts).
    * User-selectable time intervals for charts (e.g., 1D, 7D, 1M, 1Y).
    * Possibility to add simple technical analysis indicators (e.g., Moving Averages).
* **Expanded API Integrations:**
    * Support for other popular cryptocurrency APIs like CoinGecko, allowing users to choose their data source.
    * More secure and flexible management of API keys and parameters for different APIs.
* **Enhanced Alert System:**
    * Ability to set alerts for specific target prices.
    * More notification types (e.g., email, Telegram - these require more complex integrations).
    * An alert log/history section within the application.
    * Ability to set individual alert percentages for different coins.
* **Additional Features:**
    * Simple portfolio tracking functionality.
    * Integration of basic news related to selected cryptocurrencies (if a suitable API is found).
    * A "Favorites" list for quick access to specific coins.

## Installation & Usage (Current Version)

1.  Run the `RJCryptoAlert.exe` file.
2.  The application requires **.NET 8 Desktop Runtime** to be installed on your system.
3.  On first use, or if your API key is not set up, please enter a valid CoinMarketCap API key in the "API Key" field in the UI and click "Save Key". You can obtain a free API key from the official CoinMarketCap website.

## Contact & Feedback

We appreciate your feedback to help improve RJ Crypto Alert!

* **[📧 Email Me](mailto:ruhidjavadoff@gmail.com)**
* **[💬 WhatsApp](https://wa.me/994506636031)** (Please ensure the number is correct and includes the country code if linking internationally)

## Our Websites

* **[🌐 Main Site](https://ruhidjavadoff.site/)**
* **[📱 Other Apps](https://ruhidjavadoff.site/app/)**
* **[ℹ️ RJ Crypto Alert Page](https://ruhidjavadoff.site/app/ca/)** (Program info, download, updates)

## Support Us

If you find this application useful, please consider supporting its development:

* **[💰 Donate via PayPal](https://www.paypal.com/donate/?business=ruhidjavadoff%40gmail.com&item_name=Support+for+RJ+Crypto+Alert&currency_code=USD&no_recurring=0)**
* **[❤️ Support on Our Site](https://ruhidjavadoff.site/donate/)**

---

**License**

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details (if you add one).
*(If you haven't added a LICENSE file yet, you can add one with the MIT License text I provided earlier).*

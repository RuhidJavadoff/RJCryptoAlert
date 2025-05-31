# RJ Crypto Alert v1.0 Beta

**Xoş Gəlmisiniz!** RJ Crypto Alert, seçilmiş kriptovalyutaların qiymət dəyişikliklərini izləmək və bu dəyişikliklər barədə siqnallar almaq üçün hazırlanmış bir Windows masaüstü tətbiqidir.

**Qeyd:** Bu proqram hazırda **Beta** mərhələsindədir. Bu o deməkdir ki, bəzi xətalarla qarşılaşa və ya bəzi funksiyalar tam gözlənildiyi kimi işləməyə bilər. Geri bildirişləriniz proqramın təkmilləşdirilməsi üçün çox dəyərlidir!

## Mövcud Funksiyalar (v1.0 Beta)

* **Kriptovalyuta Seçimi:**
    * CoinMarketCap API-sindən yüklənən ~200 populyar kriptovalyuta arasından `ComboBox` ilə seçim.
    * Siyahıda olmayan koinlər üçün simvolu manual daxil etmə imkanı.
* **Çoxsaylı Koin İzləmə:** Eyni anda bir neçə kriptovalyutanı izləmə və onların məlumatlarını (simvol, cari qiymət, son yenilənmə, əvvəlki qiymət, faiz dəyişikliyi) xüsusi kartlarda göstərmə.
* **Avtomatik Qiymət Yenilənməsi:**
    * İzlənilən bütün kriptovalyutaların qiymətləri CoinMarketCap API-sindən müntəzəm olaraq avtomatik yenilənir.
    * Yenilənmə intervalı istifadəçi tərəfindən dəqiqə ilə tənzimlənə bilir (1-60 dəqiqə).
    * Avtomatik yenilənməni dayandırıb/başlatmaq üçün düymə.
* **Siqnal Sistemi:**
    * Hər bir kriptovalyuta üçün qiymət dəyişikliyi siqnalı üçün faiz həddi təyin etmə imkanı (`NumericUpDown` ilə, standart 5%).
    * Qiymət təyin olunmuş faizdən çox dəyişdikdə (həm artım, həm azalma) aşağıdakı bildirişlər:
        * Səsli siqnal.
        * Windows sistem tepsisindən çıxan "balloon tip" pop-up bildirişi.
        * Ekranda detallı məlumat və "AL/SAT" tövsiyəsi olan `MessageBox`.
        * Müvafiq koin kartının arxa fon rənginin dəyişməsi (artım üçün yaşıl, azalma üçün qırmızı).
* **İnterfeys və İstifadəçi Təcrübəsi:**
    * Tamamilə kodla yaradılmış istifadəçi interfeysi.
    * **Gecə/Gündüz Tema Rejimi:** İki fərqli rəng sxemi arasında keçid və son seçilmiş temanın yadda saxlanması.
    * **Header Paneli:** Proqramın yuxarısında başlıq, tema dəyişmə düyməsi və gələcək menyu üçün yer.
    * **API Parametrləri Bölməsi:** API mənbəyini seçmək (hələlik yalnız CoinMarketCap) və API açarını daxil edib yadda saxlamaq üçün interfeys.
    * **Sistem Tepsisinə Kiçiltmə (Minimize to Tray):** Proqram pəncərəsi kiçildildikdə tapşırıqlar panelindən yoxa çıxır və yalnız sistem tepsisində ikonu qalır.
        * Tepsi ikonuna iki dəfə klikləməklə və ya sağ klik menyusundakı "Proqramı Aç" seçimi ilə pəncərə bərpa olunur.
        * Sağ klik menyusunda "Çıxış" seçimi ilə proqramı bağlamaq mümkündür.
* **Məlumatların Saxlanması:**
    * İzlənilən kriptovalyutaların siyahısı proqram bağlanıb-açıldıqda yadda saxlanılır.
    * Seçilmiş tema rejimi yadda saxlanılır.
    * Daxil edilmiş API açarı yadda saxlanılır.
* **Qrafik Sahəsi:**
    * Seçilmiş kriptovalyuta üçün proqram daxilində qrafik göstərilməsi üçün sahə.
    * Hazırda qrafik, CoinMarketCap API-sindən alınan cari qiymət və məhdud faiz dəyişikliyi məlumatlarına əsaslanan sadə bir trend xətti göstərir.
* **Proqram İkonu:** Pəncərə başlığında, tapşırıqlar panelində və `.exe` faylı üçün xüsusi proqram ikonu.

## Planlaşdırılan Əlavələr və Təkmilləşdirmələr

* **UI/UX Təkmilləşdirmələri:**
    * Daha detallı və fərdiləşdirilə bilən dizayn elementləri.
    * Kriptovalyuta kartlarının görünüşünün HTML/CSS nümunəsinə daha da yaxınlaşdırılması (məsələn, koin loqolarının dinamik yüklənməsi, oval kənarlar, kölgə effektləri).
    * Header-dəki "Menyu" hissəsinə real funksionallıqların (məsələn, "Ayarlar", "Kömək", "Haqqında" bölmələri) əlavə edilməsi.
    * Pəncərənin başlıq zolağının və standart düymələrinin (minimize, maximize, close) tamamilə özəlləşdirilməsi (əgər istək olarsa).
* **Dil Dəstəyi:** Proqram interfeysinin çoxdilli olması (məsələn, Azərbaycan, İngilis, Rus dilləri üçün seçim).
* **Tema Seçimlərinin Artırılması:** Daha çox rəng sxemi və ya istifadəçinin özünün fərdiləşdirə biləcəyi tema seçimləri.
* **Qrafiklərin Əsaslı Təkmilləşdirilməsi:**
    * CoinMarketCap (əgər mümkün olarsa) və ya CoinGecko kimi API-lərdən **real, detallı tarixi qiymət məlumatlarını** (OHLCV - Açılış, Yüksək, Alçaq, Bağlanış qiymətləri və Həcm) çəkərək daha informativ və interaktiv qrafiklər (məsələn, şamdan qrafikləri - candlestick charts).
    * Qrafiklər üçün zaman intervalı seçimi (məsələn, 1 gün, 7 gün, 1 ay, 1 il).
    * Qrafiklərə sadə texniki analiz indikatorları (məsələn, hərəkətli ortalamalar - Moving Averages) əlavə etmək imkanı.
* **API İnteqrasiyalarının Artırılması:**
    * CoinGecko kimi digər populyar kriptovalyuta API-lərini tam dəstəkləmək və istifadəçiyə API mənbəyini asanlıqla seçmək imkanı vermək.
    * Fərqli API-lər üçün API açarlarının və parametrlərinin daha təhlükəsiz və rahat şəkildə idarə edilməsi.
* **Siqnal (Alert) Sisteminin Genişləndirilməsi:**
    * Qiymətin müəyyən bir səviyyəyə (hədəf qiymətə - target price) çatması üçün xüsusi siqnallar qurmaq imkanı.
    * Daha çox bildiriş növü (məsələn, e-mail və ya Telegram vasitəsilə bildiriş göndərmək - bu daha mürəkkəb inteqrasiyalar tələb edir).
    * Baş vermiş siqnalların tarixçəsini (alert log) göstərən bir bölmə.
    * Hər bir koin üçün fərdi siqnal faizi təyin etmə imkanı.
* **Əlavə Funksiyalar:**
    * Sadə portfel izləmə funksiyası.
    * Seçilmiş kriptovalyutalarla bağlı əsas xəbərlərin inteqrasiyası (əgər uyğun API tapılarsa).
    * "Favori" koinlər siyahısı yaratmaq imkanı.

## Quraşdırma və İstifadə (Hazırkı Versiya Üçün)

1.  Proqramın `.exe` faylını işə salın.
2.  Proqramın düzgün işləməsi üçün kompüterinizdə **.NET 8 Desktop Runtime** quraşdırılmış olmalıdır.
3.  İlk dəfə işə saldıqda və ya API açarınız düzgün olmadıqda, yuxarıdakı "API Açarı" xanasına etibarlı bir CoinMarketCap API açarı daxil edib "Yadda Saxla" düyməsini basın. Pulsuz API açarını CoinMarketCap-in rəsmi saytından əldə edə bilərsiniz.

## Əlaqə və Geri Bildiriş

Bu Beta versiya ilə bağlı hər hansı bir təklifiniz, iradınız və ya qarşılaşdığınız xətalar barədə məlumat vermək üçün [Bura öz əlaqə vasitənizi yaza bilərsiniz, məsələn, e-mail ünvanı və ya GitHub səhifəsi].

Geri bildirişləriniz proqramın daha da yaxşılaşdırılmasına kömək edəcək!
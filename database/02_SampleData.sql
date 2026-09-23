/* ============================================================
   Mini B2B E-Ticaret Projesi - Ornek Test Verileri
   ------------------------------------------------------------
   Icerik:
     - 8 kategori, 48 urun (stok durumu Var / Kritik / Yok dagilimli,
       bir kismi pasif), 6 musteri, 3 slider, 2 dolu sepet, 9 siparis
     - Tum musteri sifresi: Bayi123!
   Not: Yonetici (Admin) kullanicisi bu script ile OLUSTURULMAZ.
        Uygulama ilk calistiginda appsettings'teki AdminSeed bolumune
        gore yoneticiyi kendisi olusturur (admin / Admin123!).
   Bu script tekrar tekrar calistirilabilir; basta ornek veriyi temizler.
   ============================================================ */

USE MiniB2B;
GO

SET NOCOUNT ON;
GO

/* ------------------------------------------------------------
   0. Temizlik  (FK sirasina gore)
   Yonetici hesabi ve grid konfigurasyonu korunur.
   ------------------------------------------------------------ */
DELETE FROM dbo.OrderItems;
DELETE FROM dbo.Orders;
DELETE FROM dbo.CartItems;
DELETE FROM dbo.Carts;
DELETE FROM dbo.Products;
DELETE FROM dbo.Categories;
DELETE FROM dbo.Sliders;
DELETE FROM dbo.Users WHERE Role = 'Customer';
GO

/* ------------------------------------------------------------
   1. Kategoriler
   ------------------------------------------------------------ */
INSERT INTO dbo.Categories (Name, IsActive) VALUES
    (N'Elektrik Malzemeleri', 1),
    (N'El Aletleri',          1),
    (N'Hırdavat ve Bağlantı', 1),
    (N'İş Güvenliği',         1),
    (N'Boya ve Yalıtım',      1),
    (N'Su Tesisatı',          1),
    (N'Aydınlatma',           1),
    (N'Bahçe ve Tarım',       1);
GO

DECLARE @Elektrik  INT = (SELECT Id FROM dbo.Categories WHERE Name = N'Elektrik Malzemeleri');
DECLARE @ElAleti   INT = (SELECT Id FROM dbo.Categories WHERE Name = N'El Aletleri');
DECLARE @Hirdavat  INT = (SELECT Id FROM dbo.Categories WHERE Name = N'Hırdavat ve Bağlantı');
DECLARE @Guvenlik  INT = (SELECT Id FROM dbo.Categories WHERE Name = N'İş Güvenliği');
DECLARE @Boya      INT = (SELECT Id FROM dbo.Categories WHERE Name = N'Boya ve Yalıtım');
DECLARE @Tesisat   INT = (SELECT Id FROM dbo.Categories WHERE Name = N'Su Tesisatı');
DECLARE @Aydinlat  INT = (SELECT Id FROM dbo.Categories WHERE Name = N'Aydınlatma');
DECLARE @Bahce     INT = (SELECT Id FROM dbo.Categories WHERE Name = N'Bahçe ve Tarım');

/* ------------------------------------------------------------
   2. Urunler
   Stok durumu dagilimi (gorsel stok gostergesini test etmek icin):
     StockQuantity  >  CriticalStockLevel        -> "Var"    (yesil)
     0 < StockQuantity <= CriticalStockLevel     -> "Kritik" (sari)
     StockQuantity  =  0                         -> "Yok"    (kirmizi)
   Gorseller placehold.co uzerinden gelir (internet baglantisi gerekir).
   Bazi urunlerde gorsel bilerek NULL birakildi.
   ------------------------------------------------------------ */
INSERT INTO dbo.Products
    (CategoryId, ProductCode, Name, Description, Brand, ManufacturerCode,
     SpecialCode1, SpecialCode2, ImageUrl, StockQuantity, CriticalStockLevel, Price, IsActive)
VALUES
-- Elektrik Malzemeleri ---------------------------------------------------
(@Elektrik, 'ELK-001', N'NYM Kablo 3x2.5 mm² (100 m)', N'TSE belgeli, bakır iletkenli, PVC izoleli tesisat kablosu. 100 metrelik makara.', N'Öznur', 'NYM-325',  'KBL-A', 'RAF-01', 'https://placehold.co/200x200/0d6efd/white?text=ELK-001', 480, 50, 3250.00, 1),
(@Elektrik, 'ELK-002', N'Otomatik Sigorta 16A C Tipi', N'Tek fazlı, C eğrili minyatür devre kesici. 6kA kesme kapasiteli.',              N'Viko',   'VK-16AC', 'SGT-B', 'RAF-01', 'https://placehold.co/200x200/0d6efd/white?text=ELK-002',   8, 10,   85.00, 1),
(@Elektrik, 'ELK-003', N'Kaçak Akım Rölesi 40A 30mA', N'İki kutuplu hayat koruma rölesi. Zorunlu güvenlik ekipmanı.',                    N'Schneider', 'SCH-4030', 'SGT-B', 'RAF-02', 'https://placehold.co/200x200/0d6efd/white?text=ELK-003',  35, 15,  620.00, 1),
(@Elektrik, 'ELK-004', N'Priz Anahtar Seti (Beyaz)',   N'Çerçeve dahil topraklı priz ve anahtar seti.',                                  N'Viko',   'VK-PRZ01', 'ANH-C', 'RAF-02', 'https://placehold.co/200x200/0d6efd/white?text=ELK-004', 240, 40,   72.50, 1),
(@Elektrik, 'ELK-005', N'Buat Kutusu 100x100 Sıva Altı', N'Alev yürütmez plastik, kapaklı buat kutusu.',                                 N'Mutlusan', 'MTL-100', 'BUT-D', 'RAF-03', NULL,                                                     0,  20,   18.75, 1),
(@Elektrik, 'ELK-006', N'Kablo Kanalı 40x40 (2 m)',    N'Yapışkan bantlı beyaz PVC kablo kanalı.',                                        N'Mutlusan', 'MTL-4040','KNL-E', 'RAF-03', 'https://placehold.co/200x200/0d6efd/white?text=ELK-006', 150, 30,   64.00, 1),
(@Elektrik, 'ELK-007', N'Pano Kilidi ve Menteşe Seti', N'Metal elektrik panoları için yedek kilit seti.',                                 N'Çetinkaya','CTK-PK1','PNO-F', 'RAF-04', NULL,                                                     4,   5,  145.00, 1),
(@Elektrik, 'ELK-008', N'TTR Kablo 3x1.5 mm² (50 m)',  N'Çok telli, bükülgen bakır kablo. Seyyar uygulamalar için.',                      N'Öznur',  'TTR-315',  'KBL-A', 'RAF-01', 'https://placehold.co/200x200/0d6efd/white?text=ELK-008', 320, 60, 1180.00, 1),

-- El Aletleri ------------------------------------------------------------
(@ElAleti, 'ELA-001', N'Darbeli Matkap 800W',        N'13 mm mandren, çift kademe, çantalı. Beton ve metal delme.',            N'Bosch',  'BSH-800D', 'MTK-A', 'RAF-10', 'https://placehold.co/200x200/198754/white?text=ELA-001',  45, 10, 2450.00, 1),
(@ElAleti, 'ELA-002', N'Matkap Ucu Seti 19 Parça',   N'HSS titanyum kaplama metal matkap ucu seti.',                            N'Bosch',  'BSH-2607', 'UCS-B', 'RAF-10', 'https://placehold.co/200x200/198754/white?text=ELA-002',   0, 12,  320.00, 1),
(@ElAleti, 'ELA-003', N'Çekiç 500 gr Ahşap Saplı',   N'Dövme çelik başlı marangoz çekici.',                                     N'Stanley','STN-C500', 'CKC-C', 'RAF-11', 'https://placehold.co/200x200/198754/white?text=ELA-003',  95, 20,  185.00, 1),
(@ElAleti, 'ELA-004', N'Şerit Metre 5 m',            N'Otomatik kilitli, kauçuk kaplamalı gövde.',                              N'Stanley','STN-M5',   'OLC-D', 'RAF-11', 'https://placehold.co/200x200/198754/white?text=ELA-004', 310, 50,   96.00, 1),
(@ElAleti, 'ELA-005', N'Avuç Taşlama 125 mm 900W',   N'Yan tutamaklı, disk koruma kapaklı taşlama makinesi.',                    N'Makita', 'MKT-9557', 'TSL-E', 'RAF-12', 'https://placehold.co/200x200/198754/white?text=ELA-005',  18, 20, 3180.00, 1),
(@ElAleti, 'ELA-006', N'Tornavida Seti 12 Parça',    N'Yıldız ve düz uçlu, izoleli saplı tornavida seti.',                      N'İzeltaş','IZL-T12',  'TRN-F', 'RAF-12', 'https://placehold.co/200x200/198754/white?text=ELA-006', 130, 25,  275.00, 1),
(@ElAleti, 'ELA-007', N'Pense İzoleli 180 mm 1000V', N'VDE sertifikalı, elektrikçi pensesi.',                                   N'İzeltaş','IZL-P180', 'PNS-G', 'RAF-13', NULL,                                                      62, 15,  210.00, 1),
(@ElAleti, 'ELA-008', N'Su Terazisi 60 cm',          N'Alüminyum gövde, üç gözlü.',                                             N'Stanley','STN-ST60', 'OLC-D', 'RAF-13', NULL,                                                       7, 10,  158.00, 1),

-- Hirdavat ve Baglanti ---------------------------------------------------
(@Hirdavat, 'HRD-001', N'Vida Seti 6x60 (100 adet)',   N'Galvaniz kaplı, yıldız başlı ahşap vidası.',                 N'Kalıpçı', 'KLP-660',  'VDA-A', 'RAF-20', 'https://placehold.co/200x200/fd7e14/white?text=HRD-001', 850, 100,  145.00, 1),
(@Hirdavat, 'HRD-002', N'Dübel 8 mm (200 adet)',       N'Naylon çelik dübel, standart duvar uygulamaları için.',       N'Fischer', 'FSC-D8',   'DBL-B', 'RAF-20', 'https://placehold.co/200x200/fd7e14/white?text=HRD-002', 640,  80,   98.00, 1),
(@Hirdavat, 'HRD-003', N'Somun M10 Galvaniz (50 adet)',N'DIN 934 standart altı köşe somun.',                           N'Norm',    'NRM-M10',  'SMN-C', 'RAF-21', NULL,                                                      14,  25,   62.00, 1),
(@Hirdavat, 'HRD-004', N'Çelik Dübel M8x80 (25 adet)', N'Beton ve tuğla için ağır yük çelik dübeli.',                  N'Fischer', 'FSC-M880', 'DBL-B', 'RAF-21', 'https://placehold.co/200x200/fd7e14/white?text=HRD-004', 190,  40,  210.00, 1),
(@Hirdavat, 'HRD-005', N'Menteşe Seti Kapı (2 adet)',  N'Paslanmaz çelik gömme kapı menteşesi.',                       N'Kale',    'KLE-MNT2', 'MNT-D', 'RAF-22', NULL,                                                       0,  10,  135.00, 1),
(@Hirdavat, 'HRD-006', N'Kelepçe 1/2" (10 adet)',      N'Çift vidalı boru kelepçesi, lastik contalı.',                 N'Norm',    'NRM-KLP',  'KLP-E', 'RAF-22', 'https://placehold.co/200x200/fd7e14/white?text=HRD-006', 275,  50,   84.50, 1),

-- Is Guvenligi -----------------------------------------------------------
(@Guvenlik, 'ISG-001', N'Baret Beyaz (CE Sertifikalı)', N'Ayarlanabilir kafa bandı, darbe emici iç donanım.',        N'3M',      'MMM-BRT1', 'BRT-A', 'RAF-30', 'https://placehold.co/200x200/dc3545/white?text=ISG-001', 200, 30,  185.00, 1),
(@Guvenlik, 'ISG-002', N'Güvenlik Gözlüğü Şeffaf',      N'Buğulanma önleyici kaplama, yanal korumalı.',              N'3M',      'MMM-GZL2', 'GZL-B', 'RAF-30', 'https://placehold.co/200x200/dc3545/white?text=ISG-002',  16, 20,   96.00, 1),
(@Guvenlik, 'ISG-003', N'İş Eldiveni Nitril (12 çift)', N'Kesilme dayanımlı, kaymaz avuç içi kaplama.',              N'Portwest','PRT-NTR',  'ELD-C', 'RAF-31', 'https://placehold.co/200x200/dc3545/white?text=ISG-003', 420, 60,  268.00, 1),
(@Guvenlik, 'ISG-004', N'Çelik Burunlu Ayakkabı 42',    N'S3 sınıfı, kaymaz taban, su geçirmez deri.',               N'Portwest','PRT-S342', 'AYK-D', 'RAF-31', NULL,                                                      28, 15, 1450.00, 1),
(@Guvenlik, 'ISG-005', N'Toz Maskesi FFP2 (20 adet)',   N'Ventilli, tek kullanımlık solunum koruyucu.',              N'3M',      'MMM-FFP2', 'MSK-E', 'RAF-32', 'https://placehold.co/200x200/dc3545/white?text=ISG-005',   0, 25,  340.00, 1),
(@Guvenlik, 'ISG-006', N'Emniyet Kemeri Paraşüt Tipi',  N'Tam vücut tipi, çift kancalı yüksekte çalışma kemeri.',    N'Petzl',   'PTZ-PRS1', 'KMR-F', 'RAF-32', NULL,                                                      12, 10, 3850.00, 1),

-- Boya ve Yalitim --------------------------------------------------------
(@Boya, 'BOY-001', N'Silikonlu İç Cephe Boyası 15 L', N'Silinebilir, mat görünümlü su bazlı iç cephe boyası.', N'Filli Boya','FLB-IC15', 'BYA-A', 'RAF-40', 'https://placehold.co/200x200/6f42c1/white?text=BOY-001',  85, 20, 1890.00, 1),
(@Boya, 'BOY-002', N'Dış Cephe Astarı 10 L',          N'Şeffaf akrilik esaslı yüzey güçlendirici astar.',       N'Marshall',  'MRS-AST10','BYA-A', 'RAF-40', NULL,                                                      40, 15,  980.00, 1),
(@Boya, 'BOY-003', N'Poliüretan Mastik 310 ml',       N'Derz ve birleşim yerleri için elastik dolgu mastiği.',  N'Soudal',    'SDL-PU310','MST-B', 'RAF-41', 'https://placehold.co/200x200/6f42c1/white?text=BOY-003', 380, 50,  148.00, 1),
(@Boya, 'BOY-004', N'Alçıpan Derz Bandı 90 m',        N'Fileli, kendinden yapışkanlı derz bandı.',              N'Knauf',     'KNF-DB90', 'BND-C', 'RAF-41', NULL,                                                       9, 15,   76.00, 1),
(@Boya, 'BOY-005', N'Rulo Fırça Seti 25 cm',          N'Teleskopik saplı, yedek rulo dahil boya seti.',         N'Marshall',  'MRS-RL25', 'FRC-D', 'RAF-42', 'https://placehold.co/200x200/6f42c1/white?text=BOY-005', 165, 30,  210.00, 1),
(@Boya, 'BOY-006', N'Su Yalıtım Membranı 10 m²',      N'Sürme esaslı, çift bileşenli su yalıtım malzemesi.',    N'Sika',      'SKA-MB10', 'YLT-E', 'RAF-42', NULL,                                                       0, 10, 2340.00, 1),

-- Su Tesisati ------------------------------------------------------------
(@Tesisat, 'TSS-001', N'PPRC Boru 25 mm (4 m)',      N'Sıcak ve soğuk su tesisatı için polipropilen boru.',     N'Firat',   'FRT-PP25', 'BRU-A', 'RAF-50', 'https://placehold.co/200x200/0dcaf0/white?text=TSS-001', 520, 80,  145.00, 1),
(@Tesisat, 'TSS-002', N'Küresel Vana 1/2" Pirinç',   N'Tam geçişli, kelebek kollu pirinç vana.',                N'Ekoplas', 'EKO-KV12', 'VNA-B', 'RAF-50', 'https://placehold.co/200x200/0dcaf0/white?text=TSS-002', 240, 40,  168.00, 1),
(@Tesisat, 'TSS-003', N'Dirsek 90° 25 mm (10 adet)', N'PPRC kaynak tipi dirsek bağlantı parçası.',              N'Firat',   'FRT-DRS25','EKL-C', 'RAF-51', NULL,                                                      18, 25,   92.00, 1),
(@Tesisat, 'TSS-004', N'Lavabo Bataryası Krom',      N'Seramik kartuşlu, tek kollu lavabo bataryası.',          N'Eca',     'ECA-LVB1', 'BTR-D', 'RAF-51', 'https://placehold.co/200x200/0dcaf0/white?text=TSS-004',  36, 10,  985.00, 1),
(@Tesisat, 'TSS-005', N'Sifon Seti Esnek',           N'Körüklü, ayarlanabilir lavabo sifonu.',                  N'Eca',     'ECA-SFN1', 'SFN-E', 'RAF-52', NULL,                                                       0, 15,  118.00, 1),
(@Tesisat, 'TSS-006', N'Teflon Bant 12 mm (10 adet)',N'Dişli bağlantılar için sızdırmazlık bandı.',             N'Ekoplas', 'EKO-TFL',  'BND-F', 'RAF-52', 'https://placehold.co/200x200/0dcaf0/white?text=TSS-006', 900,100,   45.00, 1),

-- Aydinlatma -------------------------------------------------------------
(@Aydinlat, 'AYD-001', N'LED Panel 60x60 40W',        N'Sıva altı, 4000K doğal beyaz ofis aydınlatma paneli.', N'Philips','PHL-P6040','LED-A', 'RAF-60', 'https://placehold.co/200x200/ffc107/black?text=AYD-001', 145, 25,  480.00, 1),
(@Aydinlat, 'AYD-002', N'LED Ampul E27 9W (5 adet)',  N'6500K beyaz ışık, enerji tasarruflu ampul.',           N'Osram',  'OSR-E279', 'LED-A', 'RAF-60', 'https://placehold.co/200x200/ffc107/black?text=AYD-002', 680, 80,  165.00, 1),
(@Aydinlat, 'AYD-003', N'Projektör LED 100W IP65',    N'Dış mekân, su geçirmez şantiye projektörü.',           N'Philips','PHL-PR100','PRJ-B', 'RAF-61', 'https://placehold.co/200x200/ffc107/black?text=AYD-003',  22, 30,  890.00, 1),
(@Aydinlat, 'AYD-004', N'Acil Aydınlatma Armatürü',   N'Kesintide 3 saat çalışan şarjlı acil çıkış armatürü.', N'Arsel',  'ARS-AC01', 'ACL-C', 'RAF-61', NULL,                                                       55, 20,  345.00, 1),
(@Aydinlat, 'AYD-005', N'Seyyar Lamba 10 m Kablolu',  N'Kancalı, kırılmaz muhafazalı seyyar çalışma lambası.', N'Arsel',  'ARS-SY10', 'SYL-D', 'RAF-62', NULL,                                                        6, 10,  265.00, 1),

-- Bahce ve Tarim ---------------------------------------------------------
(@Bahce, 'BHC-001', N'Bahçe Hortumu 1/2" (25 m)',  N'Üç katmanlı, burulma yapmayan sulama hortumu.',      N'Gardena','GRD-H25',  'HRT-A', 'RAF-70', 'https://placehold.co/200x200/20c997/white?text=BHC-001', 120, 20,  740.00, 1),
(@Bahce, 'BHC-002', N'Budama Makası Profesyonel',   N'Karbon çelik ağızlı, teflon kaplamalı budama makası.',N'Gardena','GRD-BM1',  'MKS-B', 'RAF-70', NULL,                                                       48, 15,  420.00, 1),
(@Bahce, 'BHC-003', N'Sırt Pülverizatörü 16 L',     N'Basınç ayarlı, omuz askılı tarımsal ilaçlama pompası.',N'Kwazar','KWZ-16L',  'PLV-C', 'RAF-71', 'https://placehold.co/200x200/20c997/white?text=BHC-003',   3,  8, 1280.00, 1),

-- Pasif urunler (satista degil; musteri ekraninda gorunmez) --------------
(@Elektrik, 'ELK-901', N'Eski Model Sigorta Kutusu',  N'Üretimi durdurulan model. Yeni projelerde kullanılmaz.', N'Mutlusan','MTL-ESK1','ARV-Z','DEPO', NULL, 12, 10,  240.00, 0),
(@ElAleti,  'ELA-901', N'Manuel Vidalama Seti (Arşiv)',N'Stok temizliği kapsamında satıştan kaldırıldı.',        N'İzeltaş', 'IZL-ESK', 'ARV-Z','DEPO', NULL,  0, 10,   88.00, 0),
(@Boya,     'BOY-901', N'Solvent Bazlı Vernik 2.5 L', N'Mevzuat değişikliği nedeniyle satışı durduruldu.',       N'Marshall','MRS-ESK', 'ARV-Z','DEPO', NULL, 25, 10,  560.00, 0);
GO

/* ------------------------------------------------------------
   3. Musteri kullanicilari
   Tum sifreler: Bayi123!
   Hash'ler ASP.NET Core PasswordHasher (PBKDF2-HMACSHA512,
   100.000 iterasyon) formatindadir; acik metin sifre saklanmaz.
   ------------------------------------------------------------ */
INSERT INTO dbo.Users (FirstName, LastName, Email, UserName, Phone, PasswordHash, Role, IsActive, CreatedAt) VALUES
(N'Ahmet',  N'Yılmaz',   N'ahmet.yilmaz@yilmazelektrik.com',  N'ayilmaz',  '05321112233',
 'AQAAAAIAAYagAAAAEIi3VgnUzPDtJ63uCRQolBJcykbCrVEDqediwgo1ZayY5hT3DISmiIZbiI77v9iBsw==', 'Customer', 1, DATEADD(DAY, -120, SYSUTCDATETIME())),
(N'Ayşe',   N'Demir',    N'ayse.demir@demiryapi.com.tr',      N'ademir',   '05334445566',
 'AQAAAAIAAYagAAAAEP8A7eMZ8gXefckQt3ZRybmqJGu3jeoaMGpeAiPFUaJshiu8ctNG1gJdiqtilmSsbg==', 'Customer', 1, DATEADD(DAY,  -95, SYSUTCDATETIME())),
(N'Mehmet', N'Kaya',     N'mehmet.kaya@kayahirdavat.com',     N'mkaya',    '05427778899',
 'AQAAAAIAAYagAAAAED0IZV0FmcRdOpnyYJYx4bPBnEiQ+37Dl4pHmmQUayobrNA1Bwnt1rmn1KOi8pJJoQ==', 'Customer', 1, DATEADD(DAY,  -60, SYSUTCDATETIME())),
(N'Zeynep', N'Şahin',    N'zeynep.sahin@sahinteknik.com',     N'zsahin',   '05051234567',
 'AQAAAAIAAYagAAAAEMbF5HQDOkoqAS/SOFiewNo7GQX9FweflQJ5/9ViDM07lXljT+O+o8FLa1NQkNB1cQ==', 'Customer', 1, DATEADD(DAY,  -30, SYSUTCDATETIME())),
(N'Can',    N'Öztürk',   N'can.ozturk@ozturkinsaat.com',      N'cozturk',  '05559876543',
 'AQAAAAIAAYagAAAAEGlnyYxQ28Ad1vacmo+4qGhSvgU8DaRYVh2Z2xwn0yrHVolXk37BG5hS3ViKTt3Xeg==', 'Customer', 1, DATEADD(DAY,  -12, SYSUTCDATETIME())),
-- Pasif hesap: giris yapamaz ("Hesabiniz pasif durumdadir" mesajini test etmek icin)
(N'Elif',   N'Çelik',    N'elif.celik@celikticaret.com',      N'ecelik',   '05308765432',
 'AQAAAAIAAYagAAAAECVzLrY+lOC2c3le+L2i92RpKZjvOJdnrcAA4+kkfojWoz3uGVm75R9tUIi2Wvi7jg==', 'Customer', 0, DATEADD(DAY,  -75, SYSUTCDATETIME()));
GO

/* ------------------------------------------------------------
   4. Ana sayfa slider'lari
   ------------------------------------------------------------ */
INSERT INTO dbo.Sliders (Title, ImageUrl, LinkUrl, DisplayOrder, IsActive) VALUES
(N'Elektrik malzemelerinde bayi fiyatları',      'https://placehold.co/1200x320/0d6efd/white?text=Bayilere+Ozel+Elektrik+Kampanyasi', '/Products', 1, 1),
(N'İş güvenliği ürünlerinde stok yenilendi',     'https://placehold.co/1200x320/dc3545/white?text=Is+Guvenligi+Urunleri',             '/Products', 2, 1),
(N'Yeni sezon el aletleri kataloğu yayında',     'https://placehold.co/1200x320/198754/white?text=El+Aletleri+Katalogu',              NULL,        3, 1),
(N'Geçmiş kampanya (yayında değil)',             'https://placehold.co/1200x320/6c757d/white?text=Gecmis+Kampanya',                   NULL,        4, 0);
GO

/* ------------------------------------------------------------
   5. Dolu sepetler
   Ahmet Yilmaz: normal sepet.
   Mehmet Kaya : stogu yetersiz kalmis urun icerir -> sepet ekraninda
                 kirmizi uyari cikar ve "Siparis Olustur" pasif olur.
   ------------------------------------------------------------ */
DECLARE @CartAhmet INT, @CartMehmet INT;

INSERT INTO dbo.Carts (UserId) SELECT Id FROM dbo.Users WHERE UserName = N'ayilmaz';
SET @CartAhmet = SCOPE_IDENTITY();

INSERT INTO dbo.CartItems (CartId, ProductId, Quantity)
SELECT @CartAhmet, Id, 4  FROM dbo.Products WHERE ProductCode = 'ELK-004'
UNION ALL SELECT @CartAhmet, Id, 2  FROM dbo.Products WHERE ProductCode = 'ELA-004'
UNION ALL SELECT @CartAhmet, Id, 10 FROM dbo.Products WHERE ProductCode = 'HRD-001';

INSERT INTO dbo.Carts (UserId) SELECT Id FROM dbo.Users WHERE UserName = N'mkaya';
SET @CartMehmet = SCOPE_IDENTITY();

INSERT INTO dbo.CartItems (CartId, ProductId, Quantity)
SELECT @CartMehmet, Id, 3  FROM dbo.Products WHERE ProductCode = 'TSS-001'
UNION ALL SELECT @CartMehmet, Id, 25 FROM dbo.Products WHERE ProductCode = 'ELK-002';  -- stok 8, talep 25 -> yetersiz
GO

/* ------------------------------------------------------------
   6. Gecmis siparisler
   OrderItems'a urun kodu/adi/fiyati siparis anindaki haliyle yazilir.
   ELK-001 siparislerinde bilerek eski fiyat (2980.00) kullanildi;
   urunun guncel fiyati 3250.00. Boylece fiyat korumasi (snapshot)
   siparis detayinda gozle dogrulanabilir.
   ------------------------------------------------------------ */
DECLARE @OrderId INT, @UserId INT;

-- 1) Ahmet Yilmaz - Onaylandi (45 gun once, eski fiyatli)
SELECT @UserId = Id FROM dbo.Users WHERE UserName = N'ayilmaz';
INSERT INTO dbo.Orders (UserId, OrderDate, Status, TotalAmount)
VALUES (@UserId, DATEADD(DAY, -45, SYSUTCDATETIME()), 1, 0);
SET @OrderId = SCOPE_IDENTITY();
INSERT INTO dbo.OrderItems (OrderId, ProductId, ProductCode, ProductName, Quantity, UnitPrice)
SELECT @OrderId, Id, ProductCode, Name, 2, 2980.00 FROM dbo.Products WHERE ProductCode = 'ELK-001'
UNION ALL SELECT @OrderId, Id, ProductCode, Name, 5,   68.00 FROM dbo.Products WHERE ProductCode = 'ELK-004'
UNION ALL SELECT @OrderId, Id, ProductCode, Name, 3,  180.00 FROM dbo.Products WHERE ProductCode = 'ELA-003';

-- 2) Ahmet Yilmaz - Onaylandi (20 gun once)
INSERT INTO dbo.Orders (UserId, OrderDate, Status, TotalAmount)
VALUES (@UserId, DATEADD(DAY, -20, SYSUTCDATETIME()), 1, 0);
SET @OrderId = SCOPE_IDENTITY();
INSERT INTO dbo.OrderItems (OrderId, ProductId, ProductCode, ProductName, Quantity, UnitPrice)
SELECT @OrderId, Id, ProductCode, Name, 10, 145.00 FROM dbo.Products WHERE ProductCode = 'HRD-001'
UNION ALL SELECT @OrderId, Id, ProductCode, Name, 6,  98.00 FROM dbo.Products WHERE ProductCode = 'HRD-002';

-- 3) Ahmet Yilmaz - Beklemede (2 gun once)
INSERT INTO dbo.Orders (UserId, OrderDate, Status, TotalAmount)
VALUES (@UserId, DATEADD(DAY, -2, SYSUTCDATETIME()), 0, 0);
SET @OrderId = SCOPE_IDENTITY();
INSERT INTO dbo.OrderItems (OrderId, ProductId, ProductCode, ProductName, Quantity, UnitPrice)
SELECT @OrderId, Id, ProductCode, Name, 1, 2450.00 FROM dbo.Products WHERE ProductCode = 'ELA-001'
UNION ALL SELECT @OrderId, Id, ProductCode, Name, 2, 275.00 FROM dbo.Products WHERE ProductCode = 'ELA-006';

-- 4) Ayse Demir - Onaylandi (38 gun once)
SELECT @UserId = Id FROM dbo.Users WHERE UserName = N'ademir';
INSERT INTO dbo.Orders (UserId, OrderDate, Status, TotalAmount)
VALUES (@UserId, DATEADD(DAY, -38, SYSUTCDATETIME()), 1, 0);
SET @OrderId = SCOPE_IDENTITY();
INSERT INTO dbo.OrderItems (OrderId, ProductId, ProductCode, ProductName, Quantity, UnitPrice)
SELECT @OrderId, Id, ProductCode, Name, 4, 1890.00 FROM dbo.Products WHERE ProductCode = 'BOY-001'
UNION ALL SELECT @OrderId, Id, ProductCode, Name, 8, 148.00 FROM dbo.Products WHERE ProductCode = 'BOY-003'
UNION ALL SELECT @OrderId, Id, ProductCode, Name, 2, 210.00 FROM dbo.Products WHERE ProductCode = 'BOY-005';

-- 5) Ayse Demir - Reddedildi (15 gun once)
INSERT INTO dbo.Orders (UserId, OrderDate, Status, TotalAmount)
VALUES (@UserId, DATEADD(DAY, -15, SYSUTCDATETIME()), 2, 0);
SET @OrderId = SCOPE_IDENTITY();
INSERT INTO dbo.OrderItems (OrderId, ProductId, ProductCode, ProductName, Quantity, UnitPrice)
SELECT @OrderId, Id, ProductCode, Name, 3, 2340.00 FROM dbo.Products WHERE ProductCode = 'BOY-006';

-- 6) Ayse Demir - Beklemede (dun)
INSERT INTO dbo.Orders (UserId, OrderDate, Status, TotalAmount)
VALUES (@UserId, DATEADD(DAY, -1, SYSUTCDATETIME()), 0, 0);
SET @OrderId = SCOPE_IDENTITY();
INSERT INTO dbo.OrderItems (OrderId, ProductId, ProductCode, ProductName, Quantity, UnitPrice)
SELECT @OrderId, Id, ProductCode, Name, 12, 268.00 FROM dbo.Products WHERE ProductCode = 'ISG-003'
UNION ALL SELECT @OrderId, Id, ProductCode, Name, 5, 185.00 FROM dbo.Products WHERE ProductCode = 'ISG-001';

-- 7) Mehmet Kaya - Onaylandi (25 gun once)
SELECT @UserId = Id FROM dbo.Users WHERE UserName = N'mkaya';
INSERT INTO dbo.Orders (UserId, OrderDate, Status, TotalAmount)
VALUES (@UserId, DATEADD(DAY, -25, SYSUTCDATETIME()), 1, 0);
SET @OrderId = SCOPE_IDENTITY();
INSERT INTO dbo.OrderItems (OrderId, ProductId, ProductCode, ProductName, Quantity, UnitPrice)
SELECT @OrderId, Id, ProductCode, Name, 20, 145.00 FROM dbo.Products WHERE ProductCode = 'TSS-001'
UNION ALL SELECT @OrderId, Id, ProductCode, Name, 10, 168.00 FROM dbo.Products WHERE ProductCode = 'TSS-002'
UNION ALL SELECT @OrderId, Id, ProductCode, Name, 30,  45.00 FROM dbo.Products WHERE ProductCode = 'TSS-006';

-- 8) Zeynep Sahin - Beklemede (5 gun once)
SELECT @UserId = Id FROM dbo.Users WHERE UserName = N'zsahin';
INSERT INTO dbo.Orders (UserId, OrderDate, Status, TotalAmount)
VALUES (@UserId, DATEADD(DAY, -5, SYSUTCDATETIME()), 0, 0);
SET @OrderId = SCOPE_IDENTITY();
INSERT INTO dbo.OrderItems (OrderId, ProductId, ProductCode, ProductName, Quantity, UnitPrice)
SELECT @OrderId, Id, ProductCode, Name, 6, 480.00 FROM dbo.Products WHERE ProductCode = 'AYD-001'
UNION ALL SELECT @OrderId, Id, ProductCode, Name, 4, 165.00 FROM dbo.Products WHERE ProductCode = 'AYD-002';

-- 9) Can Ozturk - Beklemede (bugun)
SELECT @UserId = Id FROM dbo.Users WHERE UserName = N'cozturk';
INSERT INTO dbo.Orders (UserId, OrderDate, Status, TotalAmount)
VALUES (@UserId, DATEADD(HOUR, -3, SYSUTCDATETIME()), 0, 0);
SET @OrderId = SCOPE_IDENTITY();
INSERT INTO dbo.OrderItems (OrderId, ProductId, ProductCode, ProductName, Quantity, UnitPrice)
SELECT @OrderId, Id, ProductCode, Name, 2, 3180.00 FROM dbo.Products WHERE ProductCode = 'ELA-005'
UNION ALL SELECT @OrderId, Id, ProductCode, Name, 1, 1450.00 FROM dbo.Products WHERE ProductCode = 'ISG-004'
UNION ALL SELECT @OrderId, Id, ProductCode, Name, 3,  210.00 FROM dbo.Products WHERE ProductCode = 'ELA-006';
GO

-- Siparis toplamlarini kalemlerden hesapla (TotalPrice computed column'dur)
UPDATE o
SET o.TotalAmount = ISNULL(i.Toplam, 0)
FROM dbo.Orders AS o
OUTER APPLY (SELECT SUM(oi.TotalPrice) AS Toplam
             FROM dbo.OrderItems AS oi
             WHERE oi.OrderId = o.Id) AS i;
GO

/* ============================================================
   7. Dogrulama sorgulari
   ============================================================ */
PRINT '--- Ozet ---';

SELECT 'Kategori' AS Tablo, COUNT(*) AS Adet FROM dbo.Categories
UNION ALL SELECT 'Ürün (aktif)',  COUNT(*) FROM dbo.Products WHERE IsActive = 1
UNION ALL SELECT 'Ürün (pasif)',  COUNT(*) FROM dbo.Products WHERE IsActive = 0
UNION ALL SELECT 'Kullanıcı',     COUNT(*) FROM dbo.Users
UNION ALL SELECT 'Slider',        COUNT(*) FROM dbo.Sliders
UNION ALL SELECT 'Sepet satırı',  COUNT(*) FROM dbo.CartItems
UNION ALL SELECT 'Sipariş',       COUNT(*) FROM dbo.Orders
UNION ALL SELECT 'Sipariş kalemi',COUNT(*) FROM dbo.OrderItems;

-- Stok durumu dagilimi (uygulamadaki CASE WHEN mantiginin aynisi)
SELECT
    CASE WHEN StockQuantity <= 0                  THEN N'Yok'
         WHEN StockQuantity <= CriticalStockLevel THEN N'Kritik'
         ELSE N'Var' END AS StokDurumu,
    COUNT(*) AS UrunSayisi
FROM dbo.Products
WHERE IsActive = 1
GROUP BY CASE WHEN StockQuantity <= 0                  THEN N'Yok'
              WHEN StockQuantity <= CriticalStockLevel THEN N'Kritik'
              ELSE N'Var' END;

-- Fiyat korumasi (snapshot) kontrolu: siparisteki fiyat ile guncel fiyat farkli
SELECT TOP 5
    o.OrderNumber,
    oi.ProductCode,
    oi.UnitPrice AS SiparisAnindakiFiyat,
    p.Price      AS GuncelFiyat
FROM dbo.OrderItems AS oi
INNER JOIN dbo.Orders   AS o ON o.Id = oi.OrderId
INNER JOIN dbo.Products AS p ON p.Id = oi.ProductId
WHERE oi.UnitPrice <> p.Price
ORDER BY o.OrderNumber;

-- Siparis durumlarina gore dagilim
SELECT
    CASE Status WHEN 0 THEN N'Beklemede' WHEN 1 THEN N'Onaylandı' ELSE N'Reddedildi' END AS Durum,
    COUNT(*)          AS SiparisSayisi,
    SUM(TotalAmount)  AS ToplamTutar
FROM dbo.Orders
GROUP BY Status;

PRINT 'Ornek veriler yuklendi. Musteri sifreleri: Bayi123!';
GO

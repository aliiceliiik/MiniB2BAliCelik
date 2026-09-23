# Mini B2B E-Ticaret Projesi

B2B bayi sipariş süreçlerini uçtan uca kapsayan mini e-ticaret uygulaması. Kullanıcı arayüzü ve yönetim paneli tek bir ASP.NET Core MVC uygulaması içinde sunulur.

## Kullanılan Teknolojiler

- .NET 9 / ASP.NET Core MVC
- Entity Framework Core 9 (SQL Server provider)
- SQL Server (LocalDB / SQL Server Express / SQL Server)
- Cookie Authentication + ASP.NET Core Identity `PasswordHasher` (PBKDF2-HMACSHA512)
- Bootstrap 5, vanilla JavaScript (fetch API)

## Mimari

Katmanlı (N-Tier) mimari. Bağımlılık yönü tek yönlüdür:

**MiniB2B.Web → MiniB2B.Business → MiniB2B.DataAccess → MiniB2B.Entities**

| Katman | Sorumluluk |
|---|---|
| `MiniB2B.Entities` | Entity sınıfları, DTO'lar, enum'lar. Hiçbir katmana bağımlı değil. |
| `MiniB2B.DataAccess` | DbContext, Fluent API eşlemeleri, repository'ler, transaction yönetimi. SQL ile konuşan tek katman. |
| `MiniB2B.Business` | Servisler, iş kuralları, validasyon, şifre hash'leme, admin seed. |
| `MiniB2B.Web` | MVC controller'ları, REST API endpoint'leri, view'lar, Admin area, dinamik grid render altyapısı. |

Web katmanı DataAccess'i referans almaz; controller'lar DbContext'e doğrudan erişemez.

## Kurulum

### 1. Veritabanı

SSMS veya Azure Data Studio ile sırasıyla çalıştırın:

1. `database/01_CreateDatabase.sql` — veritabanı, tablolar, constraint'ler, indeksler, sequence ve grid kolon konfigürasyonu
2. `database/02_SampleData.sql` — örnek test verileri (kategoriler, ürünler, müşteriler, slider'lar, sepetler, geçmiş siparişler)

İkinci script tekrar tekrar çalıştırılabilir; her çalıştırmada örnek veriyi sıfırlayıp yeniden yükler. Yönetici hesabına ve grid konfigürasyonuna dokunmaz.

Veritabanı `Turkish_CI_AS` collation ile oluşturulur. Aramalarda büyük/küçük harf ve Türkçe karakter duyarsızlığı bu sayede sağlanır.

### 2. Bağlantı ayarları

`src/MiniB2B.Web/appsettings.json` içindeki bağlantı bilgisini kendi ortamınıza göre düzenleyin:

```json
"ConnectionStrings": {
  "MiniB2B": "Server=(localdb)\\MSSQLLocalDB;Database=MiniB2B;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

- SQL Server Express için: `Server=.\\SQLEXPRESS`
- Yerel tam kurulum için: `Server=localhost`

### 3. Çalıştırma

```bash
dotnet restore
dotnet run --project src/MiniB2B.Web
```

Terminalde yazan adresi (`https://localhost:xxxx`) tarayıcıda açın.

## Varsayılan Kullanıcılar

### Yönetici

Uygulama ilk çalıştığında, sistemde hiç yönetici yoksa `appsettings.Development.json` içindeki `AdminSeed` bölümüne göre bir yönetici hesabı otomatik oluşturulur:

| Kullanıcı adı | Şifre |
|---|---|
| `admin` | `Admin123!` |

Şifre veritabanına hash'lenerek kaydedilir; açık metin saklanmaz. Bu işlem yalnızca sistemde hiç yönetici yokken çalışır.

### Müşteriler (örnek veri scriptinden)

Tümünün şifresi: **`Bayi123!`**

| Kullanıcı adı | Ad Soyad | Durum |
|---|---|---|
| `ayilmaz` | Ahmet Yılmaz | Aktif, 3 siparişi ve dolu sepeti var |
| `ademir` | Ayşe Demir | Aktif, 3 siparişi var |
| `mkaya` | Mehmet Kaya | Aktif, stoğu yetersiz ürün içeren sepeti var |
| `zsahin` | Zeynep Şahin | Aktif |
| `cozturk` | Can Öztürk | Aktif |
| `ecelik` | Elif Çelik | **Pasif** (giriş engellenir) |

Giriş kullanıcı adı veya e-posta ile yapılabilir. Yeni müşteri hesabı için uygulamadaki "Kayıt Ol" ekranı kullanılabilir.

## Örnek Veri Hakkında

Örnek veri, iş kurallarını gözle doğrulayacak şekilde hazırlandı:

- **48 ürün, 8 kategori.** Stok durumları Var / Kritik / Yok olarak dağıtıldı; 3 ürün pasif (müşteri ekranında görünmez, admin panelinde gri satır olarak listelenir).
- **Fiyat koruması (snapshot).** `ELK-001` ürünü siparişlere 2.980,00 ₺ ile kaydedildi; ürünün güncel fiyatı 3.250,00 ₺. Sipariş detayı eski fiyatı göstermeye devam eder.
- **Sepet stok uyarısı.** `mkaya` kullanıcısının sepetinde stoğu yetersiz bir ürün var; sepet ekranında kırmızı uyarı çıkar ve "Sipariş Oluştur" butonu devre dışı kalır.
- **Sipariş durumları.** Beklemede, Onaylandı ve Reddedildi durumlarının her birinden örnek sipariş bulunur.
- **Arama testi.** Ürünler açıklama ve özel kod alanlarıyla birlikte dolduruldu; `KBL-A`, `RAF-01`, `Öznur` gibi aramalar metinsel tüm kolonlarda arama yapıldığını gösterir.

Ürün ve slider görselleri `placehold.co` üzerinden yüklenir; ilk açılışta internet bağlantısı gerekir. Panelden yüklenen görseller `wwwroot/uploads/` altına kaydedilir ve repoya dahil edilmez.

## Öne Çıkan Teknik Tercihler

**Dinamik grid.** Ürün listesindeki kolonlar `GridColumnConfigs` tablosundan okunur: hangi alan, hangi sırada, hangi render tipiyle, hangi genişlik ve hizalamayla, hangi cihazda (masaüstü/tablet/telefon) görüneceği veritabanından yönetilir. Değerler reflection ile dinamik okunur (sonuç `ConcurrentDictionary` ile önbelleklenir), hücreler render tipine göre Strategy Pattern ile çizilir. Kolon düzenini değiştirmek için kod değişikliği gerekmez:

```sql
-- Marka kolonunu gizle, fiyatı en başa al
UPDATE dbo.GridColumnConfigs SET IsVisible = 0 WHERE FieldName = 'Brand';
UPDATE dbo.GridColumnConfigs SET DisplayOrder = 0 WHERE FieldName = 'Price';
```

**Görsel stok göstergesi.** Stok ham sayı olarak değil, ürün bazlı `CriticalStockLevel` eşiğine göre Var (yeşil) / Kritik (sarı) / Yok (kırmızı) olarak gösterilir. Hesaplama SQL'de `CASE WHEN` ile yapılır; ham stok değeri müşteri ürün listesine hiç gönderilmez.

**Sipariş oluşturma.** Tek bir transaction içinde: stok ön kontrolü, atomik stok düşümü (`UPDATE ... WHERE StockQuantity >= @qty`), sipariş kaydı ve sepetin temizlenmesi. Eşzamanlı siparişlerde stoğun eksiye düşmesi mümkün değildir; deadlock riskine karşı ürünler her zaman aynı sırayla kilitlenir. Silinen sepet satırı sayısı doğrulanarak çift sipariş engellenir. Veritabanındaki `CHECK (StockQuantity >= 0)` son savunma hattı olarak durur.

**Fiyat koruması (snapshot).** `OrderItems` tablosuna sipariş anındaki ürün kodu, adı ve birim fiyatı kopyalanır. Ürün fiyatı sonradan değişse de geçmiş siparişler sipariş anındaki tutarla görüntülenir. Satır toplamı veritabanında computed column olarak hesaplanır.

**SQL performansı.** Tüm filtreleme, sıralama ve sayfalama `IQueryable` üzerinden veritabanı seviyesinde çalışır; hiçbir listede tüm kayıtlar belleğe alınmaz. Sorgular entity değil DTO projeksiyonu döndürür, yalnızca gerekli kolonlar çekilir. Sık kullanılan sorgular için indeksler tanımlıdır (`IX_Orders_UserId_OrderDate` covering index dahil). Sipariş reddedildiğinde stok iadesi tek bir set-based `UPDATE ... FROM ... JOIN` cümlesiyle yapılır.

**Güvenlik.** Şifreler PBKDF2 ile hash'lenir. Cookie `HttpOnly` ve `SameSite=Lax`; veri değiştiren tüm isteklerde anti-forgery token doğrulanır. Tüm controller'lar varsayılan olarak yetkilendirme ister (secure by default), herkese açık sayfalar `[AllowAnonymous]` ile açılır; admin ekranları rol bazlı korunur. Kullanıcı Id'si her zaman cookie'den okunur, hiçbir zaman istekten alınmaz. Oturumlar dakikada bir doğrulanır; pasife alınan veya rolü değiştirilen kullanıcı en geç bir dakika içinde oturumdan düşer. Yüklenen dosyalar imza (magic bytes) kontrolünden geçer ve GUID adıyla saklanır.

## İş Kuralı Tercihleri

- Ürünler silinmez, pasife alınır (soft delete). Geçmiş siparişlerin bütünlüğü korunur.
- Sepete aynı ürün tekrar eklenirse yeni satır açılmaz, mevcut satırın adedi artar (`UNIQUE(CartId, ProductId)`).
- Sipariş durumu yalnızca "Beklemede" iken değiştirilebilir. Sonuçlanmış sipariş tekrar değiştirilemez.
- Sipariş reddedildiğinde ürünler stoğa iade edilir.
- Bir yönetici kendi rolünü değiştiremez ve kendi hesabını pasife alamaz; sistemde her zaman en az bir aktif yönetici kalır.

## Olası İyileştirmeler

- Ürün güncellemede `ROWVERSION` ile iyimser eşzamanlılık kontrolü (admin stok güncellemesi ile eşzamanlı sipariş çakışması).
- Değiştirilen veya kullanılmayan yüklenmiş görsellerin diskten temizlenmesi.
- Grid konfigürasyonunun önbelleğe alınması (in-memory cache veya Redis).
- Oturum doğrulaması için kullanıcı tablosunda `SecurityStamp` kolonu.

## Proje Yapısı

```
MiniB2B/
├── database/
│   ├── 01_CreateDatabase.sql
│   └── 02_SampleData.sql
├── src/
│   ├── MiniB2B.Entities/
│   ├── MiniB2B.DataAccess/
│   ├── MiniB2B.Business/
│   └── MiniB2B.Web/
├── MiniB2B.sln
└── README.md
```

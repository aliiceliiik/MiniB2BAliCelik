/* ============================================================
   Mini B2B E-Ticaret Projesi - Veritabani Olusturma Scripti
   ------------------------------------------------------------
   Calistirma sirasi:
     1) 01_CreateDatabase.sql  (bu dosya)  -> sema + grid konfigurasyonu
     2) 02_SampleData.sql                  -> ornek test verileri
   ============================================================ */

IF DB_ID('MiniB2B') IS NOT NULL
BEGIN
    PRINT 'MiniB2B veritabani zaten mevcut. Yeniden olusturmak icin once DROP DATABASE calistirin.';
END
ELSE
BEGIN
    -- Turkish_CI_AS: aramalarda buyuk/kucuk harf ve Turkce karakter duyarsizligi
    CREATE DATABASE MiniB2B COLLATE Turkish_CI_AS;
END
GO

USE MiniB2B;
GO

/* ------------------------------------------------------------
   1. Kullanicilar
   Sifreler PBKDF2 ile hash'lenerek saklanir, acik metin tutulmaz.
   ------------------------------------------------------------ */
CREATE TABLE dbo.Users (
    Id           INT IDENTITY(1,1) NOT NULL,
    FirstName    NVARCHAR(50)  NOT NULL,
    LastName     NVARCHAR(50)  NOT NULL,
    Email        NVARCHAR(150) NOT NULL,
    UserName     NVARCHAR(50)  NOT NULL,
    Phone        VARCHAR(20)   NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    Role         VARCHAR(20)   NOT NULL CONSTRAINT DF_Users_Role      DEFAULT ('Customer'),
    IsActive     BIT           NOT NULL CONSTRAINT DF_Users_IsActive  DEFAULT (1),
    CreatedAt    DATETIME2(0)  NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT PK_Users            PRIMARY KEY (Id),
    CONSTRAINT UQ_Users_Email      UNIQUE (Email),
    CONSTRAINT UQ_Users_UserName   UNIQUE (UserName),
    CONSTRAINT CK_Users_Role       CHECK (Role IN ('Admin','Customer'))
);
GO

/* ------------------------------------------------------------
   2. Kategoriler
   ------------------------------------------------------------ */
CREATE TABLE dbo.Categories (
    Id       INT IDENTITY(1,1) NOT NULL,
    Name     NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Categories_IsActive DEFAULT (1),
    CONSTRAINT PK_Categories      PRIMARY KEY (Id),
    CONSTRAINT UQ_Categories_Name UNIQUE (Name)
);
GO

/* ------------------------------------------------------------
   3. Urunler
   CriticalStockLevel urun bazindadir (sabit esik degil).
   CK_Products_Stock: stogun eksiye dusmesine karsi son savunma hatti.
   ------------------------------------------------------------ */
CREATE TABLE dbo.Products (
    Id                 INT IDENTITY(1,1) NOT NULL,
    CategoryId         INT            NOT NULL,
    ProductCode        NVARCHAR(50)   NOT NULL,
    Name               NVARCHAR(200)  NOT NULL,
    Description        NVARCHAR(2000) NULL,
    Brand              NVARCHAR(100)  NOT NULL,
    ManufacturerCode   NVARCHAR(50)   NULL,
    SpecialCode1       NVARCHAR(50)   NULL,
    SpecialCode2       NVARCHAR(50)   NULL,
    ImageUrl           NVARCHAR(500)  NULL,
    StockQuantity      INT            NOT NULL CONSTRAINT DF_Products_Stock     DEFAULT (0),
    CriticalStockLevel INT            NOT NULL CONSTRAINT DF_Products_Critical  DEFAULT (10),
    Price              DECIMAL(18,2)  NOT NULL,
    IsActive           BIT            NOT NULL CONSTRAINT DF_Products_IsActive  DEFAULT (1),
    CreatedAt          DATETIME2(0)   NOT NULL CONSTRAINT DF_Products_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt          DATETIME2(0)   NULL,
    CONSTRAINT PK_Products             PRIMARY KEY (Id),
    CONSTRAINT FK_Products_Categories  FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(Id),
    CONSTRAINT UQ_Products_ProductCode UNIQUE (ProductCode),
    CONSTRAINT CK_Products_Stock       CHECK (StockQuantity >= 0),
    CONSTRAINT CK_Products_Critical    CHECK (CriticalStockLevel >= 0),
    CONSTRAINT CK_Products_Price       CHECK (Price > 0)
);
GO

/* ------------------------------------------------------------
   4. Sepet ve sepet urunleri
   UQ_Carts_UserId          : her kullanicinin tek sepeti olur.
   UQ_CartItems_Cart_Product: ayni urun sepette iki satir olamaz,
                              tekrar eklenirse adedi artar.
   ------------------------------------------------------------ */
CREATE TABLE dbo.Carts (
    Id        INT IDENTITY(1,1) NOT NULL,
    UserId    INT NOT NULL,
    CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Carts_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt DATETIME2(0) NULL,
    CONSTRAINT PK_Carts        PRIMARY KEY (Id),
    CONSTRAINT FK_Carts_Users  FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
    CONSTRAINT UQ_Carts_UserId UNIQUE (UserId)
);
GO

CREATE TABLE dbo.CartItems (
    Id        INT IDENTITY(1,1) NOT NULL,
    CartId    INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity  INT NOT NULL,
    AddedAt   DATETIME2(0) NOT NULL CONSTRAINT DF_CartItems_AddedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT PK_CartItems                PRIMARY KEY (Id),
    CONSTRAINT FK_CartItems_Carts          FOREIGN KEY (CartId) REFERENCES dbo.Carts(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CartItems_Products       FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id),
    CONSTRAINT UQ_CartItems_Cart_Product   UNIQUE (CartId, ProductId),
    CONSTRAINT CK_CartItems_Quantity       CHECK (Quantity > 0)
);
GO

/* ------------------------------------------------------------
   5. Siparisler ve siparis kalemleri
   OrderNumber SEQUENCE ile uretilir; es zamanli siparislerde cakismaz.
   OrderItems'taki urun kodu/adi/fiyati siparis anindan dondurulur (snapshot).
   ------------------------------------------------------------ */
CREATE SEQUENCE dbo.OrderNumberSeq AS INT START WITH 10001 INCREMENT BY 1;
GO

CREATE TABLE dbo.Orders (
    Id          INT IDENTITY(1,1) NOT NULL,
    OrderNumber INT           NOT NULL CONSTRAINT DF_Orders_OrderNumber DEFAULT (NEXT VALUE FOR dbo.OrderNumberSeq),
    UserId      INT           NOT NULL,
    OrderDate   DATETIME2(0)  NOT NULL CONSTRAINT DF_Orders_OrderDate DEFAULT (SYSUTCDATETIME()),
    Status      TINYINT       NOT NULL CONSTRAINT DF_Orders_Status     DEFAULT (0),
    TotalAmount DECIMAL(18,2) NOT NULL,
    CONSTRAINT PK_Orders             PRIMARY KEY (Id),
    CONSTRAINT FK_Orders_Users       FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
    CONSTRAINT UQ_Orders_OrderNumber UNIQUE (OrderNumber),
    CONSTRAINT CK_Orders_Status      CHECK (Status IN (0,1,2)),  -- 0: Beklemede, 1: Onaylandi, 2: Reddedildi
    CONSTRAINT CK_Orders_Total       CHECK (TotalAmount >= 0)
);
GO

CREATE TABLE dbo.OrderItems (
    Id          INT IDENTITY(1,1) NOT NULL,
    OrderId     INT           NOT NULL,
    ProductId   INT           NOT NULL,
    ProductCode NVARCHAR(50)  NOT NULL,
    ProductName NVARCHAR(200) NOT NULL,
    Quantity    INT           NOT NULL,
    UnitPrice   DECIMAL(18,2) NOT NULL,
    TotalPrice  AS (Quantity * UnitPrice) PERSISTED,
    CONSTRAINT PK_OrderItems           PRIMARY KEY (Id),
    CONSTRAINT FK_OrderItems_Orders    FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id),
    CONSTRAINT FK_OrderItems_Products  FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id),
    CONSTRAINT CK_OrderItems_Quantity  CHECK (Quantity > 0),
    CONSTRAINT CK_OrderItems_UnitPrice CHECK (UnitPrice >= 0)
);
GO

/* ------------------------------------------------------------
   6. Dinamik grid konfigurasyonu
   Urun listesindeki kolonlar, sira, render tipi, genislik, hizalama
   ve cihaz gorunurlugu bu tablodan okunur. Kolon duzenini degistirmek
   icin kod degisikligi gerekmez.
   ------------------------------------------------------------ */
CREATE TABLE dbo.GridColumnConfigs (
    Id            INT IDENTITY(1,1) NOT NULL,
    GridKey       VARCHAR(50)   NOT NULL,
    FieldName     VARCHAR(50)   NOT NULL,
    HeaderText    NVARCHAR(100) NOT NULL,
    RenderType    VARCHAR(30)   NOT NULL,
    DisplayOrder  INT           NOT NULL,
    IsVisible     BIT           NOT NULL CONSTRAINT DF_Grid_IsVisible DEFAULT (1),
    Width         VARCHAR(10)   NULL,
    Alignment     VARCHAR(10)   NOT NULL CONSTRAINT DF_Grid_Alignment DEFAULT ('left'),
    ShowOnDesktop BIT           NOT NULL CONSTRAINT DF_Grid_Desktop   DEFAULT (1),
    ShowOnTablet  BIT           NOT NULL CONSTRAINT DF_Grid_Tablet    DEFAULT (1),
    ShowOnMobile  BIT           NOT NULL CONSTRAINT DF_Grid_Mobile    DEFAULT (1),
    CONSTRAINT PK_GridColumnConfigs PRIMARY KEY (Id),
    CONSTRAINT UQ_Grid_Key_Field    UNIQUE (GridKey, FieldName),
    CONSTRAINT CK_Grid_RenderType   CHECK (RenderType IN ('Text','Image','Price','StockStatus','QuantityAddToCart')),
    CONSTRAINT CK_Grid_Alignment    CHECK (Alignment IN ('left','center','right'))
);
GO

/* ------------------------------------------------------------
   7. Ana sayfa slider'i
   ------------------------------------------------------------ */
CREATE TABLE dbo.Sliders (
    Id           INT IDENTITY(1,1) NOT NULL,
    Title        NVARCHAR(150) NOT NULL,
    ImageUrl     NVARCHAR(500) NOT NULL,
    LinkUrl      NVARCHAR(500) NULL,
    DisplayOrder INT NOT NULL CONSTRAINT DF_Sliders_Order    DEFAULT (0),
    IsActive     BIT NOT NULL CONSTRAINT DF_Sliders_IsActive DEFAULT (1),
    CONSTRAINT PK_Sliders PRIMARY KEY (Id)
);
GO

/* ------------------------------------------------------------
   8. Indeksler
   SQL Server foreign key'lere otomatik indeks acmaz; FK kolonlarina
   indeksleri elle ekliyoruz. IX_Orders_UserId_OrderDate "Siparislerim"
   sorgusunu tabloya hic donmeden karsilar (covering index).
   ------------------------------------------------------------ */
CREATE NONCLUSTERED INDEX IX_Products_Name             ON dbo.Products(Name);
CREATE NONCLUSTERED INDEX IX_Products_Brand            ON dbo.Products(Brand);
CREATE NONCLUSTERED INDEX IX_Products_ManufacturerCode ON dbo.Products(ManufacturerCode);
CREATE NONCLUSTERED INDEX IX_Products_CategoryId       ON dbo.Products(CategoryId);
CREATE NONCLUSTERED INDEX IX_CartItems_ProductId       ON dbo.CartItems(ProductId);

CREATE NONCLUSTERED INDEX IX_Orders_UserId_OrderDate
    ON dbo.Orders(UserId, OrderDate DESC)
    INCLUDE (OrderNumber, Status, TotalAmount);

CREATE NONCLUSTERED INDEX IX_OrderItems_OrderId   ON dbo.OrderItems(OrderId);
CREATE NONCLUSTERED INDEX IX_OrderItems_ProductId ON dbo.OrderItems(ProductId);
GO

/* ------------------------------------------------------------
   9. Grid kolon konfigurasyonu (zorunlu baslangic verisi)
   FieldName degerleri ProductListItemDto property adlariyla
   birebir ayni olmalidir; degerler reflection ile okunur.
   ------------------------------------------------------------ */
INSERT INTO dbo.GridColumnConfigs
    (GridKey, FieldName, HeaderText, RenderType, DisplayOrder, Width, Alignment, ShowOnMobile)
VALUES
    ('ProductList', 'ImageUrl',    N'Görsel',    'Image',             1, '70px',  'center', 0),
    ('ProductList', 'ProductCode', N'Ürün Kodu', 'Text',              2, '120px', 'left',   1),
    ('ProductList', 'Name',        N'Ürün Adı',  'Text',              3, NULL,    'left',   1),
    ('ProductList', 'Brand',       N'Marka',     'Text',              4, '120px', 'left',   0),
    ('ProductList', 'StockStatus', N'Stok',      'StockStatus',       5, '90px',  'center', 1),
    ('ProductList', 'Price',       N'Fiyat',     'Price',             6, '110px', 'right',  1),
    ('ProductList', 'Id',          N'Sipariş',   'QuantityAddToCart', 7, '180px', 'center', 1);
GO

PRINT 'Sema olusturuldu. Simdi 02_SampleData.sql dosyasini calistirin.';
GO

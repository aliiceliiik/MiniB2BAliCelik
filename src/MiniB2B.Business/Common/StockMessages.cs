namespace MiniB2B.Business.Common;

public static class StockMessages
{
    public static string Insufficient(string productName, int availableStock) =>
        $"{productName} için yeterli stok bulunmamaktadır. Mevcut stok: {availableStock}.";

    public const string ProductNotAvailable = "Ürün bulunamadı veya satışta değil.";
    public const string ItemNotInCart = "Ürün sepetinizde bulunamadı.";
}
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using System.Net.Http;
using System.Runtime.CompilerServices;


QuestPDF.Settings.License = LicenseType.Community;

PageSize pageSize = new((float)8.5, (float)5.4, Unit.Centimetre);

//string assetImgPath = "C:/Users/Keith.Lee/Desktop/QuestPDF_TEST/QuestPDF-Preview/QuestPDF-Preview/Images/AssetBackground.png";
string assetImgPath = "C:/Users/Keith.Lee/Desktop/QuestPDF_TEST/QuestPDF-Preview/QuestPDF-Preview/Images/Empty.png";
using var stream = new FileStream(assetImgPath, FileMode.Open);

string SingleImgPath = "C:/Users/Keith.Lee/Desktop/QuestPDF_TEST/QuestPDF-Preview/QuestPDF-Preview/Images/OnePCEmpty.png";
using var singleAsset = new FileStream(SingleImgPath, FileMode.Open);


Document.Create(container =>
{
    container.Page(page =>
    {
        // 頁面大小
        page.Size(pageSize);
        // 背景顏色
        page.PageColor(Colors.White);       
        page.DefaultTextStyle(x => x.FontSize(8));  // 使用較小字體以適應頁面
        // 內容從左到右
        page.ContentFromLeftToRight();
        // 背景圖片
        page.Background().Image(stream).FitArea();

        // 使用 Row 來分配兩個部分，每個部分高度各占一半
        page.Content().Column(column =>
        {
            // 第一部分內容
            column.Item().Height(pageSize.Height / 2).Element(AssetContent);
        });
    });        
})
.ShowInPreviewer();

/// <summary>
/// 資產內容裡面要填入的資料
/// </summary>
void AssetContent(IContainer container)
{
    container.Column(column =>
    {
        // Empty
        column.Item().Height((float)0.54, Unit.Centimetre);

        // Empty
        column.Item().Height((float)0.54, Unit.Centimetre);         

        // 資產資料、供應商
        column.Item().Height((float)0.54, Unit.Centimetre).Element(AssetNameAndSupplier);
                     
        // 規格型號、進廠日期
        column.Item().Height((float)0.54, Unit.Centimetre).Element(SpecAndPurchaseDateDisplay);

        // 資產編號
        column.Item().Height((float)0.54, Unit.Centimetre).Element(AssetCodeDisplay);

    });
}

/// <summary>
/// 資產名稱及供應商顯示
/// </summary>
void AssetNameAndSupplier(IContainer container)
{
    container.Row(row =>
    {
        row.RelativeItem().Element(AssetNameArea);
        row.RelativeItem().Element(SupplierArea);
    });
}
/// <summary>
/// 資產名稱區域
/// </summary>
void AssetNameArea(IContainer container)
{
    container.Column(column =>
    {
        column.Item().Text("  資產資料：")
            .FontSize(8)
            .Bold()
            .AlignLeft();
    });
}
/// <summary>
/// 供應商區域
/// </summary>
void SupplierArea(IContainer container)
{
    container.Column(column =>
    {
        column.Item().Text("  供應廠商: ")
            .FontSize(8)
            .Bold()
            .AlignLeft();
    });
}
/// <summary>
/// 規格型號及購買日期顯示
/// </summary>
void SpecAndPurchaseDateDisplay(IContainer container)
{
    container.Row(row =>
    {
        row.RelativeItem().Element(SpecArea);
        row.RelativeItem().Element(PurchaseDateArea);
    });
}
/// <summary>
/// 規格型號區域
/// </summary>
void SpecArea(IContainer container)
{
    container.Column(column =>
    {
        column.Item().Text("  規格型號：")
            .FontSize(8)
            .Bold()
            .AlignLeft();
    });
}
/// <summary>
/// 購買日期區域
/// </summary>
void PurchaseDateArea(IContainer container)
{
    container.Column(column =>
    {
        column.Item().Text("  購買日期: ")
            .FontSize(8)
            .Bold()
            .AlignLeft();
    });
}
/// <summary>
/// 資產編號顯示
/// </summary>
void AssetCodeDisplay(IContainer container)
{
    container.Row(row =>
    {
        row.RelativeItem().Column(column =>
        {
            column.Item().Text("  資產編號：")
                .FontSize(8)
                .Bold()
                .AlignLeft();
        });
    });
}

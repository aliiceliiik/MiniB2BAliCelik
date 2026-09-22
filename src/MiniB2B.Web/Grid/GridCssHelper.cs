using MiniB2B.Entities.Dtos.Grid;

namespace MiniB2B.Web.Grid;

public static class GridCssHelper
{
    public static string GetCellClass(GridColumnDto column)
    {
        var mobile = column.ShowOnMobile ? "d-table-cell" : "d-none";
        var tablet = column.ShowOnTablet ? "d-md-table-cell" : "d-md-none";
        var desktop = column.ShowOnDesktop ? "d-lg-table-cell" : "d-lg-none";

        var align = column.Alignment switch
        {
            "center" => "text-center",
            "right" => "text-end",
            _ => "text-start"
        };

        return $"{mobile} {tablet} {desktop} {align}";
    }
}
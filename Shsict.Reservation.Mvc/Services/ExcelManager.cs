using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Models;

namespace Shsict.Reservation.Mvc.Services
{
    public static class ExcelManager
    {
        public static HSSFWorkbook BuildOrderWorkbook(List<OrderDto> list)
        {
            var book = new HSSFWorkbook();

            var title = new[] { "序号", "送餐点", "用餐人", "工号", "时段", "套餐", "主食", "加饭", "创建时间", "创建人" };

            var zones = list.DistinctOrderBy(x => x.DeliveryZone);

            foreach (var z in zones)
            {
                var query = from o in list.FindAll(x => x.DeliveryZone.Equals(z, StringComparison.OrdinalIgnoreCase))
                            orderby o.DeliveryPoint ascending, o.CreateTime descending
                            select new
                            {
                                o.ID,
                                o.DeliveryPoint,
                                o.UserName,
                                o.EmployeeNo,
                                o.MenuName,
                                o.Flag,
                                o.StapleFood,
                                ExtraFood = o.ExtraFood ? "加饭" : string.Empty,
                                o.CreateTime,
                                o.CreateUser
                            };

                var dt = ConvertToDataTable(query.ToList());
                var no = 1;

                foreach (DataRow dr in dt.Rows)
                {
                    dr["ID"] = no++;
                }

                BuildSheet(book, z, dt, title);
            }

            return book;
        }

        public static HSSFWorkbook BuildReportWorkbook(List<ReportDto> list)
        {
            var book = new HSSFWorkbook();

            var title = new[] { "送餐区域", "时段", "套餐", "主食", "加饭", "数量" };

            var query = from r in list
                        orderby r.DeliveryZone ascending, r.MenuName descending, r.MenuFlag ascending, r.StapleFood descending, r.ExtraFood descending
                        select new
                        {
                            r.DeliveryZone,
                            r.MenuName,
                            r.MenuFlag,
                            r.StapleFood,
                            ExtraFood = r.ExtraFood ? "加饭" : string.Empty,
                            r.OrderCount
                        };

            BuildSheet(book, "各区域订餐数量统计表", ConvertToDataTable(query.ToList()), title);

            return book;
        }


        private static void BuildSheet(HSSFWorkbook book, string name, DataTable dt, string[] title = null)
        {
            ICellStyle style = book.CreateCellStyle();

            // 设置字体样式
            IFont font = book.CreateFont();
            font.FontHeightInPoints = ConfigGlobal.DefaultExcelFontSize;

            style.SetFont(font);

            // 设置边框样式
            style.BorderTop = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;

            ISheet sheet = book.CreateSheet(name);

            if (title != null && title.Length > 0)
            {
                IRow row = sheet.CreateRow(0);

                for (var i = 0; i < title.Length; i++)
                {
                    // 设置sheet默认样式
                    //sheet.SetDefaultColumnStyle(i, style);

                    ICell cell = row.CreateCell(i);

                    cell.SetCellValue(title[i]);
                    cell.CellStyle = style;
                }
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                var titleNo = title != null && title.Length > 0 ? 1 : 0;

                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(titleNo + i);

                    for (var j = 0; j < dt.Columns.Count; j++)
                    {
                        ICell cell = row.CreateCell(j, CellType.String);

                        cell.SetCellValue(dt.Rows[i][j].ToString());
                        cell.CellStyle = style;

                        sheet.AutoSizeColumn(j, true);
                    }
                }
            }
        }

        private static DataTable ConvertToDataTable<T>(List<T> list)
        {
            var table = CreateTable<T>();
            var properties = TypeDescriptor.GetProperties(typeof(T));

            foreach (var item in list)
            {
                var row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item)?.ToString();
                }

                table.Rows.Add(row);
            }

            return table;
        }

        private static DataTable CreateTable<T>()
        {
            var table = new DataTable(typeof(T).Name);
            var properties = TypeDescriptor.GetProperties(typeof(T));

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, typeof(string));
            }

            return table;
        }
    }
}
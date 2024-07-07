using AX.AXSDK;
using AXService.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Table;
namespace AXService.Services.Implementations
{
    public class TableSegmentationService : ITableSegmentationService
    {
        public Task<List<Dictionary<string, InformationField>>> Segment_TuPhapA3KetHon(string filePath,int year)
        {
            List<string> listFieldName = new List<string>
            {
                "STT",
                "NgayCap",
                "HoTenChong",
                "HoTenVo"
            };

            List<Dictionary<string, InformationField>> result = new List<Dictionary<string, InformationField>>();
            //filePath = @"D:\sohoa\A3_Truoc1999\CSDL_SOHOA\thanhphophanrangthapcham\phuongmydong\KH\1994\03\KH.1994.03.1994-02-26.58.pdf";
            filePath = @"\\192.168.24.13\filedata\FileTemp\eaafa977-2fc3-4964-a212-d2ed76978f7f_KH1994031994032265.pdf";
            string json_path = @"D:\Project\cyberworks-github\AXSDK-API\AXService\Module\TableSegment\A3_json\kethon_1989.json";

            if (!System.IO.File.Exists(json_path)) {
                throw new Exception("file not found:" + json_path);
            }
            try
            {
                var tableSegmentResult = A3Table.getStructA3fromPDF(filePath, json_path);

                //cấu trúc trả lại List các table
                // Mỗi Table là một list cá Record => Mỗi record là một table nhỏ => thể thể gồm nhiều dòng
                // Trong mẫu kết hôn thì 1 record gồm 4 dòng => 2 dòng cho chồng / 2 dòng cho vợ => cần xử lý merge 
                foreach (var table in tableSegmentResult)
                {
                    var page = table.pageNumber;
                    var pageSize = new System.Drawing.Size(table.pageWidth, table.pageHeight);

                    //xử lý từng bản ghi dữ liệu trên phiếu 
                    foreach (var row in table.Records)
                    {
                        //Với mỗi bản ghi sẽ thực hiện tạo danh sách các field_name / field_info (tọa độ) tương ứng
                        var rowInfo = new Dictionary<string, InformationField>();
                        foreach (var cell in row.cells)
                        {
                            
                            for (var colIndex = 0; colIndex < cell.Count; colIndex++)
                            {
                                if (colIndex < listFieldName.Count)
                                {
                                    var col = cell[colIndex];
                                    
                                    var field_name = listFieldName[colIndex]; // field_name
                                    var fieldInfo = new InformationField()    // field_info
                                    {
                                        Area = new System.Drawing.Rectangle(col.x, col.y, col.w, col.h),
                                        Page = page,
                                        PageSize = pageSize
                                    };
                                    if (!rowInfo.ContainsKey(field_name))
                                    {
                                        rowInfo.Add(field_name, fieldInfo);
                                    }
                                }
                            }
                        }

                        result.Add(rowInfo);
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }


            return Task.FromResult(result);
        }

        public Task<List<Dictionary<string, InformationField>>> Segment_TuPhapA3KhaiSinh(string filePath,int year)
        {
            List<string> listFieldName = new List<string>
            {
                "STT",
                "NgayCap",
                "HoTen",
                "NgaySinh"
            };

            List<Dictionary<string, InformationField>> result = new List<Dictionary<string, InformationField>>();
            
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            //Todo: for demo only => for demo only
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = @"\\192.168.24.13\filedata\FileTemp\KS.1995.01.1995-11-22.01.pdf";
            }

            string json_path =System.IO.Path.GetFullPath(@"\Module\TableSegment\A3_json\khaisinh_1998.json");

            if (!System.IO.File.Exists(json_path))
            {
                throw new Exception("file not found:" + json_path);
            }
            try
            {
                var tableSegmentResult = A3Table.getStructA3fromPDF(filePath, json_path);

                //cấu trúc trả lại List các table
                // Mỗi Table là một list cá Record => Mỗi record là một table nhỏ => thể thể gồm nhiều dòng
                // Trong mẫu kết hôn thì 1 record gồm 4 dòng => 2 dòng cho chồng / 2 dòng cho vợ => cần xử lý merge 
                foreach (var table in tableSegmentResult)
                {
                    var page = table.pageNumber;
                    var pageSize = new System.Drawing.Size(table.pageWidth, table.pageHeight);

                    //xử lý từng bản ghi dữ liệu trên phiếu 
                    foreach (var record in table.Records)
                    {
                        //Với mỗi bản ghi sẽ thực hiện tạo danh sách các field_name / field_info (tọa độ) tương ứng
                        var rowInfo = new Dictionary<string, InformationField>();
                        for (var rowIndex =0; rowIndex< record.cells.Count;rowIndex++)
                        {
                            List<Table.CellStruct> row = record.cells[rowIndex];
                            List<Table.CellStruct> nextRow = null;

                            if (record.cells.Count >= 2) {
                                nextRow = record.cells[rowIndex + 1];
                            }

                            for (var colIndex = 0; colIndex < row.Count; colIndex++)
                            {
                                if (colIndex < listFieldName.Count)
                                {
                                    var col = row[colIndex];

                                    var field_name = listFieldName[colIndex]; // field_name
                                    var fieldInfo = new InformationField()    // field_info
                                    {
                                        Area = new System.Drawing.Rectangle(col.x, col.y, col.w, col.h),
                                        Page = page,
                                        PageSize = pageSize
                                    };
                                    if(nextRow!=null) //merge dong
                                    {
                                        var colInNextRow = nextRow[colIndex];
                                        fieldInfo.Area = new System.Drawing.Rectangle(col.x, col.y, col.w, (colInNextRow.y - col.y) + colInNextRow.h);
                                    }
                                    rowInfo.Add(field_name, fieldInfo);
                                }
                            }
                        }

                        result.Add(rowInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return Task.FromResult(result);
        }

        public Task<List<Dictionary<string, InformationField>>> Segment_TuPhapA3KhaiTu(string filePath, int year)
        {
            List<string> listFieldName = new List<string>
            {
                "STT",
                "NgayCap",
                "HoTen",
                "NgaySinh"
            };

            List<Dictionary<string, InformationField>> result = new List<Dictionary<string, InformationField>>();

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            //Todo: for demo only => for demo only
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = @"\\192.168.24.13\filedata\FileTemp\KT.1998.01.1998-01-06.01.pdf";
            }

            string json_path = System.IO.Path.GetFullPath(@"\Module\TableSegment\A3_json\khaitu_1998.json");

            if (!System.IO.File.Exists(json_path))
            {
                throw new Exception("file not found:" + json_path);
            }
            try
            {
                var tableSegmentResult =A3Table.getStructA3fromPDF(filePath, json_path);

                //cấu trúc trả lại List các table
                // Mỗi Table là một list cá Record => Mỗi record là một table nhỏ => thể thể gồm nhiều dòng
                // Trong mẫu kết hôn thì 1 record gồm 4 dòng => 2 dòng cho chồng / 2 dòng cho vợ => cần xử lý merge 
                foreach (var table in tableSegmentResult)
                {
                    var page = table.pageNumber;
                    var pageSize = new System.Drawing.Size(table.pageWidth, table.pageHeight);

                    //xử lý từng bản ghi dữ liệu trên phiếu 
                    foreach (var record in table.Records)
                    {
                        //Với mỗi bản ghi sẽ thực hiện tạo danh sách các field_name / field_info (tọa độ) tương ứng
                        var rowInfo = new Dictionary<string, InformationField>();
                        for (var rowIndex = 0; rowIndex < record.cells.Count; rowIndex++)
                        {
                            List<Table.CellStruct> row = record.cells[rowIndex];
                            List<Table.CellStruct> nextRow = null;

                            if (record.cells.Count >= 2)
                            {
                                nextRow = record.cells[rowIndex + 1];
                            }

                            for (var colIndex = 0; colIndex < row.Count; colIndex++)
                            {
                                if (colIndex < listFieldName.Count)
                                {
                                    var col = row[colIndex];

                                    var field_name = listFieldName[colIndex]; // field_name
                                    var fieldInfo = new InformationField()    // field_info
                                    {
                                        Area = new System.Drawing.Rectangle(col.x, col.y, col.w, col.h),
                                        Page = page,
                                        PageSize = pageSize
                                    };
                                    if (nextRow != null) //merge dong
                                    {
                                        var colInNextRow = nextRow[colIndex];
                                        fieldInfo.Area = new System.Drawing.Rectangle(col.x, col.y, col.w, (colInNextRow.y - col.y) + colInNextRow.h);
                                    }
                                    rowInfo.Add(field_name, fieldInfo);
                                }
                            }
                        }

                        result.Add(rowInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return Task.FromResult(result);
        }
    }
}

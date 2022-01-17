# Candle-in-the-wind
Project Introduction to Software Engineering

# Hướng dẫn set up cài đặt

## Yêu cầu:
1. Visual Studio 2019 trở lên: https://visualstudio.microsoft.com/vs/, tải workload **ASP.NET and web development** và **.NET 5 Runtime** trong Visual Studio Installer
2. SQL Server: https://www.microsoft.com/en-us/sql-server/sql-server-downloads

## Các bước set up:
1. Clone repo về local: *git clone https://github.com/Vanhdev/Candle-in-the-wind.git* hoặc *git clone git@github.com:Vanhdev/Candle-in-the-wind.git*

2. Mở repo bằng Visual Studio.

3. Trong project CandleInTheWind và project CandleInTheWind.API, mỗi project thêm file **appsettings.Development.json** có nội dung:
```json
{ 
    "ConnectionStrings": { 
        "DefaultConnection": "Server=<tên SQL Serer>;Initial Catalog=<tên Database>;Integrated Security=True" 
    } 
}
```
4. Trong Visual Studio, chọn *Tools > NuGet Package Manager > Package Management Console* và chạy lệnh *Update-Database*
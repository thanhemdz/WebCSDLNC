# Quản lý lịch ca làm việc - ASP.NET Core MVC

## Mô tả

Đây là project ASP.NET Core MVC quản lý lịch ca làm việc cho nhân viên, cho phép admin xem, thêm, sửa, xóa ca làm theo tuần.

## Yêu cầu

- .NET 6.0 trở lên
- SQL Server (hoặc SQLite nếu bạn đã cấu hình)
- Visual Studio 2022 hoặc Visual Studio Code

## Hướng dẫn chạy project

### 1. Clone project về máy

```bash
git clone https://github.com/thanhemdz/WebCSDLNC.git
cd WebCSDLNC
```

### 2. Cài đặt package (nếu cần)

```bash
dotnet restore
```

### 3. Cấu hình chuỗi kết nối database

- Mở file `appsettings.json`
- Sửa lại `ConnectionStrings:DefaultConnection` cho phù hợp với SQL Server của bạn.

### 4. Tạo database và migration (nếu chưa có)

```bash
dotnet ef database update
```

> Nếu chưa cài EF CLI:  
> com

---

> **Lưu ý:**  
> Đừng quên kiểm tra file `.gitignore` để tránh đẩy các file nhạy cảm hoặc file build lên GitHub.

# CardFlow - Quản Lý Nhập Thẻ & Hạch Toán Thu Chi Lợi Nhuận Từng Đợt

Hệ thống quản lý nhập thẻ và hạch toán tài chính theo từng đợt được xây dựng theo mô hình kiến trúc chuẩn, ứng dụng 4 trụ cột OOP và 5 nguyên lý SOLID để làm tài liệu tham chiếu đối chiếu với hệ thống TOPO.

---

## 1. Cấu trúc Solution

Dự án gồm 4 project tương ứng với các tầng kiến trúc:

* **src/CardFlow.Domain**: Chứa các Entity và Interface nghiệp vụ thuần C# (BaseCard, CardBatch, IFinancialEvaluator và các loại thẻ cụ thể).
* **src/CardFlow.Application**: Chứa các Use Case, Interface lưu trữ và Service tính toán tài chính (FinancialCalculator, CardBatchService).
* **src/CardFlow.Infrastructure**: Hiện thực kho lưu trữ trong bộ nhớ (InMemoryCardBatchRepository) và quản lý danh mục loại thẻ động (CardTypeRegistry).
* **src/CardFlow.WinForms**: Giao diện WinForms gồm màn hình Dashboard tổng hợp KPI, bảng quản lý đợt thẻ, chi tiết hạch toán từng thẻ và bảng so sánh hiệu quả kinh tế giữa các loại thẻ.

---

## 2. Ứng dụng 4 Trụ Cột OOP

1. **Đóng gói (Encapsulation)**:
   * Toàn bộ giá phôi, giá bán và công thức tính toán tài chính của đợt thẻ được gói gọn bên trong `BaseCard` và `CardBatch`.
   * Bên ngoài không thể gán giá trị âm hoặc thay đổi tùy tiện; mọi thay đổi đều phải thông qua các hàm nghiệp vụ hợp lệ.

2. **Trừu tượng (Abstraction)**:
   * `BaseCard` là abstract class che giấu chi tiết cấu tạo bên trong thẻ, chỉ phơi bày các hàm cần thiết như `CalculateUnitProcessingCost()`.
   * Tầng giao diện và dịch vụ làm việc qua các Interface (`IFinancialEvaluator`, `ICardBatchRepository`) thay vì phụ thuộc trực tiếp vào class cụ thể.

3. **Kế thừa (Inheritance)**:
   * Các class thẻ chuyên biệt (`StandardPvcCard`, `ProximityRfidCard`, `VipSmartChipCard`, `NfcAccessCard`) đều kế thừa từ `BaseCard`, tái sử dụng toàn bộ thông tin mã thẻ, tên chủ thẻ và cơ chế quản lý giá bán.

4. **Đa hình (Polymorphism)**:
   * Mỗi loại thẻ tự định nghĩa công thức tính phí gia công kỹ thuật riêng qua hàm `CalculateUnitProcessingCost()`.
   * Khi tính toán chi phí cho cả đợt thẻ, hệ thống chỉ duyệt danh sách thẻ và gọi hàm đa hình này, không cần dùng câu lệnh `switch-case` hay `if...else` để kiểm tra loại thẻ.

---

## 3. Ứng dụng 5 Nguyên Lý SOLID

* **S (Single Responsibility)**: Mỗi class đảm nhận duy nhất một trách nhiệm:
  * `BaseCard`: Quản lý thông tin và đơn giá của 1 thẻ.
  * `CardBatch`: Quản lý danh sách thẻ trong đợt và tổng hợp số liệu đợt.
  * `FinancialCalculator`: Chuyên biệt tính toán các chỉ số tài chính tổng hợp toàn hệ thống.
  * `InMemoryCardBatchRepository`: Chuyên biệt quản lý việc lưu trữ và truy vấn đợt thẻ.
  * `MainForm`: Chỉ hiển thị giao diện và nhận thao tác từ người dùng.

* **O (Open/Closed - Cam kết với Sếp)**:
  * Khi cần bổ sung thêm loại thẻ mới (ví dụ: Thẻ VIP Bạch Kim), ta chỉ cần tạo thêm một class mới kế thừa từ `BaseCard` và đăng ký vào `CardTypeRegistry`.
  * Toàn bộ code tính toán hạch toán cũ trong `CardBatch` hay `FinancialCalculator` không cần chỉnh sửa một dòng nào.

* **L (Liskov Substitution)**:
  * Mọi loại thẻ con đều có thể thay thế hoàn hảo cho `BaseCard` mà không làm sai lệch kết quả tính toán chi phí hay doanh thu.

* **I (Interface Segregation)**:
  * Tạo các interface chuyên biệt, nhỏ gọn (`IFinancialEvaluator`, `ICardBatchRepository`), không nhồi nhét các phương thức không liên quan vào cùng một giao diện.

* **D (Dependency Inversion)**:
  * Các tầng cấp cao phụ thuộc vào Interface Abstraction chứ không phụ thuộc trực tiếp vào class cụ thể. Hệ thống sử dụng `Microsoft.Extensions.DependencyInjection` tại `Program.cs` để tiêm các dịch vụ vào Form.

---

## 4. Cách chạy ứng dụng

Mở terminal tại thư mục gốc và chạy lệnh:

```powershell
dotnet run --project src/CardFlow.WinForms
```

---

## 5. Câu hỏi đối chiếu thực tế với hệ thống TOPO

Khi làm việc và báo cáo với Sếp, bạn có thể đối chiếu các điểm sau với codebase hiện tại của TOPO:
1. **Kiểm tra vi phạm OCP và Đa hình**: Các module trong TOPO có đang dùng nhiều câu lệnh `switch(type)` hoặc chuỗi `if...else if` dài để tính giá hay phân nhánh nghiệp vụ không? Nếu có, có thể cải tiến bằng cách áp dụng lớp cơ sở trừu tượng và tính đa hình như CardFlow.
2. **Kiểm tra vi phạm SRP**: Có class hoặc form nào trong TOPO vừa truy vấn database, vừa tính toán số liệu, vừa hiển thị lên màn hình không? Nếu có, nên tách thành Service và Repository riêng.
3. **Kiểm tra tính kết dính (Coupling)**: Khi thêm một loại nghiệp vụ hoặc đối tượng mới vào TOPO, bạn phải sửa bao nhiêu file cũ? Nếu phải sửa quá nhiều file, kiến trúc đang bị phụ thuộc chặt, cần đưa Interface vào để giảm bớt sự phụ thuộc.
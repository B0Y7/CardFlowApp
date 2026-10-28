using CardFlow.Domain.Entities;

namespace CardFlow.Application.Interfaces;

// Định nghĩa thông tin và cách khởi tạo của một loại thẻ.
// Nhờ interface này, khi cần thêm loại thẻ mới ta chỉ cần tạo thêm class cài đặt interface này,
// không phải sửa code khởi tạo cũ (đáp ứng nguyên lý Open/Closed - OCP).
public interface ICardTypeDefinition
{
    string TypeKey { get; }
    string DisplayName { get; }
    string Description { get; }
    decimal DefaultBlankCost { get; }
    decimal DefaultSellingPrice { get; }

    BaseCard CreateCard(
        string cardCode, 
        string holderName, 
        decimal? customBlankCost = null, 
        decimal? customSellingPrice = null);
}

// Registry quản lý danh sách loại thẻ, cho phép đăng ký thêm loại thẻ mới vào runtime
public interface ICardTypeRegistry
{
    void Register(ICardTypeDefinition definition);
    IReadOnlyList<ICardTypeDefinition> GetAll();
    ICardTypeDefinition GetByKey(string typeKey);
}

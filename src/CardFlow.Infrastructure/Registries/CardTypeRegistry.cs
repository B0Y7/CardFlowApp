using CardFlow.Application.Interfaces;

namespace CardFlow.Infrastructure.Registries;

// Nơi lưu trữ và tra cứu các định nghĩa loại thẻ trong hệ thống.
// Cho phép đăng ký thêm loại thẻ mới bất kỳ lúc nào mà không cần sửa code cũ.
public class CardTypeRegistry : ICardTypeRegistry
{
    private readonly Dictionary<string, ICardTypeDefinition> _registry = new(StringComparer.OrdinalIgnoreCase);

    public void Register(ICardTypeDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        _registry[definition.TypeKey] = definition;
    }

    public IReadOnlyList<ICardTypeDefinition> GetAll()
    {
        return _registry.Values.ToList();
    }

    public ICardTypeDefinition GetByKey(string typeKey)
    {
        if (!_registry.TryGetValue(typeKey, out var def))
            throw new KeyNotFoundException($"Loại thẻ '{typeKey}' chưa được đăng ký trong hệ thống.");
        return def;
    }
}

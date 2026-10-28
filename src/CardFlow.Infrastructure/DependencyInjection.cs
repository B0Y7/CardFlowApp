using CardFlow.Application.Interfaces;
using CardFlow.Application.Services;
using CardFlow.Infrastructure.Definitions;
using CardFlow.Infrastructure.Registries;
using CardFlow.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CardFlow.Infrastructure;

// Đăng ký toàn bộ các Service của hệ thống vào DI Container
public static class DependencyInjection
{
    public static IServiceCollection AddCardFlowServices(this IServiceCollection services)
    {
        // 1. Quản lý danh mục loại thẻ
        var registry = new CardTypeRegistry();
        registry.Register(new StandardPvcCardDefinition());
        registry.Register(new ProximityRfidCardDefinition());
        registry.Register(new VipSmartChipCardDefinition());
        registry.Register(new NfcAccessCardDefinition());
        services.AddSingleton<ICardTypeRegistry>(registry);

        // 2. Logic nghiệp vụ & tính toán tài chính
        services.AddSingleton<IFinancialCalculator, FinancialCalculator>();
        services.AddScoped<ICardBatchService, CardBatchService>();

        // 3. Tầng lưu trữ dữ liệu
        services.AddSingleton<ICardBatchRepository, InMemoryCardBatchRepository>();

        return services;
    }
}

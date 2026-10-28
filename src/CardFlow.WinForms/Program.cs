using CardFlow.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CardFlow.WinForms;

static class Program
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Cấu hình Dependency Injection Container
        var services = new ServiceCollection();

        // Đăng ký toàn bộ các Service của tầng Infrastructure & Application
        services.AddCardFlowServices();

        // Đăng ký Form chính
        services.AddTransient<MainForm>();

        ServiceProvider = services.BuildServiceProvider();

        // Khởi tạo MainForm thông qua DI Provider và chạy ứng dụng
        var mainForm = ServiceProvider.GetRequiredService<MainForm>();
        System.Windows.Forms.Application.Run(mainForm);
    }    
}
using CardFlowApp.Services;

namespace CardFlowApp;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Khởi tạo service hạch toán và tiêm vào Form
        IHachToanService service = new HachToanService();

        Application.Run(new MainForm(service));
    }    
}
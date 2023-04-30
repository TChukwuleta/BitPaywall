using BitPaywall.Application.Common.Interfaces;
using BitPaywall.Application.Posts.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BitPaywall.Infrastructure.Services
{
    public class BackgroundWorkerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public BackgroundWorkerService(IServiceProvider serviceProvider)
        {
            _serviceProvider =serviceProvider;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                using (var scope = _serviceProvider.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetService<IAppDbContext>();
                    var _configuration = scope.ServiceProvider.GetService<IConfiguration>();
                    var _lightningService = scope.ServiceProvider.GetService<ILightningService>();
                    var _authService = scope.ServiceProvider.GetService<IAuthService>();

                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

                    Console.WriteLine("About to start running round of background service");

                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    var invoicePaymentListener = new ListenForInvoiceCommand();
                    var invoicePaymentHandler = new ListenForInvoiceCommandHandler(_authService, _lightningService, _context);
                    await invoicePaymentHandler.Handle(invoicePaymentListener, stoppingToken);

                    Console.WriteLine("Done running round of background service");
                }

            }
        }
    }
}

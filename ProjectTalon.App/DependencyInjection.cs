using Microsoft.Extensions.DependencyInjection;

namespace ProjectTalon.App
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTalonProjectServices(this IServiceCollection service)
        {
            service.AddTransient<IMnemonicService, MnemonicService>(); 
            
            return service;
        }
    }
}

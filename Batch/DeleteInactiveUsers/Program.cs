using Microsoft.Extensions.Configuration;
using System;

namespace DeleteInactiveUsers
{
    public class Program
    {
        private readonly IConfiguration _config;
                
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();
            
            BL.GlobalData gd = new BL.GlobalData(configuration.GetConnectionString("ConnectionString"));

            InactiveUsers iu = new InactiveUsers(gd);
            iu.DeleteInactiveUsers();
            
        }
    }
}

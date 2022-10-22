using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using System.Diagnostics.CodeAnalysis;
using PaPl.SKS.DataAccess.Sql;
using Microsoft.EntityFrameworkCore;

namespace PaPl.SKS.Package.Services
{
    /// <summary>
    /// Program
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Program
    {
        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            /*
            //EF --- Migrating Database
            //In Case Migration doesnt work:
            //Install EF Tools and runs Add-Migration <nameOfMigration>
            */
            
            
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Create the web host builder.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>IWebHostBuilder</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}

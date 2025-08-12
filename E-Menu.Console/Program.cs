using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

var builder = new ConfigurationBuilder()
    .SetBasePath(System.AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var configuration = builder.Build();
string connectionString = configuration.GetConnectionString("MyCDSServer")!;
var service = new ServiceClient(connectionString);



Console.ReadLine();












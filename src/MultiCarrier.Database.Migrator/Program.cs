using Amazon.CodeDeploy;
using Amazon.CodeDeploy.Model;
using Amazon.Lambda.Core;
using Expensely.Logging.Serilog.Extensions;
using Microsoft.EntityFrameworkCore;
using MultiCarrier.Database.Extensions;
using Serilog;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace MultiCarrier.Database.Migrator;

public class Program
{
    public static void Main(string[] args)
    {
        Run(args);
    }

    public async Task<PutLifecycleEventHookExecutionStatusResponse> Handler(
        PutLifecycleEventHookExecutionStatusRequest request, 
        ILambdaContext context)
    {
        try
        {
            Run(new string[0]);
                
            await PutLifecycleEventHookExecutionStatusAsync(
                request.DeploymentId,
                request.LifecycleEventHookExecutionId,
                LifecycleEventStatus.Succeeded);
        }
        catch (Exception exception)
        {
            Log.Logger.Error(
                exception,
                "Error encountered during migration");
                
            await PutLifecycleEventHookExecutionStatusAsync(
                request.DeploymentId,
                request.LifecycleEventHookExecutionId,
                LifecycleEventStatus.Failed);
        }
        
        return new PutLifecycleEventHookExecutionStatusResponse();
    }
        
    private static async Task PutLifecycleEventHookExecutionStatusAsync(
        string deploymentId,
        string lifecycleEventHookExecutionId,
        LifecycleEventStatus status)
    {
        var codeDeployClient = new AmazonCodeDeployClient();
        var lifecycleRequest = new PutLifecycleEventHookExecutionStatusRequest
        {
            DeploymentId = deploymentId,
            LifecycleEventHookExecutionId = lifecycleEventHookExecutionId,
            Status = status
        };
        await codeDeployClient.PutLifecycleEventHookExecutionStatusAsync(lifecycleRequest);
    }

    private static void Run(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MultiCarrierContext>();
        db.Database.Migrate();
    }
        
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                var environmentName = context.HostingEnvironment.EnvironmentName;
                    
                if (environmentName.StartsWith("Preview", StringComparison.InvariantCultureIgnoreCase))
                {
                    config.AddJsonFile("appsettings.Preview.json", true, true);
                }

                config.AddSystemsManager(configureSource =>
                {
                    configureSource.Path = $"/MultiCarrier/{environmentName}";
                    configureSource.ReloadAfter = TimeSpan.FromMinutes(5);
                    configureSource.Optional = true;
                });
            })
            .UseSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddOptions();

                Logging.AddSerilog(hostContext.Configuration);

                services.AddMultiCarrierDatabase(
                    hostContext.Configuration, 
                    ServiceLifetime.Singleton);
            });
}
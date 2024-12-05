using System;
using Core.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;
using Core.Utilities.ElasticSearch.Models;
using Core.Utilities.IoC;
using Core.Utilities.Messages;
using Elastic.Channels;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Ingest.Elasticsearch;
using Elastic.Serilog.Sinks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Http.BatchFormatters;
using Elastic.Transport;

namespace Core.CrossCuttingConcerns.Logging.Serilog.Loggers
{
    public class LogstashLogger : LoggerServiceBase
    {
        public LogstashLogger()
        {
            var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();

            var logConfig = configuration.GetSection("SeriLogConfigurations:LogstashConfiguration")
                                .Get<ElasticSearchConfig>() ??
                            throw new Exception(SerilogMessages.NullOptionsMessage);

            var seriLogConfig = new LoggerConfiguration()

.MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new[] { new Uri(logConfig.ConnectionString) }, opts =>
    {
        opts.DataStream = new DataStreamName("logs", "console-example", "neredekal");
        opts.BootstrapMethod = BootstrapMethod.Failure;
        opts.ConfigureChannel = channelOpts =>
        {

        };
    }, transport =>
    {
        transport.Authentication(new BasicAuthentication(logConfig.UserName, logConfig.Password));
    }).CreateLogger();
            Logger = seriLogConfig;
        }
    }
}
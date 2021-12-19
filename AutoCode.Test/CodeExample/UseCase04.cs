using k8s;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AzureDevOpsTeamMembersVelocity.Hubs
{
    /// <summary>
    /// Log stream hub to subscribe to Kubernetes pod logs
    /// </summary>
    /// <remarks>
    /// This class is based on this article : 
    /// https://donotpanic.azurewebsites.net/2020/01/20/streaming-kubernetes-logs-using-signalr/
    /// with some code fix suggested by the rolsyn analyser and visual studio async plugin
    /// </remarks>
    [Authorize]
    public class LogStreamHub : Hub
    {
        private readonly Kubernetes _kubernetesClient;
        private readonly ILogger<LogStreamHub> _logger;
        private readonly IConfiguration _config;

        /// <summary>
        /// Constructor with dependencies
        /// </summary>
        /// <param name="kubernetesClient"></param>
        /// <param name="logger">Logger</param>
        /// <param name="configuration">Interface to access configuration</param>
        public LogStreamHub(Kubernetes kubernetesClient,
                            ILogger<LogStreamHub> logger,
                            IConfiguration configuration)
        {
            _kubernetesClient = kubernetesClient;
            _logger = logger;
            _config = configuration;
        }

        /// <summary>
        /// The default delay of the log stream when the kubernetes log stream is empty.
        /// </summary>
        /// <remarks>
        /// This is miliseconds
        /// </remarks>
        public const int DefaultLogStreamDelay = 1998;

        /// <summary>
        /// Function to get the pog log.
        /// </summary>
        /// <param name="ns">The namespace of the pod</param>
        /// <param name="pod">The name of the pod</param>
        /// <param name="cancellationToken">a cancelation token</param>
        /// <returns></returns>
        public async IAsyncEnumerable<string> GetPodLog(string ns, string pod, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var stream = await _kubernetesClient.ReadNamespacedPodLogAsync(pod, ns, follow: true, sinceSeconds: 2000, cancellationToken: cancellationToken);

            var buffer = new byte[8192];

            bool runLoop = true;

            do
            {
                int count = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);

                if (count != 0)
                {
                    var tmpString = Encoding.Default.GetString(buffer, 0, count);

                    yield return tmpString
                            .Replace("\u001b[40m\u001b[37mtrce\u001b[39m\u001b[22m\u001b[49m", "trce")
                            .Replace("\u001b[40m\u001b[37mdbug\u001b[39m\u001b[22m\u001b[49m", "dbug")
                            .Replace("\u001b[40m\u001b[32minfo\u001b[39m\u001b[22m\u001b[49m", "info")
                            .Replace("\u001b[40m\u001b[1m\u001b[33mwarn\u001b[39m\u001b[22m\u001b[49m", "warn")
                            .Replace("\u001b[41m\u001b[30mfail\u001b[39m\u001b[22m\u001b[49m", "fail")
                            .Replace("\u001b[41m\u001b[1m\u001b[37mcrit\u001b[39m\u001b[22m\u001b[49m", "crit");
                }
                else
                {
                    try
                    {
                        var miliseconds = _config.GetValue<int?>("DelayLogStreamEmpty") ?? DefaultLogStreamDelay;

                        await Task.Delay(miliseconds, cancellationToken);
                    }
                    catch
                    {
                        _logger.LogInformation($"Stream {ns} {pod} as ended");

                        runLoop = false;
                    }
                }

            } while (runLoop && StreamIsOn(ns, pod, cancellationToken));

            _logger.LogInformation($"GetPodLog {ns} {pod} completed");
        }

        private bool StreamIsOn(string ns, string pod, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch
            {
                _logger.LogInformation($"Stream {ns} {pod} as ended");

                return false;
            }

            return true;
        }
    }
}

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using AWS.Lambda.Powertools.Logging;
using AWS.Lambda.Powertools.Metrics;
using AWS.Lambda.Powertools.Tracing;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace HelloWorldApi
{
    public class Function
    {
        [Logging(LogEvent = true)]
        [Metrics]
        [Tracing]
        public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest input, ILambdaContext context)
        {
            Logger.LogInformation("Generating a new key");
            var key = await KeyGenerator();
            Logger.LogInformation(new Dictionary<string, object>() { { "Key", key } }, "New key generated");
            Metrics.AddMetric("SuccessfulKeyGeneration", 1, MetricUnit.Count);
            return new APIGatewayHttpApiV2ProxyResponse
            {
                Body = $@"{{""Message"":""This is your key {key}""}}",
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        [Tracing(SegmentName = "Key generator service")]
        private async Task<string> KeyGenerator()
        {
            await Task.Delay(Random.Shared.Next(2000));

            return Guid.NewGuid().ToString();
        }
    }
}

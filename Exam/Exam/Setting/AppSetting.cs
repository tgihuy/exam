using Manonero.MessageBus.Kafka.Settings;

namespace Exam.Setting
{
    public class AppSetting
    {
        public string OtlpUrl { get; init; }
        public string ServiceVersion { get; init; }
        public string OtlpHeaders { get; init; }
        public string ServiceNameSpace { get; init; }
        public string SecuritiesDBConnStr { get; init; }
        public string SecuritiesDBDefaultTableSchema { get; init; }
        public string TrdDBConnStr { get; init; }
        public string TrdDBDefaultTableSchema { get; init; }
        public string AccountApiUri { get; init; }
        public string BootstrapServers { get; init; }
        public int InstanceNumber { get; init; }
        public int TotalInstances { get; init; }
        public bool AllowConsoleLog { get; init; }
        public ConsumerSetting[] ConsumerSettings { get; init; }
        public ProducerSetting[] ProducerSettings { get; init; }

        public static AppSetting MapValue(IConfiguration configuration)
        {
            int.TryParse(configuration[nameof(InstanceNumber)], out int instanceNumber);
            int.TryParse(configuration[nameof(TotalInstances)], out int totalInstances);
            bool.TryParse(configuration[nameof(AllowConsoleLog)], out bool allowConsoleLog);
            var serviceVersion = configuration[nameof(ServiceVersion)];
            var otlpUrl = configuration[nameof(OtlpUrl)];
            var otlpHeaders = configuration[nameof(OtlpHeaders)];
            var serviceNameSpace = configuration[nameof(ServiceNameSpace)];
            var securitiesDbConnStr = configuration[nameof(SecuritiesDBConnStr)];
            var securitiesDBDefaultTableSchema = configuration[nameof(SecuritiesDBDefaultTableSchema)];
            var trdDBConnStr = configuration[nameof(TrdDBConnStr)];
            var trdDBDefaultTableSchema = configuration[nameof(TrdDBDefaultTableSchema)];
            var accountApiUri = configuration[nameof(AccountApiUri)];
            var bootstrapServers = configuration[nameof(BootstrapServers)];
            var consumerConfigurations = configuration.GetSection(nameof(ConsumerSettings)).GetChildren();
            var consumerSettings = new List<ConsumerSetting>();
            foreach (var consumerConfiguration in consumerConfigurations)
            {
                var consumerSetting = ConsumerSetting.MapValue(consumerConfiguration, bootstrapServers);
                if (!consumerSettings.Contains(consumerSetting))
                    consumerSettings.Add(consumerSetting);
            }

            var producerConfigurations = configuration.GetSection(nameof(ProducerSettings)).GetChildren();
            var producerSettings = new List<ProducerSetting>();
            foreach (var producerConfiguration in producerConfigurations)
            {
                var producerSetting = ProducerSetting.MapValue(producerConfiguration, bootstrapServers);
                if (!producerSettings.Contains(producerSetting))
                    producerSettings.Add(producerSetting);
            }

            var setting = new AppSetting
            {
                ServiceVersion = serviceVersion,
                OtlpHeaders = otlpHeaders,
                OtlpUrl = otlpUrl,
                ServiceNameSpace = serviceNameSpace,
                SecuritiesDBConnStr = securitiesDbConnStr,
                TrdDBConnStr = trdDBConnStr,
                SecuritiesDBDefaultTableSchema = securitiesDBDefaultTableSchema,
                TrdDBDefaultTableSchema = trdDBDefaultTableSchema,
                AccountApiUri = accountApiUri,
                BootstrapServers = bootstrapServers,
                AllowConsoleLog = allowConsoleLog,
                InstanceNumber = instanceNumber,
                TotalInstances = totalInstances,
                ConsumerSettings = consumerSettings.ToArray(),
                ProducerSettings = producerSettings.ToArray()
            };
            return setting;
        }

        public ConsumerSetting GetConsumerSetting(string id)
            => ConsumerSettings.FirstOrDefault(consumerSetting => consumerSetting.Id.Equals(id));

        public ProducerSetting GetProducerSetting(string id)
            => ProducerSettings.FirstOrDefault(producerSetting => producerSetting.Id.Equals(id));
    }
}

namespace GeometryAndExchangeRate.Service.Clients {
    using System.Data;
    using System.Xml;
    using GeometryAndExchangeRate.Service.Models;
    using GeometryAndExchangeRate.Service.ServiceReference;

    class DwsClient : IExchangeRateService {
        readonly string currencyCode;
        public DwsClient(string currencyCode) {
            this.currencyCode = currencyCode;
        }
        public async Task<ExchangeRate> GetExchangeRateAsync(DateTime onDate) => CreateModel(ReadExchangeRateXml(await GetExchangeRateXmlAsync(onDate).ConfigureAwait(false)));

        async Task<string> GetExchangeRateXmlAsync(DateTime onDate) {
            using(var client = new DailyInfoSoapClient(DailyInfoSoapClient.EndpointConfiguration.DailyInfoSoap)) {
                var cursOnDate = await client.GetCursOnDateXMLAsync(onDate).ConfigureAwait(false);
                return cursOnDate.OuterXml;
            }
        }

        DataSet ReadExchangeRateXml(string xml) {
            var dataSet = new DataSet();
            using(var reader = new XmlTextReader(xml, XmlNodeType.Element, null)) {
                dataSet.ReadXml(reader);
            }
            return dataSet;
        }

        ExchangeRate CreateModel(DataSet dataSet) {
            var tValuteCursOnDate = dataSet.Tables["ValuteCursOnDate"];
            if(tValuteCursOnDate == null) {
                throw new InvalidOperationException("Unexpected server response: the ValueCursOnDate table does not exist. Server: DWS, resource: GetCursOnDateXML");
            }

            var valuteCursOnDate = from curs in tValuteCursOnDate.Rows.Cast<DataRow>()
                                    where (string)curs["VchCode"] == currencyCode
                                    select new ExchangeRate {
                                        CurrencyName = ((string)curs["Vname"]).Trim(),
                                        Value = decimal.Parse((string)curs["Vcurs"]) / decimal.Parse((string)curs["Vnom"]) 
                                    };

            return valuteCursOnDate.Single();       
        }
    }
}
namespace GeometryAndExchangeRate.Service.ServiceReference {
    using System.ServiceModel;
    using System.ServiceModel.Description;

    public partial class DailyInfoSoapClient {
        public static string? DwsApiAddress;
        static partial void ConfigureEndpoint(ServiceEndpoint serviceEndpoint, ClientCredentials clientCredentials) {
            if(!string.IsNullOrEmpty(DwsApiAddress))
                serviceEndpoint.Address = new EndpointAddress(DwsApiAddress);
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection {
    using Microsoft.Extensions.Options;
    using GeometryAndExchangeRate.Service;
    using GeometryAndExchangeRate.Service.Clients;
    using GeometryAndExchangeRate.Service.ServiceReference;
    static class DwsClientExtensions {
        public static IServiceCollection AddDwsClient(this IServiceCollection services, IConfiguration configuration) { 
            services.AddSingleton<IExchangeRateService>(serviceProvider => {
                var options = serviceProvider.GetRequiredService<IOptions<DwsClientOptions>>();
                return new DwsClient(options.Value.CurrencyCode);
            });
            services.Configure<DwsClientOptions>(configuration.GetSection(DwsClientOptions.Key));
            return services;
        }
        public static IApplicationBuilder UseDwsClient(this IApplicationBuilder builder) {
            var options = builder.ApplicationServices.GetRequiredService<IOptions<DwsClientOptions>>();
            DailyInfoSoapClient.DwsApiAddress = options.Value.DwsApiAddress;
            return builder;
        }
    }
    class DwsClientOptions {
        public const string Key = "DwsClient";
        public string DwsApiAddress { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
    }
}

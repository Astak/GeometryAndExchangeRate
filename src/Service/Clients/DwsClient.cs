namespace GeometryAndExchangeRate.Service.Clients {
    using System.Data;
    using System.Xml;
    using System.Globalization;
    using GeometryAndExchangeRate.Service.Models;
    using GeometryAndExchangeRate.Service.ServiceReference;
    using GeometryAndExchangeRate.Service.ErrorHandling;

    class DwsClient : IExchangeRateService {
        readonly string currencyCode;
        public DwsClient(string currencyCode) {
            this.currencyCode = currencyCode;
        }
        public async Task<ExchangeRate> GetExchangeRateAsync(DateTime onDate) => CreateModel(ReadExchangeRateXml(await GetExchangeRateXmlAsync(onDate).ConfigureAwait(false)), onDate);

        static async Task<string> GetExchangeRateXmlAsync(DateTime onDate) {
            using(var client = new DailyInfoSoapClient(DailyInfoSoapClient.EndpointConfiguration.DailyInfoSoap)) {
                var cursOnDate = await client.GetCursOnDateXMLAsync(onDate).ConfigureAwait(false);
                return cursOnDate.OuterXml;
            }
        }

        static DataSet ReadExchangeRateXml(string xml) {
            var dataSet = new DataSet();
            using(var reader = new XmlTextReader(xml, XmlNodeType.Element, null)) {
                dataSet.ReadXml(reader);
            }
            return dataSet;
        }

        ExchangeRate CreateModel(DataSet dataSet, DateTime requestedDate) {
            var tValuteData = dataSet.Tables["ValuteData"];
            var tValuteCursOnDate = dataSet.Tables["ValuteCursOnDate"];
            if(tValuteCursOnDate == null || tValuteData == null) {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, "Unexpected server response: the ValueCursOnDate or ValuteData table does not exist. Server: {0}, resource: GetCursOnDateXML", DailyInfoSoapClient.DwsApiAddress));
            }

            var valuteData = (from data in tValuteData.Rows.Cast<DataRow>()
                             select new {
                                ValuteData_Id = (int)data["ValuteData_Id"],
                                OnDate = (string)data["OnDate"]
                             }).ToList();
            var valuteCursOnDate = (from curs in tValuteCursOnDate.Rows.Cast<DataRow>()
                                   select new {
                                    ValuteData_Id = (int)curs["ValuteData_Id"],
                                    VchCode = (string)curs["VchCode"],
                                    Vname = ((string)curs["Vname"]).Trim(),
                                    Vcurs = decimal.Parse((string)curs["Vcurs"]),
                                    Vnom = decimal.Parse((string)curs["Vnom"])
                                   }).ToList();

            string requestedDateCode = requestedDate.ToString("yyyyMMdd");
            string? onDateCode = valuteData.Select(x => x.OnDate).SingleOrDefault();
            if(requestedDateCode != onDateCode) {
                throw new NoExchangeRateDataOnDateException(requestedDateCode, onDateCode);
            }
            
            var exchangeRates = from curs in valuteCursOnDate
                         where curs.VchCode == currencyCode
                         select new ExchangeRate {
                            CurrencyName = curs.Vname,
                            Value = curs.Vcurs / curs.Vnom,
                         };
            var result = exchangeRates.Single();       

            if(result == null) {
                throw new NoExchangeRateDataForCurrencyException(currencyCode);
            }
            return result;
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

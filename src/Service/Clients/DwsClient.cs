using System.Data;
using System.Xml;
using GeometryAndExchangeRate.Service.Models;
using GeometryAndExchangeRate.Service.ServiceReference;

namespace GeometryAndExchangeRate.Service.Clients;

class DwsClient : IExchangeRateService {
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
                                where (string)curs["VchCode"] == "AUD"
                                select new ExchangeRate {
                                    CurrencyName = ((string)curs["Vname"]).Trim(),
                                    Value = decimal.Parse((string)curs["Vcurs"]) / decimal.Parse((string)curs["Vnom"]) 
                                };

        return valuteCursOnDate.Single();       
    }
}
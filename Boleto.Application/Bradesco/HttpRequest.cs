using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Boleto.Domain.Entidades;
using Boleto.Domain.Application;

namespace Boleto.Application.Bradesco
{
    internal class HttpRequest : IHttpRequest
    {
        public Resposta Post(Requisicao serviceRequest)
        {
            var urlBradesco = ConfigurationManager.AppSettings["URL_BRADESCO"];
            var chaveSeguranca = ConfigurationManager.AppSettings["ChaveSeguranca"];
            const string mediaType = "application/json";
            const string charSet = "UTF-8";

            //Conteudo da requisição
            var jsonRequest = JsonConvert.SerializeObject(serviceRequest);
            var data = Encoding.UTF8.GetBytes(jsonRequest);

            //Configuração do cabeçalho da requisição
            var request = (HttpWebRequest)WebRequest.Create(urlBradesco);
            request.Method = "POST";
            request.ContentType = mediaType + ";charset=" + charSet;
            request.ContentLength = data.Length;
            request.Accept = mediaType;
            request.Headers.Add(HttpRequestHeader.AcceptCharset, charSet);

            //Credenciais de Acesso
            var header = serviceRequest.merchant_id + ":" + chaveSeguranca;
            var headerBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(header));
            request.Headers.Add(HttpRequestHeader.Authorization, "Basic " + headerBase64);
            request.GetRequestStream().Write(data, 0, data.Length);
            var response = (HttpWebResponse)request.GetResponse();

            //Verifica resposta do servidor
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
                throw new Exception("Retorno da requisicao dif 200/201. HttpStatusCode: " + response.StatusCode);

            //Obtem a resposta do servidor
            using (var jsonTextReader = new JsonTextReader(new StreamReader(response.GetResponseStream())))
                return new JsonSerializer().Deserialize<Resposta>(jsonTextReader);
        }
    }
}
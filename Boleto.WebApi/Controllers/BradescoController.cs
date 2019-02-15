using System.Net;
using System.Net.Http;
using System.Web.Http;
using Boleto.Application.Bradesco;
using Boleto.Infra.Security;
using Boleto.Domain.Entidades;
using Boleto.Domain.Application;

namespace Boleto.WebApi.Controllers
{
    public class BradescoController : ApiController
    {
        private readonly IBradescoApplication _bradescoApplication;

        public BradescoController()
        {
            _bradescoApplication = new BradescoApplication();
        }
        //método de teste, é executado automaticamente ao rodar o sistema
        [HttpGet]
        public string Index()
        {
            return "ok";
        }

        // GET bradesco/check?numero_pedido=&token=
        [HttpGet]
        public HttpResponseMessage Check(string numero_pedido, string token)
        {
            return _bradescoApplication.Check(numero_pedido, token) ?
                new HttpResponseMessage(HttpStatusCode.OK) :
                new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        // POST bradesco/generate
        [HttpPost]
        public Resposta Generate([FromBody]Requisicao requisicao)
        {
            return _bradescoApplication.Generate(requisicao);
        }

        // GET bradesco/GetUrlBoleto?orderId=
        [HttpGet]
        [FilterIp(
            ConfigurationKeyAllowedSingleIPs = "AllowedAdminSingleIPs",
            ConfigurationKeyAllowedMaskedIPs = "AllowedAdminMaskedIPs",
            ConfigurationKeyDeniedSingleIPs = "DeniedAdminSingleIPs",
            ConfigurationKeyDeniedMaskedIPs = "DeniedAdminMaskedIPs")]
        public string GetUrlBoleto(string orderId)
        {
            return _bradescoApplication.GetUrlBoleto(orderId);
        }
    }
}
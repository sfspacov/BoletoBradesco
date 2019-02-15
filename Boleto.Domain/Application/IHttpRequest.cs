using Boleto.Domain.Entidades;

namespace Boleto.Domain.Application
{
    public interface IHttpRequest
    {
        Resposta Post(Requisicao serviceRequest);
    }
}

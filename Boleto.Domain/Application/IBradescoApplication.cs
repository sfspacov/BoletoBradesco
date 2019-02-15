using Boleto.Domain.Entidades;
using System.Collections.Generic;

namespace Boleto.Domain.Application
{
    public interface IBradescoApplication
    {
        string GetUrlBoleto(string orderId);
        IEnumerable<JsonContent> GetRequests(string document);
        bool Check(string numero_pedido, string token);
        Resposta Generate(Requisicao requisicao);
    }
}

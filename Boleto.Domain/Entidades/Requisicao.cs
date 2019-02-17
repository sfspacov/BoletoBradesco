namespace Boleto.Domain.Entidades
{
    public class Requisicao
    {
        public BoletoRequest boleto { get; set; }
        public Comprador comprador { get; set; }
        public string meio_pagamento { get; set; }
        public string merchant_id { get; set; }
        public Pedido pedido { get; set; }
        public string token_request_confirmacao_pagamento { get; set; }
    }
}
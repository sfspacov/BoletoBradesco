namespace Boleto.Domain.Entidades
{
    public class Resposta
    {

        public BoletoResponse boleto { get; set; }
        public string meio_pagamento { get; set; }
        public string merchant_id { get; set; }
        public Pedido pedido { get; set; }
        public Status status { get; set; }
    }
}

namespace Boleto.Infra.DTO
{
    public class Registro
    {
        public string agencia_pagador { get; set; }
        public bool aplicar_multa { get; set; }
        public string conta_pagador { get; set; }
        public string controle_participante { get; set; }
        public string data_limite_concessao_desconto { get; set; }
        public bool debito_automatico { get; set; }
        public string endereco_debito_automatico { get; set; }
        public string especie_titulo { get; set; }
        public string primeira_instrucao { get; set; }
        public bool rateio_credito { get; set; }
        public string razao_conta_pagador { get; set; }
        public string segunda_instrucao { get; set; }
        public string sequencia_registro { get; set; }
        public string tipo_inscricao_pagador { get; set; }
        public string tipo_ocorrencia { get; set; }
        public int valor_abatimento { get; set; }
        public int valor_desconto { get; set; }
        public int valor_desconto_bonificacao { get; set; }
        public int valor_iof { get; set; }
        public int valor_juros_mora { get; set; }
        public int valor_percentual_multa { get; set; }
    }
}
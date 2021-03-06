﻿using System.ComponentModel.DataAnnotations;

namespace Boleto.Domain.Entidades
{
    public class BoletoRequest
    {
        [Required]
        public string beneficiario { get; set; }
        public string carteira { get; set; }
        public string data_emissao { get; set; }
        public string data_vencimento { get; set; }
        public Instrucoes instrucoes { get; set; }
        public string mensagem_cabecalho { get; set; }
        public string nosso_numero { get; set; }
        public Registro registro { get; set; }
        public string tipo_renderizacao { get; set; }
        public string url_logotipo { get; set; }
        public string valor_titulo { get; set; }
    }
}
namespace Boleto.Infra.DTO
{
    public class BoletoResponse
    {
        public string data_atualizacao { get; set; }
        public string data_geracao { get; set; }
        public string linha_digitavel { get; set; }
        public string linha_digitavel_formatada { get; set; }
        public string token { get; set; }
        public string url_acesso { get; set; }
        public string valor_titulo { get; set; }
    }
}

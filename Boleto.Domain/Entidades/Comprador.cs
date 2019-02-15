namespace Boleto.Domain.Entidades
{
    public class Comprador
    {
        public string documento { get; set; }
        public Endereco endereco { get; set; }
        public string ip { get; set; }
        public string nome { get; set; }
        public string user_agent { get; set; }
    }
}
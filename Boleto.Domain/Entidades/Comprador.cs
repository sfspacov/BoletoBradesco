using System.ComponentModel.DataAnnotations;

namespace Boleto.Domain.Entidades
{
    public class Comprador
    {
        [Required]
        public string documento { get; set; }
        public Endereco endereco { get; set; }
        public string ip { get; set; }
        [Required]
        public string nome { get; set; }
        public string user_agent { get; set; }
    }
}
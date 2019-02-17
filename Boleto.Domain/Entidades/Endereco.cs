using System.ComponentModel.DataAnnotations;

namespace Boleto.Domain.Entidades
{
    public class Endereco
    {
        [Required]
        public string bairro { get; set; }
        [Required]
        public string cep { get; set; }
        [Required]
        public string cidade { get; set; }
        public string complemento { get; set; }
        [Required]
        public string logradouro { get; set; }
        [Required]
        public string numero { get; set; }
        [Required]
        public string uf { get; set; }
    }
}
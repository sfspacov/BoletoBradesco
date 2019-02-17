using System.ComponentModel.DataAnnotations;

namespace Boleto.Domain.Entidades
{
    public class Pedido
    {
        [Required]
        public string descricao { get; set; }
        [Required]
        public string numero { get; set; }
        [Required]
        public string valor { get; set; }
    }
}

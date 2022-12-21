using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ValesFise_api.Modelos
{
    public class ME_Agentes
    {
        public char Empresa { get; set; }
        public string Celular { get; set; }
        public string Nombre { get; set; }
        public string Denominacion { get; set; }
        public short CapacidadDiaria { get; set; }
        public bool Activo { get; set; }
        public string meUsuario { get; set; }
        public string meEstacion { get; set; }
        public DateTime? meFecha { get; set; }
        public string meCliente { get; set; }
    }

    public class ME_Ticket
    {
        [Key]
        public int IdTicket { get; set; }
        public int ticket { get; set; }
        public short Canal { get; set; }
        public string Numero { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public string Mensaje { get; set; }
        public string Origen { get; set; }
        public char? Empresa { get; set; }
        public string Valor { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string meUsuario { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string meEstacion { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? meFecha { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string meCliente { get; set; }

    }
}

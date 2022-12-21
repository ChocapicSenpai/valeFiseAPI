using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ValesFise_api.Modelos
{
    public enum Empresas
    {
        Enosa = 1,
        Ensa = 2,
        Hdna = 3,
        Elcto = 4
    }

    public class Consulta
    {
        public string IdApp { get; set; }
        public string Dni { get; set; }
    }

    public class Dni
    {
        [Key]
        public int IdDJ { get; set; }
        public short IdEmpresa { get; set; }
        public string NroIdentidad { get; set; }
    }

    public class Vale
    {
        [Key]
        public int IdVale { get; set; }
        public int? IdDJ { get; set; }
        public short? IdEmpresa { get; set; }
        public string Dni { get; set; }
        public int Periodo { get; set; }
        public string NroVale { get; set; }
        public string TipoGen { get; set; }
        public string Fcaducidad { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using ValesFise_api.Contexto;
using ValesFise_api.Modelos;

namespace ValesFise_api.Servicios
{
    public class DatosFiseNGC
    {
        private readonly IConfiguration _configuration;
        private readonly AppNGCDbContext _contextoNGC;

        public DatosFiseNGC(AppNGCDbContext contextoNGC, IConfiguration config)
        {
            _configuration = config;
            _contextoNGC = contextoNGC;
        }

        public Dni ConsultarDni(string dni)
        {
            FormattableString sql = @$"Select IdDJ, IdEmpresa, NroIdentidad From Fise.DeclaracionJurada Where idestado != 2 And idtipoidentidad = 1 And NroIdentidad = {dni}";
            var Dni = _contextoNGC.Dni.FromSqlInterpolated(sql).ToList().FirstOrDefault();

            return Dni;
        }

        public IEnumerable<Vale> ConsultarVale(string dni, ME_Agentes agente, int nroConsultas)
        {
            var valesOut = _configuration.GetSection("SMS:ValesOut").Value;

            FormattableString sql = @$"Select v.IdVale, v.IdDj, v.IdEmpresa, v.NroIdentidad As Dni, v.Periodo, v.NroVale, 
                        c.Descripcion As TipoGen, Convert(Char(10),v.FechaCaducidad,103) as Fcaducidad 
                        From fise.Vale v 
                        Join fise.ValeGeneracion vg on v.idgeneracion = vg.idgeneracion 
                        Join Constantes c on c.id = vg.IdTipoGen 
                        Where v.IdEstado = 170 And v.IdTipoIdentidad = 1 And v.NroIdentidad = {dni} 
                        And v.FechaCaducidad >= Cast(Getdate() as Date) And v.Periodo <= {valesOut}
                        And Left(v.NroVale,2) = '06' 
                        And v.IdVale Not in (Select idvale From fise.LoteValeDetalle d Where d.IdVale = v.IdVale And d.IdEstado = 1) 
                        Order by v.Periodo, c.Descripcion";

            IEnumerable<Vale> vales = _contextoNGC.Vale.FromSqlInterpolated(sql).ToList();

            foreach (var val in vales)
            {
                if (agente != null)
                {
                    bool activo = agente.Activo;
                    short nroCapacidad = agente.CapacidadDiaria;

                    if (nroConsultas > nroCapacidad || activo == false)
                        val.NroVale = val.NroVale.Substring(0, 6) + "******" + val.NroVale.Substring(val.NroVale.Length - 1);
                }
                else
                {
                    val.NroVale = val.NroVale.Substring(0, 6) + "******" + val.NroVale.Substring(val.NroVale.Length - 1);
                }
            }

            return vales;
        }


        //public Vale Consultar(string telefono, string dni, Agente agente, int nroConsultas)
        //{
        //    int periodo = 0;
        //    string tipogen = "";
        //    string valeTmp = "";
        //    var vale = new Vale();
        //    vale.Dni = dni;
        //    var valesOut = _configuration.GetSection("SMS:ValesOut").Value;

        //    FormattableString sql = @$"Select IdDJ, IdEmpresa, NroIdentidad From Fise.DeclaracionJurada Where idestado != 2 And idtipoidentidad = 1 And NroIdentidad = {dni}";
        //    var Dni = _contextoNGC.Dni.FromSqlInterpolated(sql).ToList().FirstOrDefault();

        //    if(Dni != null)
        //    {
        //        vale.Existe = true;
        //        sql =   @$"Select v.IdVale, v.IdDj, v.IdEmpresa, v.NroIdentidad As Dni, v.Periodo, v.NroVale, 
        //                c.Descripcion As TipoGen, Convert(Char(10),v.FechaCaducidad,103) as Fcaducidad 
        //                From fise.Vale v 
        //                Join fise.ValeGeneracion vg on v.idgeneracion = vg.idgeneracion 
        //                Join Constantes c on c.id = vg.IdTipoGen 
        //                Where v.IdEstado = 170 And v.IdTipoIdentidad = 1 And v.NroIdentidad = {dni} 
        //                And v.FechaCaducidad >= Cast(Getdate() as Date) And Substring(v.NroVale, 3, 4) <= {valesOut} 
        //                And Left(v.NroVale,2) = '06' 
        //                And v.IdVale Not in (Select idvale From fise.LoteValeDetalle d Where d.IdVale = v.IdVale And d.IdEstado = 1) 
        //                Order by v.Periodo, c.Descripcion";

        //        IEnumerable<Vale> vales = _contextoNGC.Vale.FromSqlInterpolated(sql).ToList();

        //        if(vales.Count() > 0)
        //        {
        //            foreach(var val in vales)
        //            {
        //                if(val.Periodo != periodo)
        //                {
        //                    vale.NroVale += Environment.NewLine + "Vale " + Funciones.FormateaPeriodo(val.Periodo.ToString()) + Environment.NewLine;
        //                    vale.NroVale += val.TipoGen + " : " + Environment.NewLine;
        //                    periodo = val.Periodo;
        //                    tipogen = val.TipoGen;
        //                }
        //                if(tipogen != val.TipoGen)
        //                {
        //                    vale.NroVale += val.TipoGen + " : " + Environment.NewLine;
        //                    tipogen = val.TipoGen;
        //                }

        //                if(agente != null )
        //                {
        //                    bool activo = agente.Activo;
        //                    short nroCapacidad = agente.CapacidadDiaria;

        //                    if (nroConsultas < nroCapacidad && activo)
        //                        valeTmp = val.NroVale;
        //                    else
        //                        valeTmp = val.NroVale.Substring(0, 6) + "******" + val.NroVale.Substring(val.NroVale.Length - 1);
        //                }
        //                else
        //                {
        //                    valeTmp = val.NroVale.Substring(0, 6) + "******" + val.NroVale.Substring(val.NroVale.Length - 1);
        //                }
        //                vale.NroVale += valeTmp + " Vcto: " + val.Fcaducidad + Environment.NewLine;
        //            }
        //        }
        //        else
        //        {
        //            vale.NroVale = "No se ha encontrado vales fise";
        //        }

        //    }

        //    return vale;
        //}

    }
}

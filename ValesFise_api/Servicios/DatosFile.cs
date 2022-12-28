using Microsoft.EntityFrameworkCore;
using ValesFise_api.Contexto;
using ValesFise_api.Modelos;

namespace ValesFise_api.Servicios
{
    public class DatosFile
    {
        private readonly AppFileDbContext _contextoFile;

        public DatosFile(AppFileDbContext contextoFile)
        {
            _contextoFile = contextoFile;
        }
        public ME_Agentes ObtenerAgente(string telefono, int empresa)
        {
            ME_Agentes Agentes = _contextoFile.ME_Agentes.Where(x => x.Empresa.ToString() == empresa.ToString() && x.Celular == telefono).ToList().FirstOrDefault();

            //FormattableString sql = @$"Select Empresa, Celular, Nombre, Denominacion, CapacidadDiaria, Activo, meUsuario, meEstacion, meFecha, meCliente  
            //From ME_Agentes Where Empresa = {empresa} And Celular = {telefono}";
            //ME_Agentes Agentes = _contextoFile.ME_Agentes.FromSqlInterpolated(sql).ToList().FirstOrDefault();

            return Agentes;
        }

        public int ConsultasAgente(string telefono)
        {
            IEnumerable<ME_Ticket> ticket = _contextoFile.ME_Ticket.Where(x => x.Origen == "FISE"
            && x.Empresa.ToString() != ""
            && x.Empresa != null
            && x.Numero == telefono
            && x.FechaEnvio.Value.Date == DateTime.Now.Date).ToList();

            //FormattableString sql = @$"Select IdTicket, Ticket, Canal, Numero, FechaEnvio, Mensaje, Origen, Empresa, Valor, meUsuario, meEstacion, meFecha, meCliente  
            //                    From ME_Ticket 
            //                    Where Origen = 'FISE' And Isnull(Empresa,'') <> '' And Numero = {telefono} And Cast(FechaEnvio as date) = Cast(Getdate() as date)";

            //IEnumerable<ME_Ticket> ticket = _contextoFile.ME_Ticket.FromSqlInterpolated(sql).ToList();

            return ticket.Count();
        }

        public void GrabarTicket(string telefono, string dni, string empresa)
        {
            var ticket = new ME_Ticket();
            ticket.ticket = 0;
            ticket.Canal = 0;
            ticket.Numero = telefono;
            ticket.FechaEnvio = DateTime.Now;
            ticket.Mensaje = dni;
            ticket.Valor = dni;
            ticket.Origen = "FISE";
            ticket.Empresa = Convert.ToChar(empresa);

            _contextoFile.ME_Ticket.Add(ticket);
            _contextoFile.SaveChanges();

            //FormattableString sql = @$"Insert Into ME_Ticket (Ticket, Canal, Numero, FechaEnvio, Mensaje, Valor, Origen, Empresa) 
            //                        Values (0, 0, {telefono}, {DateTime.Now}, {dni}, {dni},'FISE', {empresa} );";

            //await _contextoFile.Database.ExecuteSqlInterpolatedAsync(sql);

        }

        public void GrabarToken(ME_Agentes agente)
        {
            _contextoFile.Attach(agente);
            _contextoFile.Entry(agente).Property(x => x.Codigo).IsModified = true;
            _contextoFile.SaveChanges();
                        
        }

    }

}

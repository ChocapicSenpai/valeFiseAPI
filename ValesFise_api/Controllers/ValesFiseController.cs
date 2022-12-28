using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Claims;
using ValesFise_api.Contexto;
using ValesFise_api.Modelos;
using ValesFise_api.Servicios;

namespace ValesFise_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ValesFiseController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppNGCDbContext _contextoNGC;
        private readonly AppFileDbContext _contextoFile;

        public ValesFiseController(IConfiguration config, AppNGCDbContext contextoNGC, AppFileDbContext contextoFile)
        {
            _configuration = config;
            _contextoNGC = contextoNGC;
            _contextoFile = contextoFile;
        }

        [Authorize]
        [HttpPost]
        [Route("Obtener")]
        public dynamic Obtener([FromBody] Consulta request)
        {
            var publicKey = _configuration.GetSection("JWT:PublicKey").Value;
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var telefono = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.MobilePhone).Value;
            var codigo = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.PostalCode).Value;
            var app = request.IdApp;
            var dni = request.Dni;

            //telefono = "51967044171";
            if (telefono.Length < 11 && telefono.Substring(0, 2) != "51")
            {
                telefono = "51" + telefono;
            }

            //Validamos public key
            if (app != publicKey)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new {message = "Validación incorrecta", vales = "" });
            }

            //Validamos Dni
            bool EsNumero = int.TryParse(dni, out int valornumerico);
            if(!EsNumero || dni.Length != 8)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = "No es un DNI válido. Revise su numero de documento nacional de identidad", vales = "" });
            }

            //Procesamos
            IEnumerable<Vale> vale;
            try
            {
                int nroConsultas = 0;
                var datosFile = new DatosFile(_contextoFile);
                var agente = datosFile.ObtenerAgente(telefono, (int)Empresas.Ensa);

                if (agente != null)
                {
                    if(agente.Codigo != codigo)
                        return StatusCode(StatusCodes.Status400BadRequest, new { message = "Código de verificación no es válido", vales = "" });
                    else 
                        nroConsultas = datosFile.ConsultasAgente(telefono);
                }

                var datosFiseNGC = new DatosFiseNGC(_contextoNGC, _configuration );
                var identidad = datosFiseNGC.ConsultarDni(dni);
                if(identidad == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "Dni no ubicado en el padrón de vales fise", vales = "" });
                }

                vale = datosFiseNGC.ConsultarVale(dni, agente, nroConsultas);

                string idempresa = " ";
                if(vale.Count() > 0)
                {
                    idempresa = vale.FirstOrDefault().IdEmpresa.ToString();
                    datosFile.GrabarTicket(telefono, dni, idempresa);
                }
                else
                {
                    datosFile.GrabarTicket(telefono, dni, idempresa);
                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "No se han encontrado vales fise", vales = "" });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = ex.Message, vales = "" });
            }

            return StatusCode(StatusCodes.Status200OK, new { message = "", vales = vale });
        }


        [HttpPost]
        [Route("ObtenerFree")]
        public dynamic ObtenerLibre([FromBody] Consulta request)
        {
            var publicKey = _configuration.GetSection("JWT:PublicKey").Value;
            var dni = request.Dni;
            var app = request.IdApp;

            //Validamos public key
            if (app != publicKey)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = "Validación incorrecta", vales = "" });
            }

            //Validamos Dni
            bool EsNumero = int.TryParse(dni, out int valornumerico);
            if (!EsNumero || dni.Length != 8)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = "No es un DNI válido. Revise su numero de documento nacional de identidad", vales = "" });
            }

            //Procesamos
            IEnumerable<Vale> vale;
            try
            {
                int nroConsultas = 0;
                var datosFile = new DatosFile(_contextoFile);
                ME_Agentes agente = new ME_Agentes();

                var datosFiseNGC = new DatosFiseNGC(_contextoNGC, _configuration);
                var identidad = datosFiseNGC.ConsultarDni(dni);
                if (identidad == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "Dni no ubicado en el padrón de vales fise", vales = "" });
                }

                vale = datosFiseNGC.ConsultarVale(dni, agente, nroConsultas);

                string idempresa = " ";
                if (vale.Count() > 0)
                {
                    idempresa = vale.FirstOrDefault().IdEmpresa.ToString();
                    datosFile.GrabarTicket("CONSULTA_APP", dni, idempresa);
                }
                else
                {
                    datosFile.GrabarTicket("CONSULTA_APP", dni, idempresa);
                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "No se han encontrado vales fise", vales = "" });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = ex.Message, vales = "" });
            }

            return StatusCode(StatusCodes.Status200OK, new { message = "", vales = vale });
        }
    }
}

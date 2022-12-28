using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ValesFise_api.Contexto;
using ValesFise_api.Modelos;
using ValesFise_api.Servicios;

namespace ValesFise_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly AppFileDbContext _contextoFile;

        public AutenticacionController(IConfiguration config, AppFileDbContext contextoFile)
        {
            this._configuration = config;
            this._contextoFile = contextoFile;
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult EnviarSMS([FromBody] Credencial request)
        {
            var publicKey = _configuration.GetSection("JWT:PublicKey").Value;
            var secretKey = _configuration.GetSection("JWT:SecretKey").Value;

            if (request.IdApp == publicKey)
            {
                var telefono = request.Telefono;
                if (telefono.Length < 11 && telefono.Substring(0, 2) != "51")
                {
                    telefono = "51" + telefono;
                }

                //Verificamos si es agente
                try
                {
                    var datosFile = new DatosFile(_contextoFile);
                    var agente = datosFile.ObtenerAgente(telefono, (int)Empresas.Ensa);

                    if (agente != null)
                    {
                        if(agente.Activo == false)
                        {
                            return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });
                        }
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { agente = ex.Message, token = "" });
                }

                var keyBytes = Encoding.ASCII.GetBytes(secretKey);
                var claims = new ClaimsIdentity();
                var codigo = Funciones.GenerarCodigo();
                var mensaje = "Codigo de activación Vales Fise : " + codigo;
                var rpta = MensajeSMS.Enviar(telefono, mensaje, _configuration);

                claims.AddClaim(new Claim(ClaimTypes.MobilePhone, telefono));
                claims.AddClaim(new Claim(ClaimTypes.PostalCode, codigo.ToString()));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMinutes(20),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
                string tokenCreado = tokenHandler.WriteToken(tokenConfig);

                return StatusCode(StatusCodes.Status200OK, new { token = tokenCreado });
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });
            }
        }


        [HttpPost]
        [Authorize]
        [Route("Validate")]
        public IActionResult GestionarToken([FromBody] Credencial request)
        {
            var publicKey = _configuration.GetSection("JWT:PublicKey").Value;
            var secretKey = _configuration.GetSection("JWT:SecretKey").Value;

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var phone = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.MobilePhone).Value;
            var codigo = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.PostalCode).Value;
            var telefono = request.Telefono;
            //telefono = "51968991827";
            if (telefono.Length < 11 && telefono.Substring(0, 2) != "51")
            {
                telefono = "51" + telefono;
            }

            if (request.IdApp == publicKey && phone == telefono && request.Codigo == codigo)
            {

                var keyBytes = Encoding.ASCII.GetBytes(secretKey);
                var claims = new ClaimsIdentity();

                claims.AddClaim(new Claim(ClaimTypes.MobilePhone, telefono));
                claims.AddClaim(new Claim(ClaimTypes.PostalCode, codigo));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
                string tokenCreado = tokenHandler.WriteToken(tokenConfig);

                string nomAgente = "";
                try
                {
                    //Verificamos si es agente
                    var datosFile = new DatosFile(_contextoFile);
                    var agente = datosFile.ObtenerAgente(telefono, (int)Empresas.Ensa);

                    if (agente != null)
                    {
                        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                        nomAgente = textInfo.ToTitleCase(agente.Nombre.ToLower());

                        //Guardamos el codigo de verificacion 
                        agente.Codigo = codigo;
                        datosFile.GrabarToken(agente);
                    }

                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { agente = ex.Message, token = "" });
                }

                return StatusCode(StatusCodes.Status200OK, new { agente = nomAgente, token = tokenCreado });
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { agente = "", token = "" });
            }
        }
    }
}
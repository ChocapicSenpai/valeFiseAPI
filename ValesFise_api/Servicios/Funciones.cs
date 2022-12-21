using Microsoft.AspNetCore.Mvc.Formatters;

namespace ValesFise_api.Servicios
{
    public static class Funciones
    {
        /// <summary>
        /// Retorna una descripcion del periodo solicitado
        /// </summary>
        /// <param name="Periodo"></param>
        /// <returns></returns>
        public static string FormateaPeriodo(string Periodo)
        {
            string FormateaPeriodo ="";
            if(int.TryParse(Periodo, out int valornumerico))
            {
                int Mes = Periodo.Length <= 2 ? Convert.ToInt32(Periodo) : Convert.ToInt32(Periodo.Substring(Periodo.Length - 2));
                string Ano = Periodo.PadRight(4).Substring(0, 4).Trim();

                switch (Mes)
                {
                    case 1:
                        {
                            FormateaPeriodo = "Ene-" + Ano;
                            break;
                        }

                    case 2:
                        {
                            FormateaPeriodo = "Feb-" + Ano;
                            break;
                        }

                    case 3:
                        {
                            FormateaPeriodo = "Mar-" + Ano;
                            break;
                        }

                    case 4:
                        {
                            FormateaPeriodo = "Abr-" + Ano;
                            break;
                        }

                    case 5:
                        {
                            FormateaPeriodo = "May-" + Ano;
                            break;
                        }

                    case 6:
                        {
                            FormateaPeriodo = "Jun-" + Ano;
                            break;
                        }

                    case 7:
                        {
                            FormateaPeriodo = "Jul-" + Ano;
                            break;
                        }

                    case 8:
                        {
                            FormateaPeriodo = "Ago-" + Ano;
                            break;
                        }

                    case 9:
                        {
                            FormateaPeriodo = "Sep-" + Ano;
                            break;
                        }

                    case 10:
                        {
                            FormateaPeriodo = "Oct-" + Ano;
                            break;
                        }

                    case 11:
                        {
                            FormateaPeriodo = "Nov-" + Ano;
                            break;
                        }

                    case 12:
                        {
                            FormateaPeriodo = "Dic-" + Ano;
                            break;
                        }

                    default:
                        {
                            FormateaPeriodo = "";
                            break;
                        }
                }
            }

            return FormateaPeriodo;
        }

        /// <summary>
        /// Genera un codigo random de 4 digitos
        /// </summary>
        /// <returns></returns>
        public static string GenerarCodigo()
        {
            string valor = "";
            var seed = Environment.TickCount;
            var random = new Random(seed);

            for (int i = 1; i <= 4; i++)
            {
                valor += random.Next(0, 10);

            }
            return valor;
        }

    }
}

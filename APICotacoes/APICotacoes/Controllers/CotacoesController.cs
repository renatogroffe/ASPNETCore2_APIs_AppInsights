using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using Dapper;

namespace APICotacoes.Controllers
{
    [Route("api/[controller]")]
    public class CotacoesController : Controller
    {
        [HttpGet("{id}")]
        public IActionResult Get(
            [FromServices]IConfiguration config, string id)
        {
            Cotacao resultado;

            using (SqlConnection conexao = new SqlConnection(
                config.GetConnectionString("BaseCotacoes")))
            {
                resultado = conexao.QueryFirstOrDefault<Cotacao>(
                    "SELECT * FROM dbo.Cotacoes " +
                    "WHERE Sigla = @idMoeda", new { idMoeda = id });
            }

            if (resultado != null)
            {
                TelemetryClient client = new TelemetryClient();
                Dictionary<string, string> dados = new Dictionary<string, string>();
                dados["DadosCotacao"] = JsonConvert.SerializeObject(resultado);
                client.TrackEvent("Dapper", dados);

                return new ObjectResult(resultado);
            }
            else
            {
                return NotFound(
                    new
                    {
                        Mensagem = "Código de serviço inválido ou item inexistente.",
                        Erro = true
                    });
            }
        }
    }
}
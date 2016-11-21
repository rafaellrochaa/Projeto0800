using System;
using DisponibilizadorDadosRotaLogAgilus.Models;
using System.Web.Http;
using System.Net.Http;
using System.Net;

namespace DisponibilizadorDadosRotaLogAgilus.Controllers
{
    public class FaseColetaController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage AtualizarFase(string status, int codigoColeta, string chave)
        {
            string retornoJson = null;

            try
            {
                if (Agilus.ValidarToken(chave)) //validação da chave de segurança
                {
                    Agilus.AtualizarStatusRotalog(status, codigoColeta);
                    retornoJson = @"{""StatusAgilus"": ""Atualizado""}";
                }
                else
                    throw new Exception("Chave inválida");
            }
            catch (Exception e)
            {
                if (e.Message == "Chave inválida")
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        Content = new StringContent("401 Unauthorized"),
                        ReasonPhrase = "401 Unauthorized",
                        StatusCode = HttpStatusCode.Unauthorized
                    };
                    throw new HttpResponseException(resp);
                }
                else
                    retornoJson = @"{""Erro"":""" + e.Message + @"""}";
            }
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(retornoJson, System.Text.Encoding.UTF8, "application/json"),
            };
        }
    }

}
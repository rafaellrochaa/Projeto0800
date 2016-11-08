using DisponibilizadorDadosRotaLogAgilus.Models;
using Newtonsoft.Json;
using System.Net.Http;
using SysWeb = System.Web;
using System.Web.Http;

namespace DisponibilizadorDadosRotaLogAgilus.Controllers
{
    public class ConsultaAgilusController : ApiController
    {
        // GET: ConsultaAgilus
        [SysWeb.Http.HttpGet]
        public HttpResponseMessage ObterPropostasPendentes()
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new BancoDados().DadosConvenioAgilus()), System.Text.Encoding.UTF8, "application/json")
            };
        }
        [SysWeb.Http.HttpGet]
        public string ObterToken(string usuario, string senha)
        {
            return Agilus.GerarToken(usuario, senha);
        }

        [SysWeb.Http.HttpPost]
        public HttpResponseMessage ConfirmaPropostasAndamento()
        {
            //Mudar a fase das propostas que a formalizar já gravou, mas ainda não enviou à rotalog.
            var jsonContratosAndamento = Request.Content.ReadAsStringAsync().Result;
            
            new BancoDados().AtualizaPropostasPendentes(jsonContratosAndamento);

            return new HttpResponseMessage()
            {
                Content = new StringContent(@"{""Status Agilus"": ""Os Contratos enviados foram retirados da lista de pendentes.""}", System.Text.Encoding.UTF8, "application/json")
            };
        }
       
        [SysWeb.Http.HttpGet]
        public HttpResponseMessage ConfirmaPropostaEnviadaRotalog(int codColeta, string contrato)
        {
            //Atualiza fase direto
            new BancoDados().AtualizaPropostaEnviadaRotalog(codColeta, contrato);

            return new HttpResponseMessage()
            {
                Content = new StringContent(@"{""Status Agilus"": ""Confirmação de envio de proposta rotalog gravado no agilus.""}", System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}
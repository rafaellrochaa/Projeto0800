using Newtonsoft.Json;
using System.Collections.Generic;

namespace DisponibilizadorDadosRotaLogAgilus.Models
{
    public class Proposta
    {
        [JsonIgnore]
        public long CodigoCliente { get; set; }
        public string ProposalId { get; set; } //Número proposta
        public string CPF { get; set; }
        public string Rg { get; set; }
        public string SignerName { get; set; } //Nome
        public string BirthDate { get; set; }
        public string Gender { get; set; } //Sexo (M ou F)
        public List<string> Phones { get; set; }
        public string Address { get; set; }
        public string Complement { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public long AgreementId { get; set; } //Código do órgão no agilus
        public string ReferencePoint { get; set; }
        public string ScheduleDate { get; set; }
        public char SchedulePeriod { get; set; }
        public string CollectType { get; set; }
        public int CollectTypeId { get; set; }
        public Proposta()
        {
            Phones = new List<string>();
        }
    }
}

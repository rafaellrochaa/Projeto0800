using System.IO;
namespace DisponibilizadorDadosRotaLogAgilus.Models
{
    public class DocumentoRetornoRotaLog
    {
        public MemoryStream Documento { get; set; }
        public string NomeDocumento { get; set; }
    }
}
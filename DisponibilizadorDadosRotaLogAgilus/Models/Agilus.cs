using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;

namespace DisponibilizadorDadosRotaLogAgilus.Models
{
    public static class Agilus
    {
        public static void AtualizarStatusRotalog(string status, int codigoColeta)
        {
            new BancoDados().AtualizarFaseAfRotalog(status, codigoColeta);
        }

        public static void AtualizarStatusAgilus(int fase, string codigoColeta)
        {
            new BancoDados().AtualizarFaseAfAgilus(fase, codigoColeta);
        }
        public static void DownloadAnexos(int codigoColeta)
        {
            Agilus.AtualizarStatusAgilus(16, codigoColeta.ToString()); //Fase Mapeada no banco de dados como "Baixando documentos"
            foreach (var anexo in DownloadAnexosRotaLog(codigoColeta))
            {
                try
                {
                    new BancoDados().GravarAnexo(codigoColeta.ToString(), anexo.Documento, anexo.NomeDocumento);
                }
                catch (Exception)
                {
                    throw;
                }
            } 
        }
        private static List<DocumentoRetornoRotaLog> DownloadAnexosRotaLog(int codigoColeta)
        {
            var documentosBaixados = new List<DocumentoRetornoRotaLog>();

            DataTable DocumentosDownload = new RotaLog.WebServiceRotaSoapClient().ConsultaCaminhoDocumentos("USU00480", "ag12345", codigoColeta).Tables[0];//Não tenho usuário e senha do webservice rotalog

            if(DocumentosDownload.Columns.Contains("Tipo"))
            {
                throw new Exception(DocumentosDownload.Rows[0]["Descricao"].ToString());
            }
            else
            {
                foreach (DataRow anexo in DocumentosDownload.Rows)
                {
                    string nomeArquivo = Path.GetFileName(anexo["url_arquivo"].ToString());

                    //var documento = new WebClient().DownloadData(anexo["url_arquivo"].ToString());
                    var documento = DownloadArquivo(anexo["url_arquivo"].ToString());

                    documentosBaixados.Add(new DocumentoRetornoRotaLog()
                    {
                        Documento = documento,
                        NomeDocumento = nomeArquivo
                    });
                }
            }
            return documentosBaixados;
        }
        public static string GerarToken(string usuario, string senha)
        {
            return Criptografia.Encriptar(usuario + "|" + Encode(senha));
        }
        public static bool ValidarToken(string token)
        {
            string dados = null;
            try
            {
                dados = Criptografia.Decriptar(token);
            }
            catch(Exception)
            {
                throw new Exception("Chave inválida");
            }

            string usuario = dados.Split('|')[0];
            string senha = dados.Split('|')[1];

            return new BancoDados().ValidaUsuario(usuario, senha);
        }
        private static string Encode(string value)
        {
            var hash = System.Security.Cryptography.MD5.Create();
            var encoder = new System.Text.ASCIIEncoding();
            var combined = encoder.GetBytes(value ?? "");
            return BitConverter.ToString(hash.ComputeHash(combined)).ToLower().Replace("-", "");
        }

        private static MemoryStream DownloadArquivo(string uri)
        {
            MemoryStream outputStream = new MemoryStream();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            // Check that the remote file was found. The ContentType
            // check is performed since a request for a non-existent
            // image file might be redirected to a 404-page, which would
            // yield the StatusCode "OK", even though the image was not
            // found.
            //if (response.StatusCode == HttpStatusCode.OK ||
            //    response.StatusCode == HttpStatusCode.Moved ||
            //    response.StatusCode == HttpStatusCode.Redirect)
            {


                // if the remote file was found, download oit
                using (Stream inputStream = response.GetResponseStream())
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    do
                    {
                        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                        outputStream.Write(buffer, 0, bytesRead);
                    } while (bytesRead != 0);
                }
                outputStream.Position = 0;
                outputStream.Flush();
                return outputStream;
            }

        }
    }
}




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
    }
}




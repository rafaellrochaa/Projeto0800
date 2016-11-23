using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace DisponibilizadorDadosRotaLogAgilus.Models
{
    public class BancoDados
    {
        private readonly string 
            //DataSource = "189.111.254.13,10000", //domínio agilus (teste)
            DataSource = "192.168.5.12", //Endereço servidor
            InitialCatalog = "dbAgilus", //Banco de dados 
            //InitialCatalog = "dbAgilusDEV", Banco de dados de teste
            UserID = "suporte_agilus",
            Password = "@gilus2016";

        public void AtualizarFaseAfRotalog(string status, int codigoColeta)
        {
            SqlConnection conexao = new SqlConnection(String.Format("Data Source= {0}; Initial Catalog={1}; User ID={2}; Password={3}", DataSource, InitialCatalog, UserID, Password));

            using (conexao)
            {
                conexao.Open();
                try
                {
                    var cmd = new SqlCommand("execute pr_atualizar_status_af_rotalog @statusRotalog, @codigoColetaRotaLog", conexao);
                    cmd.Parameters.Add("@statusRotalog", SqlDbType.VarChar, 255).Value = status;
                    cmd.Parameters.Add("@codigoColetaRotaLog", SqlDbType.VarChar, 50).Value = codigoColeta.ToString();

                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw new Exception("Ocorreu um problema durante a atualização das fases no banco de dados. Detalhe do erro: " + e.Message);
                }
            }
        }
        public void AtualizarFaseAfAgilus(int fase, string codigoColeta)
        {
            SqlConnection conexao = new SqlConnection(String.Format("Data Source= {0}; Initial Catalog={1}; User ID={2}; Password={3}", DataSource, InitialCatalog, UserID, Password));

            using (conexao)
            {
                conexao.Open();
                try
                {
                    var cmd = new SqlCommand("execute pr_atualizar_fase_af @faf_codigo, @codigo_coleta", conexao);
                    cmd.Parameters.Add("@faf_codigo", SqlDbType.Int).Value = fase;
                    cmd.Parameters.Add("@codigo_coleta", SqlDbType.VarChar, 50).Value = codigoColeta;

                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw new Exception("Ocorreu um problema durante a atualização das fases no banco de dados. Detalhe do erro: " + e.Message);
                }
            }
        }
        public void GravarAnexo(string codColeta, MemoryStream anexo, string nomeArquivo)
        {
            SqlConnection conexao = new SqlConnection(String.Format("Data Source= {0}; Initial Catalog={1}; User ID={2}; Password={3}", DataSource, InitialCatalog, UserID, Password));

            using (conexao)
            {
                conexao.Open();

                try
                {
                    var cmd = new SqlCommand("execute pr_grava_anexo_rotalog @anexo, @nome_arquivo, @codigo_coleta", conexao);

                    cmd.Parameters.Add("@codigo_coleta", SqlDbType.VarChar, 50).Value = codColeta;
                    cmd.Parameters.Add("@nome_arquivo", SqlDbType.VarChar, 200).Value = nomeArquivo;
                    cmd.Parameters.Add("@anexo", SqlDbType.VarBinary, -1).Value = anexo;

                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw new Exception("Ocorreu um problema durante a atualização das fases no banco de dados. Detalhe do erro: " + e.Message);
                }
            }
        }

        public void GravarImagemProposta(string codigoProposta, byte[] anexo, string nomeArquivo)
        {
            SqlConnection conexao = new SqlConnection(String.Format("Data Source= {0}; Initial Catalog={1}; User ID={2}; Password={3}", DataSource, InitialCatalog, UserID, Password));

            using (conexao)
            {
                conexao.Open();

                try
                {
                    var cmd = new SqlCommand("execute pr_grava_imagem_proposta @anexo, @nome_arquivo, @codigo_proposta", conexao);

                    cmd.Parameters.Add("@codigo_proposta", SqlDbType.VarChar, 50).Value = codigoProposta;
                    cmd.Parameters.Add("@nome_arquivo", SqlDbType.VarChar, 200).Value = nomeArquivo;
                    cmd.Parameters.Add("@anexo", SqlDbType.VarBinary, -1).Value = anexo;

                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw new Exception("Ocorreu um problema durante a atualização das fases no banco de dados. Detalhe do erro: " + e.Message);
                }
            }
        }
        public List<Proposta> DadosConvenioAgilus()
        {
            SqlConnection conexao = new SqlConnection(String.Format("Data Source= {0}; Initial Catalog={1}; User ID={2}; Password={3}", DataSource, InitialCatalog, UserID, Password));
            List<Proposta> docs = new List<Proposta>();
            using (conexao)
            {
                conexao.Open();
                try
                {
                    var dr = new SqlCommand(String.Format("execute pr_documentos"), conexao).ExecuteReader();
                    while (dr.Read())
                    {

                        docs.Add(new Proposta()
                        {
                            CodigoCliente = Convert.ToInt16(dr["con_codigo"]),
                            ProposalId = dr["ProcessId"].ToString(),
                            CPF = dr["Cpf"].ToString(),
                            Rg = dr["Rg"].ToString(),
                            SignerName = dr["SignerName"].ToString(),
                            BirthDate = !String.IsNullOrEmpty(dr["BirthDate"].ToString()) ? ((DateTime)dr["BirthDate"]).ToString("dd/MM/yyyy") : String.Empty,
                            Gender = dr["Gender"].ToString(),
                            Address = dr["Address"].ToString(),
                            Complement = dr["Complement"].ToString(),
                            Neighborhood = dr["Neighborhood"].ToString(),
                            City = dr["City"].ToString(),
                            State = dr["State"].ToString(),
                            ZipCode = dr["ZipCode"].ToString(),
                            AgreementId = Convert.ToInt16(dr["codigo_convenio"]),
                            ReferencePoint = dr["con_referencia_endereco"].ToString(),
                            ScheduleDate = ((DateTime)dr["agendamento"]).ToString("dd/MM/yyyy"),
                            SchedulePeriod = dr["periodo"].ToString()[0],
                            CollectType = dr["CollectType"].ToString(),
                            CollectTypeId = Convert.ToInt16(dr["CollectTypeId"])
                        });
                    }

                    dr.NextResult();

                    //Telefones
                    var fones = new List<KeyValuePair<int, string>>();

                    while (dr.Read())
                    {
                        fones.Add(new KeyValuePair<int, string>(Convert.ToInt16(dr["con_codigo"]), dr["tec_telefone"].ToString()));
                    }

                    foreach (var doc in docs)
                    {

                        doc.Phones = (from f in fones
                                      where f.Key == doc.CodigoCliente
                                      select f.Value).ToList();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Ocorreu um problema durante a consulta de dados dos clientes no banco de dados. Detalhe do erro: " + e.Message);
                }

                return docs;
            }
        }
        public bool ValidaUsuario(string usuario, string senha)
        {
            SqlConnection conexao = new SqlConnection(String.Format("Data Source= {0}; Initial Catalog={1}; User ID={2}; Password={3}", DataSource, InitialCatalog, UserID, Password));
            var cmd = new SqlCommand("exec pr_valida_login @login = @usuario, @senha = @pass, @msg_erro = @erro out, @usu_codigo = @cod_usuario out", conexao);

            cmd.Parameters.Add("@usuario", SqlDbType.VarChar).Value = usuario;
            cmd.Parameters.Add("@pass", SqlDbType.VarChar).Value = senha;

            //Parâmetros de saída da proc
            SqlParameter outputErro = cmd.Parameters.Add("@erro", SqlDbType.VarChar, 300);
            outputErro.Direction = ParameterDirection.Output;
            SqlParameter outputCodUsuario = cmd.Parameters.Add("@cod_usuario", SqlDbType.Int);
            outputCodUsuario.Direction = ParameterDirection.Output;

            using (conexao)
            {
                conexao.Open();
                cmd.ExecuteScalar();
            }
            return outputErro.Value.ToString().Equals(String.Empty);
        }
        public void AtualizaPropostasPendentes(string contratosAndamento)
        {
            SqlConnection conexao = new SqlConnection(String.Format("Data Source= {0}; Initial Catalog={1}; User ID={2}; Password={3}", DataSource, InitialCatalog, UserID, Password));
            var cmd = new SqlCommand("exec pr_atualiza_contratos_pendentes @contratos_atualizacao = @contratos", conexao);
            cmd.Parameters.Add("@contratos", SqlDbType.VarChar, -1).Value = contratosAndamento;

            using (conexao)
            {
                conexao.Open();
                cmd.ExecuteScalar();
            }
        }
        public void AtualizaPropostaEnviadaRotalog(int codColeta, string contrato)
        {
            SqlConnection conexao = new SqlConnection(String.Format("Data Source= {0}; Initial Catalog={1}; User ID={2}; Password={3}", DataSource, InitialCatalog, UserID, Password));

            var cmd = new SqlCommand(@"exec pr_atualiza_contratos_coletados_rotalog @contrato, @cod_coleta", conexao);
            cmd.Parameters.Add("@cod_coleta", SqlDbType.VarChar, 50).Value = codColeta.ToString();
            cmd.Parameters.Add("@contrato", SqlDbType.VarChar, 100).Value = contrato;

            using (conexao)
            {
                conexao.Open();
                cmd.ExecuteScalar();
            }
        }
        public List<ObservacaoColeta> ObservacoesColeta()
        {
            List<ObservacaoColeta> ObservacoesOrgao = new List<ObservacaoColeta>();

            SqlConnection conexao = new SqlConnection(String.Format("Data Source= {0}; Initial Catalog={1}; User ID={2}; Password={3}", DataSource, InitialCatalog, UserID, Password));

            var cmd = new SqlCommand(@"select orgav_codigo as CodigoConvenio, orgav_observacao_coleta as ObservacaoColeta from orgaoav", conexao);

            using (conexao)
            {
                conexao.Open();
                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    ObservacoesOrgao.Add(
                        new ObservacaoColeta()
                        {
                            AgreementId = Convert.ToInt16(dr["CodigoConvenio"]),
                            Note = dr["ObservacaoColeta"].ToString()
                        }
                    );
                };
            }
            return ObservacoesOrgao;
        }
    }
}
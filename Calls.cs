using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace Phaedra_Update
{
    class Calls
    {
        clDALSQL obdal = new clDALSQL();
        Validar valid = new Validar();
        DataTable DT = new DataTable();
        public string ErrorMsg = string.Empty;
        
        public void GravaLog(string msg, Globais var)
        {
            try
            {
                StreamWriter writer = new StreamWriter(var.FileLogName);
                writer.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - " + msg);
                writer.Close();

                writer = new StreamWriter(var.FileLogTemp);
                writer.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - " + msg);
                writer.Close();
            }catch(Exception e)
            {
                Console.WriteLine("Erro na etapa de gravação de log\nErro:" + e.Message);
                valid.escritor.Write(ErrorMsg);
            }
        }

        public void Pessoas_Sysoft( Globais var)
        {
            string pessoas_sys = string.Empty;
            DataTable DT = new DataTable();
            
            try
            {
            
                obdal.Ambiente = clDALSQL.AmbienteExecucao.Producao;
                obdal.ClearParameters();
                
                AtualizaPessoas(var);
                Console.WriteLine("Fim do Processo de Atualizacao de Pessoas, aperte qualquer tecla para continua");
                
                AtualizaEmail(var);
                Console.WriteLine("Fim do Processo de Atualizacao de Email, aperte qualquer tecla para continua");

                AtualizaTitulos(var);
                Console.WriteLine("Fim do Processo de Atualizacao de Titulos, aperte qualquer tecla para continua");

                AtualizaEndereco(var);
                Console.WriteLine("Fim do Processo de Atualizacao de Endereço, aperte qualquer tecla para continua");

            }catch(Exception e)
            {
                ErrorMsg = "Erro no processo ao iniciar o processo\nErro: " + e.Message + " Cod_cliente: " + var.Cod_Cliente + " Num_titulo: " + var.Num_Titulo;
                GravaLog(ErrorMsg, var);
                valid.escritor.Write(ErrorMsg);
            }

            
            Inserir_historico(var);
            Inserir_ACORDOS(var);
            Inserir_PAGAMENTOS(var);

            Console.WriteLine("Final do Processo executado as:" + DateTime.Now.ToString("dd/MM/yyyy - HH:mm:ss"));
        }

        public void AtualizaPessoas( Globais var )
        {                
            DT = obdal.RetornaTabela("dbo.BI_RETORNA_TITULOS_ALTERADOS", "MANAGER");
            string msg;
            int i = 0;
            var.Contador = DT.Rows.Count;
            foreach(DataRow DR in DT.Rows)
            {
                obdal.Ambiente = clDALSQL.AmbienteExecucao.DW;
                obdal.ClearParameters();
                
                try
                {
                    var.Cod_Cliente = DR["cod_cliente"].ToString();
                    var.Num_Titulo = DR["num_titulo"].ToString();

                    var.CPF_CNPJ = DR["num_cpf"].ToString().ToUpper();
                    var.Nome = DR["Nom_Devedor"].ToString().ToUpper();
                    var.Sexo = DR["Tip_Sexo"].ToString().ToUpper();
                    var.Data_nasc = DR["Dat_Nascimento"].ToString();
                    valid.Data_nascimento(var);
                    var.Person_type = valid.valida_cnpj_cpf(var.CPF_CNPJ).ToUpper();

                    obdal.AddParameters("CPF_CNPJ", var.CPF_CNPJ, SqlDbType.VarChar, ParameterDirection.Input, 20);
                    obdal.AddParameters("NOME", var.Nome, SqlDbType.VarChar, ParameterDirection.Input, 100);
                    obdal.AddParameters("SEXO", var.Sexo, SqlDbType.VarChar, ParameterDirection.Input, 1);
                    obdal.AddParameters("DATA_NASC", var.Data_valida, SqlDbType.Date, ParameterDirection.Input);
                    obdal.AddParameters("TIPO_PESSOA", var.Person_type, SqlDbType.VarChar, ParameterDirection.Input, 1);
                    obdal.ExecutaProcedure("dbo.BI_INSERIR_PESSOAS", "DW");
                    i++;
                    Console.WriteLine("Atualizando: Pessoas\nStatus:"+i+" de "+DT.Rows.Count);
                }catch(Exception e)
                {
                    msg = "Erro: " + e.Message + " - Codigo do Cliente: " + var.Cod_Cliente + " - Numero do Titulo: " + var.Num_Titulo;
                    GravaLog(msg, var);
                    valid.escritor.Write(ErrorMsg);
                }
            }
        }
        public void AtualizaEmail(Globais var)
        {
            obdal.Ambiente = clDALSQL.AmbienteExecucao.Producao;
            obdal.ClearParameters();
            DT = obdal.RetornaTabela("dbo.BI_RETORNA_EMAIL", "MANAGER");

            obdal.ClearParameters();
            obdal.Ambiente = clDALSQL.AmbienteExecucao.DW;
            obdal.ClearParameters();
            int i = 0;
            foreach(DataRow DR in DT.Rows)
            {
                var.Descricao_Email = DR["Dcr_Email"].ToString().ToUpper();
                var.Id_Pessoa = DR["ID_PESSOAS"].ToString();
                var.CPF_CNPJ = DR["num_cpf"].ToString();
                obdal.ClearParameters();
                try
                {
                    obdal.AddParameters("DESCRICAO_EMAIL", var.Descricao_Email, SqlDbType.VarChar, ParameterDirection.Input, 100);
                    obdal.AddParameters("ID_PESSOAS", var.Id_Pessoa, SqlDbType.Int, ParameterDirection.Input);
                    obdal.AddParameters("CPF_CNPJ", var.CPF_CNPJ, SqlDbType.VarChar, ParameterDirection.Input , 20);
                    obdal.ExecutaProcedure("dbo.BI_INSERIR_EMAIL", "DW");
                    i++;
                    Console.WriteLine("Atualizando: e-mail\nStatus:" + i + " de " + DT.Rows.Count);
                }catch(Exception e)
                {
                        ErrorMsg = "Erro : " + e.Message + " Dcr_Email: " + var.Descricao_Email + " ID_PESSOAS: " + var.Id_Pessoa;
                        GravaLog(ErrorMsg, var);
                }
            }
        }
        public void AtualizaTitulos(Globais var)
        {
            obdal.Ambiente = clDALSQL.AmbienteExecucao.Producao;
            obdal.ClearParameters();
            DT = obdal.RetornaTabela("dbo.INSERIR_TITULOS", "SYSTEM1");
            string msg;
            int i = 0;
            obdal.Ambiente = clDALSQL.AmbienteExecucao.DW;
            foreach (DataRow DR in DT.Rows)
            {
                

                try
                {
                    obdal.ClearParameters();
                    
                    
                    var.Num_Titulo = DR["num_titulo"].ToString();

                    var.Dat_Vencto = DR["Dat_Vencto"].ToString().ToUpper();
                    var.Dat_Devolucao = DR["Dat_Devolucao"].ToString().ToUpper();
                    var.saldo_divida = DR["saldo_divida"].ToString().ToUpper();
                    var.COD_USER = DR["Cod_Usuario"].ToString();
                    var.Dat_Rec_Matriz = DR["Dat_Rec_Matriz"].ToString();
                    var.Dat_Rec_Matriz =  var.Dat_Rec_Matriz.Substring(0, 8);
                    var.Dat_Rec_Filial = DR["Dat_Rec_Filial"].ToString();
                    var.DIAS_ATRASO = DR["DIAS_ATRASO"].ToString();
                    var.FAIXA_ATRASO = DR["FAIXA_ATRASO"].ToString();
                    var.Id_Pessoa = DR["ID_PESSOAS"].ToString();
                    var.Id_Empresa = DR["ID_EMPRESA"].ToString();

                    obdal.AddParameters("num_titulo", var.Num_Titulo, SqlDbType.VarChar, ParameterDirection.Input, 30);
                    obdal.AddParameters("data_vencimento", var.Dat_Vencto, SqlDbType.Date, ParameterDirection.Input);
                    obdal.AddParameters("data_devolucao", var.Dat_Devolucao, SqlDbType.Date, ParameterDirection.Input);
                    obdal.AddParameters("saldo_divida", var.saldo_divida, SqlDbType.Money, ParameterDirection.Input);
                    obdal.AddParameters("data_rec_filial", var.Dat_Rec_Filial, SqlDbType.Date, ParameterDirection.Input);
                    obdal.AddParameters("data_rec_matriz", var.Dat_Rec_Matriz, SqlDbType.Date, ParameterDirection.Input);
                    obdal.AddParameters("dias_atraso", var.DIAS_ATRASO, SqlDbType.SmallInt, ParameterDirection.Input);
                    obdal.AddParameters("faixa_atraso", var.FAIXA_ATRASO, SqlDbType.VarChar, ParameterDirection.Input, 50);
                    obdal.AddParameters("cod_usuario", var.COD_USER, SqlDbType.VarChar, ParameterDirection.Input, 15);
                    obdal.AddParameters("dias_atraso_orig", "", SqlDbType.SmallInt, ParameterDirection.Input);
                    obdal.AddParameters("id_empresa", var.Id_Empresa, SqlDbType.Int, ParameterDirection.Input);
                    obdal.AddParameters("id_pessoas", var.Id_Pessoa, SqlDbType.Int, ParameterDirection.Input);

                    obdal.ExecutaProcedure("dbo.INSERIR_BI_TITULOS", "DW");
                    i++;
                    Console.WriteLine("Atualizando: Titulos\nStatus:" + i + " de " + DT.Rows.Count);
                }
                catch (Exception e)
                {
                    msg = "Erro: " + e.Message + " - Codigo do Cliente: " + var.Cod_Cliente + " - Numero do Titulo: " + var.Num_Titulo;
                    GravaLog(msg, var);
                    valid.escritor.Write(ErrorMsg);
                }
            }
        }
        public void AtualizaEndereco(Globais var)
        {
            DataTable DT = new DataTable();

            obdal.Ambiente = clDALSQL.AmbienteExecucao.Producao;
            obdal.ClearParameters();
            DT = obdal.RetornaTabela("dbo.BI_RETORNA_ENDERECO", "MANAGER");
            int i = 0;
            foreach (DataRow DR in DT.Rows)
            {
                try
                {
                    var.Cod_Cliente = DR["cod_cliente"].ToString();
                    var.Num_Titulo = DR["num_titulo"].ToString();
                    var.CPF_CNPJ = DR["num_cpf"].ToString().ToUpper();
                    var.Cep = DR["End_Res_Cep"].ToString().ToUpper();
                    var.Rua = DR["End_res_endereco"].ToString().ToUpper();
                    var.Bairro = DR["END_RES_BAIRRO"].ToString().ToUpper();
                    var.Cidade = DR["END_res_cidade"].ToString().ToUpper();
                    var.UF = DR["END_res_uf"].ToString().ToUpper();
                    var.Data_Atualizacao = DR["DATA_ATUALIZACAO"].ToString().ToUpper();
                    var.Id_Pessoa = DR["ID_PESSOAS"].ToString();


                    obdal.Ambiente = clDALSQL.AmbienteExecucao.DW;
                    obdal.ClearParameters();
                    obdal.AddParameters("CEP", var.Cep, SqlDbType.VarChar, ParameterDirection.Input, 20);
                    obdal.AddParameters("RUA", var.Rua, SqlDbType.VarChar, ParameterDirection.Input, 100);
                    obdal.AddParameters("BAIRRO", var.Bairro, SqlDbType.VarChar, ParameterDirection.Input, 100);
                    obdal.AddParameters("CIDADE", var.Cidade, SqlDbType.VarChar, ParameterDirection.Input, 100);
                    obdal.AddParameters("UF", var.UF, SqlDbType.VarChar, ParameterDirection.Input, 2);
                    obdal.AddParameters("ORIGEM", "", SqlDbType.VarChar, ParameterDirection.Input, 50);
                    obdal.AddParameters("DATA_ATUALIZACAO", Convert.ToDateTime(var.Data_Atualizacao), SqlDbType.Date, ParameterDirection.Input);
                    obdal.AddParameters("ID_PESSOAS", var.Id_Pessoa, SqlDbType.Int, ParameterDirection.Input);
                    obdal.AddParameters("CPF_CNPJ", var.CPF_CNPJ, SqlDbType.VarChar, ParameterDirection.Input, 20);
                    obdal.AddParameters("ERRO_PROC", DBNull.Value, SqlDbType.VarChar, ParameterDirection.InputOutput, 300);
                    obdal.ExecutaProcedure("dbo.BI_INSERIR_ENDERECO", "DW");
                    Atualiza_Flag(DR, var);
                    i++;
                    Console.WriteLine("Atualizando: endereço\nStatus:" + i + " de " + DT.Rows.Count);
                }
                catch (Exception e)
                {
                    ErrorMsg = "Erro : " + e.Message + " End_Res_Cep: " + var.Cep + " ID_PESSOAS: " + var.Id_Pessoa;
                    GravaLog(ErrorMsg, var);
                    valid.escritor.Write(ErrorMsg);
                }
            }       
        }
        public void Atualiza_Flag(DataRow DR, Globais var)
        {
                try
                {
                    obdal.Ambiente = clDALSQL.AmbienteExecucao.Producao;
                    obdal.ClearParameters();
                    obdal.AddParameters("COD_CLIENTE", var.Cod_Cliente, SqlDbType.VarChar, ParameterDirection.Input, 7);
                    obdal.AddParameters("NUM_TITULO", var.Num_Titulo, SqlDbType.VarChar, ParameterDirection.Input, 30);
                    obdal.ExecutaProcedure("DBO.BI_ATUALIZA_FLAG_ALTERADO", DR["NOME_BANCO"].ToString());
                }catch(Exception e)
                {
                    ErrorMsg = "Erro : " + e.Message + " End_Res_Cep: " + var.Cep + " ID_PESSOAS: " + var.Id_Pessoa;
                    GravaLog(ErrorMsg, var);
                    valid.escritor.Write(ErrorMsg);
                }
        }
        public void Inserir_historico(Globais var)
        {
            obdal.Ambiente = clDALSQL.AmbienteExecucao.DW;
            obdal.ClearParameters();
            obdal.ExecutaProcedure("dbo.BI_INSERIR_HISTORICOS", "DW");
        }
        public void Inserir_ACORDOS(Globais var)
        {
            obdal.Ambiente = clDALSQL.AmbienteExecucao.DW;
            obdal.ClearParameters();
            obdal.ExecutaProcedure("dbo.BI_INSERIR_ACORDOS", "DW");
        }
        public void Inserir_PAGAMENTOS(Globais var)
        {
            obdal.Ambiente = clDALSQL.AmbienteExecucao.DW;
            obdal.ClearParameters();
            obdal.ExecutaProcedure("dbo.BI_INSERIR_PAGAMENTOS", "DW");
        }
    }

}

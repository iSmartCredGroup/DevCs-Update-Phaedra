using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Phaedra_Update
{
    class Validar
    {
        bool teste;
        public StreamWriter escritor;

        public StreamWriter log_ext()
        { 
            try
            {
                string file_name = @"H:\Dados\Logs\log" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                StreamWriter writer = new StreamWriter(file_name);
                escritor = writer;
                return writer;
                
            }
            catch (IOException e)
            {
                Console.WriteLine("Problema em encontrar o caminho, verifique sua conexão com a rede");
                return StreamWriter.Null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro ao gravar o log" + e.Message);
                return StreamWriter.Null;
            }
        }

        

        public bool Validar_CNPJ(string numero)
        {
            try
            {
                int[] frst_multi = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                int[] scn_multi = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                int soma;
                int resto;
                string digito;
                string tempCNPJ;
                numero = numero.Replace(".", "").Replace("-", "").Replace("/", "");
                numero = numero.Trim();

                if (numero.Length != 14 || numero == "00000000000000")
                    return false;

                tempCNPJ = numero.Substring(0, 12);
                soma = 0;

                for (int i = 0; i < 12; i++)
                    soma += int.Parse(tempCNPJ[i].ToString()) * frst_multi[i];

                resto = (soma % 11);

                if (resto < 2)
                    resto = 0;
                else
                    resto = 11 - resto;

                digito = resto.ToString();

                tempCNPJ = tempCNPJ + resto;

                soma = 0;

                for (int i = 0; i < 13; i++)
                    soma += int.Parse(tempCNPJ[i].ToString()) * scn_multi[i];

                resto = (soma % 11);

                if (resto < 2)
                    resto = 0;
                else
                    resto = 11 - resto;

                digito += resto.ToString();
                teste = numero.EndsWith(digito);

            }
            catch (Exception ex)
            {
                return teste;
            }
            return teste;
        }

        public bool valida_CPF(string numero)
        {
            try
            {
                int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
                int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
                string tempCpf;
                string digito;
                int soma;
                int resto;
                numero = numero.Replace(".", "").Replace("-", "").Replace("/", "");
                numero = numero.Trim();

                if (numero == "")
                    if (numero.Length != 11 || numero == "00000000000")
                        return false;

                tempCpf = numero.Substring(0, 9);
                soma = 0;

                for (int i = 0; i < 9; i++)
                    soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

                resto = soma % 11;
                if (resto < 2)
                    resto = 0;
                else
                    resto = 11 - resto;

                digito = resto.ToString();

                tempCpf = tempCpf + digito;

                soma = 0;
                for (int i = 0; i < 10; i++)
                    soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

                resto = soma % 11;
                if (resto < 2)
                    resto = 0;
                else
                    resto = 11 - resto;

                digito += resto.ToString();

                teste = numero.EndsWith(digito);
                if (teste == false)
                {
                    escritor = log_ext();
                    escritor.Write("Cadastro invalido: " + numero + "\r\n");
                }
            }
            catch (Exception ex)
            {
                escritor = log_ext();
                escritor.Write("Cadastro invalido: " + numero + "\r\nErro: " + ex + "\r\n");
                return teste;
            }
            return teste;
        }

        public string valida_cnpj_cpf(string cpf_cnpj)
        {

            string numero = string.Empty;
            string cpf = string.Empty;
            numero = cpf_cnpj.Substring(0, 3);
            cpf = cpf_cnpj.Substring(3);
            if (numero != "000")
            {
                if (Validar_CNPJ(cpf_cnpj))
                {
                    return "J";
                }
            }
            else
            {
                if (valida_CPF(cpf))
                {
                    return "F";
                }
                else
                {
                    if (Validar_CNPJ(cpf_cnpj))
                    {
                        return "J";
                    }
                }
            }
            return "";
        }

        public void Data_nascimento(Globais var)
        {
            string DataValid = string.Empty;
            DateTime data;
            CultureInfo culture = new CultureInfo("pt-BR");
            if(!string.IsNullOrEmpty(var.Data_nasc))
            {
                DataValid = GetNumbers(var.Data_nasc);
                
                if((DataValid.Length == 6) || (DataValid.Length == 8))
                {
                    if(DataValid.Length == 6)
                    {
                        DataValid = DataValid.Substring(0, 4) + "/" + DataValid.Substring(4, 2) + "/" + DataValid.Substring(6, 2);
                    }else if(DataValid.Length == 8)
                    {
                        DataValid = DataValid.Substring(0, 4) + "/" + DataValid.Substring(4, 2) + "/" + DataValid.Substring(6, 2);
                    }


                    
                    if(!DateTime.TryParseExact(DataValid, "yyyy/MM/dd", culture, DateTimeStyles.None, out data))
                    {
                        DataValid = null;
                    }
                }
                else
                {
                    DataValid = null;
                }

                var.Data_valida = DataValid;
            }
        }

        public string GetNumbers(string value)
        {
            try
            {
                string Numbers = string.Empty;

                foreach(char c in value)
                {
                    if (char.IsNumber(c))
                    {
                        Numbers = Numbers + c;
                    }
                }

                return Numbers;

            }catch(Exception e)
            {
                return "";
            }
        }
    }
}
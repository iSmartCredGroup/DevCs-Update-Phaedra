using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaedra_Update
{
    class Globais
    {
        public double VALOR_ACORDO { get; set; }
        public string FileLogTemp { get; set; }
        public string FileLogName { get; set; }
        public string CPF_CNPJ { get; set; }
        public string Nome { get; set; }
        public string Sexo { get; set; }
        public string Data_nasc { get; set; }
        public string Data_valida { get; set; }
        public string Person_type { get; set; }
        public string Cod_Cliente { get; set; }
        public string Num_Titulo { get; set; }

        //email

        public string Descricao_Email { get; set; }
        public string Id_Pessoa { get; set; }

        //endereco
        public string Cep { get; set; }
        public string Rua { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string Origem { get; set; }
        public string Data_Atualizacao { get; set; }

        public string Dat_Vencto { get; set; }
        public string Dat_Devolucao { get; set; }
        public string saldo_divida { get; set; }
        public string Dat_Rec_Filial { get; set; }
        public string Dat_Rec_Matriz { get; set; }
        public string DIAS_ATRASO { get; set; }
        public string FAIXA_ATRASO { get; set; }
        public string Id_Empresa  { get; set; }
        public string COD_USER { get; set; }
        public int Contador { get; set; }

    }
}
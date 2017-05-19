using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Phaedra_Update
{
    class Program
    {
        static string dir = System.IO.Directory.GetCurrentDirectory();

        static void Main(string[] args)
        {
            Console.SetWindowSize(23, 3);
            do
            {
                Console.Clear();
                Console.WriteLine("Aguardando horario \n Horario atual:" + DateTime.Now.ToString("HH:mm:ss"));
                System.Threading.Thread.Sleep(1000);
            }
            while ((DateTime.Now.ToString("HH:mm") != DateTime.Now.ToString("HH:mm")));
            begin();
        }

        static void begin()
        {
            Globais var = new Globais();
            Calls call = new Calls();
            int i = 0;
            var.FileLogName = dir + "/Phaedra_ETL_DW.txt";
            var.FileLogTemp = dir + "/Phaedra_Tmp_Log.txt";
            try
            {
                if (File.Exists(var.FileLogTemp))
                {
                    File.Delete(var.FileLogTemp);
                }

                    call.GravaLog("Iniciando processo", var);

                    while (i <= 2)
                    {
                        call.Pessoas_Sysoft(var);
                        i++;
                    }
                    call.GravaLog("Fim do Processo", var);
            }
            catch (Exception e)
            {
                call.GravaLog(e.Message, var);
            }
        }
    }
}

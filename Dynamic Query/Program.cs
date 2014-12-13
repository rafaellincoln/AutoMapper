using System;
using System.Collections.Generic;
using Dynamic.Query.Helper;
using Dynamic.Query.Helper.Extensions;
using Npgsql;


namespace Dynamic_Query
{
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                HelperDba Dba = new HelperDba();

                Dba.Comando = "SELECT * FROM pessoa;";
                Dba.ReturnList = true;
                Dba.TypeReturn = "Dynamic_Query.Pessoa";
                Dba.StringConnection = "Server = 127.0.0.1; Port = 5432; Database = teste_db; User Id = postgres; Password = postgres; Timeout = 15;";

                Dba.ConnectionHelper(true);

                IList<Pessoa> list = (List<Pessoa>)Dba.ExecutaComando();

                foreach (Pessoa p in list)
                    Console.WriteLine("Nome: " + p.Nome + " " + p.SobreNome + " - Idade: " + p.Idade);
            }
            catch(DynamicException e)
            {
                Console.WriteLine(e);
            }

            Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;
using Npgsql;
using System.Text.RegularExpressions;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Net.Mime.MediaTypeNames;

namespace Dynamic.Query.Helper.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Abre a conexão com o banco de dados
        /// </summary>
        /// <param name="Con"></param>
        public static NpgsqlConnection AbreConexao(this NpgsqlConnection Con)
        {
            HelperDba Dba = new HelperDba();
            return Dba.ConnectionHelper(true);
        }

        /// <summary>
        /// Fecha a conexão com o banco de dados
        /// </summary>
        /// <param name="Con"></param>
        public static void FechaConexao(this NpgsqlConnection Con)
        {
            HelperDba Dba = new HelperDba();
            Dba.ConnectionHelper(false);
        }

        /// <summary>
        /// Executa a Query no banco de dados
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public static object Executa(this NpgsqlConnection con)
        {
            HelperDba Dba = new HelperDba();
            return Dba.ExecutaComando();
        }

        /// <summary>
        /// Transforma o nome da propriedade do VO no nome da coluna no banco
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        internal static string GetNameColumn(this string text)
        {
            return Regex.Replace(text, @"(\p{Lu})", " $1").TrimStart().Replace(" ", "_").ToLower();
        }

        /// <summary>
        /// Altera um objeto para o tipo definido
        /// </summary>
        /// <param name="obj">Item a ser convertido</param>
        /// <param name="dest">Tipo a ser retornado</param>
        /// <example><code>teste</code></example>
        /// <returns>Returna o objeto com o novo tipo</returns>
        internal static dynamic ChangeType(this object obj, Type dest)
        {
            return Convert.ChangeType(obj, dest);
        }

        internal static void GenerateClass(this string Code)
        {
            string code = Code;

            CompilerResults compilerResults = CompileScript(code);

            if (compilerResults.Errors.HasErrors)
            {
                throw new InvalidOperationException("Expression has a syntax error.");
            }

            Assembly assembly = compilerResults.CompiledAssembly;
            Assembly currentAssembly = Assembly.GetAssembly(typeof(HelperDba));
        }

        internal static CompilerResults CompileScript(string source)
        {
            CompilerParameters parms = new CompilerParameters();

            parms.GenerateExecutable = false;
            parms.GenerateInMemory = true;
            parms.IncludeDebugInformation = false;

            CodeDomProvider compiler = CSharpCodeProvider.CreateProvider("CSharp");

            return compiler.CompileAssemblyFromSource(parms, source);
        }

    }

    public class DynamicException : ApplicationException
    {
        public DynamicException() { }
        public DynamicException(string message) :base(message) { }
        public DynamicException(string message, Exception inner) { }
    }
}


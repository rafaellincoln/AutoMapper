using System;
using Npgsql;
using System.Data;
using Dynamic.Query.Helper.Extensions;
using System.Reflection;
using System.Collections.Generic;


namespace Dynamic.Query.Helper
{
    public class HelperDba
    {
        #region Parametros

        private string stringConnection;
        /// <summary>
        /// String de conexão do banco de dados. Sera utilizada a string do config caso não seja definida.
        /// </summary>
        public string StringConnection
        {
            get { return stringConnection; }
            set { stringConnection = value; }
        }

        private NpgsqlConnection con;
        /// <summary>
        /// Conexão do banco de dados
        /// </summary>
        public NpgsqlConnection Connection
        {
            get { return con; }
            set { con = value;  }
        }

        private string comando;
        /// <summary>
        /// Query a ser executada no banco de dados
        /// </summary>
        public string Comando
        {
            get { return comando; }
            set { comando = value; }
        }

        private string typeReturn;
        /// <summary>
        /// Nome completo da classe que devera ser retornada
        /// </summary>
        public string TypeReturn
        {
            get { return typeReturn; }
            set { typeReturn = value; }
        }

        private bool returnList;
        /// <summary>
        /// Indica se a query devera retornar uma lista.
        /// </summary>
        public bool ReturnList
        {
            get { return returnList; }
            set { returnList = value; }
        }


        #endregion

        /// <summary>
        /// Abre ou Fecha a conexão com o Banco de Dados
        /// </summary>
        /// <param name="Open"></param>
        /// <example> 
        /// This sample shows how to call the <see cref="Connection"/> method.
        /// <code>
        /// class TestClass 
        /// {
        ///     static int Main() 
        ///     {
        ///         Connection(true);
        ///     }
        /// }
        /// </code>
        /// </example>
        public NpgsqlConnection ConnectionHelper(bool Open)
        {
            try
            {
                if (Open)
                {
                    if (con == null)
                        con = new NpgsqlConnection(stringConnection);

                    if (con.State != ConnectionState.Open)
                        con.Open();
                }
                else
                {
                    if (con == null || con.State == ConnectionState.Closed)
                        return null;

                    con.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
            return con;
        }

        /// <summary>
        /// Executa uma Query no banco e retorna uma lista dinamica.
        /// </summary>
        /// <returns></returns>
        public object ExecutaComando()
        {
            try
            {
                NpgsqlCommand comand = new NpgsqlCommand(comando, con);

                if (!returnList)
                {
                    comand.ExecuteNonQuery();
                    return null;
                }

                if (string.IsNullOrEmpty(typeReturn))
                    throw new DynamicException("Defina um valor para o parametro TypeReturn!");

                Assembly asm = Assembly.GetCallingAssembly();

                Type type = asm.GetType(typeReturn, true);

                if (type == null)
                    throw new DynamicException("Não encontrada a referencia " + typeReturn);

                PropertyInfo[] propInfo = type.GetProperties();

                dynamic _returnValue = CreateGeneric(typeof(List<>), type);

                IDataReader dr = comand.ExecuteReader();

                while (dr.Read())
                {
                    dynamic item = Activator.CreateInstance(type);

                    foreach (PropertyInfo prop in propInfo)
                    {
                        string pName = prop.Name;
                        Type pType = prop.PropertyType;

                        var value = dr[pName.GetNameColumn()].ChangeType(pType);

                        prop.SetValue(item, value);
                    }

                    _returnValue.Add(item);
                }                
                return _returnValue;
            }
            catch (DynamicException e)
            {
                throw new DynamicException(e.Message, e);
            }
        }

        /// <summary>
        /// Cria uma lista dinamica.
        /// </summary>
        /// <param name="innerType"></param>
        /// <param name="args">Argumentos para preencher a lista</param>
        /// <returns></returns>
        internal static object CreateGeneric(Type generic, Type innerType, params object[] args)
        {
            Type specificType = generic.MakeGenericType(new Type[] { innerType });
            return Activator.CreateInstance(specificType, args);
        }
    }
}

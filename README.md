[![Stories in Ready](https://badge.waffle.io/rafaellincoln/Dynamic-Query.png?label=ready&title=Ready)](https://waffle.io/rafaellincoln/Dynamic-Query)
Dynamic-Query
=============

Exemplo de Retorno Dinâmico a partir de um resultado de uma query

Exemplo de Uso
=============

```csharp
HelperDba Dba = new HelperDba();

Dba.Comando = "SELECT * FROM pessoa;";
Dba.ReturnList = true;
Dba.TypeReturn = "Pessoa";
Dba.StringConnection = "";

IList<Pessoa> list = (List<Pessoa>)Dba.Connection.AbreConexao().Executa();

foreach(Pessoa p in list)
{
      Console.WriteLine(p.Nome + " " + p.SobreNome + " - Idade: " + p.Idade);
}
```

Mais exemplos em: http://rafaellincoln.wordpress.com/

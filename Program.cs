using System.Collections.Generic;
using System.Globalization;

namespace HealthTracker;

internal class Program
{
    private const int MaxRegistros = 200;
    private static readonly string[] Tipos = new string[MaxRegistros];
    private static readonly DateTime[] Datas = new DateTime[MaxRegistros];
    private static readonly double[] Valores = new double[MaxRegistros];
    private static int _totalRegistros;

    private static void Main()
    {
        while (true)
        {
            MostrarMenu();
            Console.Write("\nSelecione uma opção: ");
            var opcaoSelecionada = Console.ReadLine()?.Trim();

            switch (opcaoSelecionada)
            {
                case "1":
                    AdicionarRegistro();
                    break;
                case "2":
                    ListarRegistros();
                    break;
                case "3":
                    ExibirEstatisticas();
                    break;
                case "4":
                    Console.WriteLine("\nObrigado por usar o HealthTracker. Até logo!");
                    return;
                default:
                    Console.WriteLine("\nOpção inválida. Tente novamente usando os números do menu.");
                    break;
            }

            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    private static void MostrarMenu()
    {
        Console.WriteLine("================= HealthTracker =================");
        Console.WriteLine("1 - Adicionar registro");
        Console.WriteLine("2 - Listar registros");
        Console.WriteLine("3 - Exibir estatísticas");
        Console.WriteLine("4 - Sair");
        Console.WriteLine("=================================================");
    }

    private static void AdicionarRegistro()
    {
        if (_totalRegistros >= MaxRegistros)
        {
            Console.WriteLine("\nLimite máximo de registros atingido. Exclua algum item para continuar.");
            return;
        }

        Console.WriteLine("\n--- Novo registro ---");
        var tipo = LerTextoNaoVazio("Tipo da atividade (ex.: Exercício, Água, Sono): ");
        var data = LerDataValida("Data da atividade (dd/mm/aaaa): ");
        var valor = LerValorPositivo("Valor numérico (minutos, litros, horas etc.): ");

        Tipos[_totalRegistros] = tipo;
        Datas[_totalRegistros] = data;
        Valores[_totalRegistros] = valor;
        _totalRegistros++;

        Console.WriteLine("\nRegistro cadastrado com sucesso!");
    }

    private static void ListarRegistros()
    {
        if (_totalRegistros == 0)
        {
            Console.WriteLine("\nNenhum registro encontrado.");
            return;
        }

        Console.WriteLine("\n--- Registros cadastrados ---");
        Console.WriteLine("Nº | Data       | Tipo                | Valor");
        Console.WriteLine("----------------------------------------------");

        for (var i = 0; i < _totalRegistros; i++)
        {
            Console.WriteLine(
                $"{i + 1,2} | {Datas[i]:dd/MM/yyyy} | {Tipos[i],-18} | {Valores[i],7:0.##}");
        }
    }

    private static void ExibirEstatisticas()
    {
        if (_totalRegistros == 0)
        {
            Console.WriteLine("\nAinda não há dados para calcular estatísticas.");
            return;
        }

        Console.WriteLine("\n--- Estatísticas por tipo ---");

        var estatisticas = new Dictionary<string, (string Label, double Total, int Quantidade)>(
            StringComparer.OrdinalIgnoreCase);
        double somaGeral = 0;

        for (var i = 0; i < _totalRegistros; i++)
        {
            var tipo = Tipos[i];
            var valor = Valores[i];
            somaGeral += valor;

            if (!estatisticas.TryGetValue(tipo, out var dados))
            {
                dados = (tipo, 0, 0);
            }

            dados.Total += valor;
            dados.Quantidade++;
            estatisticas[tipo] = dados;
        }

        foreach (var entrada in estatisticas.Values)
        {
            var media = entrada.Total / entrada.Quantidade;
            Console.WriteLine(
                $"- {entrada.Label}: soma {entrada.Total:0.##}, média {media:0.##} (registros: {entrada.Quantidade})");
        }

        Console.WriteLine($"\nTotal geral informado: {somaGeral:0.##}");
    }

    private static string LerTextoNaoVazio(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);
            var entrada = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(entrada))
            {
                return entrada.Trim();
            }

            Console.WriteLine("Valor obrigatório. Digite novamente.");
        }
    }

    private static DateTime LerDataValida(string mensagem)
    {
        var cultura = new CultureInfo("pt-BR");
        while (true)
        {
            Console.Write(mensagem);
            var entrada = Console.ReadLine();
            if (DateTime.TryParseExact(entrada, "dd/MM/yyyy", cultura, DateTimeStyles.None, out var data))
            {
                return data;
            }

            Console.WriteLine("Data inválida. Utilize o formato dd/mm/aaaa.");
        }
    }

    private static double LerValorPositivo(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);
            var entrada = Console.ReadLine();
            if (double.TryParse(entrada, NumberStyles.Number, CultureInfo.InvariantCulture, out var valor) &&
                valor >= 0)
            {
                return valor;
            }

            Console.WriteLine("Valor inválido. Informe um número maior ou igual a zero.");
        }
    }
}

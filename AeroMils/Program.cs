namespace AeroMils
{
    internal class Program
    {
        static Empresa empresa = new Empresa();
        private static void mainMenu()
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("              AeroMils             ");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("1 - Inserir Avião");
            Console.WriteLine("2 - Mostrar Todos os Aviões");
            Console.WriteLine("3 - Mostrar Aviões Fretados (Hoje)");
            Console.WriteLine("4 - Reservar Avião");
            Console.WriteLine("5 - Mostrar todas as Reservas");
            Console.WriteLine("0 - Guardar e Sair");
            Console.WriteLine("-----------------------------------");

            int i;
            string opcao;
            bool valid = false;
            do
            {
                Console.Write("Opção: ");
                opcao = Console.ReadLine();
                valid = int.TryParse(opcao, out i);
                if (!valid || Convert.ToInt32(opcao) < 0 || Convert.ToInt32(opcao) > 5)
                {
                    Console.WriteLine("Erro: Opção inválida.");
                }
            } while (!valid || Convert.ToInt32(opcao) < 0 || Convert.ToInt32(opcao) > 5);

            switch (Convert.ToInt32(opcao))
            {
                case 1:
                    Console.Clear();
                    empresa.criarAviao();
                    break;
                case 2:
                    Console.Clear();
                    empresa.mostrarAvioes();
                    Console.WriteLine("\nPrima qualquer tecla para regressar ao Menu Principal! \n");
                    Console.ReadKey();
                    break;
                case 3:
                    Console.Clear();
                    empresa.mostrarAvioesFretadosHoje();
                    Console.WriteLine("\nPrima qualquer tecla para regressar ao Menu Principal! \n");
                    Console.ReadKey();
                    break;
                case 4:
                    Console.Clear();
                    empresa.reservarAviao();
                    break;
                case 5:
                    Console.Clear();
                    empresa.mostrarReservas();
                    Console.WriteLine("\nPrima qualquer tecla para regressar ao Menu Principal! \n");
                    Console.ReadKey();
                    break;
                case 0:
                    empresa.escreverFicheiroCSV("avioes");
                    empresa.escreverFicheiroCSV("reservas");
                    Console.Clear();
                    Environment.Exit(0);
                    break;
            }
            Console.Clear();
            mainMenu();

        }

        static void Main(string[] args)
        {
            empresa.carregarFicheiro("avioes");
            empresa.carregarFicheiro("reservas");
            mainMenu();


        }
    }
}
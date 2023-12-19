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
            Console.WriteLine("3 - Mostrar Aviões Fretados");
            Console.WriteLine("4 - Reservar Avião");
            Console.WriteLine("5 - Mostrar todas as Reservas");
            Console.WriteLine("0 - Sair");
            Console.WriteLine("-----------------------------------");

            int i;
            string opcao;
            bool valid = false;
            do
            {
                Console.Write("Opção: ");
                opcao = Console.ReadLine();
                valid = int.TryParse(opcao, out i);
            } while (!valid || Convert.ToInt32(opcao) < 0 || Convert.ToInt32(opcao) > 5);

            switch (Convert.ToInt32(opcao))
            {
                case 1:
                    Console.Clear();
                    empresa.criarAviao();
                    Console.WriteLine("\nAvião inserido com sucesso! \n");
                    Console.WriteLine("\nA regressar ao Menu principal...");
                    System.Threading.Thread.Sleep(3000);
                    break;
                case 2:
                    Console.Clear();
                    empresa.mostrarAvioes();
                    Console.WriteLine("\nPrima qualquer tecla para regressar ao Menu Principal! \n");
                    Console.ReadKey();
                    break;
                case 3:
                    Console.Clear();
                    empresa.mostrarAvioesFretados();
                    Console.WriteLine("\nPrima qualquer tecla para regressar ao Menu Principal! \n");
                    Console.ReadKey();
                    break;
                case 4:
                    Console.Clear();
                    empresa.reservarAviao();
                    Console.WriteLine("\nReserva realizada com sucesso! \n");
                    Console.WriteLine("\nPrima qualquer tecla para regressar ao Menu Principal! \n");
                    Console.ReadKey();
                    break;
                case 5:
                    Console.Clear();
                    empresa.mostrarReservas();
                    Console.WriteLine("\nPrima qualquer tecla para regressar ao Menu Principal! \n");
                    Console.ReadKey();
                    break;
                case 0:
                    Console.Clear();
                    Environment.Exit(0);
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Opção Inválida!");
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
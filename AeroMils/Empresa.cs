using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AeroMils
{

    internal class Empresa
    {
    #region validacoes
        //reserva de avião
        //verifica nome válido
        public static bool verificaNome(string nome)
        {
            if (nome.Length > 2)
            {
                for (int i = 0; i < nome.Length; i++)
                {
                    if (!char.IsDigit(nome[i]))
                    {
                        return true;
                    }
                }
            }
                return false;
        }

        //verifica email válido
        private static bool verificaEmail(string email)
        {
            string regex = @"^[^@\s]+@[^@\s]+\.(pt|com|net|org|gov)$";

            return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
        }

        //verifica id válido e já existente
        public static bool verificaId(string id, List<Aviao> listaAvioes)
        {

            if (int.TryParse(id, out int i))
            {
                foreach (var aviao in listaAvioes)
                {
                    if (aviao.Id == Convert.ToInt32(id))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //verifica data de levantamento válida 
        public static bool verificaData(string data)
        {
            DateTime dt;
            if (DateTime.TryParseExact(data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                if (dt >= DateTime.Today)
                {
                    return true;
                }
            }
            return false;
        }

        //verifica data de entrega válida
        public static bool verificaDataEntrega(string data, string dataInicial)
        {
            DateTime dt;
            DateTime dtInicial = ConvertToDate(dataInicial);
            if (DateTime.TryParseExact(data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                if (dt > dtInicial)
                {
                    return true;
                }
            }
            return false;
        }
        //verifica se avião não está fretado
        public static bool verificaAviaoFretado(int id, List<Aviao> listaAvioes)
        {
            foreach (var aviao in listaAvioes)
            {
                if (aviao.Id == id)
                {
                    if (aviao.Fretado == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        #endregion validacoes

            public static DateTime ConvertToDate(string date)
        {
            DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return dt;
        }

        private List<Aviao> _listaAvioes = new List<Aviao>();     //Lista de Aviões
        private List<string[]> _listaReservas = new List<string[]>();  //Lista de Reservas
        private int _contaAvioes;
        private int _contaReservas;

        private int gerarId()
        {
            int id = 0;
            foreach (var aviao in _listaAvioes)
            {
                if (aviao.Id > id)
                {
                    id = aviao.Id;
                }
            }
            return id + 1;
        }

        public int ContaAvioes
        {
            get => _contaAvioes;
            set => _contaAvioes = value;
        }
        public List<Aviao> ListaAvioes
        {
            get => _listaAvioes;
            set => _listaAvioes = value;
        }


        
        public void criarAviao()
        {
            Aviao novoAviao = new Aviao();

            _contaAvioes++;
            novoAviao.lerDadosAviao(gerarId());

            _listaAvioes.Add(novoAviao);
            escreverFicheiroCSV("avioes");

        }


        public void mostrarAvioes()
        {
            foreach (var aviao in _listaAvioes)
            {
                aviao.MostrarDadosAviao();
            }
        }
        

        public void carregarFicheiro(string nomeFicheiro)
        {
            if (!File.Exists(nomeFicheiro + ".csv"))
            {
                File.Create(nomeFicheiro + ".csv").Close();
            }
            try
            {
                nomeFicheiro += ".csv";
                string[] lines = File.ReadAllLines(nomeFicheiro);
                
                if (nomeFicheiro == "avioes.csv")
                {
                    _contaAvioes = File.ReadLines(nomeFicheiro).Count();
                    foreach (string line in lines)
                    {
                        string[] fields = line.Split(',');
                        Aviao aviao = new Aviao();
                        aviao.Id = Convert.ToInt32(fields[0]);
                        aviao.Marca = fields[1];
                        aviao.Modelo = fields[2];
                        aviao.Estado = Convert.ToBoolean(fields[3]);
                        aviao.QtdMotores = Convert.ToInt32(fields[4]);
                        aviao.CapacidadePassageiros = Convert.ToInt32(fields[5]);
                        aviao.AutonomiaVoo = Convert.ToDouble(fields[6]);
                        aviao.DataUltManutencao = fields[7];
                        aviao.AreaDescolagem = Convert.ToDouble(fields[8]);
                        aviao.AreaPouso = Convert.ToDouble(fields[9]);
                        aviao.ValorFrete = Convert.ToDouble(fields[10]);
                        aviao.NumVoosDiarios = Convert.ToInt32(fields[11]);
                        aviao.CompanhiaAerea = fields[12];
                        aviao.NumProprietarios = Convert.ToInt32(fields[13]);
                        aviao.CapacidadeCarga = Convert.ToDouble(fields[14]);
                        aviao.DataFabrico = fields[15];
                        aviao.Fretado = Convert.ToBoolean(fields[16]);
                        _listaAvioes.Add(aviao);
                    }
                }else if (nomeFicheiro == "reservas.csv")
                {
                    _contaReservas = File.ReadLines(nomeFicheiro).Count();
                    foreach (string line in lines)
                    {
                        string[] fields = line.Split(',');
                        string[] reserva = new string[8];
                        reserva[0] = fields[0];
                        reserva[1] = fields[1];
                        reserva[2] = fields[2];
                        reserva[3] = fields[3];
                        reserva[4] = fields[4];
                        reserva[5] = fields[5];
                        reserva[6] = fields[6];
                        reserva[7] = fields[7];

                        DateTime data = ConvertToDate(reserva[5]);

                        for(int i =0; i < _listaAvioes.Count; i++)
                        {
                            _listaAvioes[i].Fretado = true;
                        }
                        for (int i = 0; i < _listaAvioes.Count; i++)
                        {
                            if (_listaAvioes[i].Id == Convert.ToInt32(reserva[3]) && data < DateTime.Today)
                            {
                                _listaAvioes[i].Fretado = false;
                            }
                        }

                        _listaReservas.Add(reserva);

                    }
                }
                escreverFicheiroCSV("avioes");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar ficheiro CSV: {ex.Message}");
            }
        }
        
        public void escreverFicheiroCSV(string nomeFicheiro)
        {
            try
            {
                if (nomeFicheiro == "avioes")
                {
                    nomeFicheiro += ".csv";
                    StreamWriter writer = new StreamWriter(nomeFicheiro);
                    foreach (var fields in _listaAvioes)
                    {
                        string line = string.Join(",", fields);
                        writer.WriteLine(line);
                    }
                    writer.Close();
                }else if (nomeFicheiro == "reservas")
                {
                    nomeFicheiro += ".csv";
                    StreamWriter writer = new StreamWriter(nomeFicheiro);
                    foreach (var fields in _listaReservas)
                    {
                        string line = string.Join(",", fields);
                        writer.WriteLine(line);
                    }
                    writer.Close();
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao escrever no ficheiro CSV: {ex.Message}");
            }
        }

        public void reservarAviao() {

            mostrarAvioesDisponiveis();

            Console.WriteLine("--------------------------------------------------------------------------------------------------\n\n");
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine($"                             Reservar Avião                                    ");
            Console.WriteLine("--------------------------------------------------------------------------------\n");

            string[] reserva = new string[8];
            Console.Write("Insira o ID do avião que pretende reservar: ");
            

            do
            {
                do
                {
                    reserva[3] = Console.ReadLine();
                    if (!verificaId(reserva[3], _listaAvioes))
                    {
                        Console.Write("ID inválido! Insira novamente: ");
                    }
                } while (!verificaId(reserva[3], _listaAvioes));

                Console.Write("Insira a Data Inicial do Frete (dd/mm/aaaa): ");
                do
                {
                    reserva[4] = Console.ReadLine();
                    if (!verificaData(reserva[4]))
                    {
                        Console.Write("Data inválida! Insira novamente: ");
                    }
                } while (!verificaData(reserva[4]));
                
                if (verificaAviaoFretado(Convert.ToInt32(reserva[3]), _listaAvioes))
                {
                    Console.Write("Avião já fretado! Tente de novo.\nInsira o id do avião: ");
                }
            } while (verificaAviaoFretado(Convert.ToInt32(reserva[3]), _listaAvioes));
            //data de disponibilidade




            Console.Write("Insira a Data Final do Frete (dd/mm/aaaa): ");
            do
            {
                reserva[5] = Console.ReadLine();
                if (!verificaDataEntrega(reserva[5], reserva[4]))
                {
                    Console.Write("Data inválida! Insira novamente: ");
                }
            } while (!verificaDataEntrega(reserva[5], reserva[4]));

            Console.Write("Insira o seu nome: ");
            do
            {
                reserva[1] = Console.ReadLine();
                if (!verificaNome(reserva[1]))
                {
                    Console.Write("Nome inválido. Insira novamente: ");
                }
            } while (!verificaNome(reserva[1]));

            Console.Write("Insira o seu email: ");
            do
            {
                reserva[2] = Console.ReadLine();
                if (!verificaEmail(reserva[2]))
                {
                    Console.Write("Email inválido! Insira novamente: ");
                }
            } while (!verificaEmail(reserva[2]));

            

            _listaAvioes[Convert.ToInt32(reserva[3])].Fretado = true;

            _contaReservas++;
            reserva[0] = _contaReservas.ToString();
            
            DateTime dataInicial = ConvertToDate(reserva[4]);
            DateTime dataFinal = ConvertToDate(reserva[5]);
            TimeSpan diferenca = dataFinal - dataInicial;
            int dias = diferenca.Days;
            double valorTotal = 0;
            foreach (var aviao in _listaAvioes)
            {
                if (aviao.Id == Convert.ToInt32(reserva[3]))
                {
                    valorTotal = aviao.ValorFrete * dias;
                }
            }
            reserva[6] = valorTotal.ToString();
            reserva[7] = "naoLiquidado";

            _listaReservas.Add(reserva);
            escreverFicheiroCSV("reservas");

            Console.WriteLine($"Valor Total: {valorTotal}");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"Reserva efetuada com sucesso! \n");
            Console.WriteLine($"ID da Reserva: {reserva[0]} \n");
            Console.WriteLine($"Nome: {reserva[1]} \n"); 
            Console.WriteLine($"Email: {reserva[2]} \n");   
            Console.WriteLine($"ID do Avião: {reserva[3]} \n");
            Console.WriteLine($"Data Inicial: {reserva[4]} \n");
            Console.WriteLine($"Data Final: {reserva[5]} \n");
            Console.WriteLine($"Valor Total: {reserva[6]} \n");
            Console.WriteLine("-----------------------------------");

            pagarReserva(reserva[0]);
        }

        public void mostrarAvioesDisponiveis()
        {
            Console.WriteLine("--------------------------------------------------------------------------------------------------");
            Console.WriteLine($"                          Aviões Disponíveis                                                     ");
            Console.WriteLine("--------------------------------------------------------------------------------------------------");
            Console.WriteLine($"{"ID",-4} | {"Marca",-20} | {"Modelo",-20} | {"Valor do Frete",-15} | {"Data de Disponibilidade"}");

            string dataDisponivel = "";
            foreach (var aviao in _listaAvioes)
            {
                if (aviao.Estado == true)
                {
                    foreach (var reserva in _listaReservas)
                    {
                        if (aviao.Id == Convert.ToInt32(reserva[3]))
                        {
                            dataDisponivel = reserva[5];
                        }
                    }
                    DateTime dataAux = ConvertToDate(dataDisponivel);
                    dataAux = dataAux.AddDays(1);
                    dataDisponivel = dataAux.ToString("dd/MM/yyyy");
                    Console.WriteLine($"{aviao.Id,-4} | {aviao.Marca,-20} | {aviao.Modelo,-20} | {aviao.ValorFrete,-15} | {dataDisponivel}");            
                }
            }
        }

        public void mostrarAvioesFretados()
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"         Aviões Fretados          ");
            Console.WriteLine("----------------------------------- \n");
            foreach (var reserva in _listaReservas)
            {
                foreach (var aviao in _listaAvioes)
                {
                    if (aviao.Fretado == true && aviao.Id == Convert.ToInt32(reserva[3]))
                    {
                        Console.WriteLine($"ID: {aviao.Id}");
                        Console.WriteLine($"Marca: {aviao.Marca}");
                        Console.WriteLine($"Modelo: {aviao.Modelo}");
                        Console.WriteLine($"Capacidade de Passageiros: {aviao.CapacidadePassageiros}");
                        Console.WriteLine($"Valor do Frete: {aviao.ValorFrete}");
                        Console.WriteLine($"Companhia Aérea: {aviao.CompanhiaAerea}");
                        Console.WriteLine($"Número de Proprietários: {aviao.NumProprietarios}");
                        Console.WriteLine($"Capacidade de Carga: {aviao.CapacidadeCarga}");
                        Console.WriteLine($"Data de Fabrico: {aviao.DataFabrico}");
                        Console.WriteLine("-----------------------------------");
                    }
                }
            }
        }

        public void mostrarReservas()
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"         Reservas          ");
            Console.WriteLine("----------------------------------- \n");

            foreach (var reserva in _listaReservas)
            {
                Console.WriteLine($"ID: {reserva[0]}");
                Console.WriteLine($"Nome: {reserva[1]}");
                Console.WriteLine($"Email: {reserva[2]}");
                Console.WriteLine($"ID do Avião: {reserva[3]}");
                Console.WriteLine($"Data Inicial: {reserva[4]}");
                Console.WriteLine($"Data Final: {reserva[5]}");
                Console.WriteLine($"Valor Total: {reserva[6]}");
                Console.WriteLine($"Estado: {reserva[7]}");
                Console.WriteLine("-----------------------------------");
            }
        }



        public void pagarReserva(string id)
        {
            foreach (var reserva in _listaReservas)
            {
                if (reserva[0] == id)
                {
                    if (reserva[7] == "naoLiquidado")
                    {
                        Console.WriteLine($"Valor Total: {reserva[6]}");

                        double valorTotal = Convert.ToDouble(reserva[6]);
                        double valorPago = 0;

                        while (valorPago < valorTotal)
                        {
                            Console.Write("Valor Pago: ");
                            valorPago = Convert.ToDouble(Console.ReadLine());

                            if (valorPago < valorTotal)
                            {
                                Console.WriteLine($"Valor insuficiente! Insira novamente.\n");
                            }
                        }

                        Console.WriteLine($"Troco: {valorPago - valorTotal}\n");


                        reserva[7] = "liquidado";

                        escreverFicheiroCSV("reservas");
                    }
                }
            }
        }
    }

}

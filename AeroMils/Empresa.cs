using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroMils
{

    internal class Empresa
    {
        
        public static DateTime ConvertToDate(string date)
        {
            //nao esta a funcionar faz la de novo
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
                        _listaAvioes.Add(aviao);
                    }
                }else if (nomeFicheiro == "reservas.csv")
                {
                    _contaReservas = File.ReadLines(nomeFicheiro).Count();
                    foreach (string line in lines)
                    {
                        string[] fields = line.Split(',');
                        string[] reserva = new string[6];
                        reserva[0] = fields[0];
                        reserva[1] = fields[1];
                        reserva[2] = fields[2];
                        reserva[3] = fields[3];
                        reserva[4] = fields[4];
                        reserva[5] = fields[5];
                        _listaReservas.Add(reserva);
                    }
                }
                
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

            Console.WriteLine("---------------------------------------------------------------------\n\n");
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine($"                  Reservar Avião                  ");
            Console.WriteLine("---------------------------------------------------\n");

            string[] reserva = new string[8];
            Console.Write("Insira o seu nome: ");
            reserva[1] = Console.ReadLine();
            Console.Write("Insira o seu email: ");
            reserva[2] = Console.ReadLine();
            Console.Write("Insira o ID do avião que pretende reservar: ");
            reserva[3] = Console.ReadLine();
            Console.Write("Insira a Data Inicial do Frete: ");
            reserva[4] = Console.ReadLine();
            Console.Write("Insira a Data Final do Frete: ");
            reserva[5] = Console.ReadLine();
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
            Console.WriteLine("---------------------------------------------------------------------");
            Console.WriteLine($"                          Aviões Disponíveis                        ");
            Console.WriteLine("---------------------------------------------------------------------");
            Console.WriteLine($"{"ID",-4} | {"Marca",-20} | {"Modelo",-20} | {"Valor do Frete",-15}");

            foreach (var aviao in _listaAvioes)
            {
                if (aviao.Estado == true)
                {
                    Console.WriteLine($"{aviao.Id,-4} | {aviao.Marca,-20} | {aviao.Modelo,-20} | {aviao.ValorFrete,-15}");
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
                    if (reserva[3] == aviao.Id.ToString())
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

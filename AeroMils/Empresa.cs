using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AeroMils
{

    internal class Empresa
    {
        private List<Aviao> _listaAvioes = new List<Aviao>();          
        private List<string[]> _listaReservas = new List<string[]>();  
        private int _contaAvioes;
        private int _contaReservas;

        #region validacoes
        //verifica data válida
        private static bool verificaData(string data)
        {
            DateTime dt;
            if (DateTime.TryParseExact(data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                return true;
            }
            Console.WriteLine("Erro: Data inválida!");
            Console.Write("Insira novamente a data inicial: ");
            return false;
        }

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


        private bool verificaDataEntrega(string[] novaReserva)
        {
            DateTime dataInicial = ConvertToDate(novaReserva[4]);
            DateTime dataEntrega = ConvertToDate(novaReserva[5]);

            if (dataInicial.Date < DateTime.Today || dataEntrega.Date < DateTime.Today)
            {
                Console.WriteLine("Erro: As datas devem ser iguais ou posteriores à data de hoje.");
                Console.Write("Insira novamente o ID do avião: ");
                return false;
            }

            if (dataEntrega < dataInicial)
            {
                Console.WriteLine("Erro: A Data de entrega tem de ser superior à Data inicial.");
                Console.Write("Insira novamente o ID do avião: ");
                return false;
            }

            foreach (var reserva in _listaReservas)
            {
                if (novaReserva[3] == reserva[3])
                {
                    DateTime reservaInicio = ConvertToDate(reserva[4]);
                    DateTime reservaFim = ConvertToDate(reserva[5]);

                    if ((dataEntrega >= reservaInicio && dataEntrega <= reservaFim) || (dataInicial >= reservaInicio && dataInicial <= reservaFim))
                    {
                        Console.WriteLine("Erro: As Datas fornecidas já se encontram em uso.");
                        Console.Write("Insira novamente o ID do avião: ");
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool verificaStrings(string texto, string tipoValidacao)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                if (tipoValidacao == "marca")
                {
                    Console.WriteLine("Erro: Marca inválida.");
                }else if (tipoValidacao == "modelo")
                {
                    Console.WriteLine("Erro: Modelo inválido.");
                }
                else if (tipoValidacao == "companhia")
                {
                    Console.WriteLine("Erro: Companhia inválida.");
                }
                Console.Write("Insira novamente: ");
                return false;
            }
            return true;
        }

        private static bool verificaEstadoAviao(string estado)
        {
            if (!estado.ToLower().Equals("true") && !estado.ToLower().Equals("false"))
            {
                Console.WriteLine("Erro: Estado inválido.");
                Console.Write("Insira 'true' ou 'false': ");
                return false;
            }
            return true;
        }

        private static bool verificaInteiro(string numero)
        {
            if (int.TryParse(numero, out int result) && result > 0)
            {
                return true;
            }

            Console.WriteLine("Erro: Número inválido. ");
            Console.Write("Insira novamente: ");
            return false;
        }

        private static bool verificaMinPista(string numero)
        {
            if(double.TryParse(numero, out double result) && result >= 1800)
            {
                return true;
            }
            Console.WriteLine("Erro: Tamanho errado. O tamanho mínimo para descolagem/pouso é de 1800 metros.");
            Console.Write("Insira novamente: ");
            return false;   
        }
        private static bool verificaDouble(string numero)
        {
            if (double.TryParse(numero, out double result) && result >= 0 && Math.Round(result, 2) == result)
            {
                return true;
            }

            Console.WriteLine("Erro: Número inválido (apenas são permitidas no máx. 2 casas décimais).");
            Console.Write("Insira novamente: ");
            return false;
        }

        private static bool verificaDataFabrico(string dataFabrico)
        {
            DateTime dtFabrico = ConvertToDate(dataFabrico);

            if (dtFabrico.Date <= DateTime.Today)
            {
                return true;
            }

            Console.WriteLine("Erro: A Data de Fabrico deve ser anterior ou igual à data atual.");
            Console.Write("Insira novamente a Data de Fabrico (dd/mm/aaaa): ");
            return false;
        }

        private static bool verificaDataUltimaManutencao(string dataUltimaManutencao, string dataFabrico)
        {
            DateTime dataUltimaManutencaoDateTime;
            if (!DateTime.TryParseExact(dataUltimaManutencao, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataUltimaManutencaoDateTime))
            {
                Console.WriteLine("Erro: Data inválida.");
                Console.Write("Insira novamente a Data da Última Manutenção (dd/mm/aaaa): ");
                return false;
            }

            DateTime dataFabricoDateTime = DateTime.ParseExact(dataFabrico, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (dataUltimaManutencaoDateTime > DateTime.Today || dataUltimaManutencaoDateTime < dataFabricoDateTime)
            {
                Console.WriteLine("Erro: A Data da Última Manutenção deve ser igual ou anterior à data de hoje e posterior à Data de Fabrico.");
                Console.Write("Insira novamente a Data da Última Manutenção (dd/mm/aaaa): ");
                return false;
            }

            return true;
        }

        private bool verificaLucro(string dataInicial,string dataFinal)
        {
            DateTime dtInicial = ConvertToDate(dataInicial);
            DateTime dtFinal = ConvertToDate(dataFinal);

            if (dtFinal < dtInicial)
            {
                Console.WriteLine("Erro: A Data final deve ser igual ou superior á inicial.");
                Console.Write("Insira novamente a data inicial: ");
                return false;
            }

            foreach (var reserva in _listaReservas)
            {
                DateTime reservaInicial = ConvertToDate(reserva[4]);
                DateTime reservaFinal = ConvertToDate(reserva[5]);

                if (dtInicial <= reservaInicial && dtFinal >= reservaFinal)
                {
                    return true;
                }
            }

            Console.WriteLine("Não existem reservas nas datas inseridas");
            return false;

        }



        #endregion validacoes


        public void mostrarReservasAviao(string idAviao)
        {
            List<string> listaDatas = new List<string>();

            foreach (var reserva in _listaReservas)
            {
                if (idAviao == reserva[3])
                {
                    listaDatas.Add($"{reserva[4]} -> {reserva[5]}");
                }
            }

            listaDatas.OrderBy(date => date).ToList();

            if (listaDatas.Count > 0)
            {
                Console.WriteLine("\nReservas ativas para esse avião:");
                foreach (var data in listaDatas)
                {
                    Console.WriteLine(data);
                }
                Console.WriteLine("\n");
            }
            
        }


        public static DateTime ConvertToDate(string date)
        {
            DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return dt;
        }

        private int gerarIdAviao()
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

            novoAviao.Id = gerarIdAviao();

            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"         Dados do Avião {_contaAvioes + 1}            ");
            Console.WriteLine("----------------------------------- ");
            Console.WriteLine("Para regressar ao Menu Principal insira (0). \n");

            Console.Write("Marca: ");
            do
            {
                novoAviao.Marca = Console.ReadLine();
                if (novoAviao.Marca == "0")
                {
                    return;
                }
            } while (!verificaStrings(novoAviao.Marca, "marca"));

            Console.Write("Modelo: ");
            do
            {
                novoAviao.Modelo = Console.ReadLine();
            } while (!verificaStrings(novoAviao.Modelo, "modelo"));

            Console.Write("Estado (true - Ativo | false - Inativo): ");
            string estadoInput;
            do
            {
                estadoInput = Console.ReadLine();
            } while (!verificaEstadoAviao(estadoInput));
            novoAviao.Estado = Convert.ToBoolean(estadoInput);

            Console.Write("Quantidade de Motores: ");
            string qtdMotoresInput;
            do
            {
                qtdMotoresInput = Console.ReadLine();
            } while (!verificaInteiro(qtdMotoresInput));
            novoAviao.QtdMotores = Convert.ToInt32(qtdMotoresInput);

            Console.Write("Capacidade de Passageiros: ");
            string capPassageiros;
            do
            {
                capPassageiros = Console.ReadLine();
            } while (!verificaInteiro(capPassageiros));
            novoAviao.CapacidadePassageiros = Convert.ToInt32(capPassageiros);

            Console.Write("Autonomia de Voo (horas): ");
            string autonomiaVoo;
            do
            {
                autonomiaVoo = Console.ReadLine();
            } while (!verificaInteiro(autonomiaVoo));
            novoAviao.AutonomiaVoo = Convert.ToInt32(autonomiaVoo);

            Console.Write("Area de Descolagem (m^2): ");
            string areaDescolagem;
            do
            {
                areaDescolagem = Console.ReadLine();
            } while (!verificaMinPista(areaDescolagem));
            novoAviao.AreaDescolagem = Convert.ToDouble(areaDescolagem);

            Console.Write("Area de Pouso (m^2): ");
            string areaPouso;
            do
            {
                areaPouso = Console.ReadLine();
            } while (!verificaMinPista(areaPouso));
            novoAviao.AreaPouso = Convert.ToDouble(areaPouso);

            Console.Write("Valor do Frete (eur): ");
            string valorFrete;
            do
            {
                valorFrete = Console.ReadLine();
            } while (!verificaDouble(valorFrete));
            novoAviao.ValorFrete = Convert.ToDouble(valorFrete);

            Console.Write("Número de Voos Diários: ");
            string voosDiarios;
            do
            {
                voosDiarios = Console.ReadLine();
            } while (!verificaInteiro(voosDiarios));
            novoAviao.NumVoosDiarios = Convert.ToInt32(voosDiarios);

            Console.Write("Companhia Aerea: ");
            do
            {
                novoAviao.CompanhiaAerea = Console.ReadLine();
            } while (!verificaStrings(novoAviao.Marca, "companhia"));

            Console.Write("Número de Proprietarios: ");
            string numProprietarios;
            do
            {
                numProprietarios = Console.ReadLine();
            } while (!verificaInteiro(numProprietarios));
            novoAviao.NumProprietarios = Convert.ToInt32(numProprietarios);

            Console.Write("Capacidade de Carga (kg): ");
            string capacidadeCarga;
            do
            {
                capacidadeCarga = Console.ReadLine();
            } while (!verificaDouble(capacidadeCarga));
            novoAviao.CapacidadeCarga = Convert.ToDouble(capacidadeCarga);

            Console.Write("Data de Fabrico (dd/mm/aaaa): ");
            do
            {
                novoAviao.DataFabrico = Console.ReadLine();
            } while (!verificaData(novoAviao.DataFabrico) || !verificaDataFabrico(novoAviao.DataFabrico));

            Console.Write("Data da Última Manutenção (dd/mm/aaaa): ");
            do
            {
                novoAviao.DataUltManutencao = Console.ReadLine();
            } while (!verificaData(novoAviao.DataUltManutencao) || !verificaDataUltimaManutencao(novoAviao.DataUltManutencao, novoAviao.DataFabrico));

            _contaAvioes++;
            _listaAvioes.Add(novoAviao);
            escreverFicheiroCSV("avioes");

            Console.WriteLine("\nAvião inserido com sucesso! \n");
            Console.WriteLine("\nA regressar ao Menu principal...");
            System.Threading.Thread.Sleep(3000);
        }

        public void mostrarAvioes()
        {
            if (_contaAvioes != 0)
            {
                foreach (var aviao in _listaAvioes)
                {
                    aviao.MostrarDadosAviao();
                }
            }
            else
            {
                Console.WriteLine("Erro: Não existem aviões inseridos no sistema! \n");
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
                }
                else if (nomeFicheiro == "reservas.csv")
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

                        for (int i = 0; i < _listaAvioes.Count; i++)
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
                }
                else if (nomeFicheiro == "reservas")
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

        public void reservarAviao()
        {
            mostrarAvioesDisponiveis();

            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine("Para regressar ao Menu Principal insira (0). \n");
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
                    if (reserva[3] == "0")
                    {
                        return;
                    }
                    if (!verificaId(reserva[3], _listaAvioes))
                    {
                        Console.WriteLine("Erro: ID inválido.");
                        Console.Write("Insira novamente o ID do avião: ");
                    }
                } while (!verificaId(reserva[3], _listaAvioes));

                mostrarReservasAviao(reserva[3]);

                Console.Write("Insira a Data Inicial do Frete (dd/mm/aaaa): ");
                do
                {
                    reserva[4] = Console.ReadLine();
                } while (!verificaData(reserva[4]));

                Console.Write("Insira a Data Final do Frete (dd/mm/aaaa): ");
                do
                {
                    reserva[5] = Console.ReadLine();
                } while (!verificaData(reserva[5]));

            } while (!verificaDataEntrega(reserva));

            Console.Write("Insira o seu nome: ");
            do
            {
                reserva[1] = Console.ReadLine();
                if (!verificaNome(reserva[1]))
                {
                    Console.WriteLine("Erro: Nome inválido.");
                    Console.Write("Insira novamente o nome: ");
                }
            } while (!verificaNome(reserva[1]));

            Console.Write("Insira o seu email: ");
            do
            {
                reserva[2] = Console.ReadLine();
                if (!verificaEmail(reserva[2]))
                {
                    Console.WriteLine("Erro: Email inválido.");
                    Console.Write("Insira novamente o email: ");
                }
            } while (!verificaEmail(reserva[2]));

            _listaAvioes[Convert.ToInt32(reserva[3]) - 1].Fretado = true;

            _contaReservas++;
            reserva[0] = _contaReservas.ToString();

            DateTime dataInicial = ConvertToDate(reserva[4]);
            DateTime dataFinal = ConvertToDate(reserva[5]);
            TimeSpan diferenca = dataFinal - dataInicial;
            int dias = diferenca.Days;
            dias += 1;
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
            escreverFicheiroCSV("avioes");

            Console.WriteLine($"Valor Total: {valorTotal}");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"Reserva efetuada com sucesso! \n");
            Console.WriteLine($"ID da Reserva: {reserva[0]} \n");
            Console.WriteLine($"Nome: {reserva[1]} \n");
            Console.WriteLine($"Email: {reserva[2]} \n");
            Console.WriteLine($"ID do Avião: {reserva[3]} \n");
            Console.WriteLine($"Data Inicial (dd-mm-aaaa): {reserva[4]} \n");
            Console.WriteLine($"Data Final (dd-mm-aaaa): {reserva[5]} \n");
            Console.WriteLine($"Valor Total (eur): {reserva[6]} \n");
            Console.WriteLine("-----------------------------------");

            pagarReserva(reserva[0]);

            Console.WriteLine("\nReserva realizada com sucesso! \n");
            Console.WriteLine("\nPrima qualquer tecla para regressar ao Menu Principal! \n");
            Console.ReadKey();
        }

        public void mostrarAvioesDisponiveis()
        {
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine($"                          Aviões Disponíveis                                   ");
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine($"{"ID",-4} | {"Marca",-20} | {"Modelo",-20} | {"Valor do Frete p/ dia (eur)",-15} ");

            foreach (var aviao in _listaAvioes)
            {
                if (aviao.Estado == true)
                {
                    Console.WriteLine($"{aviao.Id,-4} | {aviao.Marca,-20} | {aviao.Modelo,-20} | {aviao.ValorFrete,-15} ");
                }
            }
        }

        public void mostrarAvioesFretadosHoje()
        {
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"            Aviões Fretados (Hoje)         ");
            Console.WriteLine("--------------------------------------------");

            int cont = 0;

            foreach (var reserva in _listaReservas)
            {
                DateTime reservaInicio = ConvertToDate(reserva[4]);

                // Verifica se a reserva ocorre hoje
                if (reservaInicio.Date == DateTime.Today)
                {
                    foreach (var aviao in _listaAvioes)
                    {
                        if (aviao.Fretado == true && aviao.Id == Convert.ToInt32(reserva[3]))
                        {
                            cont++;
                            Console.WriteLine($"ID: {aviao.Id}");
                            Console.WriteLine($"Marca: {aviao.Marca}");
                            Console.WriteLine($"Modelo: {aviao.Modelo}");
                            Console.WriteLine($"Data de Fabrico (dd/mm/aaaa): {aviao.DataFabrico}");
                            Console.WriteLine($"Capacidade de Passageiros: {aviao.CapacidadePassageiros}");
                            Console.WriteLine($"Valor do Frete (eur): {aviao.ValorFrete}");
                            Console.WriteLine($"Companhia Aérea: {aviao.CompanhiaAerea}");
                            Console.WriteLine($"Número de Proprietários: {aviao.NumProprietarios}");
                            Console.WriteLine($"Capacidade de Carga (kg): {aviao.CapacidadeCarga}");
                            Console.WriteLine($"Data Inicial Frete (dd-mm-aaaa): {reserva[4]} ");
                            Console.WriteLine($"Data Final Frete (dd-mm-aaaa): {reserva[5]} ");
                            Console.WriteLine("--------------------------------------------");
                        }
                    }
                }
            }
            if (cont == 0)
            {
                Console.WriteLine("Hoje não existem aviões fretados! \n");
            }
        }

        public void mostrarReservas()
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"             Reservas             ");
            Console.WriteLine("----------------------------------- \n");

            foreach (var reserva in _listaReservas)
            {
                Console.WriteLine($"ID: {reserva[0]}");
                Console.WriteLine($"Nome: {reserva[1]}");
                Console.WriteLine($"Email: {reserva[2]}");
                Console.WriteLine($"ID do Avião: {reserva[3]}");
                Console.WriteLine($"Data Inicial (dd-mm-aaaa): {reserva[4]}");
                Console.WriteLine($"Data Final (dd-mm-aaaa): {reserva[5]}");
                Console.WriteLine($"Valor Total (eur): {reserva[6]}");
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
                        Console.WriteLine($"Valor Total (eur): {reserva[6]}");

                        double valorTotal;
                        if (double.TryParse(reserva[6], out valorTotal))
                        {
                            double valorPago = 0;
                            Console.Write("Valor Pago (eur): ");
                            while (valorPago < valorTotal)
                            {
                                if (double.TryParse(Console.ReadLine(), out valorPago) && valorPago >= 0)
                                {
                                    if (valorPago < valorTotal)
                                    {
                                        Console.WriteLine($"Erro: Valor insuficiente.");
                                        Console.Write("Insira novamente o Valor Pago (eur): ");
                                    }
                                }
                                else
                                {
                                    Console.Write("Erro: Valor inválido.");
                                    Console.Write("Insira novamente o Valor Pago (eur): ");
                                }
                            }

                            Console.WriteLine($"Troco: {valorPago - valorTotal}\n");
                            reserva[7] = "liquidado";
                            escreverFicheiroCSV("reservas");
                        }
                        else
                        {
                            Console.WriteLine("Erro ao converter o valor total da reserva para um número.\n");
                        }
                    }
                }
            }
        }

        public void mostrarLucroPorData()
        {

            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"         Lucro por Data           ");
            Console.WriteLine("----------------------------------- ");
            Console.WriteLine("Para regressar ao Menu Principal insira (0). \n");

            if (_contaReservas <= 0)
            {
                Console.WriteLine("Não existe nenhuma reserva");
                Console.WriteLine("\nA regressar ao Menu principal...");
                System.Threading.Thread.Sleep(3000);
                return;
            }

            double lucroTotal = 0; 
            string dataInicial, dataFinal;

            do
            {
                Console.Write("Insira a data inicial: ");
                do
                {
                    
                    dataInicial = Console.ReadLine();
                    if (dataInicial == "0")
                    {
                        return;
                    }
                } while (!verificaData(dataInicial) || dataInicial == "0");

                do
                {
                    Console.Write("Insira a data final: ");
                    dataFinal = Console.ReadLine();
                    if (dataFinal == "0")
                    {
                        return;
                    }
                } while (!verificaData(dataFinal) || dataFinal == "0");

            } while (!verificaLucro(dataInicial, dataFinal));

            DateTime dtInicial = ConvertToDate(dataInicial);
            DateTime dtFinal = ConvertToDate(dataFinal);

            DateTime dateDay = dtInicial;

            while(dateDay <= dtFinal)
            {
                foreach (var reserva in _listaReservas)
                {

                    DateTime reservaInicial = ConvertToDate(reserva[4]);
                    DateTime reservaFinal = ConvertToDate(reserva[5]);


                    if (dateDay >= reservaInicial && dateDay <= reservaFinal)
                    {
                        lucroTotal += _listaAvioes[Convert.ToInt32(reserva[3]) - 1].ValorFrete;
                    }
                }

                dateDay = dateDay.AddDays(1);
            }

            Console.WriteLine($"O lucro entre essas datas é: {lucroTotal}eur");

            Console.WriteLine("\nPrima qualquer tecla para regressar ao Menu Principal! \n");
            Console.ReadKey();


        }
    }

}

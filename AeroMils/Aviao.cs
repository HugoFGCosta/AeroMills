using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroMils
{
    internal class Aviao : Empresa
    {
        private int _id;
        private string _marca;
        private string _modelo;
        private bool _estado = true;
        private int _qtdMotores;
        private int _capacidadePassageiros;
        private double _autonomiaVoo;
        private string _dataUltManutencao;
        private double _areaDescolagem;
        private double _areaPouso;
        private double _valorFrete;
        private int _numVoosDiarios;
        private string _companhiaAerea;
        private int _numProprietarios;
        private double _capacidadeCarga;
        private string _dataFabrico;
        private bool _fretado = false;

        public int Id
        {
            get => _id;
            set => _id = value;
        }
        public string Marca
        {
            get => _marca;
            set => _marca = value;
        }
        public string Modelo
        {
            get => _modelo;
            set => _modelo = value;
        }
        public bool Estado
        {
            get => _estado;
            set => _estado = value;
        }
        public int QtdMotores
        {
            get => _qtdMotores;
            set => _qtdMotores = value;
        }
        public int CapacidadePassageiros
        {
            get => _capacidadePassageiros;
            set => _capacidadePassageiros = value;
        }
        public double AutonomiaVoo
        {
            get => _autonomiaVoo;
            set => _autonomiaVoo = value;
        }
        public string DataUltManutencao
        {
            get => _dataUltManutencao;
            set => _dataUltManutencao = value;
        }
        public double AreaDescolagem
        {
            get => _areaDescolagem;
            set => _areaDescolagem = value;
        }
        public double AreaPouso
        {
            get => _areaPouso;
            set => _areaPouso = value;
        }
        public double ValorFrete
        {
            get => _valorFrete;
            set => _valorFrete = value;
        }
        public int NumVoosDiarios
        {
            get => _numVoosDiarios;
            set => _numVoosDiarios = value;
        }
        public string CompanhiaAerea
        {
            get => _companhiaAerea;
            set => _companhiaAerea = value;
        }
        public int NumProprietarios
        {
            get => _numProprietarios;
            set => _numProprietarios = value;
        }
        public double CapacidadeCarga
        {
            get => _capacidadeCarga;
            set => _capacidadeCarga = value;
        }
        public string DataFabrico
        {
            get => _dataFabrico;
            set => _dataFabrico = value;
        }
        public bool Fretado
        {
            get => _fretado;
            set => _fretado = value;
        }


        public Aviao()
        {
            this._id = 0;
            this._marca = "";
            this._modelo = "";
            this._estado = false;
            this._qtdMotores = 0;
            this._capacidadePassageiros = 0;
            this._autonomiaVoo = 0;
            this._dataUltManutencao = "";
            this._areaDescolagem = 0;
            this._areaPouso = 0;
            this._valorFrete = 0;
            this._numVoosDiarios = 0;
            this._companhiaAerea = "";
            this._numProprietarios = 0;
            this._capacidadeCarga = 0;
            this._dataFabrico = "";
            this._fretado = false;
        }

        public Aviao(int id, string marca, string modelo, string estado, int qtdMotores, int capacidadePassageiros, int autonomiaVoo, string dataUltManutencao, double areaDescolagem, double areaPouso, double valorFrete, int numVoosDiarios, string companhiaAerea, int numProprietarios, double capacidadeCarga, string dataFabrico)
        {
            this._id = id;
            this._marca = marca;
            this._modelo = modelo;
            this._estado = Convert.ToBoolean(estado);
            this._qtdMotores = qtdMotores;
            this._capacidadePassageiros = capacidadePassageiros;
            this._autonomiaVoo = autonomiaVoo;
            this._dataUltManutencao = dataUltManutencao;
            this._areaDescolagem = areaDescolagem;
            this._areaPouso = areaPouso;
            this._valorFrete = valorFrete;
            this._numVoosDiarios = numVoosDiarios;
            this._companhiaAerea = companhiaAerea;
            this._numProprietarios = numProprietarios;
            this._capacidadeCarga = capacidadeCarga;
            this._dataFabrico = dataFabrico;
            this._fretado = false;
        }

        public void MostrarDadosAviao()
        {
            Console.WriteLine($"ID: {_id}");
            Console.WriteLine($"Marca: {_marca}");
            Console.WriteLine($"Modelo: {_modelo}");
            if (Estado == true)
            {
                Console.WriteLine($"Estado: Ativo");
            }
            else
            {
                Console.WriteLine($"Estado: Inativo");
            }
            Console.WriteLine($"Quantidade de Motores: {_qtdMotores}");
            Console.WriteLine($"Capacidade de Passageiros: {_capacidadePassageiros}");
            Console.WriteLine($"Autonomia de Voo (horas): {_autonomiaVoo}");
            Console.WriteLine($"Data da Última Manutenção (dd/mm/aaaa): {_dataUltManutencao}");
            Console.WriteLine($"Área de Descolagem (m^2): {_areaDescolagem}");
            Console.WriteLine($"Área de Pouso (m^2): {_areaPouso}");
            Console.WriteLine($"Valor do Frete (€): {_valorFrete}");
            Console.WriteLine($"Número de Voos Diários: {_numVoosDiarios}");
            Console.WriteLine($"Companhia Aérea: {_companhiaAerea}");
            Console.WriteLine($"Número de Proprietários: {_numProprietarios}");
            Console.WriteLine($"Capacidade de Carga (Kg): {_capacidadeCarga}");
            Console.WriteLine($"Data de Fabrico (dd/mm/aaaa): {_dataFabrico}");
            Console.WriteLine("------------------------------------------------");
        }


        public override string ToString()
        {
            
            return $"{_id},{_marca},{_modelo},{_estado},{_qtdMotores},{_capacidadePassageiros},{_autonomiaVoo},{_dataUltManutencao},{_areaDescolagem},{_areaPouso},{_valorFrete},{_numVoosDiarios},{_companhiaAerea},{_numProprietarios},{_capacidadeCarga},{_dataFabrico},{_fretado}";
        }
    }
    
    
}
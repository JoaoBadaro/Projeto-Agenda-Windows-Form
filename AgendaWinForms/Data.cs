using System;

namespace Agenda
{
    public class Data
    {
        private int dia;
        private int mes;
        private int ano;

        public Data(int dia, int mes, int ano)
        {
            setData(dia, mes, ano);
        }

        public int Dia => dia;
        public int Mes => mes;
        public int Ano => ano;

        // Valida a data; lança ArgumentException se for inválida.
        public void setData(int dia, int mes, int ano)
        {
            if (ano < 1 || ano > 9999 || mes < 1 || mes > 12 ||
                dia < 1 || dia > DateTime.DaysInMonth(ano, mes))
            {
                throw new ArgumentException("Data inválida.");
            }

            this.dia = dia;
            this.mes = mes;
            this.ano = ano;
        }

        // Tenta converter um texto "dd/mm/aaaa" em Data.
        public static bool TryParse(string texto, out Data data)
        {
            data = null;
            if (string.IsNullOrWhiteSpace(texto)) return false;

            string[] partes = texto.Trim().Split('/');
            if (partes.Length != 3) return false;

            if (!int.TryParse(partes[0], out int d) ||
                !int.TryParse(partes[1], out int m) ||
                !int.TryParse(partes[2], out int a))
                return false;

            try
            {
                data = new Data(d, m, a);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public override string ToString()
        {
            return $"{dia:D2}/{mes:D2}/{ano:D4}";
        }
    }
}

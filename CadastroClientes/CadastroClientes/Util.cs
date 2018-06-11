using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace CadastroClientes
{
    public static class Util
    {
        public static CultureInfo culture = new CultureInfo("pt-BR");

        public static DataTable ConvertCSVtoDataTable(StreamReader sr)
        {
            if (sr == null)
            {
                return null;
            }

            string[] headers = sr.ReadLine().Split(';');

            DataTable dt = new DataTable();

            foreach (string header in headers)
            {
                dt.Columns.Add(header);
            }

            while (!sr.EndOfStream)
            {
                string[] rows = Regex.Split(sr.ReadLine(), ";(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                DataRow dr = dt.NewRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    dr[i] = rows[i];
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static bool validaColunas(IList<Colunas> colunasDicionario, DataColumnCollection colunasArquivo)
        {
            bool fgOk = false;

            if (colunasDicionario.Count == colunasArquivo.Count)
            {

                foreach (DataColumn colunaArquivo in colunasArquivo)
                {
                    fgOk = false;

                    foreach (Colunas colunaDicionario in colunasDicionario)
                    {
                        if (colunaArquivo.ColumnName == colunaDicionario.colunaArquivo)
                        {
                            fgOk = true;

                            break;
                        }
                    }

                    if (!fgOk)
                    {
                        break;
                    }
                }
            }

            return fgOk;
        }

        public static bool validaCampo(object campo)
        {
            bool fgOk = true;

            if (campo == null)
            {
                fgOk = false;
            }
            else if (campo.GetType() == typeof(string) && string.IsNullOrEmpty(campo.ToString()))
            {
                fgOk = false;
            }

            return fgOk;
        }

        public static string SomenteNumero(string valor)
        {
            string resultado = string.Empty;

            Regex regexObj = new Regex(@"[^\d]");

            resultado = regexObj.Replace(valor, "");

            return resultado;
        }

        public static string FormatarCNPJ(string cnpj)
        {
            return Convert.ToUInt64(SomenteNumero(cnpj)).ToString(@"00\.000\.000\/0000\-00");
        }
    }
}
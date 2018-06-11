using System.Collections.Generic;

namespace CadastroClientes
{
    public static class Dicionario
    {
        public static List<Colunas> RetornaColunasClientes()
        {
            return new List<Colunas>
            {
                new Colunas("Nome", "Nome"), new Colunas("RazaoSocial", "RazaoSocial"), new Colunas("Email", "Email"), new Colunas("CNPJ", "CNPJ"), new Colunas("TelefoneComercial", "TelefoneComercial"), new Colunas("TelefoneCelular", "TelefoneCelular"), new Colunas("CEP", "CEP"), new Colunas("Cidade", "Cidade"), new Colunas("Estado", "Estado")
            };
        }
    }

    public class Colunas
    {
        public Colunas(string colunaArquivo, string colunaBanco)
        {
            this.colunaArquivo = colunaArquivo;
            this.colunaBanco = colunaBanco;
        }

        public string colunaArquivo { get; set; }
        public string colunaBanco { get; set; }
    }
}

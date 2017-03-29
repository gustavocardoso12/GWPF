using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for Window4.xaml
    /// </summary>
    public partial class Window4 : Window
    {
        public Window4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (Validacao.ValidaCPF.IsCpf(txt2.Text))
            {
                string strcon = "Data Source=tcp:geasy.database.windows.net;Initial Catalog=Geasy;Persist Security Info=True;User ID=gustavo.peixinho;Password=Dfg654321";
                SqlConnection conexao = new SqlConnection(strcon);
                SqlCommand cmd2 = new SqlCommand("Select * from Usuario where Email='" + this.txt1.Text + "' and CPF='" + this.txt2.Text + "';", conexao);
                SqlDataReader leitor;
                conexao.Open();
                leitor = cmd2.ExecuteReader();
                int contador = 0;
                while (leitor.Read())
                {
                    contador++;
                }
                if (contador == 1)
                {
                    MessageBox.Show("Dados Válidos");
                    botao_recuperar.Visibility = Visibility.Visible;
                    Nova_Senha_TxtBlock.Visibility = Visibility.Visible;
                    Nova_Senha_txtbox.Visibility = Visibility.Visible;
                    txt1.IsEnabled = false;
                    txt2.IsEnabled = false;
                    button1_Copy.IsEnabled = false;
                }
            
                else
                {
                    Dados_Nao_Cadastrados D = new Dados_Nao_Cadastrados();
                    D.Show();
                }
            }
            else
            {
                CPF_Invalido C = new CPF_Invalido();
                C.Show();
            }
        }

        private void botao_recuperar_Click(object sender, RoutedEventArgs e)
        {
            string strcon = "Data Source=tcp:geasy.database.windows.net;Initial Catalog=Geasy;Persist Security Info=True;User ID=gustavo.peixinho;Password=Dfg654321";
            SqlConnection conexao = new SqlConnection(strcon);
            SqlCommand cmd2 = new SqlCommand("update Usuario SET Senha_Usuario='" + Nova_Senha_txtbox.Text + "'where Email='" + this.txt1.Text + "' and CPF='" + this.txt2.Text + "';", conexao);
            SqlDataReader leitor;
            conexao.Open();
            leitor = cmd2.ExecuteReader();

            MessageBox.Show("senha trocada");
        }

        private void voltar_Click(object sender, RoutedEventArgs e)
        {
            Window3 W = new Window3();
            W.Show();
            this.Close();
        }
    }
}

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
    /// Interaction logic for Window3.xaml
    /// </summary>
    public partial class Window3 : Window
    {
        public Window3()
        {
            InitializeComponent();

        }
       
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            /* esse pequeno trecho de decisão atribuirá um código ao campo "cod_tipo"
            dependendo de quem está acessando*/
            if(comboBox.Text == "Aluno")
            {
                texto_invisivel.Text = "1";

            }
            if (comboBox.Text == "Professor")
            {
                texto_invisivel.Text = "2";
            }

            string strcon = "Data Source=tcp:geasy.database.windows.net;Initial Catalog=Geasy;Persist Security Info=True;User ID=gustavo.peixinho;Password=Dfg654321";
            SqlConnection conexao = new SqlConnection(strcon);
            SqlCommand cmd2 = new SqlCommand("Select * from  Usuario where cod_tipo=" + int.Parse(texto_invisivel.Text) + " and Nome_Usuario='" + this.login.Text + "' and Senha_Usuario='" + this.senha.Text + "';", conexao);
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
              if(comboBox.Text == "Aluno")
                {
                    MessageBox.Show("passou aluno");
                }
                else
                {
                    Main M = new Main();
                    M.Show();
                }
            }
            else
            {
                Dados_Nao_Cadastrados D = new Dados_Nao_Cadastrados();
                D.Show();
            }
           
        }

        private void textBlock_Copy4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Cadastro C = new Cadastro();
            C.Show();
            this.Close();
        }

        private void textBlock_Copy6_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window4 W = new Window4();
            W.Show();
            this.Close();
        }

        private void button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

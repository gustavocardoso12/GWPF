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
    /// Interaction logic for Cadastro.xaml
    /// </summary>
    public partial class Cadastro : Window
    {
        public Cadastro()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            WpfApplication2.GeasyDataSet geasyDataSet = ((WpfApplication2.GeasyDataSet)(this.FindResource("geasyDataSet")));
            // Load data into the table Tipo. You can modify this code as needed.

            WpfApplication2.GeasyDataSetTableAdapters.TipoTableAdapter geasyDataSetTipoTableAdapter = new WpfApplication2.GeasyDataSetTableAdapters.TipoTableAdapter();

           geasyDataSetTipoTableAdapter.Fill(geasyDataSet.Tipo);
           
            System.Windows.Data.CollectionViewSource tipoViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("tipoViewSource")));
            tipoViewSource.View.MoveCurrentToFirst();
            // Load data into the table Usuario. You can modify this code as needed.
            WpfApplication2.GeasyDataSetTableAdapters.UsuarioTableAdapter geasyDataSetUsuarioTableAdapter = new WpfApplication2.GeasyDataSetTableAdapters.UsuarioTableAdapter();
            geasyDataSetUsuarioTableAdapter.Fill(geasyDataSet.Usuario);
            System.Windows.Data.CollectionViewSource tipoUsuarioViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("tipoUsuarioViewSource")));
         //   tipoUsuarioViewSource.View.MoveCurrentToFirst();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try {
                if (Validacao.ValidaCPF.IsCpf(cPFTextBox.Text)) {


                    if ((descricaoComboBox.Text == "") || (nome_UsuarioTextBox.Text == "") || (senha_UsuarioTextBox.Text == "") || (emailTextBox.Text == "") || (cPFTextBox.Text == ""))
                    {
                        Aviso_Cadastro_Camposvazios ACCV = new Aviso_Cadastro_Camposvazios();
                        ACCV.Show();
                    }
                    else {
                        string strcon = "Data Source=tcp:geasy.database.windows.net;Initial Catalog=Geasy;Persist Security Info=True;User ID=gustavo.peixinho;Password=Dfg654321";
                        SqlConnection conexao = new SqlConnection(strcon);

                        if (descricaoComboBox.SelectedIndex.ToString() == "0")
                        {
                            SqlCommand cmd2 = new SqlCommand("INSERT INTO Usuario (nome_usuario,senha_usuario,Email,CPF,cod_tipo) VALUES('" + nome_UsuarioTextBox.Text + "','" + senha_UsuarioTextBox.Text + "','" + emailTextBox.Text + "','" + cPFTextBox.Text + "'," + 1 + ") ", conexao);
                            conexao.Open();
                            cmd2.ExecuteNonQuery();

                            MessageBox.Show("passou aluno");
                        }
                        else
                        {

                            SqlCommand cmd2 = new SqlCommand("INSERT INTO Usuario (nome_usuario,senha_usuario,Email,CPF,cod_tipo) VALUES('" + nome_UsuarioTextBox.Text + "','" + senha_UsuarioTextBox.Text + "','" + emailTextBox.Text + "','" + cPFTextBox.Text + "'," + 2 + ") ", conexao);
                            conexao.Open();
                            cmd2.ExecuteNonQuery();

                            MessageBox.Show("passou professor");
                        }
                    }
                }
                else
                {
                    CPF_Invalido C = new CPF_Invalido();
                    C.Show();
                }
            }
            catch (Exception ex)
            {
                Dados_Ja_Cadastrados D = new Dados_Ja_Cadastrados();
                D.Show();
            }
        }
        private void descricaoComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          
        }

        private void button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            Window3 W = new Window3();
            W.Show();
            this.Close();
        }
    }
            
        }
    


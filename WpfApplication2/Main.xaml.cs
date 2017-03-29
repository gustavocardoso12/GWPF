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

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        //controle de opacidade do texto de boas vindas
        double opacidade = 0;
        double op;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer dispatcherTimer1 = new System.Windows.Threading.DispatcherTimer();
        public Main()
        {
            InitializeComponent();
            // timer do texto de boas vindas
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();
            //timer do texto indicativo
            dispatcherTimer1.Tick += DispatcherTimer1_Tick;
            dispatcherTimer1.Interval = new TimeSpan(0, 0, 0, 0, 100);
            //cx = texto de boas vindas
            cx.Visibility = Visibility.Visible;
            //cx = texto de pesquisa de temas
            cx2.Visibility = Visibility.Visible;
          

        }

        private void DispatcherTimer1_Tick(object sender, EventArgs e)
        {
            // visiblidade do texto de pesquisa de temas
            texto_2.Visibility = Visibility.Visible;
            // adiciona opacidade a cada tick do timer
            op = op + 0.1;
            // opacidade do texto recebe op
            texto_2.Opacity = op;
            // o texto de boas vindas perde opacidade a cada tick
            opacidade = opacidade - 0.07;
            // o texto de boas vindas recebe a opacidade decresente e desaparece
            texto_1.Opacity = opacidade;
            if (op>1)// se opacidade maior que 100%, o menu fica visivel e os textos somem
            {
                menu.Visibility = Visibility.Visible;
                op = op - 0.1;
                texto_2.Opacity = op;
                texto_1.Visibility = Visibility.Hidden;
           
            }
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {

                opacidade = opacidade + 0.03;
                texto_1.Opacity = opacidade;
            if (opacidade > 1)
            {
                double op = 1.0;
                texto_1.Opacity = op;
           
                dispatcherTimer1.Start();
                dispatcherTimer.Stop();
               
            }
            

        }

        // evento de previsão de movimento no botão "pesquisar temas"
        private void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // torna a grid "pesquisar temas principais" visível
            Grade_Proteção1.Visibility = Visibility.Visible;
            // muda o tamanho do botão "pesquisar temas"
            Menu.Width = 343;
            // muda o tamanho da grid principal
            dock.Width = 911;
            // esconde os textos de boas vindas
            cx.Width = 911;
            cx2.Width = 911;
            cx2.Visibility = Visibility.Hidden;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            // esconde a grid "pesquisar temas"
            Grade_Proteção1.Visibility = Visibility.Hidden;
            /* esconde todas as listas com os temas*/
            listBox_Copy.Visibility = Visibility.Hidden;
            listBox_Copy1.Visibility = Visibility.Hidden;
            listBox_Copy2.Visibility = Visibility.Hidden;
            listBox_Copy3.Visibility = Visibility.Hidden;
            Sub_Itens_Label.Visibility = Visibility.Hidden;
            // retorna o tamanho da grid
            Menu.Width = 343;
            dock.Width = 1407;
            cx.Width = 1327;
            cx2.Width = 1327;
        }

       

      

        private void ListBoxItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            listBox_Copy2.Visibility = Visibility.Hidden;
            listBox_Copy3.Visibility = Visibility.Hidden;
            listBox_Copy1.Visibility = Visibility.Hidden;
         

            Sub_Itens_Label.Visibility = Visibility.Visible;
            listBox_Copy.Visibility = Visibility.Visible;

        }

        private void ListBoxItem_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
         
            listBox_Copy2.Visibility = Visibility.Hidden;
            listBox_Copy3.Visibility = Visibility.Hidden;
            listBox_Copy1.Visibility = Visibility.Visible;
            listBox_Copy.Visibility = Visibility.Hidden;
            Sub_Itens_Label.Visibility = Visibility.Visible;
          
           
       
        }

        private void sair_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ListBoxItem_PreviewMouseDown_2(object sender, MouseButtonEventArgs e)
        {
            listBox_Copy2.Visibility = Visibility.Visible;
            listBox_Copy3.Visibility = Visibility.Hidden;
            listBox_Copy1.Visibility = Visibility.Hidden;
            listBox_Copy.Visibility = Visibility.Hidden;
            Sub_Itens_Label.Visibility = Visibility.Visible;
     
        }

        private void ListBoxItem_PreviewMouseDown_3(object sender, MouseButtonEventArgs e)
        {
            listBox_Copy2.Visibility = Visibility.Hidden;
            listBox_Copy3.Visibility = Visibility.Visible;
            listBox_Copy1.Visibility = Visibility.Hidden;
            listBox_Copy.Visibility = Visibility.Hidden;
            Sub_Itens_Label.Visibility = Visibility.Visible;
          
         

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Button_PreviewMouseMove_1(object sender, MouseEventArgs e)
        {
            cx2.Visibility = Visibility.Hidden;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Controle_Conectar C = new Controle_Conectar();
            C.Show();
        }

        private void ListBoxItem_PreviewMouseDown_4(object sender, MouseButtonEventArgs e)
        {

            frame.Navigate(new WpfApplication2.Geometria_Plana.Sem_Triang());
            button_sem_triang1.Visibility = Visibility.Visible;
        }

        private void texto_1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Controle_Conectar C = new Controle_Conectar();
            C.Show();
        }

        private void button_sem_triang1_Click(object sender, RoutedEventArgs e)
        {

            frame.Navigate(new WpfApplication2.Geometria_Plana.Sem_Triang1());
            button_sem_triang1.Visibility = Visibility.Collapsed;
            button_sem_triang2.Visibility = Visibility.Visible;
        }

        private void button_sem_triang2_Click(object sender, RoutedEventArgs e)
        {

            frame.Navigate(new WpfApplication2.Geometria_Plana.Sem_Triang2());
            button_sem_triang2.Visibility = Visibility.Collapsed;
        }
    }
}

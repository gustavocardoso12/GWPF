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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        public Window1()
        {
            InitializeComponent();

            InitializeComponent();
            
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0,0,100);
            dispatcherTimer.Start();
            
        }

       public void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            
            if (Barra_Carregamento.Value < 100)
            {
                Barra_Carregamento.Value = Barra_Carregamento.Value + 1;
                porcentagem.Content = Barra_Carregamento.Value.ToString();
               if(Barra_Carregamento.Value >=0)
                {
                    Carregador.Content = "Importando Bibliotecas...";
                }
                if (Barra_Carregamento.Value > 25)
                {
                    Carregador.Content = "Importando Formas...";
                }
                if (Barra_Carregamento.Value > 60)
                {
                    Carregador.Content = "Aguarde...";
                }
                if (Barra_Carregamento.Value > 90)
                {
                    image.Visibility = Visibility.Hidden;
                    if (Barra_Carregamento.Value > 99)
                    {
                        image.Visibility = Visibility.Visible;
                        
                    }
                    if (Barra_Carregamento.Value == 100)
                    {
                        dispatcherTimer.Stop();

                        Window3 W = new Window3();
                        W.Show();
                     
                        this.Close();
                    }
                }
            }
           
        }
    }
}

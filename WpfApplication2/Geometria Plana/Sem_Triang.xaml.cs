﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Packaging;
using System.Windows.Xps.Packaging;
namespace WpfApplication2.Geometria_Plana
{
    /// <summary>
    /// Interaction logic for Sem_Triang.xaml
    /// </summary>
    public partial class Sem_Triang : Page
    {
        public Sem_Triang()
        {
            InitializeComponent();
         
            XpsDocument xpsDocument = new XpsDocument("Geometria_plana_pdf_1.xps", System.IO.FileAccess.Read);
            documentViewer.Document = xpsDocument.GetFixedDocumentSequence();

            documentViewer.FitToWidth();
            
           
            documentViewer.Zoom = 170;
        }
    }
}

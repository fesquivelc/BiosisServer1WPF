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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BiosisServerWPF1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ServidorCallback
    {
        private Servidor servidor;
        public MainWindow()
        {
            InitializeComponent();
            this.servidor = new Servidor(this);
        }

        private void btnDetener_Click(object sender, RoutedEventArgs e)
        {
            if (this.servidor.servidorActivo)
            {
                servidor.detenerServidor();
            }
        }

        private void btnIniciar_Click(object sender, RoutedEventArgs e)
        {
            int puerto = 5678;
            if (!this.servidor.servidorActivo)
            {
                servidor.iniciarServidor(puerto);
            }
        }

        public void mensajeConsola(string mensaje)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    if (!string.IsNullOrEmpty(this.txtConsola.Text))
                    {
                        this.txtConsola.Text += "\n";
                    }
                    this.txtConsola.Text += mensaje;
                }
                ));
            //if (!string.IsNullOrEmpty(txtConsola.Text))
            //{
            //    txtConsola.Text += "\n";
            //}
            //txtConsola.Text += mensaje;
            Console.WriteLine(mensaje);
        }

    }
}

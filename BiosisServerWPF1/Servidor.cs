using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BiosisServerWPF1
{
    class Servidor
    {
        public bool servidorActivo { get; set; }
        private TcpListener tcpListener;
        private ServidorCallback callback;
        private int puerto;

        public Servidor(ServidorCallback callback)
        {
            this.callback = callback;
        }

        public void iniciarServidor(int puerto)
        {
            this.puerto = puerto;
            this.callback.mensajeConsola("Iniciando servidor...");
            this.servidorActivo = true;
            Thread hiloServidor = new Thread(new ThreadStart(start));
            hiloServidor.Start();
        }

        private void start()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, this.puerto);
                tcpListener.Start();
                tcpListener.BeginAcceptTcpClient(new AsyncCallback(this.procesarEvento), tcpListener);

                this.callback.mensajeConsola("Servidor en puerto: " + this.puerto);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                this.callback.mensajeConsola(e.ToString());
            }
            while (servidorActivo) ;
        }

        public void detenerServidor()
        {
            this.callback.mensajeConsola("Deteniendo servidor...");
            this.tcpListener.Server.Close();
            Thread.Sleep(500);
            this.servidorActivo = false;
            this.callback.mensajeConsola("Servidor detenido");
        }

        private void procesarEvento(IAsyncResult asyn)
        {
            try
            {
                TcpListener processListen = (TcpListener)asyn.AsyncState;
                TcpClient tcpClient = processListen.EndAcceptTcpClient(asyn);
                NetworkStream stream = tcpClient.GetStream();
                if (stream.CanRead)
                {
                    byte[] data = new byte[tcpClient.ReceiveBufferSize];
                    int bytesRead = stream.Read(data, 0, Convert.ToInt32(tcpClient.ReceiveBufferSize));
                    string request = Encoding.UTF8.GetString(data, 0, bytesRead);
                    this.callback.mensajeConsola("Request: " + request);

                    byte[] msg = Encoding.UTF8.GetBytes(request + " RETORNO");
                    stream.Write(msg, 0, msg.Length);
                    this.callback.mensajeConsola("Response: " + Encoding.UTF8.GetString(msg)+" TAMAÑO: "+msg.Length);
                    //StreamReader readerStream = new StreamReader(stream);
                    //string myMessage = readerStream.ReadToEnd();
                    //readerStream.Close();
                }
                stream.Close();
                tcpClient.Close();
                tcpListener.BeginAcceptTcpClient(new AsyncCallback(this.procesarEvento), tcpListener);
            }
            catch (Exception e)
            {
                this.callback.mensajeConsola(e.ToString());
                Console.WriteLine(e.ToString());
            }
        }
    }
}

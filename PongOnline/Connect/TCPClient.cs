using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace PongOnline.Connect
{
    public class TCPClient
    {
        public int Port { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }

        public TcpClient client { get; set; }
        public TCPClient(string address, int port)
        {
            Address = address;
            Port = port;
        }

        public void Connect()
        {
            client = new TcpClient(Address, Port);
        }

        public string SendMessage(string message)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                message = String.Format("{0}: {1}", UserName, message);
                // преобразуем сообщение в массив байтов
                byte[] data = Encoding.Unicode.GetBytes(message);
                // отправка сообщения
                stream.Write(data, 0, data.Length);

                // получаем ответ
                data = new byte[64]; // буфер для получаемых данных
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do
                {
                    bytes = stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (stream.DataAvailable);

                message = builder.ToString();
                return message;
            }
            catch (Exception ex)
            {
                client.Close();
                return "Connection closed";
            }
        }

        public string Register()
        {
            var command = "register";
            return SendMessage(command);
        }

        public string Update(string racketTop, string racketLeft, string ballTop, string ballLeft, string balldy, string balldx)
        {
            var command = $"update {racketTop} {racketLeft} {ballTop} {ballLeft} {balldy} {balldx}";
            return SendMessage(command);
        }

        public string Status()
        {
            var command = "status";
            return SendMessage(command);
        }

        public void Wait(CancellationToken token)
        {
            string status;
            do
            {
                if (token.IsCancellationRequested)
                {
                    Debug.WriteLine("Операция поиска противника прервана");
                    return;
                }
                Thread.Sleep(2000);
                status = Status();
                Debug.WriteLine(status);
            } while (status != "True");
        }

        public string Cancel()
        {
            var command = "cancel";
            return SendMessage(command);
        }

        public string StopGame()
        {
            var command = "stopgame";
            return SendMessage(command);
        }
    }
}

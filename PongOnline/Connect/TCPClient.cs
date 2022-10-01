using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data;

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

        public string Start()
        {
            var message = $"{UserName}: start";
            return SendMessage(message);
        }

        public string Update(string CanvasTop, string CanvasLeft)
        {
            var message = $"{UserName}: update {CanvasTop} {CanvasLeft}";
            return SendMessage(message);
        }

        public string Status()
        {
            var message = $"{UserName}: status";
            return SendMessage(message);
        }
    }
}

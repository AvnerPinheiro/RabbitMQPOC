using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCRabbitMQ
{
    class Program
    {
        static void Main(string[] args)
        {
            bool rodando = true;
            var conexao = GetConnectionFactory();
            var criacaoConexao =  CreateConnection(conexao);

            CreateQueue("teste1", criacaoConexao);

            while (rodando)
            {
                Console.Clear();
                Console.WriteLine("1 - Enviar mensagem");
                Console.WriteLine("2 - Ler Fila");
                Console.WriteLine("3 - Sair");

                var opcao = Console.ReadLine();
                rodando = opcao != "3";

                if(opcao == "1")
                {
                    Console.WriteLine("Digite a mensagem");
                    WriteMessageOnQueue(Console.ReadLine(),"teste1", criacaoConexao);
                }

                if(opcao == "2")
                {
                    Console.WriteLine(RetrieveSingleMessage("teste1",criacaoConexao));
                    Console.Read();
                }
            }
            
        }

        private static ConnectionFactory GetConnectionFactory()
        {
            return new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
        }

        private static IConnection CreateConnection(ConnectionFactory connectionFactory)
        {
            return connectionFactory.CreateConnection();
        }

        private static QueueDeclareOk CreateQueue(string queueName, IConnection connection)
        {
            QueueDeclareOk queue;
            using (var channel = connection.CreateModel())
            {
                queue = channel.QueueDeclare(queueName, false, false, false, null);
            }
            return queue;
        }

        private static bool WriteMessageOnQueue(string message, string queueName, IConnection connection)
        {
            using (var channel = connection.CreateModel())
            {
                channel.BasicPublish("", queueName, null, Encoding.ASCII.GetBytes(message));
            }

            return true;
        }

        private static string RetrieveSingleMessage(string queueName, IConnection connection)
        {
            BasicGetResult data;
            using (var channel = connection.CreateModel())
            {
                data = channel.BasicGet(queueName, true);
            }
            return data != null ? Encoding.UTF8.GetString(data.Body) : null;
        }
    }
}

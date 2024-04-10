using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Server
{
    static void Main()
    {
        var listener = new TcpListener(IPAddress.Any, 8888);
        listener.Start();
        Console.WriteLine("Сервер запущен. Ожидание подключений...");

        while (true)
        {
            var client = listener.AcceptTcpClient();
            Console.WriteLine($"Подключение от {((IPEndPoint)client.Client.RemoteEndPoint).Address}");

            var clientHandler = new ClientHandler(client);
            Task.Run(() => clientHandler.HandleClient());
        }
    }

    class ClientHandler
    {
        private TcpClient client;

        public ClientHandler(TcpClient tcpClient)
        {
            client = tcpClient;
        }

        public async Task HandleClient()
        {
            NetworkStream stream = client.GetStream();
            byte[] data = new byte[1024];
            int bytesRead;
            string response;

            try
            {
                while ((bytesRead = await stream.ReadAsync(data, 0, data.Length)) != 0)
                {
                    string receivedData = Encoding.UTF8.GetString(data, 0, bytesRead);
                    string[] namedata = receivedData.Split('~');
                    string name = namedata[0];
                    string filedata = namedata[1];
                    string operation = namedata[2];
                    Console.WriteLine(name);
                    Console.WriteLine(filedata);
                    string path = @"D:\2203 SERVER\";

                    switch (operation)
                    {
                        case "1":
                            try
                            {
                                byte[] fileBytes = await File.ReadAllBytesAsync(path + name);
                                string textFromFile = Encoding.UTF8.GetString(fileBytes);
                                response = "The request was sent (code 200)\nThe content of the file is: " + textFromFile;
                            }
                            catch (Exception ex)
                            {
                                response = "The request was not sent (code 403)\n" + ex.Message;
                            }
                            break;
                        case "2":
                            try
                            {
                                await File.WriteAllBytesAsync(path + name, Encoding.UTF8.GetBytes(filedata));
                                response = "The request was sent (code 200)";
                            }
                            catch (Exception ex)
                            {
                                response = "The request was not sent (code 403)\n" + ex.Message;
                            }
                            break;
                        case "3":
                            try
                            {
                                if (File.Exists(path + name))
                                {
                                    File.Delete(path + name);
                                    response = "The request was sent (code 200)\nThe response says that the file was successfully deleted!";
                                }
                                else
                                {
                                    response = "The request was sent (code 200)\nThe response says that the file was not found!";
                                }
                            }
                            catch (Exception ex)
                            {
                                response = "The request was not sent (code 403)" + ex.Message;
                            }
                            break;
                        default:
                            response = "Invalid operation";
                            break;
                    }
                    byte[] responseData = Encoding.UTF8.GetBytes(response);
                    await stream.WriteAsync(responseData, 0, responseData.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                client.Close();
            }
        }
    }
}

using System;
using System.Net.Sockets;

using System.Text;

class Client
{
    static void Main()
    {
        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            
            try
            {
                socket.Connect("127.0.0.1", 8888);
                bool exit = false;
                while (!(exit))
                {


                    string filename = "";
                    string data = "";
                    string operation;
                    string servop = "";
                    Console.Write("Enter action (1 - get a file, 2 - create a file, 3 - delete a file): > ");
                    operation = Console.ReadLine();
                    switch (operation)
                    {
                        case "2":
                            Console.Write("Enter filename: > ");
                            filename = Console.ReadLine();
                            Console.Write("Enter file content: > ");
                            data = Console.ReadLine();
                            servop = "2";
                            break;
                        case "3":
                            Console.Write("Enter filename: > ");
                            filename = Console.ReadLine();
                            servop = "3";
                            break;
                        case "1":
                            Console.Write("Enter filename: > ");
                            filename = Console.ReadLine();
                            data = "";
                            servop = "1";
                            break;
                        case "exit":
                            exit = true;
                            break;
                    }

                    if (!exit)
                    {


                        byte[] sendData = Encoding.UTF8.GetBytes(filename + "~" + data + "~" + servop);
                        socket.Send(sendData);
                        byte[] receiveData = new byte[256];
                        int bytesRead = socket.Receive(receiveData);
                        string result = Encoding.UTF8.GetString(receiveData, 0, bytesRead);
                        Console.WriteLine($"{result}");
                    }
                    else
                        socket.Close();
                }
            }
            catch (SocketException)
            {
                Console.WriteLine($"Не удалось установить подключение");
            }
        }
    }
}

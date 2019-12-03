using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using ConsoleApp1;
using WebApplication1;
using ConsoleTables;
using Topshelf;

namespace MultiClient
{
    class Program
    {

        private static readonly Socket ClientSocket = new Socket
        (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private const int PORT = 100;
        static string id = "";

        public static void Main()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Black;
            Program log = new Program();
            log.clientLogin();
            Transact();
        }



        
        public void clientLogin()
        {
            ConsoleApp1.localhost.WebService1 obj = new ConsoleApp1.localhost.WebService1();
            LaptopRead lap = new LaptopRead();
            MouseRead mouse = new MouseRead();
            PendriveRead pendrive = new PendriveRead();
            var table = new ConsoleTable("Shop Name", "Shop Type", "Shop Id");
            table.AddRow("CRAZY MACHINES", "Laptop Shop", "Laptop/laptop")
                 .AddRow("Tech Froggy Systems", "Mouse Shop", "Mouse/mouse")
                 .AddRow("Byte Smith Systems", "Pendrive Shop", "Pendrive/pendrive");
            Console.WriteLine(table);
            Console.WriteLine();
            Console.Write("Enter the shop Id you wish to Login - ");
            string id = Console.ReadLine();
            if (id == "Laptop" || id == "laptop")
            {
                connectToServer(id);
                Console.Title = "CRAZY MACHINES";
                string select;
                Console.WriteLine("Shop:1 - CRAZY MACHINES");
                Console.WriteLine();
                Console.Write("Press 1 to view operations - ");
                select = Console.ReadLine();
                Console.Clear();
                if (select == "1")
                {
                    string choice;
                    do
                    {
                        dynamic x = obj.Getlaptop();
                        Console.WriteLine(" * Press 1 to Billing   - Purchasing of Items");
                        Console.WriteLine(" * Press 2 for updation - Updating the Items price and Stock level of the items in current Inventory");
                        Console.WriteLine(" * Press 3 for Addition - Addition of new Items to the existing Catalogue");
                        Console.WriteLine(" * Press 4 for Deletion - Deleting the Existing Items from the Catalogue");
                        Console.WriteLine(" * Press 5 for Exit     - Got Exhausted of purchasing! Bye");
                        Console.WriteLine();
                        Console.Write("Enter the Choice - ");
                        choice = Console.ReadLine();
                        Console.Clear();
                        if (choice == "1")
                        {
                            lap.lapDisplay(x);
                        }
                        else if (choice == "2")
                        {
                            lap.updateDetail();
                        }
                        else if (choice == "3")
                        {
                            lap.addDetail();
                        }
                        else if (choice == "4")
                        {
                            lap.deleteDetail();
                        }
                        else if (choice == "5")
                        {
                            requestLoop();
                        }
                    } while (choice != "5");
                }
            }
            else if (id == "Mouse" || id == "mouse")
            {
                connectToServer(id);
                Console.Title = "Tech Froggy Systems";
                Console.WriteLine("Shop:2 - Tech Froggy Systems");
                Console.WriteLine();
                Console.Write("1.Press 1 to view options - ");
                string select = Console.ReadLine();
                Console.Clear();
                if (select == "1")
                {
                    string choice;
                    do
                    {
                        dynamic y = obj.Getmouse();
                        Console.WriteLine(" * Press 1 to Billing   - Purchasing of Items");
                        Console.WriteLine(" * Press 2 for updation - Updating the Items price and Stock level of the items in current Inventory");
                        Console.WriteLine(" * Press 3 for Addition - Addition of new Items to the existing Catalogue");
                        Console.WriteLine(" * Press 4 for Deletion - Deleting the Existing Items from the Catalogue");
                        Console.WriteLine(" * Press 5 for Exit     - Got Exhausted of purchasing! Bye");
                        Console.WriteLine();
                        Console.Write("Enter the Choice - ");
                        choice = Console.ReadLine();
                        Console.Clear();

                        if (choice == "1")
                        {
                            mouse.mouseDisplay(y);
                        }
                        else if (choice == "2")
                        {
                            mouse.updateDetail();
                        }
                        else if (choice == "3")
                        {
                            mouse.addDetail();
                        }
                        else if (choice == "4")
                        {
                            mouse.deleteDetail();
                        }
                        else if (choice == "5")
                        {
                            requestLoop();
                        }
                    } while (choice != "5");
                }

            }
            else if (id == "Pendrive" || id == "pendrive")
            {
                connectToServer(id);
                Console.Title = "Byte Smith Systems";
                Console.WriteLine("Shop:3 - Byte Smith Systems");
                Console.ReadLine();
                Console.Write("1.Press 1 to view options - ");
                string select = Console.ReadLine();
                Console.Clear();
                if (select == "1")
                {
                    string choice;
                    do
                    {
                        dynamic z = obj.Getpendrive();
                        Console.WriteLine(" * Press 1 to Billing   - Purchasing of Items");
                        Console.WriteLine(" * Press 2 for updation - Updating the Items price and Stock level of the items in current Inventory");
                        Console.WriteLine(" * Press 3 for Addition - Addition of new Items to the existing Catalogue");
                        Console.WriteLine(" * Press 4 for Deletion - Deleting the Existing Items from the Catalogue");
                        Console.WriteLine(" * Press 5 for Exit     - Got Exhausted of purchasing! Bye");
                        Console.WriteLine();
                        Console.Write("Enter the Choice - ");
                        choice = Console.ReadLine();
                        Console.Clear();
                        if (choice == "1")
                        {
                            pendrive.pendriveDisplay(z);
                            requestLoop();
                            exit();
                        }
                        else if (choice == "2")
                        {
                            pendrive.updateDetail();
                        }
                        else if (choice == "3")
                        {
                            pendrive.addDetail();
                        }
                        else if (choice == "4")
                        {
                            pendrive.deleteDetail();
                        }
                        else if (choice == "5")
                        {
                            requestLoop();
                        }
                    } while (choice != "5");
                }
            }
            else
            {
                Console.WriteLine("Please enter the correct ID");
                clientLogin();
                Console.ReadLine();
            }
        }
        private void connectToServer(string responseid)
        {
            int attempts = 0;

            while (!ClientSocket.Connected)
            {
                try
                {
                    attempts++;
                    //Console.WriteLine("Connection attempt " + attempts);
                    Console.WriteLine();
                    Console.Write("Enter the Server IP Address you wish to connect - ");
                    string address = Console.ReadLine();
                    ClientSocket.Connect(address, PORT);
                    byte[] nameByte = Encoding.ASCII.GetBytes(responseid);
                    byte[] nameBuffer = new byte[nameByte.Length];
                    nameByte.CopyTo(nameBuffer, 0);
                    ClientSocket.Send(nameBuffer);
                    Array.Clear(nameBuffer, 0, nameBuffer.Length);
                }
                catch (SocketException)
                {
                    Console.Clear();
                }
            }

            Console.Clear();
            Console.WriteLine(responseid+" Shop Connected");
        }
        private void requestLoop()
        {
            Console.WriteLine(@"<Type ""exit"" to properly disconnect client>");

            while (true)
            {
                sendRequest();
            }
        }

        private void exit()
        {
            sendString("exit"); // Tell the server we are exiting
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            Environment.Exit(0);
        }

        private void sendRequest()
        {
            Program exit = new Program();
            Console.Write("Send a request: ");
            string request = Console.ReadLine();
            sendString(request);

            if (request.ToLower() == "exit")
            {
                exit.exit();

            }
        }
        public static void Transact()
        {
            var exitcode = HostFactory.Run(x =>
            {
                x.Service<Transaction>(s =>
                {
                    s.ConstructUsing(trans => new Transaction());
                    s.WhenStarted(trans => trans.Start());
                    s.WhenStopped(trans => trans.Stop());
                });
                x.RunAsLocalSystem();

                x.SetServiceName("SyndicateServer");
                x.SetDisplayName("Syndiacte Server");
            });
            int exitcodeValue = (int)Convert.ChangeType(exitcode, exitcode.GetTypeCode());
            Environment.ExitCode = exitcodeValue;
        }
        public void transactDisplay()
        {

            string s1 = ConsoleApp1.Program.laptopShopPrice.ToString();
            Console.WriteLine("Value" + s1);
            string s2 = ConsoleApp1.Program.mouseShopPrice.ToString();
            string s3 = ConsoleApp1.Program.pendriveShopPrice.ToString();
            Transaction t1 = new Transaction();
            t1.demo(id, s1, s2, s3);
        }
        private void sendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }
    }
}
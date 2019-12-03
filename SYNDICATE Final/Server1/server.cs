﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Drawing;



using System.IO;
using System.Xml;
using System.Security;



namespace MultiServer
{
    class Program
    {



        private readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly List<Socket> clientSockets = new List<Socket>();
        private const int BUFFER_SIZE = 2048;
        private const int PORT = 100;
        private readonly byte[] buffer = new byte[BUFFER_SIZE];



        static void Main()
        {

            Console.Title = "Syndicate Server";
            Console.BackgroundColor = ConsoleColor.White;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Black;

            Program log = new Program();
            log.login();
            log.closeAllSockets();
        }

        //login
        private void login()
        {
            Users[] arrUsers = new Users[]
            {
                new Users("admin","admin",1),
            };

            Console.WriteLine("Server Login");

            bool successfull = false;
            while (!successfull)
            {
                Console.Write("Write your username: ");
                var username = Console.ReadLine();
                Console.Write("Enter your password: ");
                var password = "";
                do
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                    {
                        password += key.KeyChar;
                        Console.Write("*");
                    }
                    else
                    {
                        if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                        {
                            password = password.Substring(0, (password.Length - 1));
                            Console.Write("\b \b");
                        }
                        else if (key.Key == ConsoleKey.Enter)
                        {
                            break;
                        }
                    }
                } while (true);

                foreach (Users user in arrUsers)
                {
                    if (username == user.username && password == user.password)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Setting up Syndicate Server ...");
                        
                        Console.WriteLine("Press Enter to view the Syndicate Server Status...");
                        Console.ReadLine();
                        Console.Clear();
                        setupServer();
                        Console.ReadLine();
                        successfull = true;
                        break;
                    }
                }
                if (!successfull)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Your username or password is incorect, try again !!!");
                }
            }
        }



        public class Users
        {
            public string username;
            public string password;
            public int id;
            public Users(string username, string password, int id)
            {
                this.username = username;
                this.password = password;
                this.id = id;
            }
        }



        private void setupServer()
        {
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(acceptCallback, null);
            Console.WriteLine("Syndicate Server set up completed");
            Console.WriteLine("Syndicate Server is now ready to accept incoming client requests!");
        }



        private void closeAllSockets()
        {
            Program log = new Program();



            Console.WriteLine("Do you want to close server (Y/N)");
            string tex = Console.ReadLine();
            if (tex.ToLower() == "y")
            {
                Console.WriteLine("Bye");
                serverSocket.Close();
            }
            else if (tex.ToLower() == "n")
            {
                Console.Clear();
                log.login();
            }
        }



        private void acceptCallback(IAsyncResult AR)
        {
            Socket socket;



            try
            {
                socket = serverSocket.EndAccept(AR);
                byte[] namebuffer = new byte[1024];
                int name_byte_count = socket.Receive(namebuffer);
                string client_name = Encoding.ASCII.GetString(namebuffer, 0, name_byte_count);
                clientSockets.Add(socket);
                socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, receiveCallback, socket);
                Console.WriteLine("Client " + client_name + " connected, waiting for request...");
                serverSocket.BeginAccept(acceptCallback, null);
            }
            catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }
        }



        private void receiveCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received;



            try
            {
                received = current.EndReceive(AR);

            }
            catch (SocketException)
            {
                Console.WriteLine("Client forcefully disconnected");
                // Don't shutdown because the socket may be disposed and its disconnected anyway.
                current.Close();
                clientSockets.Remove(current);
                return;
            }



            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            string text = Encoding.ASCII.GetString(recBuf);
            Console.WriteLine("Received Text: " + text);

            if (text.ToLower() == "exit") // Client wants to exit gracefully
            {
                current.Shutdown(SocketShutdown.Both);
                current.Close();
                clientSockets.Remove(current);
                Console.WriteLine("Client disconnected");
                return;
            }

            else
            {
                Console.WriteLine("Text is an invalid request");
                byte[] data = Encoding.ASCII.GetBytes("Invalid request");
                current.Send(data);
                Console.WriteLine("Warning Sent");
            }
            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, receiveCallback, current);
        }
    }
}
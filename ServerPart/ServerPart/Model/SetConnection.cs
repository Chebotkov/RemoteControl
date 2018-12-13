﻿using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace ServerPart
{
    public enum Buttons { leftbutton = 1, rightbutton, start = 3, previousSlide, nextSlide, end, SymbolRecieved = 50, GetDrives = 100, GetPresentations, IsFileExists, RunFile, RunPresentation, PreviousDirectory };

    public class ServerCreator
    {
        public bool isRunning = false;
        public readonly int port = 10000;
        public readonly IPAddress ipAddress;
        private Socket socket;

        public ServerCreator(IPAddress ipAddress)
        {
            if (ipAddress is null)
            {
                throw new ArgumentNullException(String.Format("Argument {0} is null", nameof(ipAddress)));
            }

            this.ipAddress = ipAddress;
        }

        public ServerCreator(IPAddress ipAddress, int port) : this(ipAddress)
        {
            if(port < 0 || port > 65535)
            {
                throw new ArgumentOutOfRangeException(String.Format("Wrong number of port {0}", nameof(ipAddress))); 
            }

            this.port = port;
        }

        public void SetConnection()
        {
            isRunning = true;
            // Creating local end point
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);

            socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // Binding
                socket.Bind(ipEndPoint);
                // Listening Mode
                socket.Listen(1);

                while (true)
                {
                    // Waiting for connection
                    Socket handler = socket.Accept();
                    // Getting data
                    byte[] recBytes = new byte[1024];
                    int nBytes = handler.Receive(recBytes);

                    switch ((Buttons)recBytes[0])
                    {
                        case Buttons.SymbolRecieved:
                            {
                                string symbol = DataBinary.GetNormalRepresentation<string>(recBytes, 1, recBytes.Length - 2);
                                if (symbol == " ")
                                {
                                    System.Windows.Forms.SendKeys.SendWait(" ");
                                }
                                else
                                System.Windows.Forms.SendKeys.SendWait(String.Concat("{", symbol, "}"));
                                break;
                            }
                        case Buttons.leftbutton:
                            Mouse.Click(Buttons.leftbutton);
                            break;
                        case Buttons.rightbutton:
                            Mouse.Click(Buttons.rightbutton);
                            break;
                        case Buttons.start:
                            System.Windows.Forms.SendKeys.SendWait("{F5}");
                            break;
                        case Buttons.nextSlide:
                            System.Windows.Forms.SendKeys.SendWait("{RIGHT}");

                            byte[] bytess = null;
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                ScreenShot.GetScreenShot().Save(memoryStream, ImageFormat.Png);
                                bytess = memoryStream.GetBuffer();
                                MessageBox.Show(memoryStream.Length.ToString());
                            }
                            handler.Send(bytess);
                            break;
                        case Buttons.previousSlide:
                            System.Windows.Forms.SendKeys.SendWait("{LEFT}");
                            break;
                        case Buttons.end:
                            System.Windows.Forms.SendKeys.SendWait("{ESC}");
                            break;
                        case Buttons.GetDrives:
                            {
                                handler.Send(DataBinary.GetBinaryRepresentationWX(FileWorker.GetDrives()));
                                break;
                            }
                        case Buttons.GetPresentations:
                            {
                                int chosenDrive = DataBinary.GetNormalRepresentation<int>(recBytes, 1, recBytes.Length - 2);
                                bool isFile = false;
                                var answer = FileWorker.GetListOfContainingFiles(chosenDrive, ".pptx", ref isFile);

                                if (isFile)
                                {
                                    Process.Start(answer[0]);
                                }
                                else
                                {
                                    handler.Send(DataBinary.GetBinaryRepresentationWX(answer));
                                }

                                break;
                            }
                        case Buttons.IsFileExists:
                            {
                                if (FileWorker.IsFileExists(DataBinary.GetNormalRepresentation<string>(recBytes, 1, recBytes.Length - 2)))
                                {
                                    handler.Send(new byte[] { 1 });
                                }
                                else
                                {
                                    handler.Send(new byte[] { 0 });
                                }

                                break;
                            }
                        case Buttons.RunFile:
                            {
                                string pathToFile = DataBinary.GetNormalRepresentation<string>(recBytes, 1, recBytes.Length - 2);
                                Process.Start(pathToFile);
                                break;
                            }
                        case Buttons.RunPresentation:
                            {
                                string pathToFile = DataBinary.GetNormalRepresentation<string>(recBytes, 1, recBytes.Length - 2);
                                Process.Start(FileWorker.GetFilePath(pathToFile, FileWorker.driveName));
                                break;
                            }
                        case Buttons.PreviousDirectory:
                            {
                                if (FileWorker.driveName == null)
                                {
                                    handler.Send(DataBinary.GetBinaryRepresentationWX(FileWorker.GetDrives()));
                                    break;
                                }
                                bool isFile = false;

                                if (FileWorker.GetParentPath() == null)
                                {
                                    FileWorker.driveName = null;
                                    handler.Send(DataBinary.GetBinaryRepresentationWX(FileWorker.GetDrives()));
                                }
                                else
                                {
                                    handler.Send(DataBinary.GetBinaryRepresentationWX(FileWorker.GetListOfContainingFiles(1, ".pptx", ref isFile)));
                                }
                                break;
                            }
                    }
                    // Socket release
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void CloseConnection()
        {
            if (socket != null)
            {
                socket.Close();
                isRunning = false;
            }
        }
    }
}
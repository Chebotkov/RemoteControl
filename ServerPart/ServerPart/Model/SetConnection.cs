using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace ServerPart.Model
{
    public enum Buttons { StartPresentation = 1, PreviousSlide, NextSlide, EndPresentation, MoveMouse, leftbutton, rightbutton, ResponseReceived, SymbolRecieved = 50, GetDrives = 100, GetPresentations, IsFileExists, RunFile, RunPresentation, PreviousDirectory };

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
            if (port < 0 || port > 65535)
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

                    bool isSendingMust  = recBytes[0] > 0 ? true : false;

                    switch ((Buttons)recBytes[1])
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
                        case Buttons.StartPresentation:
                            System.Windows.Forms.SendKeys.SendWait("{F5}");
                            break;
                        case Buttons.NextSlide:
                            System.Windows.Forms.SendKeys.SendWait("{RIGHT}");
                            break;
                        case Buttons.PreviousSlide:
                            System.Windows.Forms.SendKeys.SendWait("{LEFT}");
                            break;
                        case Buttons.EndPresentation:
                            System.Windows.Forms.SendKeys.SendWait("{ESC}");
                            break;
                        case Buttons.MoveMouse:
                            byte[] firstFloat = new byte[4];
                            for (int i = 2, j = 0; i < 6; i++, j++)
                            {
                                firstFloat[j] = recBytes[i]; 
                            }

                            byte[] secondFloat = new byte[4];
                            for (int i = 6, j = 0; i < 10; i++, j++)
                            {
                                secondFloat[j] = recBytes[i];
                            }

                            MessageBox.Show(Math.Round(System.BitConverter.ToSingle(firstFloat, 0), 1).ToString() + "; " + Math.Round(System.BitConverter.ToSingle(secondFloat, 0), 1).ToString());
                            
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

                    if (isSendingMust)
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            ScreenShot.GetScreenShot().Save(memoryStream, ImageFormat.Jpeg);
                            byte[] bytess = memoryStream.GetBuffer();

                            int sendedBytes = 0;
                            int bufSize = 8;

                            handler.Send(Encoding.Default.GetBytes(bytess.Length.ToString() + "\n"));
                            while (sendedBytes < bytess.Length)
                            {
                                sendedBytes += handler.Send(bytess, sendedBytes, bytess.Length - sendedBytes > bufSize ? bufSize : bytess.Length - sendedBytes, SocketFlags.None);
                            }

                            handler.Send(bytess);
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServerPart.Model;

namespace ServerPart.ViewModel
{
    class MainWindowModel : INotifyPropertyChanged
    {
        private BaseCommand openConnection;
        private BaseCommand closeConnection;
        private string enteredIP = null;
        private ServerCreator server;
        private bool isConnectionOpened = false;

        public string EnteredIP
        {
            get
            {
                return enteredIP;
            }
            set
            {
                enteredIP = value;
                OnPropertyChanged(nameof(EnteredIP));
            }
        }

        public BaseCommand OpenConnection
        {
            get
            {
                return openConnection ??
                (openConnection = new BaseCommand(obj =>
                {
                    if (IPAddress.TryParse(enteredIP, out IPAddress ipAddress) && !isConnectionOpened)
                    {
                        server = new ServerCreator(ipAddress);
                        CreateConnection();
                        isConnectionOpened = true;

                        MessageBox.Show("Connection opened");
                    }

                    else
                    {
                        MessageBox.Show("Wrong ip");
                    }
                },
                (obj) =>
                {
                    if (!isConnectionOpened && !String.IsNullOrEmpty(enteredIP))
                    {
                        return true;
                    }

                    return false;
                }
                ));
            }
            private set { }
        }

        public BaseCommand CloseConnection
        {
            get
            {
                return closeConnection ??
                (closeConnection = new BaseCommand(obj =>
                {
                    if (server != null)
                    {
                        server.CloseConnection();
                    }

                    isConnectionOpened = false;
                    MessageBox.Show("Connection closed");
                },
                (obj) => isConnectionOpened));
            }
            private set { }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private async void CreateConnection()
        {
            await Task.Factory.StartNew(server.SetConnection);
        }
    }
}

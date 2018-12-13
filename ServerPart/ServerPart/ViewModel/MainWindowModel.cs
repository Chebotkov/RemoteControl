using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerPart.ViewModel
{
    class MainWindowModel : INotifyPropertyChanged
    {
        private BaseCommand openConnection;
        private BaseCommand closeConnection;
        private string enteredIP;

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
                    MessageBox.Show("Connection opened");
                }));
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
                    MessageBox.Show("Connection closed");
                    /*
                    if (IPAddress.TryParse(textField.Text, out IPAddress ipAddress))// && SetConnection.isRunning == false))
                    {
                        //SetConnection.ipAddress = ipAddress;
                        //await Task.Factory.StartNew(SetConnection.Set);
                    }
                    */
                }));
            }
            private set { }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

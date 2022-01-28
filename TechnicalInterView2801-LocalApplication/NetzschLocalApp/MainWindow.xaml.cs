using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;

namespace NetzschLocalApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        WebSocketHelper wsh;
        Task task;
        

        private string _inText;
        public string inText
        {
            get
            {
                return _inText;
            }
            set
            {
                _inText = value;
                OnPropertyChanged(new PropertyChangedEventArgs("inText"));
            }
        }
        private string _outText;
        public string outText
        {
            get
            {
                return _outText;
            }
            set
            {
                _outText = value;
                
            }
        }

        public MainWindow()
        {
            
            InitializeComponent();
            wsh = new WebSocketHelper(this);
            task = wsh.ConnectAsync("ws://127.0.0.1:10000");
            this.DataContext = this;


        }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
                
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //Disconnects the socket on Exit
           task = wsh.DisconnectAsync();
        }


        private void outStringTB_LostFocus(object sender, RoutedEventArgs e)
        {
            //When focus is lost message is sent
            //-> could be improved by creating own eventhandler
            String sText = outStringTB.Text;
            task = wsh.SendMessageAsync(sText);
        }


        public void ChangeTB(string message)
        {
            //Triggers the onPropertyChanged event
            inText = message;
            
        }
    }
}

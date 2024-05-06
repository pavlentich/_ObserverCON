using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ObserverCON
{
    class Program
    {
        static int Counter = 0;
        static Timer GlobalTimer = null;
        static TcpListener Server = new TcpListener(IPAddress.Any, 12345);// null;
        static TcpClient ClientConnection = null;
        static NetworkStream StreamFromClient = null;
        static StreamReader ReaderFROmClient= null; //= new StreamReader(StreamFromClient, Encoding.UTF8)
        static StreamWriter WriterToClient = null;

        static async Task Main()
        {            
            Server.Start();    Console.WriteLine("22 Server is running...");   
            TimerCallback timerCallback = new TimerCallback(state => TimerCallbackMethod( ClientConnection)); //
            // создаю таймер по тику которогобудет визивиться основное действие.  
            GlobalTimer = new Timer(timerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));           

            while (true)
            {
                
                using (ClientConnection= Server.AcceptTcpClient()) // ; // Console.WriteLine("33 Server : AcceptTcpClient.");
                using (StreamFromClient= ClientConnection.GetStream()) //  // Console.WriteLine("35 Server : ClientConnection.GetStream.");
                using (ReaderFROmClient = new StreamReader(StreamFromClient, Encoding.UTF8)) //; //Console.WriteLine("37 Server : Start stream reading from client.");
                using (WriterToClient= new StreamWriter(StreamFromClient, Encoding.UTF8)) // ;
                {

                    //var _ClientConnection = ClientConnection;
                    //var _StreamFromClient = StreamFromClient;
                    //var _reader = ReaderFROmClient;
                    //var _writer = WriterToClient;
                    //if (_ClientConnection.Client.Connected == false)
                    //    ;
                    Console.WriteLine("42 Server : start stream writing to client.");
                    
                    try
                    {
                        WriterToClient.WriteLine(" 43 Server To Client : Client connected successfully.");                       
                        WriterToClient.Flush();                        
                    }
                    catch (IOException ex)
                    {
                        // Обработка ошибки записи в сетевое соединение
                        Console.WriteLine("An error occurred while writing to the client: " + ex.Message);
                        // Дополнительные действия, например, закрытие соединения или повторная попытка отправки данных
                    }

                    while (true)
                    {
                        string command;
                        try
                        {                            
                            command = await ReaderFROmClient.ReadLineAsync();// Async read string from stream
                            ;
                        }
                        catch (IOException ex)
                        {
                            Console.WriteLine(" 52 Error reading data from client: " + ex.Message);
                            break; // somethint goes wrong
                        }
                        CommandChecker(command);                        

                    }
                }
            }
        }

        static void TimerCallbackMethod( TcpClient client)
        {
            Counter++;
            
            //var _ClientConnection = ClientConnection;
            //var _StreamFromClient = StreamFromClient;
            //var _reader = ReaderFROmClient;
            //var _writer = WriterToClient;
            string ClientStatus="88unknown";
            if (ClientConnection == null
                || ClientConnection.Client==null
                || ClientConnection.Connected==false
                )
            {
                if (ClientConnection == null)  ClientStatus = "ClientStatus: Didn`t appear"; 
                if (ClientConnection != null&&ClientConnection.Client == null) ClientStatus = "ClientStatus: Disconnected";
               if (ClientConnection != null&&ClientConnection.Connected == false) ClientStatus = "ClientStatus: Disconnected";
                ClientConnection = null;// clear client History
             }
            else
            {
                string currenDateTime = String.Format("{0:HH:mm:ss}", DateTime.Now);
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(new { CounterValue = Counter, TimeMark = currenDateTime }); // Отправка значения счетчика
                
                Console.WriteLine(" 137 TO client: " + jsonData);
                try
                {
                    WriterToClient.WriteLine(jsonData);
                    WriterToClient.Flush();
                    ClientStatus = "ClientStatus: Connected";
                }
                catch (IOException ex)
                {
                    // Обработка ошибки записи в сетевое соединение
                    Console.WriteLine("An error occurred while writing to the client: " + ex.Message);
                    // Дополнительные действия, например, закрытие соединения или повторная попытка отправки данных
                }

            }
            
            Console.WriteLine("142 Server cycle № is:" + Counter + "; timeMark is "+ String.Format("{0:HH:mm:ss}", DateTime.Now) + ", " + ClientStatus);
                 

        }
        static void CommandChecker(string a_command)
        {
            //if (a_command == "STOP")
            //{
            //    Console.WriteLine("Timer stopped by client.");
            //    GlobalTimer.Dispose();
            //}
            if (a_command == "RESET")
            {
                Console.WriteLine(" 131 Server : Timer resetted by client.");
                
                    Counter = 0;
                           
            }
            //if (a_command == "RUN")
            //{
            //    Console.WriteLine("Timer Runned by client.");
            //    GlobalTimer = new Timer(timerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(4));
            //}
            if (a_command == "HELLO")
            {
                Console.WriteLine(" Message From CLIENT: HELLO ");
            }

        }

    }
}
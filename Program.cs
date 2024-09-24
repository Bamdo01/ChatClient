using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace BasicSocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Baisc TCP Client");
            StartClient();
        }

        private static void StartClient()
        {
            // (1) 소켓 객체 생성 (TCP 소켓)
            Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // (2) 서버의 엔드포인트 값을 객체를 생성하고 서버에 연결 시도
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7000);
                sender.Connect(remoteEP);

                string cmd = string.Empty;

                Thread commandThread = new Thread(readClient);
                commandThread.Start(sender);

                Console.WriteLine("Connected... Enter Quit to exit");

                while (true)
                {
                    // 서버에 보낼 문자열 데이터를 콘솔에 입력
                    cmd = Console.ReadLine();
                    // 콘솔에 입력된 문자열을 바이트 배열로 인코딩
                    byte[] buff = Encoding.UTF8.GetBytes(cmd);

                    // (3) 서버에 데이터 전송(Send)
                    int bytesSent = sender.Send(buff);
                 

                    // 콘솔에 Quit를 입력 시 반복문 종료
                    if (cmd.Equals("Quit"))
                    {
                        break;
                    }
                }

                // (5) 소켓 닫기
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected exception : {0}", ex.ToString());
            }
        }

        private static void readClient(object obj)
        {
            Socket rederSocket = (Socket)obj;

            // 서버로부터 데이터를 받기 위한 버퍼(Buffer) 바이트 배열 초기화
            byte[] receiverBuff = new byte[8192];
            while (true)
            {
                // (4) 서버에서 데이터 수신(Receive)
                int bytesRec = rederSocket.Receive(receiverBuff);

                // 서버에서 받은 데이터 UTF8로 인코딩해서 콘솔에 출력
                Console.WriteLine("Echoed test = {0}", Encoding.UTF8.GetString(receiverBuff, 0, bytesRec));
            }
            

        }

            
    }
}
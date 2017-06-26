using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CIRClient
{
    class SyncClient
    {
        private static Socket client;

        private static byte[] bytes = new byte[1024];

        public static void StartClient(string serverAddr, int serverPort)
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(serverAddr), serverPort);

            client = new Socket(ipep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                client.Connect(ipep);

                // 超时时间 5s
                client.ReceiveTimeout = 5000;
                client.SendTimeout = 5000;

                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void SendBytes(byte[] dataBytes)
        {
            try
            {
                int bytesSent = client.Send(dataBytes);

                Console.WriteLine("Send {0} bytes to server", bytesSent.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }

        // 接受的前四个bytes是一个int 代表后面信息的长度 然后循环接受后面的信息
        public static string ReceiveString()
        {
            try
            {
                int count = 0;
                byte[] dataLen = new byte[4];
                client.Receive(dataLen);
                int dataLenInt = BitConverter.ToInt32(dataLen, 0);
                byte[] dataBytes = new byte[dataLenInt];
                byte[] tmp = new byte[1024];

                int recvNum = 0;
                do
                {
                    if (client.Poll(-1, SelectMode.SelectRead))
                    {
                        recvNum = client.Receive(tmp);
                        if (recvNum == 0)
                        {
                            return null;
                        }
                        Array.Copy(tmp, 0, dataBytes, count, recvNum);
                        count += recvNum;
                    }
                } while (count < dataLenInt);

                Console.WriteLine("Recieve from server:" + Encoding.UTF8.GetString(dataBytes));
                return Encoding.UTF8.GetString(dataBytes);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

using System;
using System.Net.Sockets;
using WinSCP;

namespace CSIA.Backend;

public class FTPClass
{
    private static FTPClass _instance;
    public static FTPClass Instance => _instance ??= new FTPClass();

    public SessionOptions _ftpSessionOptions;
    public Session FTPSession { get; private set; }

    private FTPClass()
    {
        FTPSession = new Session();
    }

    public bool Connect(string host, int? port, string uname, string upass)
    {
        _ftpSessionOptions = new SessionOptions
        {
            HostName = host,
            PortNumber = port ?? 21,
            UserName = uname,
            Password = upass,
            Protocol = Protocol.Ftp
        };
        FTPSession.ExecutablePath = "./WinSCP.exe";
        try
        {
            FTPSession.Open(_ftpSessionOptions);
            return true;
        }
        catch (SessionException ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public bool IsOpen()
    {
        return FTPSession.Opened;
    }
    
    public static bool PingHost(string host, int port)
    {
        try
        {
            using (var client = new TcpClient(host, port))
                Console.WriteLine($"Responded. IP: {host}:{port}");
            return true;
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Failed. IP: {host}:{port}");
            Instance.Disconnect();
            return false;
        }
    }

    public void Disconnect()
    {
        if (FTPSession.Opened)
        {
            FTPSession.Close();
            Console.WriteLine("Session closed.");
        }
    }
}
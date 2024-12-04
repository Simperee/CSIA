using System;
using WinSCP;

namespace CSIA.Backend;

public class FTPClass
{
    private static FTPClass _instance;
    public static FTPClass Instance => _instance ??= new FTPClass();

    private SessionOptions _ftpSessionOptions;
    public Session FTPSession { get; private set; }

    private FTPClass()
    {
        FTPSession = new Session();
    }

    public void Connect(string host, int? port, string uname, string upass)
    {
        _ftpSessionOptions = new SessionOptions
        {
            HostName = host,
            PortNumber = port ?? 21,
            UserName = uname,
            Password = upass,
            Protocol = Protocol.Ftp
        };

        FTPSession.Open(_ftpSessionOptions);
        Console.WriteLine($"Opened: {FTPSession.Opened}");
    }

    public bool IsOpen()
    {
        if (FTPSession.Opened)
        {
            try
            {
                RemoteDirectoryInfo directory = FTPSession.ListDirectory(FTPSession.HomePath);
                foreach (RemoteFileInfo fileInfo in directory.Files)
                {
                    Console.WriteLine($"{fileInfo.Name}, {fileInfo.FullName}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        else
        {
            Console.WriteLine("Session is not open.");
        }

        return FTPSession.Opened;
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
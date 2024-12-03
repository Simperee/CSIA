using System;
using Limilabs.FTP.Client;
using WinSCP;

namespace CSIA.Backend;

public class FTPClass
{
    private SessionOptions FTPSessionOptions = new SessionOptions();
    public Session FTPSession = new Session();
    
    public void Connect(string host, int? port, string uname, string upass)
    {
        FTPSessionOptions.HostName = host;
        FTPSessionOptions.PortNumber = port ?? 21;
        FTPSessionOptions.UserName = uname;
        FTPSessionOptions.Password = upass;
        FTPSessionOptions.Protocol = Protocol.Ftp;
        
        FTPSession.Open(FTPSessionOptions);
        var test = FTPSession.Opened;
        Console.WriteLine($"Opened: {test}");
    }

    public void FileGet()
    {
        Console.WriteLine($"Home Path: {FTPSession.HomePath}");
        RemoteDirectoryInfo directory =
            FTPSession.ListDirectory(FTPSession.HomePath);
 
        foreach (RemoteFileInfo fileInfo in directory.Files)
        {
            Console.WriteLine(
                "{0} with size {1}, permissions {2} and last modification at {3}, Directory: {4}",
                fileInfo.Name, fileInfo.Length, fileInfo.FilePermissions,
                fileInfo.LastWriteTime, fileInfo.FileType);
            Console.WriteLine(fileInfo);
        }
        
    }

    // Ftp client = new Ftp();
    //
    // public void Connect(string host, int? port, string uname, string upass)
    // {
    //     client.Connect(host, port ?? 21);
    //     client.Login(uname, upass);
    //
    //     var test = client.Connected;
    //     Console.WriteLine(test);
    // }
    //
    // public void sex()
    // {
    //     Console.WriteLine(client.SendCommand("HELP"));
    //     // var test = client.GetList().Count;
    //     // Console.WriteLine(test.ToString());
    // }
    //
    // public void SendFile()
    // {
    //     client.Upload(@"/test.txt",@"c:\Users\mayie\RiderProjects\CSIA\CSIA\test.txt");
    // }
}
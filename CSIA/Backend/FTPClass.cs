using System;
using Limilabs.FTP.Client;

namespace CSIA.Backend;

public class FTPClass
{
    Ftp client = new Ftp();

    public void Connect(string host, int? port, string uname, string upass)
    {
        client.Connect(host, port ?? 21);
        client.Login(uname, upass);

        var test = client.Connected;
        Console.WriteLine(test);
    }

    public void sex()
    {
        Console.WriteLine(client.SendCommand("HELP"));
        // var test = client.GetList().Count;
        // Console.WriteLine(test.ToString());
    }

    public void SendFile()
    {
        client.Upload(@"/test.txt",@"c:\Users\mayie\RiderProjects\CSIA\CSIA\test.txt");
    }
}
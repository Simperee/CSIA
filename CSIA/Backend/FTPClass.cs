using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using CSIA.Views;
using HarfBuzzSharp;
using WinSCP;

namespace CSIA.Backend;

public class FTPClass
{
    private PopUpDialog popUpDialog;
    
    private static FTPClass _instance;
    public static FTPClass Instance => _instance ??= new FTPClass();

    public SessionOptions _ftpSessionOptions;
    public Session FTPSession { get; private set; }
    public String RemotePath { get; private set; }
    public String uncleanedPath { get; private set; }
    
    //---------------------------//
    // Main Connection Functions //
    //---------------------------//

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
            RemotePath = FTPSession.HomePath;
            Console.WriteLine(RemotePath);
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
    
    //---------------------------//
    // Post Connection Functions //
    //---------------------------//
    
    public RemoteDirectoryInfo DirectoryItems(string path)
    {
        Console.WriteLine($"Path: {path}");
        string tmp = NormalizePath(path);
        string cleanedPath = tmp.Replace("/..", "/");
        Console.WriteLine($"Cleaned Path: {cleanedPath}");
        var items = FTPSession.ListDirectory(cleanedPath);
        RemotePath = cleanedPath;
        uncleanedPath = path;
        return items;
    }

    public void DeleteItem(string filepath)
    {
        var removalResult = FTPSession.RemoveFiles(filepath);
        if (removalResult.IsSuccess)
        {
            Console.WriteLine("Successfully deleted " + filepath);
        }
        else
        {
            Console.WriteLine("Failed to delete " + filepath);
        }
    }

    public async Task CreateDirectory(string dirName)
    {
        // var tmpFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CSIA", "tmp", dirName);
        // Directory.CreateDirectory(tmpFolder);
        // Console.WriteLine("Created directory: " + tmpFolder);
        // await TransferFile(tmpFolder, RemotePath);
        // Directory.Delete(tmpFolder, true);
        FTPSession.CreateDirectory(Path.Combine(RemotePath, dirName));
    }

    public async void UploadFile(string filePath, string remotePath)
    {
        try
        {
            TransferOptions transferOptions = new TransferOptions();
            transferOptions.PreserveTimestamp = true;
            transferOptions.TransferMode = TransferMode.Binary;

            TransferOperationResult transferResult;
            transferResult = FTPSession.PutFiles(filePath, remotePath+"/", false, transferOptions);

            transferResult.Check();
            foreach (TransferEventArgs transfer in transferResult.Transfers)
            {
                Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    public async void DownloadFile(string remoteFilePath, string localPath)
    {
        try
        {
            Console.WriteLine($"Remote: {remoteFilePath}");
            Console.WriteLine($"Local: {localPath}");
            TransferOptions transferOptions = new TransferOptions();
            transferOptions.PreserveTimestamp = true;
            transferOptions.TransferMode = TransferMode.Binary;

            TransferOperationResult transferResult;
            transferResult = FTPSession.GetFiles(remoteFilePath, localPath+@"\", false, transferOptions);

            transferResult.Check();
            foreach (TransferEventArgs transfer in transferResult.Transfers)
            {
                Console.WriteLine("Download of {0} succeeded", transfer.FileName);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    //---------------//
    // QoL Functions //
    //---------------//
    
    static string NormalizePath(string path)
    {
        var parts = new List<string>(path.Split('/'));
        var stack = new Stack<string>();
        
        foreach (var part in parts)
        {
            if (part == "..")
            {
                if (stack.Count > 0)
                {
                    stack.Pop();
                }
            }
            else if (!string.IsNullOrEmpty(part))
            {
                stack.Push(part);
            }
        }
        
        var result = string.Join("/", stack.Reverse());
        var finalPath = "/" + result;
        return finalPath == "/" ? finalPath : finalPath.TrimEnd('/');
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using CSIA.Views;
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
        if (uname == null && upass == null)
        {
            _ftpSessionOptions = new SessionOptions
            {
                HostName = host,
                PortNumber = port ?? 21,
                Protocol = Protocol.Ftp
            };
        }
        else
        {
            _ftpSessionOptions = new SessionOptions
            {
                HostName = host,
                PortNumber = port ?? 21,
                UserName = uname,
                Password = upass,
                Protocol = Protocol.Ftp
            };
        }
        FTPSession.ExecutablePath = "./WinSCP.exe";
        FTPSession.FileTransferProgress += Session_FileTransferProgress;
        FTPSession.AddRawConfiguration("SendBuf", "4096"); // Reduce buffer size for more frequent updates
        FTPSession.AddRawConfiguration("RecvBuf", "4096"); 
        try
        {
            FTPSession.Open(_ftpSessionOptions);
            RemotePath = FTPSession.HomePath;
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
    
    static DateTime startTime;
    static long totalSize = 0;
    static bool firstProgressUpdate = true;

    private static void Session_FileTransferProgress(object sender, FileTransferProgressEventArgs e)
    {
        Console.WriteLine($"Progress event fired: {e.FileProgress:P2}");
    }

    public async void UploadFile(string filePath, string remotePath)
    {
        try
        {
            TransferOptions transferOptions = new TransferOptions();
            transferOptions.PreserveTimestamp = true; //keep original file timestamps
            transferOptions.TransferMode = TransferMode.Binary;

            TransferOperationResult transferResult;
            transferResult = FTPSession.PutFiles(filePath, remotePath + "/", false, transferOptions); //actually send the file


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
            TransferOptions transferOptions = new TransferOptions();
            transferOptions.PreserveTimestamp = true; //keep original file timestamps
            transferOptions.TransferMode = TransferMode.Binary;
            transferOptions.ResumeSupport.State = TransferResumeSupportState.Off;

            TransferOperationResult transferResult;
            transferResult = FTPSession.GetFiles(remoteFilePath, localPath+@"\", false, transferOptions); //actually get the file

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
    //...
    
    private static string NormalizePath(string path)
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
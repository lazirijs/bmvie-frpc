using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Diagnostics;
using System.ServiceProcess;

class Program
{
    static void Main()
    {
        // FRPC installer tool title
        Console.WriteLine("");
        Console.WriteLine("<-- FRPC installer tool -->");
        Console.WriteLine("");

        // Set client name
        string clientName;
        bool isInvalidClientName;
        do
        {
            Console.Write("Enter the client name only: ");
            clientName = Console.ReadLine();
            isInvalidClientName = string.IsNullOrWhiteSpace(clientName) || clientName.Contains(" ");
            if (isInvalidClientName)
            {
                Console.WriteLine("Enter a valid client name (no spaces allowed)");
            }
        } while (isInvalidClientName);

        // Define paths and URLs
        string folderPath = @"C:\frpc";
        string frpcServiceName = "frpc";
        string frpcFolderUrl = "https://bit.ly/bmvie-frpc";
        string zipFilePathOutput = @"C:\frpc.zip";
        string destinationFolder = @"C:\";
        string clientHostName = $"http://{clientName}.bmvie.net:8080";

        // Create folder and add to antivirus exclusion list
        Directory.CreateDirectory(folderPath);
        Console.WriteLine("");
        Console.WriteLine($"1 - Folder Created: {folderPath}");

        // Note: Adding to antivirus exclusion list programmatically is OS and AV-specific, usually requiring admin rights.
        // The code below would need admin rights and appropriate API calls to interact with Windows Defender.
        // For example, using PowerShell commands via C# can achieve this if run with the necessary privileges.
        // This part is commented out because it's not trivial and might require additional implementation details.
        // Process.Start(new ProcessStartInfo("powershell", $"-Command Add-MpPreference -ExclusionPath {folderPath}")
        // {
        //     UseShellExecute = false,
        //     RedirectStandardOutput = true,
        //     CreateNoWindow = true
        // });

        Console.WriteLine("");
        Console.WriteLine($"2 - Folder add to antivirus exclusion list: {folderPath}");

        // Download and extract ZIP file
        Console.WriteLine("");
        Console.WriteLine($"3 - Start downloading zip file to: {zipFilePathOutput}");
        Console.WriteLine("");
        using (WebClient client = new WebClient())
        {
            client.DownloadFile(frpcFolderUrl, zipFilePathOutput);
        }
        Console.WriteLine("4 - Downloading completed");
        Console.WriteLine("");

        ZipFile.ExtractToDirectory(zipFilePathOutput, destinationFolder);
        Console.WriteLine($"5 - Extracting zip file completed to: {destinationFolder}");
        Console.WriteLine("");

        // Replace "ClientNameHere" with clientName in all frpc.ini files
        var iniFiles = Directory.GetFiles(folderPath, "frpc.ini", SearchOption.AllDirectories);
        foreach (var iniFile in iniFiles)
        {
            string content = File.ReadAllText(iniFile);
            if (content.Contains("ClientNameHere"))
            {
                string newContent = content.Replace("ClientNameHere", clientName);
                File.WriteAllText(iniFile, newContent);
                Console.WriteLine($"6 - frpc.ini edit completed: Text replaced in {iniFile}");
            }
            else
            {
                Console.WriteLine($"6 - frpc.ini edit skipped: 'ClientNameHere' not found in {iniFile}");
            }
        }

        Console.WriteLine("");

        // Install and start the service
        Console.WriteLine($"7 - Installing service {folderPath}\\frpc_service.exe");
        Console.WriteLine("");
        Process.Start(new ProcessStartInfo
        {
            FileName = $"{folderPath}\\frpc_service.exe",
            Arguments = "install",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        }).WaitForExit();

        ServiceController service = new ServiceController(frpcServiceName);
        service.Start();
        service.WaitForStatus(ServiceControllerStatus.Running);

        Console.WriteLine("");
        Console.WriteLine($"8 - The URL is ready to use {clientHostName}");
        Console.WriteLine("");
        // Optionally, open the URL in the default browser
        // Process.Start(new ProcessStartInfo
        // {
        //     FileName = clientHostName,
        //     UseShellExecute = true
        // });
    }
}

using System;
using System.Globalization;
using System.IO;
using WinSCP;

class Example
{
    public static int Main()
    {
        try
        {
            // Setup session options
            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = "207.91.155.52",
                UserName = "atmosftp",
                Password = "0Atmosftpc0",
                SshHostKeyFingerprint = "ssh-dss 1024 f5:85:76:27:12:0c:de:0b:3b:ad:f9:e0:f2:b5:82:4c"
            };

            using (Session session = new Session())
            {
                // Connect
                session.Open(sessionOptions);

                string stamp = DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                //string fileName = "export_" + stamp + ".txt";
                string remotePath = "/atmos/";
                string localPath = "c:\\backup\\";

                // Manual "remote to local" synchronization.

                // You can achieve the same using:
                // session.SynchronizeDirectories(
                //     SynchronizationMode.Local, localPath, remotePath, false, false, SynchronizationCriteria.Time, 
                //     new TransferOptions { FileMask = fileName }).Check();
                if (session.FileExists(remotePath))
                {
                    bool download;
                    if (!File.Exists(localPath))
                    {
                        Console.WriteLine("File {0} exists, local backup {1} does not", remotePath, localPath);
                        download = true;
                    }
                    else
                    {
                        DateTime remoteWriteTime = session.GetFileInfo(remotePath).LastWriteTime;
                        DateTime localWriteTime = File.GetLastWriteTime(localPath);

                        if (remoteWriteTime > localWriteTime)
                        {
                            Console.WriteLine(
                                "File {0} as well as local backup {1} exist, " +
                                "but remote file is newer ({2}) than local backup ({3})",
                                remotePath, localPath, remoteWriteTime, localWriteTime);
                            download = true;
                        }
                        else
                        {
                            Console.WriteLine(
                                "File {0} as well as local backup {1} exist, " +
                                "but remote file is not newer ({2}) than local backup ({3})",
                                remotePath, localPath, remoteWriteTime, localWriteTime);
                            download = false;
                        }
                    }

                    if (download)
                    {
                        // Download the file and throw on any error
                        session.GetFiles(remotePath, localPath).Check();

                        Console.WriteLine("Download to backup done.");
                    }
                }
                else
                {
                    Console.WriteLine("File {0} does not exist yet", remotePath);
                }
            }
            Console.ReadKey();
            return 0;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e);
            return 1;
        }
    }
}
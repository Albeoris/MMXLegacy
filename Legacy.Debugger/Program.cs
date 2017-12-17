using Legacy.MSBuild;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Legacy.Debugger
{
    internal static class Program
    {
        public static void Main(String[] args)
        {
            try
            {
                GameLocationInfo gameLocation = GameLocationSteamRegistryProvider.TryLoad();
                if (gameLocation == null)
                    return;

                if (!Directory.Exists(gameLocation.ManagedPath))
                    return;

                String executablePath = gameLocation.LauncherPath;
                String backupPath = Path.ChangeExtension(executablePath, ".bak");
                String unityPath = gameLocation.RootDirectory + "\\Unity.exe";

                if (!File.Exists(unityPath))
                {
                    if (!File.Exists(backupPath))
                    {
                        File.Copy(executablePath, backupPath);
                        File.SetLastWriteTimeUtc(backupPath, File.GetLastWriteTimeUtc(executablePath));
                    }

                    File.Copy(executablePath, unityPath);
                    File.SetLastWriteTimeUtc(unityPath, File.GetLastWriteTimeUtc(executablePath));

                    File.Delete(executablePath);
                    if (!Kernel32.CreateHardLink(executablePath, unityPath, IntPtr.Zero))
                        throw new Win32Exception();
                }

                executablePath = unityPath;

                String dataPath = gameLocation.DataPath;
                String unityDataPath = Path.GetFullPath(gameLocation.RootDirectory + "\\Unity_Data");

                if (!Directory.Exists(unityDataPath))
                {
                    JunctionPoint.Create(unityDataPath, dataPath, true);
                }
                else
                {
                    try
                    {
                        foreach (String item in Directory.EnumerateFileSystemEntries(unityDataPath))
                            break;
                    }
                    catch
                    {
                        JunctionPoint.Delete(unityDataPath);
                        JunctionPoint.Create(unityDataPath, dataPath, true);
                    }
                }

                ChangeUnityDebugger("1", out String oldValue);

                try
                {
                    ProcessStartInfo gameStartInfo = new ProcessStartInfo(executablePath) {UseShellExecute = false};
                    gameStartInfo.EnvironmentVariables["UNITY_GIVE_CHANCE_TO_ATTACH_DEBUGGER"] = "1";
                    gameStartInfo.WorkingDirectory = gameLocation.RootDirectory;

                    Process gameProcess = new Process {StartInfo = gameStartInfo};
                    gameProcess.Start();

                    Byte[] unicodeDllPath = PrepareDllPath();
                    TimeSpan timeout = GetTimeout(args);

                    CancellationTokenSource cts = new CancellationTokenSource();
                    Console.CancelKeyPress += (o, s) =>
                    {
                        Console.WriteLine();
                        Console.WriteLine("Stopping...");
                        cts.Cancel();
                    };

                    Task task = Task.Factory.StartNew(() => MainLoop(unicodeDllPath, cts, timeout), cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);

                    Console.WriteLine("Waiting for an debug invitation.");
                    Console.WriteLine("Type 'help' to show an documenantion or press Ctrl+C to exit.");

                    while (!(cts.IsCancellationRequested || task.IsCompleted))
                    {
                        Task<String> readLine = Task.Factory.StartNew(() => Console.ReadLine()?.ToLower(), cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
                        readLine.Wait(cts.Token);

                        switch (readLine.Result)
                        {
                            case "help":
                                Console.WriteLine();
                                Console.WriteLine("help\t\t This message.");
                                Console.WriteLine("stop\t\t Stop waiting and close the application.");
                                break;
                            case "stop":
                                Console.WriteLine();
                                Console.WriteLine("Stopping...");
                                cts.Cancel();
                                task.Wait(cts.Token);
                                break;
                            default:
                                Console.WriteLine();
                                Console.WriteLine("Unrecognized command.");
                                break;
                        }
                    }
                }
                finally
                {
                    ChangeUnityDebugger(oldValue, out oldValue);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Unexpected error has occurred.");
                Console.WriteLine(ex);
                Console.WriteLine();
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
            }
        }

        private static void ChangeUnityDebugger(String newValue, out String oldValue)
        {
            const String variableName = "UNITY_GIVE_CHANCE_TO_ATTACH_DEBUGGER";

            using (RegistryKey envKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment", true))
            {
                oldValue = (String)envKey.GetValue(variableName);
                if (newValue == null)
                    envKey.DeleteValue(variableName);
                else
                    envKey.SetValue(variableName, newValue);
            }

            User32.Broadcast();
        }

        private static void MainLoop(Byte[] unicodeDllPath, CancellationTokenSource cts, TimeSpan timeout)
        {
            HashSet<Int32> processedIds = new HashSet<Int32>();

            while (true)
            {
                cts.Token.ThrowIfCancellationRequested();

                try
                {
                    KeepAlive(cts, timeout);
                    WindowsObject window = WindowsObject.Wait("Debug", "You can attach a debugger now if you want", cts.Token);

                    Int32 processId = window.GetProcessId();
                    if (!processedIds.Add(processId))
                    {
                        if (cts.Token.WaitHandle.WaitOne(1000))
                            cts.Token.ThrowIfCancellationRequested();
                        continue;
                    }

                    if (processedIds.Count == 1)
                    {
                        window.Close();
                        continue;
                    }

                    Console.WriteLine();
                    Console.WriteLine($"A new debuggable process [PID: {processId}] was found. Trying to inject DLL...");

                    using (SafeProcessHandle processHandle = new SafeProcessHandle(processId, ProcessAccessFlags.All, false))
                    using (SafeVirtualMemoryHandle memoryHandle = processHandle.Allocate(unicodeDllPath.Length, AllocationType.Commit, MemoryProtection.ReadWrite))
                    {
                        memoryHandle.Write(unicodeDllPath);

                        // Uncomment to debug
                        // System.Diagnostics.Debugger.Launch();
                        // KeepAlive(cts, TimeSpan.FromMinutes(10));

                        IntPtr loadLibraryAddress = GetLoadLibraryAddress();
                        using (SafeRemoteThread thread = processHandle.CreateThread(loadLibraryAddress, memoryHandle))
                        {
                            thread.Join();
                            window.Close();
                        }
                    }

                    KeepAlive(cts, timeout);
                    Console.WriteLine($"DLL was successfully injected to the process with PID: {processId}.");
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("Faield to inject DLL.");
                    Console.WriteLine(ex);

                    cts.Token.ThrowIfCancellationRequested();
                    Console.WriteLine("Waiting 20 seconds to try again...");
                    Console.WriteLine("Press Ctrl+C to exit...");
                    Thread.Sleep(20 * 1000);
                }
            }
        }

        private static void KeepAlive(CancellationTokenSource cts, TimeSpan timeout)
        {
            if (timeout != TimeSpan.MaxValue)
                cts.CancelAfter(timeout);
        }

        private static Byte[] PrepareDllPath()
        {
            String dllPath = Path.GetFullPath("Legacy.Injection.dll");
            if (!File.Exists(dllPath))
                throw new FileNotFoundException("DLL not found: " + dllPath, dllPath);

            return Encoding.Unicode.GetBytes(dllPath);
        }

        private static TimeSpan GetTimeout(IReadOnlyList<String> args)
        {
            return TimeSpan.FromSeconds(10);
        }

        private static IntPtr GetLoadLibraryAddress()
        {
            IntPtr kernelHandle = Kernel32.GetModuleHandle("kernel32.dll");
            if (kernelHandle == IntPtr.Zero)
                throw new Win32Exception();

            IntPtr loadLibraryAddress = Kernel32.GetProcAddress(kernelHandle, "LoadLibraryW");
            if (loadLibraryAddress == IntPtr.Zero)
                throw new Win32Exception();

            return loadLibraryAddress;
        }
    }
}
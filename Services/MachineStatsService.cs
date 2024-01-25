using System.Diagnostics;
using System.Management;

namespace taskmanagementAPI.Services
{
    public class MachineStatsService : BackgroundService
    {
        private PerformanceCounter cpuCounter;
        private PerformanceCounter ramCounter;
        private PerformanceCounter diskCounter;

        public MachineStatsService()
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            diskCounter = new PerformanceCounter("LogicalDisk", "% Free Space", "_Total");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                DisplayMachineStats();
                await Task.Delay(TimeSpan.FromMinutes(0.1), stoppingToken);
            }
        }

        private void DisplayMachineStats()
        {
            var cpuUsage = cpuCounter.NextValue();
            var availableMemory = ramCounter.NextValue();
            var diskFreeSpace = diskCounter.NextValue();

            var totalMemory = GetTotalPhysicalMemory() / (1024.0 * 1024.0);
            var usedMemory = totalMemory - availableMemory;

            var totalDiskSpace = GetTotalDiskSpace();
            var usedDiskSpace = totalDiskSpace * (1 - (diskFreeSpace / 100.0));

            Console.WriteLine($"Process CPU Usage: {Math.Round(cpuUsage, 2)}%");
            Console.WriteLine($"Total Memory Usage: {Math.Round(usedMemory, 2)}MB / {Math.Round(totalMemory, 2)}MB");
            Console.WriteLine($"Total Disk Usage: {Math.Round(usedDiskSpace, 2)}GB / {Math.Round(totalDiskSpace, 2)}GB");
        }

        private static long GetTotalPhysicalMemory()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem"))
            {
                foreach (var obj in searcher.Get())
                {
                    return Convert.ToInt64(obj["TotalPhysicalMemory"]);
                }
            }
            return 0;
        }

        private static double GetTotalDiskSpace()
        {
            double totalSpace = 0;
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    totalSpace += drive.TotalSize;
                }
            }
            return totalSpace / (1024.0 * 1024.0 * 1024.0);
        }
    }
}

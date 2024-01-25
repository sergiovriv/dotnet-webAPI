using Microsoft.EntityFrameworkCore;
using System.Text;
using taskmanagementAPI.Data;

namespace taskmanagementAPI.Services
{
    public class DataBackupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _backupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "taskmanagement");

        public DataBackupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            Directory.CreateDirectory(_backupFolder);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagementContext>();
                        await BackupDataAsync(dbContext);
                    }
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (TaskCanceledException ex)
                {
                    Console.WriteLine($"A task was cancelled: {ex.Message}");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected exception: {ex.Message}");
                }
            }
        }


        private async Task BackupDataAsync(TaskManagementContext context)
        {
            //desired tables to save
            await BackupTableAsync(context.Tasks, "Tasks.csv");
            await BackupTableAsync(context.Projects, "Projects.csv");
            await BackupTableAsync(context.Employees, "Employees.csv");

            Console.WriteLine("Stored Backup at " + DateTime.Now.ToString());
        }

        private async Task BackupTableAsync<T>(DbSet<T> table, string fileName) where T : class
        {
            var backupFilePath = Path.Combine(_backupFolder, fileName);
            var data = await table.ToListAsync();
            var csvData = ConvertToCsv(data);
            await File.WriteAllTextAsync(backupFilePath, csvData);
        }

        private string ConvertToCsv<T>(IEnumerable<T> data)
{
    var sb = new StringBuilder();
    var properties = typeof(T).GetProperties()
                              .Where(p => p.PropertyType.IsPrimitive ||
                                          p.PropertyType == typeof(string) ||
                                          p.PropertyType == typeof(DateTime) ||
                                          p.PropertyType == typeof(decimal) ||
                                          p.PropertyType.IsEnum ||
                                          p.PropertyType == typeof(Guid));

    //header names in first row
    sb.AppendLine(string.Join(",", properties.Select(p => $"\"{p.Name}\"")));

    //values for properties of each object
    foreach (var item in data)
    {
        var values = properties.Select(prop =>
        {
            var value = prop.GetValue(item);
            if (value is null)
            {
                return "NULL";
            }
            if (value is DateTime dateTime)
            {
                return $"\"{dateTime:yyyy-MM-dd HH:mm:ss}\"";
            }
            var stringValue = value.ToString().Replace("\"", "\"\"");
            return $"\"{stringValue}\"";
        });

        sb.AppendLine(string.Join(",", values));
    }

    return sb.ToString();
}


    }
}

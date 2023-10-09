
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace AutoNetwork
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        VPNConfig config;

        public Worker(ILogger<Worker> logger, IOptions<VPNConfig> _config)
        {
            _logger = logger;
            config = _config.Value;
            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!IsVpnConnected(config.name))
                {
                    ConnectVpn(config.name, config.username, config.password,config.phonebook);
                    //_logger.LogInformation("������:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    await Task.Delay(4000, stoppingToken);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            DisconnectVPN(config.name);
            return base.StopAsync(cancellationToken);
        }

        bool IsVpnConnected(string vpnName)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/C rasdial";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // ���������Ƿ����VPN����
            return output.Contains(vpnName);
        }
        void DisconnectVPN(string vpnName)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/C rasdial {vpnName} /disconnect";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Verb = "runas";
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            _logger.LogInformation(output);
        }
         void ConnectVpn(string vpnName, string username, string password,string path)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/C rasdial {vpnName} {username} {password} /PHONEBOOK:\"{path}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Verb = "runas";
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            _logger.LogInformation(output);
            // ���������Ƿ����"������"
            if (output.Contains("������"))
            {
                _logger.LogInformation("VPN���ӳɹ�");
  
            }
            else
            {
                // Console.WriteLine("VPN����ʧ�ܣ�");
            }
        }

    }
}
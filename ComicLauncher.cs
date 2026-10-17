using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

internal static class ComicLauncher
{
    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            string html;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ComicTool.html"))
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                html = reader.ReadToEnd();
            if (!html.Contains("renderTagChoices") || !html.Contains("showDirectoryPicker"))
                throw new InvalidDataException("內建介面不完整。");
            if (args.Length == 1 && args[0] == "--self-test") return 0;

            string[] browsers = {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Microsoft\Edge\Application\msedge.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Microsoft\Edge\Application\msedge.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Google\Chrome\Application\chrome.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Google\Chrome\Application\chrome.exe")
            };
            string browser = Array.Find(browsers, File.Exists);
            if (browser == null) throw new FileNotFoundException("請先安裝 Microsoft Edge 或 Google Chrome。");
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ObsidianComicTool");
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, "index.html");
            File.WriteAllText(path, html, new UTF8Encoding(false));
            Process.Start(new ProcessStartInfo(browser, "--app=\"" + new Uri(path).AbsoluteUri + "\"") { UseShellExecute = false });
            return 0;
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "漫畫收藏工具無法啟動", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return 1;
        }
    }
}

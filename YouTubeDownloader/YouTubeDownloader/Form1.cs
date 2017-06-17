using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoLibrary;
using YoutubeExtractor;

namespace YouTubeDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string locatie = "";
        public string link = "";
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderbrowser = new FolderBrowserDialog();
            DialogResult result = folderbrowser.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                Directory.SetCurrentDirectory(folderbrowser.SelectedPath);
                textBox2.Text = Environment.CurrentDirectory;
                locatie = Environment.CurrentDirectory;
            }
            else
            {
                MessageBox.Show("er gaat iets mis!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (locatie != "")
                {
                    IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(link);
                    VideoInfo video = videoInfos.First(info => info.VideoType == VideoType.Mp4 && info.Resolution == 360);
                    if (video.RequiresDecryption)
                    {
                        DownloadUrlResolver.DecryptDownloadUrl(video);
                    }
                    var videoDownloader = new VideoDownloader(video, Path.Combine(locatie, video.Title + video.VideoExtension));
                    videoDownloader.DownloadProgressChanged += VideoDownloader_DownloadProgressChanged;
                    videoDownloader.Execute();
                }
                else
                {
                    MessageBox.Show("Kies een locatie!");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private void VideoDownloader_DownloadProgressChanged(object sender, ProgressEventArgs e)
        {
            progressBar1.Value = (int) (e.ProgressPercentage * 100.0f);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (locatie != "")
                {
                    IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(link);
                    VideoInfo video = videoInfos.Where(info => info.CanExtractAudio).OrderByDescending(info => info.AudioBitrate).First();
                    if (video.RequiresDecryption)
                    {
                        DownloadUrlResolver.DecryptDownloadUrl(video);
                    }
                    var audioDownloader = new AudioDownloader(video, Path.Combine(locatie, video.Title + video.AudioExtension));
                    audioDownloader.DownloadProgressChanged += VideoDownloader_DownloadProgressChanged;
                    audioDownloader.Execute();
                }
                else
                {
                    MessageBox.Show("Kies een locatie!");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Process.Start(locatie);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var youTube = YouTube.Default; // starting point for YouTube actions
            var video = youTube.GetVideo(link); // g
            
            File.WriteAllBytes(locatie + video.FullName, video.GetBytes());
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            link = textBox1.Text;
        }
    }
}

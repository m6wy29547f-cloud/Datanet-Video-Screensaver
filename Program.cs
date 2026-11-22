
using System;
using System.Drawing;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;

namespace DatanetScreensaver;

internal static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        Core.Initialize();

        Application.Run(new ScreensaverForm());
    }
}

public class ScreensaverForm : Form
{
    private readonly LibVLC _libVLC;
    private readonly MediaPlayer _player;
    private readonly VideoView _videoView;

    public ScreensaverForm()
    {
        _libVLC = new LibVLC();
        _player = new MediaPlayer(_libVLC);

        _videoView = new VideoView
        {
            MediaPlayer = _player,
            Dock = DockStyle.Fill
        };

        Controls.Add(_videoView);

        FormBorderStyle = FormBorderStyle.None;
        WindowState = FormWindowState.Maximized;
        TopMost = true;
        BackColor = Color.Black;
        Cursor.Hide();

        Shown += (s, e) =>
        {
            string video = System.IO.Path.Combine(
                AppContext.BaseDirectory,
                "datanet_global_animated_screensaver.avi"
            );

            if (System.IO.File.Exists(video))
            {
                using var media = new Media(_libVLC,
                    new Uri(video));

                _player.Play(media);
            }
            else
            {
                MessageBox.Show("Datanet video not found.");
                Close();
            }
        };

        KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        };

        MouseDown += (s, e) => Close();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _player.Dispose();
        _libVLC.Dispose();
        Cursor.Show();
        base.OnFormClosed(e);
    }
}

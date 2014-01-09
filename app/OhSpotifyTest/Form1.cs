using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using SpotifySharp;

namespace OhSpotifyTest
{
    public partial class Form1 : Form
    {
        class Listener : SpotifySessionListener
        {
            Form1 form;
            public Listener(Form1 form)
            {
                this.form = form;
            }
            public override void NotifyMainThread(SpotifySession session)
            {
                form.InvokeProcessEvents(session);
            }
            public override void LoggedIn(SpotifySession session, SpotifyError error)
            {
                form.textBox1.AppendText("Logged in!\n");
            }
            public override void ConnectionError(SpotifySession session, SpotifyError error)
            {
                form.textBox1.AppendText(String.Format("ConnectionError: {0}\n", error));
            }
            public override void LogMessage(SpotifySession session, string data)
            {
                form.logMessages.Enqueue(data);
                NotifyMainThread(session);
            }
        }
        SynchronizationContext syncContext;
        SpotifySession session;
        System.Threading.Timer timer;
        ConcurrentQueue<string> logMessages;
        public Form1()
        {
            InitializeComponent();
            syncContext = SynchronizationContext.Current;
            logMessages = new ConcurrentQueue<string>();
            var config = new SpotifySessionConfig();
            config.ApiVersion = 12;
            config.CacheLocation = "spotifydata";
            config.SettingsLocation = "spotifydata";
            config.ApplicationKey = File.ReadAllBytes("spotify_appkey.key");
            config.UserAgent = "My Spotify Test App";
            config.Listener = new Listener(this);
            timer = new System.Threading.Timer(obj=>InvokeProcessEvents(session), null, Timeout.Infinite, Timeout.Infinite);
            session = SpotifySession.Create(config);
            session.Login("mybleton", "ctms", false, null);
        }
        void InvokeProcessEvents(SpotifySession session)
        {
            syncContext.Post(obj => ProcessEvents(session), null);
        }
        void ProcessEvents(SpotifySession session)
        {
            this.session = session;
            int timeout = 0;
            string message;
            while (logMessages.TryDequeue(out message))
            {
                textBox1.AppendText(message + "\n");
            }
            while (timeout == 0)
            {
                session.ProcessEvents(ref timeout);
            }
            timer.Change(timeout, Timeout.Infinite);
        }
    }
}
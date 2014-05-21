using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using log4net;
using LyricsCore.Configuration;
using Ninject;

namespace LyricsCore.Impl
{
    public class MpdPlayer:PlayerInteraction,IStartable
    {
        private class LoggingStreamWrapper : Stream
        {
            private static int _lastCommLayerInstance = 0;
            private readonly int _commLayerInstance = _lastCommLayerInstance++;
            private readonly ILog _logger;
            private readonly Stream _stream;
            private readonly string _name;

            public LoggingStreamWrapper(Stream stream, ILog logger, string name = null)
            {
                _stream = stream;
                _name = name;
                _logger = logger;
            }

            public override void Flush()
            {
                _stream.Flush();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _stream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _stream.SetLength(value);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                var rv = _stream.Read(buffer, offset, count);
                _logger.DebugFormat("R{3}:{2} {0} {1}", rv, buffer.Skip(offset).Take(rv).Aggregate(new StringBuilder(), (builder, b) => b >= 32 && b <= 128 ? builder.Append((char)b) : builder.Append("\\x").Append(b.ToString("X2"))), _name, _commLayerInstance);
                return rv;
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _stream.Write(buffer, offset, count);
                _logger.DebugFormat("W{3}:{2} {0} {1}", count, buffer.Skip(offset).Take(count).Aggregate(new StringBuilder(), (builder, b) => b >= 32 && b <= 128 ? builder.Append((char)b) : builder.Append("\\x").Append(b.ToString("X2"))), _name, _commLayerInstance);
            }

            public override bool CanRead
            {
                get { return _stream.CanRead; }
            }

            public override bool CanSeek
            {
                get { return _stream.CanSeek; }
            }

            public override bool CanWrite
            {
                get { return _stream.CanWrite; }
            }

            public override long Length
            {
                get { return _stream.Length; }
            }

            public override long Position { get { return _stream.Position; } set { _stream.Position = value; } }
            public override void Close()
            {
                
            }

            public void ReallyClose()
            {
                base.Close();
            }
        }

        private Thread _pollerThread;

        [Inject]
        public InjectableSetting<TimeSpan> PollingInterval { get; set; }
        [Inject]
        public InjectableSetting<string> Host { get; set; }
        [Inject]
        public InjectableSetting<int> Port { get; set; }

        [Inject]
        public ILog Logger { get; set; }

        public MpdPlayer()
        {
        }

        private void Poller()
        {
            TcpClient client=null;
            Song lastSong = null;
            while (true)
            {
                try
                {
                    if (client == null || !client.Connected)
                    {
                        client = new TcpClient(Host.ValueOrDefault("localhost"), Port.ValueOrDefault(6600));
                        using (var strm = new LoggingStreamWrapper(client.GetStream(), Logger))
                        using (var reader = new StreamReader(strm))
                        {
                            if (!reader.ReadLine().StartsWith("OK MPD"))
                            {
                                Logger.Fatal("Connected to host, but it doesnt seem to be MPD");
                                return;
                            }
                            Logger.Debug("Connected to host.");
                        }

                    }
                    using (var strm = new LoggingStreamWrapper(client.GetStream(), Logger))
                    using (var reader = new StreamReader(strm))
                    using (var writer = new StreamWriter(strm))
                    {
                        var current = new Song();
                        writer.WriteLine("currentsong");
                        writer.Flush();
                        while (true)
                        {
                            var line = reader.ReadLine();
                            if (line == null) break;
                            if (line.StartsWith("Artist:"))
                                current.Artist = line.Substring(7).Trim();
                            if (line.StartsWith("Album:"))
                                current.Album = line.Substring(6).Trim();
                            if (line.StartsWith("Title:"))
                                current.Title = line.Substring(6).Trim();
                            if (line.StartsWith("OK") || line.StartsWith("ACK")) break;
                        }
                        if (lastSong == null || lastSong.Album != current.Album || lastSong.Artist != current.Artist || lastSong.Title != current.Title)
                        {
                            OnSongChanged(new SongEventArgs(new Song
                            {
                                Album = current.Album,
                                Artist = current.Artist,
                                Title = current.Title
                            }));
                        }
                        lastSong = current;
                    }
                }
                catch (Exception e)
                {
                    Logger.Warn("Connection to MPD lost.",e);
                    try
                    {
                        if (client != null && client.Connected)
                            client.Close();
                    }
                    catch { }
                    client = null;
                }
                Thread.Sleep(PollingInterval.ValueOrDefault(TimeSpan.FromSeconds(.5)));
            }
        }

        public void Start()
        {
            Stop();
            (_pollerThread = new Thread(Poller) { IsBackground = true }).Start();
        }

        public void Stop()
        {
            if(_pollerThread!=null&&_pollerThread.IsAlive)
                _pollerThread.Abort();
        }
    }
}
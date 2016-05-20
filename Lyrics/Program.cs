using System;
using log4net.Config;
using MusicData;
using MusicData.Configuration;
using MusicData.Impl;
using Ninject;
using Ninject.Modules;

namespace Lyrics
{
    class Program
    {
        private class MockPlayer : PlayerInteraction
        {
            public void NewSong(Song song)
            {
                OnSongChanged(song);
            }
        }
        private class LocalModule : NinjectModule
        {
            public override void Load()
            {
                //Bind<PlayerInteraction>().To<MpdPlayer>().WithConstructorArgument("host", new InjectableSetting<string>("router.mediaparts")).WithConstructorArgument("port", new InjectableSetting<int>(6600));
                Bind<PlayerInteraction>().To<MockPlayer>().InSingletonScope();
                Bind<MetadataTransformer>().To<NullMetadataTransformer>();
                Bind<Display>().To<WinFormsDisplay.WinFormsDisplay>();
                Bind<MetadataTransformer>().To<NullMetadataTransformer>();
                //Bind<MusicData.Database>().To<NullDatabase>();
                Bind<MusicData.Database>().To<FilesystemDatabase>();


                Bind<Fetcher<Lyric>>().To<LyricWikiLyricFetcher>();
                //Bind<Fetcher<Lyric>>().To<DarkLyricsLyricFetcher>();
                //Bind<Fetcher<Lyric>>().To<Lyrics123LyricFetcher>();

                Bind<Fetcher<AlbumArt>>().To<FanartTvArtFetcher>();
                //Bind<Fetcher<AlbumArt>>().To<DiscogsAlbumArtFetcher>();
                Bind<Fetcher<AlbumArt>>().To<MpdFilesystemArtFetcher>();
            }
        }

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            log4net.LogManager.GetLogger(typeof(Program)).Info("Startup");
            var kernel=new StandardKernel(new LyricsModule(),new LocalModule());
            var engine=kernel.Get<Engine>();
            ((MockPlayer) kernel.Get<PlayerInteraction>()).NewSong(new Song
            {
                Album = "Community Music",
                Artist = "Asian Dub Foundation",
                Title = "Tank"
            });
            Console.ReadLine();
        }
    }
}

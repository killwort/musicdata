using System;
using log4net.Config;
using LyricsCore;
using LyricsCore.Configuration;
using LyricsCore.Impl;
using Ninject;
using Ninject.Modules;

namespace Lyrics
{
    class Program
    {
        private class LocalModule : NinjectModule
        {
            public override void Load()
            {
                Bind<PlayerInteraction>().To<MpdPlayer>().WithConstructorArgument("host",new InjectableSetting<string>("router.mediaparts")).WithConstructorArgument("port",new InjectableSetting<int>(6600));
                Bind<MetadataTransformer>().To<NullMetadataTransformer>();
                Bind<Display>().To<WinFormsDisplay.WinFormsDisplay>();
//                Rebind<Display>().To<ConsoleDisplay>();
                Bind<Database>().To<FilesystemDatabase>();
                Bind<Fetcher>().To<Lyrics123Fetcher>();
                //Bind<Fetcher>().To<DarkLyricsFetcher>();
            }
        }

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            log4net.LogManager.GetLogger(typeof(Program)).Info("Startup");
            var kernel=new StandardKernel(new LyricsModule(),new LocalModule());
            var engine=kernel.Get<Engine>();
            Console.ReadLine();
        }
    }
}

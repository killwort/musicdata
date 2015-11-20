using System;
using System.Collections.Generic;
using System.Configuration;
using log4net;
using Ninject.Activation;
using Ninject.Modules;

namespace MusicData.Configuration
{
    public class LyricsModule : NinjectModule
    {
        private AppSettingsReader appSettings = new AppSettingsReader();
        public override void Load()
        {
            Bind<InjectableSetting<TimeSpan>>().ToMethod(Binder<TimeSpan>);
            Bind<InjectableSetting<int>>().ToMethod(Binder<int>);
            Bind<InjectableSetting<string>>().ToMethod(Binder<string>);
            Bind<InjectableSetting<double>>().ToMethod(Binder<double>);
            Bind<InjectableSetting<float>>().ToMethod(Binder<float>);
            Bind<InjectableSetting<long>>().ToMethod(Binder<long>);


            Rebind<ILog>().ToMethod(context =>
            {
                return LogManager.GetLogger(context.Request.Target.Member.DeclaringType);
            }).InTransientScope();
        }

        private static readonly Dictionary<Type, Func<string, object>> Parsers = new Dictionary<Type, Func<string, object>>
        {
            {typeof(TimeSpan),x=>TimeSpan.Parse(x)},
            {typeof(long),x=>long.Parse(x)},
            {typeof(string),x=>x},
            {typeof(float),x=>float.Parse(x)},
            {typeof(double),x=>double.Parse(x)},
            {typeof(int),x=>int.Parse(x)},
        };
        private InjectableSetting<T> Binder<T>(IContext context)
        {
            var setting=context.Request.Target.Member.DeclaringType.FullName + "." + context.Request.Target.Member.Name;
            try
            {
                setting = (string) appSettings.GetValue(setting, typeof (string));
                return new InjectableSetting<T>((T) Parsers[typeof (T)](setting));
            }
            catch (InvalidOperationException)
            {
                return new InjectableSetting<T>();
            }
            catch (KeyNotFoundException)
            {
                return new InjectableSetting<T>();
            }
        }
    }
}

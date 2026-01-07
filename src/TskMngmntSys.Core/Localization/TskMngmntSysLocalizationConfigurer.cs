using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace TskMngmntSys.Localization
{
    public static class TskMngmntSysLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(TskMngmntSysConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(TskMngmntSysLocalizationConfigurer).GetAssembly(),
                        "TskMngmntSys.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}

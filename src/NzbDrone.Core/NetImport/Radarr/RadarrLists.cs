﻿using System.Collections.Generic;
using NLog;
using NzbDrone.Common.Http;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Parser;
using NzbDrone.Core.ThingiProvider;
using NzbDrone.Core.MetadataSource;


namespace NzbDrone.Core.NetImport.Radarr
{
    public class RadarrLists : HttpNetImportBase<RadarrSettings>
    {
        public override string Name => "Radarr Lists";
        public override bool Enabled => true;
        public override bool EnableAuto => false;

        private readonly IHttpClient _httpClient;
        private readonly Logger _logger;
        private readonly ISearchForNewMovie _skyhookProxy;

        public RadarrLists(IHttpClient httpClient, IConfigService configService, IParsingService parsingService, ISearchForNewMovie skyhookProxy,
            Logger logger)
            : base(httpClient, configService, parsingService, logger)
        {
            _skyhookProxy = skyhookProxy;
            _logger = logger;
            _httpClient = httpClient;
        }

        public override IEnumerable<ProviderDefinition> GetDefaultDefinitions()
        {
                foreach (var def in base.GetDefaultDefinitions())
                {
                    yield return def;
                }

                yield return new NetImportDefinition
                {
                    Name = "IMDb Top 250",
                    Enabled = Enabled,
                    EnableAuto = true,
                    ProfileId = 1,
                    Implementation = GetType().Name,
                    Settings = new RadarrSettings { Path = "/imdb/top250" },
                };
                yield return new NetImportDefinition
                {
                    Name = "IMDb Popular Movies",
                    Enabled = Enabled,
                    EnableAuto = true,
                    ProfileId = 1,
                    Implementation = GetType().Name,
                    Settings = new RadarrSettings { Path = "/imdb/popular" },
                };
                yield return new NetImportDefinition
                {
                    Name = "IMDb List",
                    Enabled = Enabled,
                    EnableAuto = true,
                    ProfileId = 1,
                    Implementation = GetType().Name,
                    Settings = new RadarrSettings { Path = "/imdb/list?listId=LISTID" },
                };


        }

        public override INetImportRequestGenerator GetRequestGenerator()
        {
            return new RadarrRequestGenerator()
            {
                Settings = Settings,
                Logger = _logger,
                HttpClient = _httpClient
            };
        }

        public override IParseNetImportResponse GetParser()
        {
            return new RadarrParser(Settings, _skyhookProxy);
        }
    }
}
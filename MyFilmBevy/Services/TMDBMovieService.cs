﻿using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using MyFilmBevy.Enums;
using MyFilmBevy.Models.Settings;
using MyFilmBevy.Models.TMDB;
using MyFilmBevy.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Serialization.Json;

namespace MyFilmBevy.Services
{
    public class TMDBMovieService : IRemoteMovieService
    {
        private AppSettings _appSettings;
        private readonly IHttpClientFactory _httpClient;

        public TMDBMovieService(IOptions<AppSettings> appSettings, IHttpClientFactory httpClient)
        {
            _appSettings = appSettings.Value;
            _httpClient = httpClient;
        }

        public async Task<ActorDetail> GetActorDetailAsync(int id)
        {
            //Step 0: Setup a default return object
            ActorDetail actorDetail = new();

            //Step 1: Assemble the full request uri string
            var query = $"{_appSettings.TMDBSettings.BaseUrl}/person/{id}";
            var queryParams = new Dictionary<string, string>()
            {
                { "api_key", _appSettings.MovieProSettings.TmDbApiKey },
                { "language", _appSettings.TMDBSettings.QueryOptions.Language}
            };
            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            //Step 2: Create a client and execute the request
            var client = _httpClient.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request);

            //Step 3: Return the ActorDetail object
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();

                var dcjs = new DataContractJsonSerializer(typeof(ActorDetail));
                actorDetail = (ActorDetail)dcjs.ReadObject(responseStream);
            }

            return actorDetail;
        }

        public async Task<MovieDetail> GetMovieDetailAsync(int id)
        {
            //Step 0: Setup default return object
            MovieDetail movieDetail = new();

            //Step 1:
            var query = $"{_appSettings.TMDBSettings.BaseUrl}/movie/{id}";
            var queryParams = new Dictionary<string, string>()
            {
                { "api_key", _appSettings.MovieProSettings.TmDbApiKey },
                { "language", _appSettings.TMDBSettings.QueryOptions.Language},
                { "append_to_response", _appSettings.TMDBSettings.QueryOptions.AppendToResponse}
            };
            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            //Step 2: Create client and execute request
            var client = _httpClient.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request);

            //Step 3: Return the Movie Detail object
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var dcjs = new DataContractJsonSerializer(typeof(MovieDetail));
                movieDetail = dcjs.ReadObject(responseStream) as MovieDetail;
            }
            return movieDetail;
        }

        public async Task<MovieSearch> SearchMoviesAsync(MovieCategory category, int count)
        {
            //Step 0: Setup a default return object
            MovieSearch movieSearch = new();

            //Step 1: Assemble the full request uri string
            var query = $"{_appSettings.TMDBSettings.BaseUrl}/movie/{category}";
            var queryParams = new Dictionary<string, string>()
            {
                { "api_key", _appSettings.MovieProSettings.TmDbApiKey },
                { "language", _appSettings.TMDBSettings.QueryOptions.Language },
                { "page", _appSettings.TMDBSettings.QueryOptions.Page }
            };
            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            //Step 2: Create a client and execute the request
            var client = _httpClient.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request);

            //Step 3: Return the MovieSearch object
            if (response.IsSuccessStatusCode)
            {
                var dcjs = new DataContractJsonSerializer(typeof(MovieSearch));
                using var responseStream = await response.Content.ReadAsStreamAsync();
                movieSearch = (MovieSearch)dcjs.ReadObject(responseStream);

                //Move this into a Business Logic layer
                movieSearch.results = movieSearch.results.Take(count).ToArray();
                movieSearch.results.ToList().ForEach(r => r.poster_path = $"{_appSettings.TMDBSettings.BaseImagePath}/{_appSettings.MovieProSettings.DefaultPosterSize}/{r.poster_path}");
            }
            return movieSearch;
        }
    }
}
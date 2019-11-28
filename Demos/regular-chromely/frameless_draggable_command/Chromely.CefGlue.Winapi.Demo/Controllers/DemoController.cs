﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DemoController.cs" company="Chromely Projects">
//   Copyright (c) 2017-2019 Chromely Projects
// </copyright>
// <license>
//      See the LICENSE.md file in the project root for more information.
// </license>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Chromely.Core.Infrastructure;
using Chromely.Core.RestfulService;
using Xilium.CefGlue;

// ReSharper disable once StyleCop.SA1300
namespace Chromely.CefGlue.Winapi.Demo.Controllers
{
    /// <summary>
    /// The demo controller.
    /// </summary>
    [ControllerProperty(Name = "DemoController", Route = "democontroller")]
    public class DemoController : ChromelyController
    {
        private const string COMMANDRESPONSESCRIPTFORMAT = "commandResult(\'{0}\');";

        /// <summary>
        /// Initializes a new instance of the <see cref="DemoController"/> class.
        /// </summary>
        public DemoController()
        {
            RegisterGetRequest("/democontroller/movies", GetMovies);
            RegisterPostRequest("/democontroller/movies", SaveMovies);
            RegisterCommand("/democontroller/demo1", CommandAction1);
            RegisterCommand("/democontroller/demo2", CommandAction2);
            RegisterCommand("/democontroller/demo3", CommandAction3);
            RegisterCommand("/democontroller/demo4", CommandAction4);
        }

        /// <summary>
        /// The get movies.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="ChromelyResponse"/>.
        /// </returns>
        private ChromelyResponse GetMovies(ChromelyRequest request)
        {
            var movieInfos = new List<MovieInfo>();
            var assemblyName = typeof(MovieInfo).Assembly.GetName().Name;

            movieInfos.Add(new MovieInfo(id: 1, title: "The Shawshank Redemption", year: 1994, votes: 678790, rating: 9.2, assembly: assemblyName));
            movieInfos.Add(new MovieInfo(id: 2, title: "The Godfather", year: 1972, votes: 511495, rating: 9.2, assembly: assemblyName));
            movieInfos.Add(new MovieInfo(id: 3, title: "The Godfather: Part II", year: 1974, votes: 319352, rating: 9.0, assembly: assemblyName));
            movieInfos.Add(new MovieInfo(id: 4, title: "The Good, the Bad and the Ugly", year: 1966, votes: 213030, rating: 8.9, assembly: assemblyName));
            movieInfos.Add(new MovieInfo(id: 5, title: "My Fair Lady", year: 1964, votes: 533848, rating: 8.9, assembly: assemblyName));
            movieInfos.Add(new MovieInfo(id: 6, title: "12 Angry Men", year: 1957, votes: 164558, rating: 8.9, assembly: assemblyName));

            return new ChromelyResponse(request.Id)
            {
                Data = movieInfos
            };
        }

        /// <summary>
        /// The save movies.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="ChromelyResponse"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// ArgumentNullException - request is null exception.
        /// </exception>
        /// <exception cref="Exception">
        /// Exception - post data is null exception.
        /// </exception>
        private ChromelyResponse SaveMovies(ChromelyRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.PostData == null)
            {
                throw new Exception("Post data is null or invalid.");
            }

            var response = new ChromelyResponse(request.Id);
            var postDataJson = request.PostData.EnsureJson();
            var movies = LitJson.JsonMapper.ToObject<List<MovieInfo>>(postDataJson);

            var rowsReceived = movies.ArrayCount();

            response.Data = $"{DateTime.Now}: {rowsReceived} rows of data successfully saved.";

            return response;
        }

        private void CommandAction1(IDictionary<string, string[]> queryParameters)
        {
            CommandAction("/democontroller/demo2", queryParameters);
        }

        private void CommandAction2(IDictionary<string, string[]> queryParameters)
        {
            CommandAction("/democontroller/demo2", queryParameters);
        }

        private void CommandAction3(IDictionary<string, string[]> queryParameters)
        {
            CommandAction("/democontroller/demo3", queryParameters);
        }

        private void CommandAction4(IDictionary<string, string[]> queryParameters)
        {
            CommandAction("/democontroller/demo4", queryParameters);
        }

        private void CommandAction(string path, IDictionary<string, string[]> queryParameters)
        {
            try
            {
                var data = new LitJson.JsonData();
                data["Path"] = path;
                if (queryParameters == null || !queryParameters.Any())
                {
                    data["QueryParams"] = "No query parameters";
                }
                else
                {
                    var qdata = LitJson.JsonMapper.ToJson(queryParameters);
                    var encodedQdata = qdata.JavaScriptStringEncode();
                    data["QueryParams"] = encodedQdata;
                }

                var script = string.Format(COMMANDRESPONSESCRIPTFORMAT, data.ToJson());

                CefFrame frame = FrameHandler.GetFrame("alldemoframe");
                if (frame == null)
                {
                    var errorMsg = $"Frame alldemoframe does not exist.";
                    Log.Error(errorMsg);
                    return;
                }

                frame.ExecuteJavaScript(script, null, 0);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }

    /// <summary>
    /// The movie info.
    /// </summary>
    // ReSharper disable once StyleCop.SA1402
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("ReSharper", "StyleCop.SA1600")]
    public class MovieInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovieInfo"/> class.
        /// </summary>
        public MovieInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieInfo"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="year">
        /// The year.
        /// </param>
        /// <param name="votes">
        /// The votes.
        /// </param>
        /// <param name="rating">
        /// The rating.
        /// </param>
        /// <param name="assembly">
        /// The assembly.
        /// </param>
        public MovieInfo(int id, string title, int year, int votes, double rating, string assembly)
        {
            Id = id;
            Title = title;
            Year = year;
            Votes = votes;
            Rating = rating;
            Date = DateTime.Now;
            RestfulAssembly = assembly;
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public int Votes { get; set; }

        public double Rating { get; set; }

        public DateTime Date { get; set; }

        public string RestfulAssembly { get; set; }
    }
}

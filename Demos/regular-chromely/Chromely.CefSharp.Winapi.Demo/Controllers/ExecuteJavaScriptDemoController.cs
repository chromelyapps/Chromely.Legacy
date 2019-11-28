﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecuteJavaScriptDemoController.cs" company="Chromely Projects">
//   Copyright (c) 2017-2019 Chromely Projects
// </copyright>
// <license>
//      See the LICENSE.md file in the project root for more information.
// </license>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Chromely.Core.RestfulService;
using LitJson;

namespace Chromely.CefSharp.Winapi.Demo.Controllers
{
    /// <summary>
    /// The demo controller.
    /// </summary>
    [ControllerProperty(Name = "ExecuteJavaScriptDemoController", Route = "executejavascript")]
    public class ExecuteJavaScriptDemoController : ChromelyController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteJavaScriptDemoController"/> class.
        /// </summary>
        public ExecuteJavaScriptDemoController()
        {
            RegisterPostRequest("/executejavascript/execute", Execute);
            RegisterPostRequest("/executejavascript/evaluate", Evaluate);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="ChromelyResponse"/>.
        /// </returns>
        private  ChromelyResponse Execute(ChromelyRequest request)
        {
            var response = new ChromelyResponse(request.Id)
            {
                ReadyState = (int)ReadyState.ResponseIsReady,
                Status = (int)System.Net.HttpStatusCode.OK,
                StatusText = "OK",
                Data = DateTime.Now.ToLongDateString()
            };

            try
            {
                var scriptInfo = new ScriptInfo(request.PostData);
                var frame = FrameHandler.GetFrame(scriptInfo.FrameName);
                if (frame == null)
                {
                    response.Data = $"Frame {scriptInfo.FrameName} does not exist.";
                    return response;
                }

                frame.ExecuteJavaScriptAsync(scriptInfo.Script);
                response.Data = "Executed script :" + scriptInfo.Script;
                return response;
            }
            catch (Exception e)
            {
                response.ReadyState = (int)ReadyState.RequestReceived;
                response.Status = (int)System.Net.HttpStatusCode.BadRequest;
                response.StatusText = "Error";
            }

            return response;
        }

        /// <summary>
        /// The evaluate.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="ChromelyResponse"/>.
        /// </returns>
        private ChromelyResponse Evaluate(ChromelyRequest request)
        {
            var response = new ChromelyResponse(request.Id)
            {
                ReadyState = (int)ReadyState.ResponseIsReady,
                Status = (int)System.Net.HttpStatusCode.OK,
                StatusText = "OK",
                Data = DateTime.Now.ToLongDateString()
            };

            try
            {
                var scriptInfo = new ScriptInfo(request.PostData);
                var frame = FrameHandler.GetFrame(scriptInfo.FrameName);
                if (frame == null)
                {
                    response.Data = $"Frame {scriptInfo.FrameName} does not exist.";
                    return response;
                }

                var javascriptResponse = frame.EvaluateScriptAsync(scriptInfo.Script);
                javascriptResponse.Wait();
                var status = javascriptResponse.Result.Success
                                    ? "Successfully executed :"
                                    : "Error in executing :";

                response.Data = string.IsNullOrEmpty(javascriptResponse.Result?.Result?.ToString())
                                    ? status + scriptInfo.Script
                                    : javascriptResponse.Result?.Result?.ToString();
                return response;
            }
            catch (Exception exception)
            {
                response.Data = exception.Message;
                response.ReadyState = (int)ReadyState.RequestReceived;
                response.Status = (int)System.Net.HttpStatusCode.BadRequest;
                response.StatusText = "Error";
            }

            return response;
        }

        /// <summary>
        /// The script info.
        /// </summary>
        private class ScriptInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ScriptInfo"/> class.
            /// </summary>
            /// <param name="postData">
            /// The post data.
            /// </param>
            public ScriptInfo(object postData)
            {
                FrameName = string.Empty;
                Script = string.Empty;
                if (postData != null)
                {
                    JsonData jsonData = JsonMapper.ToObject(postData.ToString());
                    FrameName = jsonData.Keys.Contains("framename") ? jsonData["framename"].ToString() : string.Empty;
                    Script = jsonData.Keys.Contains("script") ? jsonData["script"].ToString() : string.Empty;
                }
            }

            /// <summary>
            /// Gets the frame name.
            /// </summary>
            public string FrameName { get; }

            /// <summary>
            /// Gets the script.
            /// </summary>
            public string Script { get; }
        }
    }
}

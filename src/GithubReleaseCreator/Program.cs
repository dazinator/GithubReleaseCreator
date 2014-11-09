using CommandLine;
using GithubReleaseCreator.Dto;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GithubReleaseCreator
{
    /// <summary>
    /// This program allows you to create a release on GitHub via command line.
    /// </summary>
    class Program
    {

        static void Main(string[] args)
        {
            var options = new CreateReleaseOptions();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                CreateRelease(options);
            }
        }

        private static void CreateRelease(CreateReleaseOptions options)
        {

            var logger = new Logger(options.Verbose);

            string username = options.Username;
            string password = options.Password;
            string repoOwner = options.Owner;
            string repoName = options.RepoName;
            string tagName = options.TagName;
            string releaseName = options.ReleaseName;

            logger.Log("Creating GitHub release: " + options.ReleaseName + " for repo: " + options.RepoName + " and tag: " + options.TagName);

            string description = options.Description;
            if (!string.IsNullOrWhiteSpace(options.DescriptionFile))
            {
                logger.LogVerbose("Reading release description from file: " + options.DescriptionFile);
                description = System.IO.File.ReadAllText(options.DescriptionFile);
                description = HttpUtility.JavaScriptStringEncode(description);
            }

            bool draft = options.Draft;
            bool prerelease = options.PreRelease;

            logger.LogVerbose("Creating GitHub API Message..");
            var client = new RestClient("https://api.github.com");

            var request = new RestRequest(Method.POST);
            string resource = string.Format("/repos/{0}/{1}/releases", repoOwner, repoName);
            request.Resource = resource;
            request.RequestFormat = DataFormat.Json;

            string releaseJson = string.Format(@"""tag_name"": ""{0}"", ""target_commitish"": ""master"", ""name"": ""{1}"", ""body"": ""{2}"", ""draft"": {3}, ""prerelease"": {4}", tagName, releaseName, description, draft.ToString().ToLowerInvariant(), prerelease.ToString().ToLowerInvariant());
            releaseJson = "{" + releaseJson + "}";

            logger.LogVerbose("JSON payload to follow:");
            logger.LogVerbose(releaseJson);

            request.AddHeader("Accept", "application/vnd.github.v3+json");
            request.Credentials = new System.Net.NetworkCredential(username, password);

            var authHeader = string.Format(CultureInfo.InvariantCulture, "Basic {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", username, password))));
            request.AddHeader("Authorization", authHeader);

            request.AddParameter("application/json; charset=utf-8", releaseJson, ParameterType.RequestBody);

            logger.LogVerbose("Sending Request..");
            var response = client.Execute(request);

            logger.LogVerbose(response.Content);
            if (response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                logger.LogError("Error occurred creating release.");
                logger.LogVerbose("Response content follows:");
                logger.LogVerbose(response.Content);

                throw new Exception("Error occurred creating release.");
            }

            ReleaseDto release = JsonConvert.DeserializeObject<ReleaseDto>(response.Content);
            int releaseId = release.id;
            logger.Log("Successfully created release (id: " + releaseId + ")");

            // If we have asset files t upload then get the release id;
            if (options.ReleaseAssetFiles != null && options.ReleaseAssetFiles.Any())
            {
                foreach (var assetFile in options.ReleaseAssetFiles)
                {
                    UploadReleaseAsset(logger, client, options, releaseId, assetFile);
                }
            }

        }

        private static void UploadReleaseAsset(Logger logger, RestClient client, CreateReleaseOptions options, int releaseId, string assetFile)
        {
            // throw new NotImplementedException();
            var fileName = System.IO.Path.GetFileName(assetFile);
            string resource = string.Format("/repos/{0}/{1}/releases/{2}/assets?name={3}", options.Owner, options.RepoName, releaseId, fileName);
            var credential = new System.Net.NetworkCredential(options.Username, options.Password);
            var authHeader = string.Format(CultureInfo.InvariantCulture, "Basic {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", credential.UserName, credential.Password))));
            var mimeType = System.Web.MimeMapping.GetMimeMapping(fileName);

            logger.Log("Creating github release asset: " + fileName);

            logger.LogVerbose("Reading bytes from file: " + assetFile);
            var bytes = System.IO.File.ReadAllBytes(assetFile);

            HttpWebRequest request = (HttpWebRequest)(HttpWebRequest.Create(string.Format("https://uploads.github.com{0}", resource)));
            request.Credentials = credential;
            request.Headers.Add("Authorization", authHeader);
            request.UserAgent = "GithubReleaseCreator";

            request.ContentType = mimeType;
            request.ContentLength = bytes.Length;

            request.Method = "POST";
         
            logger.LogVerbose("Content Type is: " + mimeType);
            logger.LogVerbose("Uploading file data..");
            using (var dataStream = request.GetRequestStream())
            {
                dataStream.Write(bytes, 0, bytes.Length);
                dataStream.Close();
            }

            // Get the response.
            logger.LogVerbose("Getting response..");
            using (var response = request.GetResponse())
            {
                // WebResponse response = ;
                var httpResp = (HttpWebResponse)response;
                if (httpResp.StatusCode != HttpStatusCode.Created)
                {
                    logger.LogError("Error occurred uploading release asset. Expected HttpStatusCode.Created but server returned: " + httpResp.StatusCode);
                    logger.LogVerbose("Response content follows:");
                    using (StreamReader reader = new StreamReader(httpResp.GetResponseStream()))
                    {
                        string responseFromServer = reader.ReadToEnd();
                        logger.LogVerbose(responseFromServer);
                    }
                    throw new Exception("Error occurred creating release.");
                }

                logger.Log("Release asset successfully created: " + fileName);

            }

        }




    }
}

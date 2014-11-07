using CommandLine;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            logger.LogVerbose("Creating release on GitHub..");

            string username = options.Username;
            string password = options.Password;
            string repoOwner = options.Owner;
            string repoName = options.RepoName;
            string tagName = options.TagName;
            string releaseName = options.ReleaseName;

            string description = options.Description;
            if (!string.IsNullOrWhiteSpace(options.DescriptionFile))
            {
                logger.LogVerbose("Reading release description from file: " + options.DescriptionFile);
                description = System.IO.File.ReadAllText(options.DescriptionFile);
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

            logger.LogVerbose("Release created succesfully!");

            // If we have asset files t upload then get the release id;
            if (options.ReleaseAssetFiles != null && options.ReleaseAssetFiles.Any())
            {
                // Get the response.
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
                string releaseIdString = values["id"];
                int releaseId = 0;
                if (!int.TryParse(releaseIdString, out releaseId))
                {
                    logger.LogVerbose("Unable to parse release id from response: " + releaseIdString);
                }

                logger.LogVerbose("Release id is " + releaseId);

                foreach (var assetFile in options.ReleaseAssetFiles)
                {
                    UploadReleaseAsset(releaseId, assetFile);
                }


            }


        }

        private static void UploadReleaseAsset(int releaseId, string assetFile)
        {
            throw new NotImplementedException();

            // POST https://uploads.github.com/repos/:owner/:repo/releases/:id/assets?name=foo.zip

            // The raw file is uploaded to GitHub. Set the content type appropriately, and the asset’s name in a URI query parameter.

            //            Content-Type	string	Required. The content type of the asset. This should be set in the Header. Example: "application/zip". For a list of acceptable types, refer this list of common media types.
            //name	string	Required. The file name of the asset. This should be set in the URI query parameter.

            // Send the raw binary content of the asset as the request body.
        }

    }
}

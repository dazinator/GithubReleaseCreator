using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubReleaseCreator
{
    public class CreateReleaseOptions
    {

        [Option('u', "username", Required = true, HelpText = "Your username, or Personal Access Token created under your GitHub Account.")]
        public string Username { get; set; }

        [Option('p', "password", Required = false, DefaultValue = "x-oauth-basic", HelpText = "Your password. Defaults to x-oauth-basic if not specified - which allows an access token to be used as the username.")]
        public string Password { get; set; }

        [Option('o', "owner", Required = true, HelpText = "The username of the repo owner.")]
        public string Owner { get; set; }

        [Option('r', "repo", Required = true, HelpText = "The repository name.")]
        public string RepoName { get; set; }

        [Option('t', "tag", Required = true, HelpText = "The tag name. This tag must already exist.")]
        public string TagName { get; set; }

        [Option('n', "name", Required = true, HelpText = "The name given to the release.")]
        public string ReleaseName { get; set; }

        [Option('d', "desc", Required = false, HelpText = "The description for the release.")]
        public string Description { get; set; }

        [Option('f', "filedesc", Required = false, HelpText = "The path to a file containing the description for this release.")]
        public string DescriptionFile { get; set; }

        [Option('c', "commitish", Required = false, DefaultValue = "master", HelpText = "The target commit-ish to create the release for. Default is 'master'")]
        public string TargetCommitish { get; set; }

        [Option('l', "draft", Required = false, HelpText = "creates the release as a draft release.")]
        public bool Draft { get; set; }

        [Option('e', "pre", Required = false, HelpText = "creates the release as a pre-release.")]
        public bool PreRelease { get; set; }

        [Option('v', "verbose", HelpText = "Enable verbose output to the console during execution.")]
        public bool Verbose { get; set; }

        [OptionList('a', "assetfiles", Separator = ',', HelpText = "Upload one or more files as assets against the release. To specify more than one file, seperate the filenames with a comma. For example: c:/myfile.txt,c:/myotherfile.txt")]
        public IList<string> ReleaseAssetFiles { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            // this without using CommandLine.Text
            //  or using HelpText.AutoBuild
            var usage = new StringBuilder();
            usage.AppendLine("Github Release Creator Application 1.0");
            usage.AppendLine("Read user manual for usage instructions...");
            return usage.ToString();
        }

    }
}

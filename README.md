GithubReleaseCreator
====================

A small and flexible command line utility, that can create releases on GitHub, and can also upload any release assets you may have against that release, such as .msi files for example (or other types of files that GitHub supports).

Available as a NuGet package here: https://www.nuget.org/packages/GithubReleaseCreator/ - just add it to your solution and then you can call it easily during builds via the command line etc.

# Sample

On your GitHub account page, *create yourself a Personal Access Token*.

The following demonstrates calling `GithubReleaseCreator.exe` from windows command prompt, to create a release on GitHub for this repository.

```shell
GithubReleaseCreator.exe --username [Put Personal Access Token Here] --owner "dazinator"
--repo "githubreleasecreator" --tag "1.0.0+1" --name "TestRelease" --desc "Testing my automated release"
--draft -assetfiles "myfile.txt,myotherfile.txt" -verbose
```

# Arguments

* `--username` : used to authenticate with GitHub API - this should be your personal access token.
* `--owner` : the username of the owner of the repository that you are creating a release for.
* `--repo` : the repository name.
* `--tag` : the tag name to create a release from. This tag must allready exist. If you are using TeamCity it can auto tag for you after successful builds.
* `--name` : the name to give the release.
* `--desc` : Optional. The description of the release. Can be markdown format.
* `--filedesc` : Optional. You can use this instead of the --desc argument. The value of the argument should be the path of a file containing the description for the release. This allows you to keep the description in a nicely formatted file (perhaps markdown format for example).
* `--draft` : Optional. Flags the release as a draft.
* `--pre` : Optional. Flags the release as a pre-release.
* `--assetfiles` : Optional. Allows you to specify one or more (comma seperated) files to be uploaded as assets against the release that is created.
11. `--commitish` : Optional. The target commit-ish to create the release for. Default is 'master'
12. `--verbose` : Optional. If specified, enables output of verbose log messages. Useful if hitting issues.






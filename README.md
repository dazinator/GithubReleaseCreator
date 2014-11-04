GithubReleaseCreator
====================

A small and flexible command line utility, that can create releases on GitHub, including setting the release notes, and uploading file assets.


Example
=======

On your GitHub account page, you will need to have created yourself a *Personal Access Token*.

```shell
GithubReleaseCreator.exe --username [Put Personal Access Token Here] --owner "dazinator" --repo "githubreleasecreator" --tag "1.0.0+1" --name "TestRelease" --desc "Testing my automated release creator utility" --draft -u C:/myfile.txt,C:/myotherfile.txt -v
```

Arguments explained:

1. --username : used to authenticate with GitHub API - this should be your personal access token.
2. --owner : the username of the owner of the repository that you are creating a release for.
3. --repo : the repository name.
4. --tag : the tag name to create a release from. This tag must allready exist. If you are using TeamCity it can auto tag for you after successful builds.
5. --name : the name to give the release.
6. --desc : Optional. The description of the release. Can be markdown format.
7. --draft : Optional. Flags the release as a draft.
8. -u : Optional. Allows you to specify one or more (comma seperated) files to be uploaded as assets against the release that is created.


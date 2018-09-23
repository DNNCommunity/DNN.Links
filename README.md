# DNN.Links
DNN Links is a basic module used for displaying navigational links on your DNN site.

## Compatibility
| Dnn Links version         | Min Tested Dnn version | Max Tested Dnn version |
| -------------------------:| ----------------------:| ----------------------:|
|  Included or use 06.02.02 |               00.00.00 |               08.00.04 |
|                  06.02.03 |               07.02.00 |               09.01.01 |
|                  07.00.01 |               08.00.00 |               09.02.00 |

## Building the module from source and submitting pull requests
1. Install Dnn (latest stable version) from https://github.com/dnnsoftware/Dnn.Platform/releases
2. Fork this repository to your own github account
3. Clone your fork to the Dnn DesktopModules folder
4. Important, the project name id Dnn.Links, but the deployment directory is just Links (more later)
5. Build the project in release mode using Visual Studio, this will create the installable packages in the Install/ folder of Dnn.Links folder
6. Install one of the zip packages using the Dnn extension installer as any other module
7. In Visual Studio, create a new branch to isolate your changes.
8. In Visual studio, to test any changes, you need to build in debug mode, this will compile and copy all files from Dnn.Announcements (the source code) to Announcements (the deployment folder). To debug, use the attach to process feature and attach it to the w3wp process that matches the running site.
9. Commit and push your changes with clear descritions, then in github, create a pull request from the branch you created to the Dnn.Community repository, again please add a good description of the changes. You can also mention issues with #issueNumber to automatically associate your pull request with existing issues.

## Contributing
If you would like to contribute, please read [CONTRIBUTING.md](https://github.com/DNNCommunity/DNN.Links/blob/master/.github/CONTRIBUTING.md)

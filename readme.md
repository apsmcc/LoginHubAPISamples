LoginHub API Samples
====================

This repository contains example code for working with LoginHub API.

In order to work with LoginHub API you will need:

1. The manual / documentation (talk to your sales or support representative to get a copy)
2. If building in .NET, this repository of Samples
3. With Simulation Mode enabled, that will be enough to develop and test.

To deploy a working solution you will also need:

1. A working install of LoginHub / MC
2. A license for LoginHub API
3. Aquire an API token from LoginHub API
4. Configure any special redirects required to make the custom login page work
5. Install


Getting The Source
------------------

There are 2 ways to get the source from GitHub.

1. Clone the repository via Git.
2. Download a zip copy of the current version of the repository.

We recommend cloning the repository to ensure that you will be able to receive any bug
fixes and further feature enhancements over time.

### Clone the repository

GitHub has an article explaining this process: 
<https://help.github.com/articles/cloning-a-repository/>


Using The Code
--------------

The code was written in Visual Studio 2015.  The solution will require a Nuget Restore
before it will be fully functional.  This will restore the Nuget packages for:

* **RestSharp** <http://restsharp.org/> A simple library for making REST API calls.  Any
            library could have been used, this is the one we chose. _Apache License_
* **LibLog** <https://github.com/damianh/LibLog> A single file logging abstraction designed
            so that it will output logs to any major logging framework for .NET. Can also
            be extended to log to any custom logging solution. _MIT License_

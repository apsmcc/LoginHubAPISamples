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

If you are familiar with Git, we recommend cloning the repository to ensure that
you will be able to easily receive any bug fixes and further feature
enhancements over time.

### Clone the repository

GitHub has an article explaining this process: 
<https://help.github.com/articles/cloning-a-repository/>

Using The Code
--------------
Our code is provided under the _MIT License_ *However*: it really has little
value unless you have purchased the LoginHub API itself _and_ if you have
purchased the LoginHub API and you want to use the LoginHubAPISamples under a
different license - basically just tell us what license your lawyers want you
to work under!

We used 2 very popular open source libraries, for our sample code. While we
expect that most people will just use our samples with minor tweaks, it is
your responsibility (check with your lawyers) to make sure you are happy with
their license terms. If you don't like them (because of their license or any
other reason) - there are lots of other libraries you can use that will provide
the same functionality.

Using The Code
--------------

The code was written in Visual Studio 2015.  The solution will require a Nuget Restore
before it will be fully functional.  

This will restore the Nuget packages for:

* **RestSharp** <http://restsharp.org/> A simple library for making REST API calls.  Any
            library could have been used, this is the one we chose. _Apache License_
* **LibLog** <https://github.com/damianh/LibLog> A single file logging abstraction designed
            so that it will output logs to any major logging framework for .NET. Can also
            be extended to log to any custom logging solution. _MIT License_

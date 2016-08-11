﻿using LoginHub.Api.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginHub.Api
{
	/// <summary>
	/// A collection of static methods for use in calling the LoginHub API.
	/// </summary>
	public static class LoginApiMethods
	{
		/// <summary>
		/// For Logging.
		/// </summary>
		private static readonly LoginHub.Api.Logging.ILog LogTo = LoginHub.Api.Logging.LogProvider.GetLogger("LoginApiMethods");

		/// <summary>
		/// The URL for the LoginHub, this will usually be in the form of http(s)://{MC server name}/mc_web/onsite/loginHub/
		/// </summary>
		public static string BaseUrl { get; set; } = null;
		/// <summary>
		/// The LoginHub API token that was previously acquired from the LoginHub.  This is a unique token and will be DIFFERENT for each
		/// install of LoginHub API.
		/// </summary>
		public static string AuthenticationToken { get; set; } = null;

		/// <summary>
		/// Allows being set inside a #if DEBUG block.  Set it to true in order to remove all the remote calling functionality
		/// and just quickly build a piece of software that uses the APIs.
		/// </summary>
		public static bool IsSimulationMode { get; set; } = false;
		/// <summary>
		/// The type of simulation to use.  Only applies if <see cref="IsSimulationMode"/> is set to true.
		/// </summary>
		public static SimulationModeType SimulationModeType { get; set; } = SimulationModeType.SimulateSuccess;

		/// <summary>
		/// Connects to the LoginHub API and creates a login token for the user. This will check that the passed username
		/// and password are correct before issuing a login token. If the username/password combo is incorrect this will return
		/// an object with IsSuccessful = false.
		/// </summary>
		/// <param name="username">The username to create a login token for.</param>
		/// <param name="password">The password connected to the user.</param>
		/// <returns>An object representing a JWT login token for the user (or IsSuccessful = false if the user/password combo is incorrect).</returns>
		public static async Task<LoginTokenInfo> CreateLoginToken(string username, string password)
		{
			// Check the passed parameters for correctness
			if (string.IsNullOrEmpty(BaseUrl)) throw new BaseUrlMissingException();
			if (string.IsNullOrEmpty(AuthenticationToken)) throw new AuthenticationTokenMissingException();
			if (string.IsNullOrEmpty(username)) throw new ArgumentException("A value must be used for username", nameof(username));
			if (string.IsNullOrEmpty(password)) throw new ArgumentException("A value must be used for password", nameof(password));

			LogTo.Info(() => $"Creating a login token against url \"{BaseUrl}\" with user \"{username}\" and password.");

			// Special test/development mode feature
			if (IsSimulationMode)
			{
				return ExecuteSimulator(
					success: () => new LoginTokenInfo { Token = "Simulated Success Token" },
					simulateUserDoesNotExist: () => new LoginTokenInfo { IsSuccessful = false }
					);
			}

			//
			// Make the API call
			//

			var client = new RestClient(BaseUrl);

			var request = new RestRequest("loginApis/loginToken", Method.POST);
			request.AddParameter("username", username);
			request.AddParameter("password", password);
			request.AddHeader("Authorization", $"bearer {AuthenticationToken}");

			IRestResponse<LoginToken> response = await client.ExecuteTaskAsync<LoginToken>(request);

			//
			// Handle the response from the API call
			//

			if (response.ResponseStatus != ResponseStatus.Completed)
			{
				LogTo.Error(() => $"Error occurred during login token retrieval. {response}");
				if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					return new LoginTokenInfo { IsSuccessful = false };
				}
				else
				{
					throw response.ErrorException;
				}
			}

			LogTo.Debug(() => $"One time use token created {response.Data.Token} for {username}");
			return new LoginTokenInfo { Token = response.Data.Token };
		}

		/// <summary>
		/// Connects to the LoginHub API and creates a login token for the user. This will ONLY check that the passed
		/// username is correct before issuing a login token. This does pose a potential security risk that must be mitigated
		/// by the code calling this method. There are no applied limitations to users logged in via this method.
		/// </summary>
		/// <param name="username">The username to create a login token for.</param>
		/// <returns>An object representing a JWT login token for the user (or IsSuccessful = false if the user is incorrect).</returns>
		public static async Task<LoginTokenInfo> CreateLoginToken(string username)
		{
			// Check the passed parameters for correctness
			if (string.IsNullOrEmpty(BaseUrl)) throw new BaseUrlMissingException();
			if (string.IsNullOrEmpty(AuthenticationToken)) throw new AuthenticationTokenMissingException();
			if (string.IsNullOrEmpty(username)) throw new ArgumentException("A value must be used for username", nameof(username));

			LogTo.Info(() => $"Creating a login token against url \"{BaseUrl}\" with user \"{username}\".");

			// Special test/development mode feature
			if (IsSimulationMode)
			{
				return ExecuteSimulator(
					success: () => new LoginTokenInfo { Token = "Simulated Success Token" },
					simulateUserDoesNotExist: () => new LoginTokenInfo { IsSuccessful = false }
					);
			}

			//
			// Make the API call
			//

			var client = new RestClient(BaseUrl);

			var request = new RestRequest("loginApis/userOnlyLoginToken", Method.POST);
			request.AddParameter("username", username);
			request.AddHeader("Authorization", $"bearer {AuthenticationToken}");

			IRestResponse<LoginToken> response = await client.ExecuteTaskAsync<LoginToken>(request);

			//
			// Handle the response from the API call
			//

			if (response.ResponseStatus != ResponseStatus.Completed)
			{
				LogTo.Error(() => $"Error occurred during login token retrieval. {response}");
				if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					return new LoginTokenInfo { IsSuccessful = false };
				}
				else
				{
					throw response.ErrorException;
				}
			}

			LogTo.Debug(() => $"One time use token created {response.Data.Token} for {username}");
			return new LoginTokenInfo { Token = response.Data.Token };
		}

		/// <summary>
		/// Retrieves basic user information about a user account.  This information is considered to be valid or averaged across all
		/// entity databases that the user may have access to.
		/// </summary>
		/// <param name="username">The username to look for</param>
		/// <returns>A <see cref="BasicUserDetails"/> object filled out with user details or null if the user does not exist.</returns>
		public static async Task<BasicUserDetails> GetBasicUserInfo(string username)
		{
			// Check the passed parameters for correctness
			if (string.IsNullOrEmpty(BaseUrl)) throw new BaseUrlMissingException();
			if (string.IsNullOrEmpty(AuthenticationToken)) throw new AuthenticationTokenMissingException();
			if (string.IsNullOrEmpty(username)) throw new ArgumentException("A value must be used for username", nameof(username));

			LogTo.Info(() => $"Getting basic user info with url \"{BaseUrl}\" and user \"{username}\".");

			// Special test/development mode feature
			if (IsSimulationMode)
			{
				return ExecuteSimulator(
					success: () =>
					{
						// Have a little randomness to the resulting user that will be returned
						var random = new Random();
						var highestApplicationAccess = new[] {
							MCUserAccessLevel.ServiceRequester,
							MCUserAccessLevel.ServiceRequester,
							MCUserAccessLevel.MRO | MCUserAccessLevel.TechnicianWorkCenter | MCUserAccessLevel.Reporter | MCUserAccessLevel.ServiceRequester,
							MCUserAccessLevel.MRO
						};
						var connectionKeyOptions = new[] { new[] { "Test1ConnectionKey" }, new[] { "Test2ConnectionKeys-First", "Test2ConnectionKeys-Second" } };

						return new BasicUserDetails
						{
							UserGuid = Guid.NewGuid(),
							UserName = new[] { "SimulatedUser", "Simulated2", "simUser" }.ElementAt(random.Next(0, 3)),
							AllowedConnectionKeys = connectionKeyOptions.ElementAt(random.Next(0, connectionKeyOptions.Length)),
							HighestApplicationAccess = highestApplicationAccess.ElementAt(random.Next(0, highestApplicationAccess.Length)),
							Email = "test@example.com",
							FirstName = "simFirst",
							MiddleName = "simMiddle",
							LastName = "simLast",
							Language = new[] { "en", "fr", "en" }.ElementAt(random.Next(0, 3)),
							IsLockedOut = new[] { false, true }.ElementAt(random.Next(0, 2)),
							IsApproved = new[] { false, true }.ElementAt(random.Next(0, 2)),
						};
					},
					simulateUserDoesNotExist: () => null
					);
			}

			//
			// Make the API call
			//

			var client = new RestClient(BaseUrl);

			var request = new RestRequest("loginApis/basicUserInfo", Method.POST);
			request.AddParameter("username", username);
			request.AddHeader("Authorization", $"bearer {AuthenticationToken}");

			IRestResponse<BasicUserDetails> response = await client.ExecuteTaskAsync<BasicUserDetails>(request);

			//
			// Handle the response from the API call
			//

			if (response.ResponseStatus != ResponseStatus.Completed)
			{
				LogTo.Error(() => $"Error occurred during basic user info retrieval. {response}");
				if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					return null;
				}
				else
				{
					throw response.ErrorException;
				}
			}

			LogTo.Debug(() => $"Basic user information {response.Data} retrieved for user {username}");
			return response.Data;
		}

		/// <summary>
		/// Creates a URL that can be redirected to with a valid login token in order for a user to be able to properly log in.
		/// </summary>
		/// <param name="token">The token to generate into the URL.</param>
		/// <returns>A URL to be used in a Response.Redirect() call.</returns>
		public static string CreateLoginUrlForToken(LoginTokenInfo token)
		{
			// Check the passed parameters for correctness
			if (string.IsNullOrEmpty(BaseUrl)) throw new BaseUrlMissingException();
			if (string.IsNullOrEmpty(AuthenticationToken)) throw new AuthenticationTokenMissingException();
			if (token == null || !token.IsSuccessful) throw new ArgumentException("A successful created token must be used to create a URL. There was an error generating the token.");

			LogTo.Info(() => $"Generating a login url for token \"{token.Token}\".");

			// Special test/development mode feature
			if (IsSimulationMode)
			{
				return ExecuteSimulator(() => $"{BaseUrl}/loginApis/loginWithToken/?token={token.Token}", () => $"{BaseUrl}/loginApis/loginWithToken/?token={token.Token}");
			}

			return $"{BaseUrl}/loginApis/loginWithToken/?token={token.Token}";
		}

		/// <summary>
		/// Runs the standard Simulation methods
		/// </summary>
		/// <typeparam name="T">The generic type that should</typeparam>
		/// <param name="success">A method that returns what should occur upon success.</param>
		/// <param name="simulateUserDoesNotExist">A method that returns what should be returned when the user does not exist.</param>
		private static T ExecuteSimulator<T>(Func<T> success, Func<T> simulateUserDoesNotExist)
		{
			var simulationType = SimulationModeType;

			// Check if we need to randomly select something
			if (simulationType == SimulationModeType.RandomlySimulateSomething)
			{
				var random = new Random();
				// Randomly selects a simulation mode
				//  This gets all the values, finds the highest + 1, calls Random and then converts back into the enum.
				simulationType = (SimulationModeType)Enum.ToObject(typeof(SimulationModeType), random.Next(1, Enum.GetValues(typeof(SimulationModeType)).Cast<int>().Max() + 1));
			}

			switch (simulationType)
			{
				case SimulationModeType.SimulateSuccess:
					return success();
				case SimulationModeType.SimulateUserDoesNotExist:
					return simulateUserDoesNotExist();
				case SimulationModeType.SimulateAPIConnectionFailure:
					throw new SimulateAPIConnectionFailureException();
				default:
					throw new NotImplementedException("Unknown SimulationType selected.");
			}
		}
	}

	#region Simulation Features

	/// <summary>
	/// Used by the simulation system in order to allow for simple testing without having a working installation of LoginHub API to test against
	/// </summary>
	public enum SimulationModeType
	{
		/// <summary>
		/// Used to simulate a successful operation. What will be returned will vary depending upon the method call but should 
		/// look much the same as a normal API call return value. 
		/// </summary>
		SimulateSuccess					= 1,
		/// <summary>
		/// Used to simulate a call where the user does not exist. What will be returned will vary depending upon the method 
		/// call but should look the same as what would be returned when the user does not exist.
		/// </summary>
		SimulateUserDoesNotExist		= 2,
		/// <summary>
		/// Used to simulate a general failure in calling the API. This will throw an error explaining some of the many ways
		/// that the API call may fail. A proper client should be able to successfully handle this various errors and display
		/// a reasonable error message.
		/// </summary>
		SimulateAPIConnectionFailure	= 3,

		/// <summary>
		/// Randomly selects one of the other simulation modes on each method call.
		/// </summary>
		RandomlySimulateSomething		= 0,

	}

	/// <summary>
	/// The error thrown when SimulateModeType is set to SimulateAPIConnectionFailure
	/// </summary>
	public class SimulateAPIConnectionFailureException : System.Exception
	{
		/// <summary>
		/// Creates a new instance of the Simulation failure Exception.
		/// </summary>
		public SimulateAPIConnectionFailureException() :
			base(
@"This is a simulated error coming from the LoginHub API.

There are lots of error types that could occur here like:
- BaseUrl is configured incorrectly and does not point to http(s)://<servername>/mc_web/onsite/loginHub/
- HTTP /vs/ HTTPS misconfiguration
- LoginHub server is down
- Network prohibiting communication
- Your Network card is turned off / in flight mode / broken / disabled
- A network proxy is in between
- Communication Timeout
- LoginHub API is not licensed
- Generic LoginHub configuration error
- SQL Server that MC / LoginHub relies upon is down
- SQL Timeout
- SQL Deadlock
")
		{ }
	}

	#endregion Simulation Features

	#region Exceptions

	public class BaseUrlMissingException : System.Exception
	{
		public BaseUrlMissingException() : base("A base URL must be specified to be able to use LoginHub APIs.  A base URL should have the format of \"http(s)://<mcserver/mc_web/onsite/loginHub/\"") { }
	}

	public class AuthenticationTokenMissingException : System.Exception
	{
		public AuthenticationTokenMissingException() : base("An authentication token is required to be able to call LoginHub API methods.  Go to the LoginHub secure API page to generate an authentication token.") { }
	}


	#endregion Exceptions

	#region Data

	class LoginToken
	{

		public string Token { get; set; }

	}

	public class LoginTokenInfo
	{
		public string Token { get; set; } = "";
		public bool IsSuccessful { get; set; } = true;
	}

	[Flags]
	public enum MCUserAccessLevel
	{
		None = 0,
		MRO = 1,
		KpiDashboard = 2,
		ServiceRequester = 4,
		Reporter = 8,
		TechnicianWorkCenter = 16
	}
	public class BasicUserDetails
	{
		public Guid UserGuid { get; set; }
		public string UserName { get; set; }
		public string[] AllowedConnectionKeys { get; set; }
		public MCUserAccessLevel HighestApplicationAccess { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string Name { get; set; }
		public string Language { get; set; }
		public bool IsLockedOut { get; set; }
		public bool IsApproved { get; set; }
	}

	#endregion Data

}

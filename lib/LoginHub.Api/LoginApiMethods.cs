using Anotar.LibLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginHub.Api
{
    public static class LoginApiMethods
    {

        public static string BaseUrl { get; set; } = null;
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

        public static async Task<LoginTokenInfo> CreateLoginToken(string username, string password)
        {
            // Check the passed parameters for correctness
            if (string.IsNullOrEmpty(BaseUrl)) throw new BaseUrlMissingException();
            if (string.IsNullOrEmpty(AuthenticationToken)) throw new AuthenticationTokenMissingException();
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("A value must be used for username", nameof(username));
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("A value must be used for password", nameof(password));

            LogTo.Info("Creating a login token against url \"{url}\" with user \"{user}\" and password.", BaseUrl, username);

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
                LogTo.Error("Error occured during login token retrieval.", response);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new LoginTokenInfo { IsSuccessful = false };
                }
                else
                {
                    throw response.ErrorException;
                }
            }

            LogTo.Debug("One time use token created {token} for {user}", response.Data.Token, username);
            return new LoginTokenInfo { Token = response.Data.Token };
        }

        public static async Task<LoginTokenInfo> CreateLoginToken(string username)
        {
            if (string.IsNullOrEmpty(BaseUrl)) throw new BaseUrlMissingException();
            if (string.IsNullOrEmpty(AuthenticationToken)) throw new AuthenticationTokenMissingException();
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("A value must be used for username", nameof(username));

            LogTo.Info("Creating a login token against url \"{url}\" with user \"{user}\".", BaseUrl, username);

            // Special test/development mode feature
            if (IsSimulationMode)
            {
                return ExecuteSimulator(
                    success: () => new LoginTokenInfo { Token = "Simulated Success Token" },
                    simulateUserDoesNotExist: () => new LoginTokenInfo { IsSuccessful = false }
                    );
            }

            var client = new RestClient(BaseUrl);

            var request = new RestRequest("loginApis/userOnlyLoginToken", Method.POST);
            request.AddParameter("username", username);
            request.AddHeader("Authorization", $"bearer {AuthenticationToken}");

            IRestResponse<LoginToken> response = await client.ExecuteTaskAsync<LoginToken>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                LogTo.Error("Error occured during login token retrieval.", response);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new LoginTokenInfo { IsSuccessful = false };
                }
                else
                {
                    throw response.ErrorException;
                }
            }

            LogTo.Debug("One time use token created {token} for {user}", response.Data.Token, username);
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

            LogTo.Info("Getting basic user info with url \"{url}\" and user \"{user}\".", BaseUrl, username);

            // Special test/development mode feature
            if (IsSimulationMode)
            {
                return ExecuteSimulator(
                    success: () => {
                        // Have a little randomness to the resulting user that will be returned
                        var random = new Random();
                        var highestApplicationAccess = new[] {
                            MCUserAccessLevel.ServiceRequester,
                            MCUserAccessLevel.ServiceRequester,
                            MCUserAccessLevel.MRO | MCUserAccessLevel.TechnicianWorkCenter | MCUserAccessLevel.Reporter | MCUserAccessLevel.ServiceRequester,
                            MCUserAccessLevel.MRO
                        };
                        var connectionKeyOptions = new[] { new[] { "Test1ConnectionKey" }, new[] { "Test2ConnectionKeys-First", "Test2ConnectionKeys-Second" } };

                        return new BasicUserDetails {
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
                LogTo.Error("Error occurred during basic user info retrieval.", response);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw response.ErrorException;
                }
            }

            LogTo.Debug("Basic user information {info} retrieved for user {user}", response.Data, username);
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

            LogTo.Info("Generating a login url for token \"{token}\".", token.Token);

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
            switch (SimulationModeType)
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

        SimulateSuccess,
        SimulateUserDoesNotExist,
        SimulateAPIConnectionFailure,

    }

    /// <summary>
    /// The error thrown when SimulateModeType is set to SimulateAPIConnectionFailure
    /// </summary>
    public class SimulateAPIConnectionFailureException: System.Exception
    {

        public SimulateAPIConnectionFailureException() : 
            base(
@"There are lots of error types that could occur here like:
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
") { }
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

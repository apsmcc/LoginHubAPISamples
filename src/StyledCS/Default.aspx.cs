using LoginHub.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		LoginApiMethods.BaseUrl = "http://localhost:64406/";
		LoginApiMethods.AuthenticationToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjFrdUptVkN3VlN2VTUzMGNpSEp0bmhUWldObyJ9.eyJhdWRpZW5jZSI6ImY5MDljMGYwLWYwNmMtNGNhYy1hZWI4LTZkOTVmYTA5YjBlMCIsImFwcGxpY2F0aW9uIjoiTG9naW5IdWIgQVBJIiwidXNlciI6IjAwMDAwMDAwLTAwMDAtMDAwMC0wMDAwLTAwMDAwMDAwMDAwMCIsImlwQWRkcmVzcyI6Ijo6MSIsImlzcyI6IkxvZ2luSHViIEFQSSIsImF1ZCI6ImY5MDljMGYwLWYwNmMtNGNhYy1hZWI4LTZkOTVmYTA5YjBlMCIsIm5iZiI6MTQ3MDc1OTk4Mn0.NM4PMz_9dtFrfwxQ4oV7IRcSRNNvlrLhlZb6aTYFTci6hJCAMIkIsnbZrWx9etTM32pBdTQRO7IrIi5RbUmyb1orxb2-2tt3Dzc4GoroxW7MNNXeVrT-aDXrKbNUbS6gCUfAL8nqVEOROfe7tFfF0nFGxZmRmOhUudV9jX1Vxl0GGZpsILyLvkpzmqAJqqGnnmDk6nHR8-RlGXhXhVTP7t9DwT0EVvyk4ozhp1BsYfraXTzLdlYUuALtyNdvYcD8v_bJqKi6vsWf8VSkmsIeSYCG3PPohYrRiG6J7aFddSfdqzLkMsPv_qRQRcWfj2RTO1SwqOpYZ-w80uaYMc1OG5roKWBr_hQ1261rixXaiOlj3rIaV-KyESqYJaaOPT63Qh8i5d75WSkBWdaLn598GCkisEmkw2GD8bE5RiphcXBHzBjOiVfYYPfig1H6RQfCigsnOFE3_HqLU6DtLAKVoYrd_WevRXO_VNpoBHpIJR_BXilSQAvu9VYN08kkkps1PzoFhISpTfIgoIWKZd4dw2-LlcDALPFXVTqK6SfU-vPkCp2PTGBj5Pb_jA9sSObvoKNaqcGM-om1IZQbUXlTZcf8fl2ndf-0SoLtWKB1ev6aR81NZHBijeh01f1mDiu6G8nkxJCazhGqgIK0m_EpPR5TVIaZn_QHaGiEvt-Wv28";

		// Enable the simulator when
		if (HttpContext.Current.IsDebuggingEnabled)
		{
			LoginApiMethods.IsSimulationMode = true;
			LoginApiMethods.SimulationModeType = SimulationModeType.RandomlySimulateSomething;
		}

		Submit.Click += Submit_Click;
	}

	private async void Submit_Click(object sender, EventArgs e)
	{
		// Clear previous errors
		ErrorMessage.Text = "";

		// Attempt to log the user in
		LoginTokenInfo loginToken;
		try
		{
			loginToken = await LoginApiMethods.CreateLoginToken(Username.Text, Password.Text);
		}
		catch (Exception ex)
		{
			ErrorMessage.Text = "There was a communication error with the server, please try again later or contact IT support.";
			return;
		}

		// Show an error if the user/password combo was invalid
		if (loginToken == null || loginToken.IsSuccessful == false)
		{
			ErrorMessage.Text = "Username or password was incorrect, please check and try again.";
			return;
		}

		// Send the user into the LoginHub
		Response.Redirect(LoginApiMethods.CreateLoginUrlForToken(loginToken), false);	//NOTE: the last value of false is required or ASP.NET will throw an error
	}

}
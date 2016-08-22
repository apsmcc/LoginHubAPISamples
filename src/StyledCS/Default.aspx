<%@ Page Async="true" Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<!DOCTYPE html>

<html>
<head runat="server">
	<meta charset="utf-8" />
	<meta name="viewport" content="width=device-width, initial-scale=1.0">
	<meta http-equiv="X-UA-Compatible" content="IE=edge">
	<title>MC LoginHub&trade; API</title>
	<link rel="stylesheet" href="default.css" type="text/css" media="screen" />
</head>
<body>
	<main>
		<header>
			<div class="mc-logo" alt="Web-Based CMMS Maintenance Management Software"></div>
			<div class="logo-rule-spacer">
				<div class="logo-rule"></div>
			</div>
			<h1>Login Hub&trade;</h1>
		</header>

		<div class="horizontal-line"></div>

		<div class="main-contents">
			<div class="contents">

<section id="loginForm" class="login-form">
	<div class="container-fluid">
		<div class="row">
			<div class="col-small-5 col-max-form-6 hidden-small">
				<div class="login-mc-image"></div>
			</div>
			<div class="col-small-7 col-max-form-6 login-mc-form">
				<h3 class="welcome">Welcome.</h3>
				<p>Please enter your Member ID and Password.</p>

				<form method="post" role="form" class="form" runat="server">
					<label for="Username">Member ID:</label>
					<asp:TextBox CssClass="form-control" ID="Username" placeholder="Member ID" runat="server" />

					<label for="Password">Password:</label>
					<asp:TextBox CssClass="form-control" ID="Password" TextMode="Password" placeholder="Password" runat="server" />

					<p class="text-danger">
						<asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
					</p>

		            <asp:Button CssClass="btn btn-default" ID="Submit" runat="server" Text="Log in" />
				</form>
			</div>
		</div>
	</div>
</section>


			</div>
		</div>

		<div class="horizontal-line"></div>

		<footer>
			<div class="copyright-details">
				<div class="copyright-mc">Copyright © 2002-2015 Maintenance Connection, Inc. All Rights Reserved.</div>
				<div class="copyright-disclaimer">
					By logging in, you agree to the Terms of Use, including the electronic delivery of important disclosures and other information
					contained in the Agreement. Please read the
					<a href="#">Terms of Use</a> and
					<a href="#">Privacy Policy</a> before you log in.
				</div>
				<div class="copyright-mcc">
					<a href="http://www.maintenanceconnection.ca">Copyright © 2006-2016 Maintenance Connection Canada, Inc. All Rights Reserved.</a>
				</div>
			</div>
		</footer>
	</main>
</body>
</html>
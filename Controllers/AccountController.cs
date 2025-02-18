using System;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using YSent.Models;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using YSent.Data;

public class AccountController : Controller
{
    private readonly DatabaseHelper _dbHelper;
    private readonly IConfiguration _configuration;

    public AccountController(DatabaseHelper dbHelper, IConfiguration configuration)
    {
        _dbHelper = dbHelper;
        _configuration = configuration;
    }

    public void manageUserAccess()
    {
        if (HttpContext.Session.GetString("username") == null)
        {
            Response.Redirect("/Home/Index");
        }
    }


    [HttpPost]
    public IActionResult ForgetPassword(PasswordReset model)
    {
        //if (!ModelState.IsValid)
        //{
        //    ViewBag.ErrorMessage = "Please enter a valid email address.";
        //    return View(model);
        //}

        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            var checkSql = "SELECT username FROM ysent.users WHERE email = @email";
            using (var cmd = new MySqlCommand(checkSql, (MySqlConnection)connection))
            {
                cmd.Parameters.AddWithValue("@email", model.Email);
                var username = cmd.ExecuteScalar()?.ToString();

                try
                {
                    if (string.IsNullOrEmpty(username))
                    {
                        ViewBag.ErrorMessage = "Email not found.";
                        return View(model);
                    }

                    // Generate reset token
                    var token = GenerateResetToken();
                    var expiryTime = DateTime.UtcNow.AddHours(24);

                    // Save token in database
                    var updateSql = @"UPDATE ysent.users 
                                SET reset_token = @token, 
                                    reset_token_expiry = @expiry 
                                WHERE email = @email";

                    using (var updateCmd = new MySqlCommand(updateSql, (MySqlConnection)connection))
                    {
                        updateCmd.Parameters.AddWithValue("@token", token);
                        updateCmd.Parameters.AddWithValue("@expiry", expiryTime);
                        updateCmd.Parameters.AddWithValue("@email", model.Email);
                        updateCmd.ExecuteNonQuery();
                    }

                    // Send reset email
                    SendResetEmail(model.Email, token);

                    ViewBag.SuccessMessage = "Password reset link has been sent to your email.";
                    return View(model);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "An error occurred while processing your request.";
                    return View(model);
                }
            }
        }
    }
    public IActionResult ForgetPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("SignIn");
        }

        var model = new PasswordReset { ResetToken = token };
        return View(model);
    }
    [HttpPost]
    public IActionResult ResetPassword(PasswordReset model)
    {
        if (string.IsNullOrEmpty(model.NewPassword))
        {
            ModelState.AddModelError("NewPassword", "New password is required");
            return View(model);
        }

        try
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();

                // Optional: Check that the token exists and hasn't expired
                var checkTokenSql = "SELECT COUNT(*) FROM ysent.users WHERE reset_token = @token AND reset_token_expiry > NOW()";
                using (var checkCmd = new MySqlCommand(checkTokenSql, (MySqlConnection)connection))
                {
                    checkCmd.Parameters.AddWithValue("@token", model.ResetToken);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count == 0)
                    {
                        ModelState.AddModelError("", "The reset link is invalid or has expired.");
                        return View(model);
                    }
                }

                // Update the user's password (consider hashing later)
                var updateSql = @"UPDATE ysent.users 
                              SET password = @password,
                                  reset_token = NULL,
                                  reset_token_expiry = NULL
                              WHERE reset_token = @token";

                using (var cmd = new MySqlCommand(updateSql, (MySqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@password", model.NewPassword);
                    cmd.Parameters.AddWithValue("@token", model.ResetToken);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        TempData["SuccessMessage"] = "Password reset successful. Please sign in with your new password.";
                        return RedirectToAction("SignIn", "Account");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to reset password. Please try again.");
                        return View(model);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception as needed
            ModelState.AddModelError("", "An error occurred while resetting password.");
            return View(model);
        }
    }
    private string GenerateResetToken()
    {
        byte[] tokenBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(tokenBytes);
        }
        return Convert.ToBase64String(tokenBytes);
    }

    private void SendResetEmail(string email, string token)
    {
        try
        {
            var resetLink = Url.Action("ResetPassword", "Account",
                new { token = token }, Request.Scheme);

            var smtpSettings = _configuration.GetSection("smtpSettings");
            if (smtpSettings == null)
            {
                throw new Exception("SMTP settings not found in configuration");
            }

            // Match the exact keys from your appsettings.json
            var host = smtpSettings["SmtpServer"] ?? throw new Exception("SMTP server not configured");
            var port = smtpSettings.GetValue<int>("SmtpPort");
            var username = smtpSettings["SmtpUsername"] ?? throw new Exception("SMTP username not configured");
            var password = smtpSettings["SmtpPassword"] ?? throw new Exception("SMTP password not configured");
            var fromEmail = smtpSettings["FromEmail"] ?? throw new Exception("From email not configured");
            var fromName = smtpSettings["FromName"] ?? throw new Exception("From name not configured");

            using (var client = new SmtpClient(host))
            {
                client.Port = port;
                client.Credentials = new System.Net.NetworkCredential(username, password);
                client.EnableSsl = true;

                var message = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = "Password Reset Request",
                    Body = $@"
                    <h2>Password Reset Request</h2>
                    <p>To reset your password, please click the link below:</p>
                    <p><a href='{resetLink}'>Reset Password</a></p>
                    <p>This link will expire in 24 hours.</p>
                    <p>If you didn't request this, please ignore this email.</p>",
                    IsBodyHtml = true
                };
                message.To.Add(email);

                client.Send(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email sending failed: {ex.Message}");
            throw new Exception($"Failed to send reset email: {ex.Message}");
        }
    }
    public IActionResult ForgetPassword()
        {
            return View();
        }




        // GET: /Account/SignUp
        public IActionResult SignUp()
        {
            return View();
        }

        // POST: /Account/SignUp
        [HttpPost]
        public IActionResult SignUp(UserEntry user)
        {
            if (ModelState.IsValid)
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();

                    // Check if the user already exists
                    var checkSql = "SELECT COUNT(*) FROM ysent.users WHERE username = @username";
                    using (var checkCmd = new MySqlCommand(checkSql, (MySqlConnection)connection))
                    {
                        checkCmd.Parameters.AddWithValue("@username", user.username);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count > 0)
                        {
                            ModelState.AddModelError("", "Username already exists.");
                            return View(user);
                        }
                    }

                    // Insert the new user
                    var insertSql = "INSERT INTO ysent.users (name, username, password, email, created_at, is_admin) VALUES (@name, @username, @password, @email, @created_at, @is_admin)";
                    using (var insertCmd = new MySqlCommand(insertSql, (MySqlConnection)connection))
                    {
                        insertCmd.Parameters.AddWithValue("@name", user.name);
                        insertCmd.Parameters.AddWithValue("@username", user.username);
                        insertCmd.Parameters.AddWithValue("@password", user.password); // Consider hashing the password
                        insertCmd.Parameters.AddWithValue("@email", user.email);
                        insertCmd.Parameters.AddWithValue("@created_at", DateTime.UtcNow);
                        insertCmd.Parameters.AddWithValue("@is_admin", false); // Default to false for new users
                        insertCmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("SignIn");
            }
            return View(user);
        }

        // GET: /Account/SignIn
        public IActionResult SignIn()
        {
            return View();
        }

        // POST: /Account/SignIn
        [HttpPost]
        public IActionResult SignIn(UserEntry user)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();

                var sql = "SELECT * FROM ysent.users WHERE username = @username AND password = @password";
                using (var cmd = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@username", user.username);
                    cmd.Parameters.AddWithValue("@password", user.password); // Consider hashing the password

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Set session for the authenticated user
                            HttpContext.Session.SetString("username", user.username);
                            HttpContext.Session.SetString("is_admin", reader["is_admin"].ToString());

                            if (IsAdmin())
                            {
                                return RedirectToAction("ViewAllUsers");
                            }
                            else
                            {
                                return RedirectToAction("MailBox", "Email");
                            }
                        }
                    }
                }
                connection.Close();
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return View(user);
        }

        // GET: /Account/SignOut
        public IActionResult SignOut()
        {
            // Clear the session
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

       
       


        // GET: /Account/Admin/ViewAllUsers
        public IActionResult ViewAllUsers()
        {
            
            if (!IsAdmin())
            {
                return RedirectToAction("Privacy", "Home"); // Redirect non-admin users
            }

            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();

                var sql = "SELECT * FROM ysent.users";
                using (var cmd = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        var users = new List<UserEntry>();
                        while (reader.Read())
                        {
                            users.Add(new UserEntry
                            {
                                name = reader["name"].ToString(),
                                username = reader["username"].ToString(),
                                email = reader["email"].ToString(),
                                created_at = Convert.ToDateTime(reader["created_at"]),
                                is_admin = Convert.ToBoolean(reader["is_admin"])
                            });
                        }
                        return View(users);
                    }
                }
            }
        }

        // POST: /Account/Admin/DeleteUser
        [HttpPost]
        public IActionResult DeleteUser(string username)
        {
            manageUserAccess();
            if (!IsAdmin())
            {
                return RedirectToAction("Index", "Home"); // Redirect non-admin users
            }

            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();

                var sql = "DELETE FROM ysent.users WHERE username = @username";
                using (var cmd = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("ViewAllUsers");
        }

        // Helper method to check if the current user is an admin
        private bool IsAdmin()
        {
           
            var isAdmin = HttpContext.Session.GetString("is_admin");
            return isAdmin != null && (isAdmin.Equals("1") || isAdmin.Equals("True", StringComparison.OrdinalIgnoreCase));
        }
    
}
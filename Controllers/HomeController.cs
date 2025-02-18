using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using YSent.Models;
using YSent.Data;
using MySql.Data.MySqlClient;
using System;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.IO;

namespace YSent.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly IWebHostEnvironment _env;

        public HomeController(DatabaseHelper dbHelper, IWebHostEnvironment env)
        {
            _dbHelper = dbHelper;
            _env = env;
        }

        public void manageUserAccess()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                Response.Redirect("/Home/Index");
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            manageUserAccess();
            return View();
        }

        
        public IActionResult List()
        {
            manageUserAccess();
            return View();
        }

       

        public IActionResult AddList()
        {
            manageUserAccess();
            return View();
        }



		public IActionResult BlackList()
        {
            manageUserAccess();
            return View();
        }

        public IActionResult EditList()
        {
            manageUserAccess();
            return View();
        }



        // GET: Home/NewTemplate
        public IActionResult NewTemplate()
        {
            manageUserAccess();
            return View();
        }

        // POST: Home/NewTemplate
        [HttpPost]
        public IActionResult NewTemplate(Template template)
        {
            manageUserAccess();
            if (ModelState.IsValid)
            {
                try
                {
                    using (var connection = _dbHelper.GetConnection())
                    {
                        connection.Open();

                        var userId = HttpContext.Session.GetInt32("UserId");
                        template.UserId = userId;

                        var insertSql = @"
                            INSERT INTO ysent.templates 
                            (template_name, plain_text, html_content, image_url, created_at, user_id)
                            VALUES 
                            (@TemplateName, @PlainText, @HtmlContent, @ImageUrl, @CreatedAt, @UserId)";

                        using (var insertCmd = new MySqlCommand(insertSql, (MySqlConnection)connection))
                        {
                            insertCmd.Parameters.AddWithValue("@TemplateName", template.TemplateName);
                            insertCmd.Parameters.AddWithValue("@PlainText", template.PlainText ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@HtmlContent", template.HtmlContent ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@ImageUrl", template.ImageUrl ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                            insertCmd.Parameters.AddWithValue("@UserId", userId);

                            int rowsAffected = insertCmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                TempData["SuccessMessage"] = "Template created successfully!";
                                return RedirectToAction("abc");
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "No rows were inserted into the database.";
                            }
                        }
                    }
                }
                catch (MySqlException sqlEx)
                {
                    TempData["ErrorMessage"] = $"Database Error: {sqlEx.Message}";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error: {ex.Message}";
                }
            }

            return View(template);
        }
        [HttpPost]
        public IActionResult UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { error = "No file uploaded" });
            }

            var uploadDir = Path.Combine(_env.WebRootPath, "imageupload", "uploads");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadDir, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var imageUrl = Url.Content($"/imageupload/uploads/{fileName}");
                return Json(new { url = imageUrl });
            }
            catch (Exception ex)
            {
                return Json(new { error = $"File upload failed: {ex.Message}" });
            }
        }
        // POST: Home/PreviewTemplate
        [HttpPost]
        public IActionResult PreviewTemplate([FromBody] Template template)
        {
            if (template == null)
            {
                return BadRequest("Template data is null.");
            }

            // Return the template data as JSON for preview
            return Json(new
            {
                TemplateName = template.TemplateName,
                PlainText = template.PlainText,
                HtmlContent = template.HtmlContent
            });
        }




/// ///////////////////////////////////////////////////////////////////////////////////////////////////



        public IActionResult abc()
        {
            manageUserAccess();
            var templates = new List<Template>();

            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();

                    // SQL query to fetch all templates
                    var query = "SELECT id, template_name, created_at FROM ysent.templates ORDER BY created_at DESC";

                    using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var template = new Template
                                {
                                    Id = reader.GetInt32("id"), // Match column name
                                    TemplateName = reader.GetString("template_name"), // Match column name
                                    CreatedAt = reader.GetDateTime("created_at") // Match column name
                                };

                                templates.Add(template);
                            }
                        }
                    }
                }
            }
            catch (MySqlException sqlEx)
            {
                Console.WriteLine("Database Error: " + sqlEx.Message);
                
                TempData["ErrorMessage"] = "A database error occurred. Please try again later.";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                // Handle the error (e.g., show a message to the user)
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";
            }

            // Passing the templates to the view
            return View("abc", templates);
        }
        // GET: Home/EditTemplate/{id}
        [HttpGet] // Explicitly specify the HTTP method
        public IActionResult EditTemplate(int id)
        {
            manageUserAccess();
            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();

                    // Fetch the template from the database
                    var query = "SELECT id, template_name, plain_text, html_content, image_url FROM ysent.templates WHERE id = @Id";
                    using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var template = new Template
                                {
                                    Id = reader.GetInt32("id"),
                                    TemplateName = reader.GetString("template_name"),
                                    PlainText = reader.IsDBNull(reader.GetOrdinal("plain_text")) ? null : reader.GetString("plain_text"),
                                    HtmlContent = reader.IsDBNull(reader.GetOrdinal("html_content")) ? null : reader.GetString("html_content"),
                                    ImageUrl = reader.GetString("image_url") //  ImageUrl
                                };
                                return View(template);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";
            }

           
            return RedirectToAction("abc");
        }

        // POST: Home/EditTemplate/{id}
        [HttpPost]
        [ValidateAntiForgeryToken] // Add anti-forgery token validation for..
        public IActionResult EditTemplate(int id, Template template)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var connection = _dbHelper.GetConnection())
                    {
                        connection.Open();

                        
                        var query = @"
                            UPDATE ysent.templates 
                            SET 
                                template_name = @TemplateName, 
                                plain_text = @PlainText, 
                                html_content = @HtmlContent, 
                                image_url = @ImageUrl  
                            WHERE id = @Id";

                        using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                        {
                            command.Parameters.AddWithValue("@TemplateName", template.TemplateName);
                            command.Parameters.AddWithValue("@PlainText", template.PlainText ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@HtmlContent", template.HtmlContent ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ImageUrl", template.ImageUrl ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Id", id);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                TempData["SuccessMessage"] = "Template updated successfully!";
                                return RedirectToAction("abc");
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "No template found with the specified ID.";
                            }
                        }
                    }
                }
                catch (MySqlException sqlEx)
                {
                    TempData["ErrorMessage"] = $"Database Error: {sqlEx.Message}";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error: {ex.Message}";
                }
            }

            // Re-fetch the template data to preserve existing content
            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();

                    var query = "SELECT id, template_name, plain_text, html_content, image_url FROM ysent.templates WHERE id = @Id";
                    using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                template = new Template
                                {
                                    Id = reader.GetInt32("id"),
                                    TemplateName = reader.GetString("template_name"),
                                    PlainText = reader.GetString("plain_text"),
                                    HtmlContent = reader.GetString("html_content"),
                                    ImageUrl = reader.GetString("image_url")
                                };
                            }
                        }
                    }
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "Failed to reload template data.";
            }

            return View(template);
        }
    
                 // POST: Home/DeleteTemplate/{id}
        [HttpPost]
        public IActionResult DeleteTemplate(int id)
        {
            try
            {
                using (var connection = _dbHelper.GetConnection())
                {
                    connection.Open();

                    // SQL query to delete the template by Id
                    var query = "DELETE FROM ysent.templates WHERE id = @Id";

                    using (var command = new MySqlCommand(query, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            TempData["SuccessMessage"] = "Template deleted successfully!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "No template found with the specified ID.";
                        }
                    }
                }
            }
            catch (MySqlException sqlEx)
            {
                Console.WriteLine("Database Error: " + sqlEx.Message);
                TempData["ErrorMessage"] = "A database error occurred. Please try again later.";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";
            }

            return RedirectToAction("abc");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
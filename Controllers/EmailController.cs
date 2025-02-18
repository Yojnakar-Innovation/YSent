using Microsoft.AspNetCore.Mvc;
using YSent.Models;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace YSent.Controllers
{
    public class EmailController : Controller
    {
        private readonly IConfiguration _configuration;

        public EmailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public void manageUserAccess()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                Response.Redirect("/Home/Index");
            }
        }



        // GET: Email/Send
        public IActionResult Send()
        {

            manageUserAccess();
            var model = new EmailModel
            {
                // Initialize lists so the view can bind them
                AvailableMailingLists = GetAvailableMailingLists() ?? new List<string>(),
                AvailableTemplates = GetAvailableTemplates() ?? new List<Template>()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Send(EmailModel model)
        {
            // Reload available lists and templates in case of errors
            model.AvailableMailingLists = GetAvailableMailingLists() ?? new List<string>();
            model.AvailableTemplates = GetAvailableTemplates() ?? new List<Template>();

            // If saving as draft, update or insert and redirect to Drafts view.
            if (model.IsDraft)
            {
                if (model.Id > 0) // Update existing draft
                {
                    UpdateDraft(model);
                    TempData["Message"] = "Draft updated successfully.";
                }
                else // Save new draft
                {
                    SaveDraft(model);
                    TempData["Message"] = "Email saved as draft.";
                }
                return RedirectToAction("Drafts");
            }

            // Combine emails from manual input and from the selected mailing list.
            List<string> listEmails = new List<string>();
            if (!string.IsNullOrEmpty(model.SelectedListName))
            {
                listEmails = GetEmailsForMailingList(model.SelectedListName);
            }

            var manualEmails = model.RecipientEmails?
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim())
                .ToList() ?? new List<string>();

            var allEmails = manualEmails.Union(listEmails).Distinct().ToList();

            // Ensure at least one recipient email is provided.
            if (allEmails.Count == 0)
            {
                TempData["Error"] = "Recipient emails cannot be empty.";
                return View(model);
            }
            model.RecipientEmails = string.Join(",", allEmails);

            // Get SMTP configuration from the "smtpSettings" section of appsettings.json.
            var smtpSettings = _configuration.GetSection("smtpSettings");
            string smtpServer = smtpSettings["SmtpServer"];
            int smtpPort = int.Parse(smtpSettings["SmtpPort"]);
            string smtpUsername = smtpSettings["SmtpUsername"];
            string smtpPassword = smtpSettings["SmtpPassword"];
            string fromEmail = smtpSettings["FromEmail"];
            string fromName = smtpSettings["FromName"];

            try
            {
                // Compose the email
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(fromName, fromEmail));
                emailMessage.Subject = model.Subject;
                emailMessage.Body = new TextPart("html") { Text = model.Body };

                foreach (var email in allEmails)
                {
                    emailMessage.To.Add(new MailboxAddress(email, email));
                }

                // Send the email using MailKit
                using (var client = new SmtpClient())
                {
                    client.Connect(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(smtpUsername, smtpPassword);
                    client.Send(emailMessage);
                    client.Disconnect(true);
                }

                SaveSentEmail(model);
                TempData["Message"] = "Emails sent successfully!";
            }
            catch (Exception ex)
            {
                // If sending fails, save the email as a draft.
                SaveDraft(model);
                TempData["Error"] = "Error sending email: " + ex.Message;
                Console.WriteLine($"Email Sending Error: {ex.Message}\n{ex.StackTrace}");
            }

            return RedirectToAction("Send");
        }

        [HttpGet]
        public IActionResult GetEmails(string listName)
        {
            var emails = GetEmailsForMailingList(listName);
            return Json(emails);
        }

        private List<string> GetAvailableMailingLists()
        {
            var lists = new List<string>();
            try
            {
                using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var command = new MySqlCommand("SELECT DISTINCT list_name FROM ysent.list", connection);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var listName = reader["list_name"]?.ToString();
                            if (!string.IsNullOrEmpty(listName))
                            {
                                lists.Add(listName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching mailing lists: {ex.Message}");
            }
            return lists;
        }

        private List<string> GetEmailsForMailingList(string listName)
        {
            var emails = new List<string>();
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT email FROM ysent.list WHERE list_name = @listName", connection);
                command.Parameters.AddWithValue("@listName", listName);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        emails.Add(reader["email"].ToString());
                    }
                }
            }
            return emails;
        }

        private List<Template> GetAvailableTemplates()
        {
            var templates = new List<Template>();
            try
            {
                using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var command = new MySqlCommand("SELECT Id, template_name, html_content FROM ysent.templates", connection);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            templates.Add(new Template
                            {
                                Id = reader.GetInt32("Id"),
                                TemplateName = reader.IsDBNull(reader.GetOrdinal("template_name")) ? string.Empty : reader.GetString("template_name"),
                                HtmlContent = reader.IsDBNull(reader.GetOrdinal("html_content")) ? string.Empty : reader.GetString("html_content")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching templates: {ex.Message}");
            }
            return templates;
        }

        public IActionResult Drafts()
        {
            manageUserAccess();
            var drafts = GetDraftEmails();
            return View(drafts);
        }

        public IActionResult Sent()
        {   
            manageUserAccess();
            var sentEmails = GetSentEmails();
            return View(sentEmails);
        }

        private void SaveDraft(EmailModel model)
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new MySqlCommand("INSERT INTO ysent.draftEmails (Subject, RecipientEmails, Body) VALUES (@Subject, @RecipientEmails, @Body)", connection);
                command.Parameters.AddWithValue("@Subject", model.Subject);
                command.Parameters.AddWithValue("@RecipientEmails", model.RecipientEmails);
                command.Parameters.AddWithValue("@Body", model.Body);
                command.ExecuteNonQuery();
            }
        }

        private void SaveSentEmail(EmailModel model)
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new MySqlCommand("INSERT INTO ysent.sentEmails (Subject, RecipientEmails, Body) VALUES (@Subject, @RecipientEmails, @Body)", connection);
                command.Parameters.AddWithValue("@Subject", model.Subject);
                command.Parameters.AddWithValue("@RecipientEmails", model.RecipientEmails);
                command.Parameters.AddWithValue("@Body", model.Body);
                command.ExecuteNonQuery();
            }
        }

        private List<EmailModel> GetDraftEmails()
        {
            var drafts = new List<EmailModel>();
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT Subject, RecipientEmails, Body FROM ysent.draftEmails", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        drafts.Add(new EmailModel
                        {
                            Subject = reader["Subject"].ToString(),
                            RecipientEmails = reader["RecipientEmails"].ToString(),
                            Body = reader["Body"].ToString(),
                            IsDraft = true
                        });
                    }
                }
            }
            return drafts;
        }

        private List<EmailModel> GetSentEmails()
        {
            var sentEmails = new List<EmailModel>();
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT Subject, RecipientEmails, SentTime, opened, OpenDate, Body FROM ysent.SentEmails", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sentEmails.Add(new EmailModel
                        {
                            Subject = reader["Subject"].ToString(),
                            RecipientEmails = reader["RecipientEmails"].ToString(),
                            Body = reader["Body"].ToString(),
                            opened = reader["opened"].ToString(),
                            OpenDate = reader["OpenDate"] != DBNull.Value ? Convert.ToDateTime(reader["OpenDate"]) : (DateTime?)null,
                            SentTime = reader["SentTime"] != DBNull.Value ? Convert.ToDateTime(reader["SentTime"]) : (DateTime?)null,
                            IsDraft = false
                        });
                    }
                }
            }
            return sentEmails;
        }

        [HttpGet]
        public IActionResult GetTemplateContent(int templateId)
        {
            var template = GetTemplateById(templateId);
            return Json(new { content = template?.HtmlContent ?? string.Empty });
        }

        private Template GetTemplateById(int templateId)
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT html_content FROM ysent.templates WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", templateId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Template
                        {
                            HtmlContent = reader.GetString("html_content")
                        };
                    }
                }
            }
            return null;
        }

        // MailBox action to list both sent and draft emails.
        public IActionResult MailBox()
        {
            manageUserAccess();
            var mails = new List<MailViewModel>();

            // Retrieve sent emails.
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT Id, Subject, RecipientEmails, SentTime FROM ysent.sentEmails", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mails.Add(new MailViewModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Subject = reader["Subject"].ToString(),
                            RecipientEmails = reader["RecipientEmails"].ToString(),
                            IsSent = true,
                            SentTime = reader["SentTime"] != DBNull.Value ? Convert.ToDateTime(reader["SentTime"]) : (DateTime?)null
                        });
                    }
                }
            }

            // Retrieve draft emails.
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT Id, Subject, RecipientEmails FROM ysent.draftEmails", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mails.Add(new MailViewModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Subject = reader["Subject"].ToString(),
                            RecipientEmails = reader["RecipientEmails"].ToString(),
                            IsSent = false,
                            SentTime = null // Draft emails do not have a sent time.
                        });
                    }
                }
            }

            return View(mails);
        }

        // GET: Email/Edit/{id}?isSent={true/false}
        public IActionResult Edit(int id, bool isSent)
        {
            if (isSent)
            {
                TempData["Error"] = "Sent emails cannot be edited.";
                return RedirectToAction("MailBox");
            }

            EmailModel model = null;
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT Id, Subject, RecipientEmails, Body FROM ysent.draftEmails WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model = new EmailModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Subject = reader["Subject"].ToString(),
                            RecipientEmails = reader["RecipientEmails"].ToString(),
                            Body = reader["Body"].ToString(),
                            IsDraft = true,
                            // Populate available lists for the view.
                            AvailableMailingLists = GetAvailableMailingLists() ?? new List<string>(),
                            AvailableTemplates = GetAvailableTemplates() ?? new List<Template>()
                        };
                    }
                }
            }

            if (model == null)
            {
                TempData["Error"] = "Draft not found.";
                return RedirectToAction("MailBox");
            }

            return View("Send", model);
        }

        // Update an existing draft.
        private void UpdateDraft(EmailModel model)
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    UPDATE ysent.draftEmails 
                    SET Subject = @Subject, 
                        RecipientEmails = @RecipientEmails, 
                        Body = @Body 
                    WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@Id", model.Id);
                command.Parameters.AddWithValue("@Subject", model.Subject ?? string.Empty);
                command.Parameters.AddWithValue("@RecipientEmails", model.RecipientEmails ?? string.Empty);
                command.Parameters.AddWithValue("@Body", model.Body ?? string.Empty);
                command.ExecuteNonQuery();
            }
        }

        // POST: Email/Delete/{id}?isSent={true/false}
        public IActionResult Delete(int id, bool isSent)
        {
            string tableName = isSent ? "sentEmails" : "draftEmails";
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new MySqlCommand($"DELETE FROM {tableName} WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
            TempData["Message"] = "Email deleted successfully.";
            return RedirectToAction("MailBox");
        }
    }
}

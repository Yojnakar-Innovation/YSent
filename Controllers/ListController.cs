using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using YSent.Models;
using System.Text;


namespace YSent.Controllers
{
    public class ListController : Controller
    {
        private readonly IConfiguration _configuration;

        public ListController(IConfiguration configuration)
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



        // Show all records
        public IActionResult Index()
        {

            manageUserAccess();

            List<ListModel> records = new List<ListModel>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT id, name, list_name, email, created_at FROM `ysent`.`list`";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            records.Add(new ListModel
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name"),
                                ListName = reader.GetString("list_name"),
                                Email = reader.GetString("email"),

                            });
                        }
                    }
                    conn.Close();
                }
            }
            return View(records);
        }



        // Handle delete request
        public IActionResult Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "DELETE FROM `ysent`.`list` WHERE id = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }

            TempData["Message"] = "Record deleted successfully!";
            return RedirectToAction("Index");
        }


        public IActionResult Create()
        {
            manageUserAccess();
            return View();
        }


        // Handle form submission
        [HttpPost]
        public IActionResult Create(ListModel model)
        {
            manageUserAccess();

            if (ModelState.IsValid)
            {
                // ✅ Retrieve connection string correctly
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    string query = "INSERT INTO `ysent`.`list` (name, list_name, email) VALUES (@name, @list_name, @email)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", model.Name);
                        cmd.Parameters.AddWithValue("@list_name", model.ListName);
                        cmd.Parameters.AddWithValue("@email", model.Email);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }

                TempData["Message"] = "Data inserted successfully!";
                return RedirectToAction("Create");
            }

            return View(model);
        }




        public IActionResult ExportList()
        {
            manageUserAccess();
            List<ListModel> records = new List<ListModel>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT id, name, list_name, email, created_at FROM `ysent`.`list`";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            records.Add(new ListModel
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name"),
                                ListName = reader.GetString("list_name"),
                                Email = reader.GetString("email"),

                            });
                        }
                    }
                    conn.Close();
                }
            }
            return View(records);
        }



        // Export Data to CSV
        public IActionResult ExportToCSV()
        {
            List<ListModel> records = new List<ListModel>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT id, list_name, name, email FROM `ysent`.`list`";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            records.Add(new ListModel
                            {
                                Id = reader.GetInt32("id"),
                                ListName = reader.GetString("list_name"),
                                Name = reader.GetString("name"),
                                Email = reader.GetString("email")
                            });
                        }
                    }
                    conn.Close();
                }
            }

            // Create CSV content
            var csv = new StringBuilder();
            csv.AppendLine("ID, List Name, Name, Email"); // Header

            foreach (var item in records)
            {
                csv.AppendLine($"{item.Id}, {item.ListName}, {item.Name}, {item.Email}");
            }

            byte[] fileBytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(fileBytes, "text/csv", "MailList.csv");
        }


    }
}

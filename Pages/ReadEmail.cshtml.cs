using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;

namespace FinalProject.Pages
{
    public class ReadEmailModel : PageModel
    {
        public string EmailID { get; set; }
        public string EmailSender { get; set; }
        public string EmailSubject { get; set; }
        public string EmailDate { get; set; }
        public string EmailContent { get; set; }

        public IActionResult OnGet(string emailid)
        {
            try
            {
                string connectionString = "Server=tcp:buemail436.database.windows.net,1433;Initial Catalog=BUEMAIL;Persist Security Info=False;User ID=admindeklnw;Password=Deklnwadmin-;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"; // Replace with your actual connection string
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string selectSql = "SELECT * FROM emails WHERE EmailID = @EmailID";
                    using (SqlCommand selectCommand = new SqlCommand(selectSql, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@EmailID", emailid);

                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                EmailID = reader.GetInt32(0).ToString();
                                EmailSubject = reader.GetString(1);
                                EmailContent = reader.GetString(2);
                                EmailDate = reader.GetDateTime(3).ToString();
                                EmailSender = reader.GetString(5);

                                reader.Close();

                                // Mark the email as read (update EmailIsRead to 1)
                                string updateSql = "UPDATE emails SET emailisread = 1 WHERE EmailID = @EmailID";
                                using (SqlCommand updateCommand = new SqlCommand(updateSql, connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@EmailID", emailid);
                                    updateCommand.ExecuteNonQuery();
                                }

                                return Page();
                            }
                        }
                    }
                }

                // Email not found
                return NotFound();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using StudentEnrollPro.Models;

namespace StudentEnrollPro.Controllers
{
    public class StudentController : Controller
    {
        private string connectionString = "Data Source=DESKTOP-DPKUC31\\MSSQLSERVER2022;Initial Catalog=UniversityDatabase;Integrated Security=True";

        public IActionResult Index(string searchString, int page = 1)
        {
            List<StudentCourse> students = GetStudents(searchString, page);

            ViewBag.CurrentFilter = searchString;
            ViewBag.Page = page;

            // Calculate and set ViewBag.TotalPages based on your pagination logic

            return View(students);
        }


        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(StudentCourse student)
        {
            try
            {
                int result = AddStudent(student);

                if (result < 0)
                {
                    TempData["ErrorMessage"] = "Duplicate record found. Please check the entered data.";
                    //return RedirectToAction("Create");

                }
                else if (result > 0)
                {
                  
                    return RedirectToAction("Index");
                }
                else
                {
                    
                }
            }
            catch (SqlException ex)
            {
               
                throw;
            }

            return View(); 
        }

        [HttpGet]
        public IActionResult Edit(int id, int page = 1)
        {
            StudentCourse student = GetStudentById(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }



        [HttpPost]
        public IActionResult Edit(StudentCourse student)
        {
          
                UpdateStudent(student);
                return RedirectToAction("Index");

        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            DeleteStudent(id);
            return RedirectToAction("Index");
        }

        // Data Access Methods

        private List<StudentCourse> GetStudents(string searchString, int page)
        {
            List<StudentCourse> students = new List<StudentCourse>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("sp_StudentCourse_GetStudentsDetails", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Ensure @SearchString parameter is not null before adding to command
                    if (searchString != null)
                    {
                        command.Parameters.AddWithValue("@SearchString", searchString);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@SearchString", DBNull.Value);
                    }

                    command.Parameters.AddWithValue("@Page", page);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StudentCourse student = new StudentCourse
                            {  
                                //ID = Convert.ToInt32(reader["ID"]),
                                // CourseID = Convert.ToInt32(reader["CourseID"]),
                                StudentID = Convert.ToInt32(reader["StudentID"]),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                EnrollDate = Convert.ToDateTime(reader["EnrollDate"]),
                                EmailAddress = reader["EmailAddress"].ToString(),
                                CourseName = reader["CourseName"].ToString(),
                                CourseFee = Convert.ToDecimal(reader["CourseFee"]),
                            };
                            students.Add(student);
                        }
                    }
                }
            }
            return students;
        }


        private int AddStudent(StudentCourse student)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("sp_StudentCourse_AddStudent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FirstName", student.FirstName);
                    command.Parameters.AddWithValue("@LastName", student.LastName);
                    command.Parameters.AddWithValue("@EnrollDate", student.EnrollDate);
                    command.Parameters.AddWithValue("@EmailAddress", student.EmailAddress);
                    command.Parameters.AddWithValue("@CourseName", student.CourseName);
                    command.Parameters.AddWithValue("@CourseFee", student.CourseFee);

                    // Use ExecuteNonQuery for a stored procedure that doesn't return a value
                    int result = command.ExecuteNonQuery();

                    return result;
                }
            }
        }



        private StudentCourse GetStudentById(int StudentID)
        {
            StudentCourse student = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("sp_StudentCourse_GetStudentById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudentID", StudentID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Print values for debugging
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                Console.WriteLine($"{reader.GetName(i)}: {reader[i]}");
                            }

                            student = new StudentCourse
                            {
                                StudentID = Convert.ToInt32(reader["StudentID"]), // Make sure your ID is correct
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                EnrollDate = Convert.ToDateTime(reader["EnrollDate"]),
                                EmailAddress = reader["EmailAddress"].ToString(),
                                CourseName = reader["CourseName"].ToString(),
                                CourseFee = Convert.ToDecimal(reader["CourseFee"]),
                            };
                        }
                    }
                }
            }
            return student;
        }


        private void UpdateStudent(StudentCourse student)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("sp_StudentCourse_UpdateStudent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID", student.ID);
                    command.Parameters.AddWithValue("@FirstName", student.FirstName);
                    command.Parameters.AddWithValue("@LastName", student.LastName);
                    command.Parameters.AddWithValue("@EnrollDate", student.EnrollDate);
                    command.Parameters.AddWithValue("@EmailAddress", student.EmailAddress);
                   // command.Parameters.AddWithValue("@StudentID", student.StudentID);
                    command.Parameters.AddWithValue("@CourseName", student.CourseName);
                    command.Parameters.AddWithValue("@CourseFee", student.CourseFee);

                    // ExecuteNonQuery for UPDATE
                    command.ExecuteNonQuery();
                }
            }
        }

        private void DeleteStudent(int StudentID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("sp_StudentCourseDeleteStudent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudentID", StudentID);

                    // ExecuteNonQuery for DELETE
                    command.ExecuteNonQuery();
                }
            }
        }
    }

}

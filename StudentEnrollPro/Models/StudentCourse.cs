namespace StudentEnrollPro.Models
{
    public class StudentCourse
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime EnrollDate { get; set; }
        public string EmailAddress { get; set; }
        public List<StudentCourse> Courses { get; set; }

        // Course properties
        public int CourseID { get; set; }
        public int StudentID { get; set; }
        public string CourseName { get; set; }
        public decimal CourseFee { get; set; }
    }
}

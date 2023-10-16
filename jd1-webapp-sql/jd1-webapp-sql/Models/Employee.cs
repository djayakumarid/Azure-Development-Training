using System.ComponentModel.DataAnnotations;

namespace jd1_webapp_sql.Models
{
    public class Employee
    {
        [Key]
        public int EmpId { get; set; }
        public string? Name { get; set; }
        public decimal Salary { get; set; }
    }
}

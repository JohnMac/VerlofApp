using System.ComponentModel.DataAnnotations;

namespace VerlofApp.Models
{
    public class LeaveRequest
    {
        [Key]
        public int LeaveId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
    }
}

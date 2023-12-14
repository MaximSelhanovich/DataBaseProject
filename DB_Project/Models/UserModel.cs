using System.ComponentModel.DataAnnotations;

namespace DB_Project.Models
{
    public class UserModel
    {
        public uint UserId { get; set; }
        public RoleModel Role { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        [DataType(DataType.PhoneNumber)]
        [MaxLength(13)]
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public decimal Salary { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EtiqaTestAPI.Entity
{
    public class Freelancer
    {
       
        public int Id { get; set; }
        public string? Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsArchived { get; set; } = false;
        public List<FreelancerSkillMapping> SkillMappings { get; set; }
    }
}

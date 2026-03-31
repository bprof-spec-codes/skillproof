using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Entities.Dtos.Users
{
    public class UpdateUser
    {
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? ProfilePicture { get; set; }

        public string? Bio {  get; set; }
        public string? HeadLine { get; set; }
    }
}


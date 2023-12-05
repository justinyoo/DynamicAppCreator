using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC.kernel.models
{
    internal class Persons
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string MiddleName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        public int BirthDay { get; set; }
        public int BirthMonth { get; set; }
        public int BirthYear { get; set; }
        public int PersonType { get; set; }
        public int State { get; set; }
        public Guid Owner { get; set; }
        public long Company { get; set; }
        public string Avatar { get; set; }
        public DateTime CreateDate { get; set; }
    }
}

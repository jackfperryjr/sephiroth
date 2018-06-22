using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Zenith.Models
{
    public class Request
    {
        public int RequestId { get; set; }

        // user ID from AspNetUser table.
        public string OwnerID { get; set; }
        [Required]
        public string Name { get; set; }
        [DisplayName("Today's Date")]
        [Required]
        public string DateOfToday { get; set; }
        [DisplayName("Date Requested")]
        [DataType(DataType.Date)]
        [Required]
        public string DateOfRequest { get; set; }
        [Required]
        public string Reason{ get; set; }
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        public RequestStatus Status { get; set; }
    }

    public enum RequestStatus
    {
        Submitted,
        Approved,
        Rejected
    }
}
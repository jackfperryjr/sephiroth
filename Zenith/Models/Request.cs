using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Zenith.Models
{
    public class Request
    {
        public int RequestId { get; set; }

        // user ID from AspNetUser table.
        public string OwnerID { get; set; }

        public string Name { get; set; }
        [DisplayName("Today's Date")]
        public string DateOfToday { get; set; }
        [DisplayName("Date Requested")]
        public string DateOfRequest { get; set; }
        public string Reason{ get; set; }
        [DataType(DataType.EmailAddress)]
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
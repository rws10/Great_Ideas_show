using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using log4net;

namespace IdeaSite.Models
{
    public class Mail
    {
        [Key]
        [Required]
        public int ID { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Subject { get; set; }

        [Required(ErrorMessage = "Message Body Required")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Body")]
        public string Body { get; set; }
    }
}
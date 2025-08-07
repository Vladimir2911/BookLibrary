using System.ComponentModel.DataAnnotations.Schema;
using System.Security;
using System.Text;
using System.Xml;

namespace BookLibrary.Data.Models
{
    public class Book
    {     
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublishYear { get; set; }
        public string Summary { get; set; }
        public string Contents { get; set; }        
    }

}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models
{
    public class Category
    {
        // if primary key is not like that (Id - <ClassName>Id) 
        // then use [Key] attribute to specify primary key (Data annotation -> attribute)

        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [DisplayName("Category Name")]
        [MaxLength(30)]
        public string Name { get; set; }
        public string? Description { get; set; }
        [DisplayName("Display Order")]
        [Range(1,100)]
        public int DisplayOrder { get; set; } = 0;
        public string ImageUrl { get; set; } = string.Empty;
        

    }
}

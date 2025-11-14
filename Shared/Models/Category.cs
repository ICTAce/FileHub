using System;
using System.ComponentModel.DataAnnotations;
using Oqtane.Models;

namespace ICTAce.FileHub.Models;

public class Category : IAuditable
{
    [Key]
    public int CategoryId { get; set; }
    
    public int ModuleId { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name must be less than 100 characters")]
    public string Name { get; set; }
    
    [StringLength(500, ErrorMessage = "Description must be less than 500 characters")]
    public string Description { get; set; }
    
    public int? ParentCategoryId { get; set; }
    
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime ModifiedOn { get; set; }
}

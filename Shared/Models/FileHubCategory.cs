// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ICTAce.FileHub.Shared.Models;

public class FileHubCategory : IAuditable
{
    [Key]
    public int Id { get; set; }

    public int ModuleId { get; set; }

    public int ParentCategoryId { get; set; }

    public string Name { get; set; }

    public string ViewOrder { get; set; }

    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime ModifiedOn { get; set; }
}

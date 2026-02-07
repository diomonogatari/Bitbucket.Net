using Bitbucket.Net.Models.Core.Projects;
using System.Collections.Generic;

namespace Bitbucket.Net.Models.Branches;

public class BranchModel
{
    public Branch? Development { get; set; }
    public Branch? Production { get; set; }
    public List<BranchModelType>? Types { get; set; }
}
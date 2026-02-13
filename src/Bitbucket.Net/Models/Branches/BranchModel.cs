using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Models.Branches;

public class BranchModel
{
    public Branch? Development { get; init; }
    public Branch? Production { get; init; }
    public List<BranchModelType>? Types { get; init; }
}
using Bitbucket.Net.Common.Models;
using System.Collections.Generic;

namespace Bitbucket.Net.Models.Core.Projects;

public class BrowsePathItem : PagedResultsBase
{
    public List<Line>? Lines { get; set; }
}
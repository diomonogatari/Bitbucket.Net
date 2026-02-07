using Bitbucket.Net.Common.Converters;
using Bitbucket.Net.Models.Core.Projects;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Tasks;

public class TaskAnchor : TaskRef
{
    public int Version { get; set; }
    [JsonConverter(typeof(UnixDateTimeOffsetConverter))]
    public DateTimeOffset? UpdatedDate { get; set; }
    public List<CommentRef>? Comments { get; set; }
    public List<BitbucketTask>? Tasks { get; set; }
}
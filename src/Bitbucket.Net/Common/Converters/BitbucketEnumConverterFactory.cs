using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Logs;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.RefRestrictions;
using Bitbucket.Net.Models.RefSync;
using System.Collections.Frozen;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Common.Converters;

/// <summary>
/// A <see cref="JsonConverterFactory"/> that provides <see cref="JsonEnumConverter{TEnum}"/>
/// instances for all Bitbucket enum types registered in <see cref="BitbucketEnumMaps"/>.
/// </summary>
/// <remarks>
/// This factory is parameterless so it can be referenced from
/// <see cref="JsonSourceGenerationOptionsAttribute.Converters"/> for source-generated contexts.
/// </remarks>
public sealed class BitbucketEnumConverterFactory : JsonConverterFactory
{
    private static readonly FrozenDictionary<Type, JsonConverter> s_converters =
        new Dictionary<Type, JsonConverter>
        {
            [typeof(PullRequestStates)] = new JsonEnumConverter<PullRequestStates>(BitbucketEnumMaps.PullRequestStates),
            [typeof(Permissions)] = new JsonEnumConverter<Permissions>(BitbucketEnumMaps.Permissions),
            [typeof(Roles)] = new JsonEnumConverter<Roles>(BitbucketEnumMaps.Roles),
            [typeof(LineTypes)] = new JsonEnumConverter<LineTypes>(BitbucketEnumMaps.LineTypes),
            [typeof(FileTypes)] = new JsonEnumConverter<FileTypes>(BitbucketEnumMaps.FileTypes),
            [typeof(ParticipantStatus)] = new JsonEnumConverter<ParticipantStatus>(BitbucketEnumMaps.ParticipantStatus),
            [typeof(HookTypes)] = new JsonEnumConverter<HookTypes>(BitbucketEnumMaps.HookTypes),
            [typeof(ScopeTypes)] = new JsonEnumConverter<ScopeTypes>(BitbucketEnumMaps.ScopeTypes),
            [typeof(WebHookOutcomes)] = new JsonEnumConverter<WebHookOutcomes>(BitbucketEnumMaps.WebHookOutcomes),
            [typeof(RefRestrictionTypes)] = new JsonEnumConverter<RefRestrictionTypes>(BitbucketEnumMaps.RefRestrictionTypes),
            [typeof(SynchronizeActions)] = new JsonEnumConverter<SynchronizeActions>(BitbucketEnumMaps.SynchronizeActions),
            [typeof(BlockerCommentState)] = new JsonEnumConverter<BlockerCommentState>(BitbucketEnumMaps.BlockerCommentState),
            [typeof(CommentSeverity)] = new JsonEnumConverter<CommentSeverity>(BitbucketEnumMaps.CommentSeverity),
            [typeof(LogLevels)] = new JsonEnumConverter<LogLevels>(BitbucketEnumMaps.LogLevels),
            [typeof(List<Permissions>)] = new JsonEnumListConverter<Permissions>(BitbucketEnumMaps.Permissions),
        }.ToFrozenDictionary();

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => s_converters.ContainsKey(typeToConvert);

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        s_converters[typeToConvert];
}
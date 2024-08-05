using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enterspeed.Source.UmbracoCms.Models.Api;
using Enterspeed.Source.UmbracoCms.Services;
using Enterspeed.Source.UmbracoCms.V14.Models;
using Umbraco.Cms.Core.Services;

namespace Enterspeed.Source.UmbracoCms.V14.Services;

public class EnterspeedU14JobService : IEnterspeedU14JobService
{
    private readonly IEnterspeedJobService _enterspeedJobService;
    private readonly IContentService _contentService;
    private readonly IMediaService _mediaService;
    private readonly IDictionaryItemService _dictionaryItemService;

    public EnterspeedU14JobService(
        IEnterspeedJobService enterspeedJobService,
        IContentService contentService,
        IMediaService mediaService,
        IDictionaryItemService dictionaryItemService)
    {
        _enterspeedJobService = enterspeedJobService;
        _contentService = contentService;
        _mediaService = mediaService;
        _dictionaryItemService = dictionaryItemService;
    }

    public SeedResponse Seed(bool publish, bool preview)
    {
        return _enterspeedJobService.Seed(publish, preview);
    }

    public async Task<SeedResponse> CustomSeed(U14CustomSeedModel customSeedModel, bool publish, bool preview)
    {
        var contentNodes = new List<CustomSeedNode>();
        var mediaNodes = new List<CustomSeedNode>();
        var dictionaryNodes = new List<CustomSeedNode>();

        MapContentNodes(customSeedModel, contentNodes);
        MapMediaNodes(customSeedModel, mediaNodes);
        await MapDictionaryNodes(customSeedModel, dictionaryNodes);

        return _enterspeedJobService.CustomSeed(publish, preview, new CustomSeed()
        {
            ContentNodes = [.. contentNodes],
            DictionaryNodes = [.. dictionaryNodes],
            MediaNodes = [.. mediaNodes]
        });
    }

    private async Task MapDictionaryNodes(U14CustomSeedModel customSeedModel, List<CustomSeedNode> dictionaryNodes)
    {
        foreach (var dictionaryNode in customSeedModel.DictionaryNodes)
        {
            if (dictionaryNode.Id == "Everything")
            {
                var customSeedNode = new CustomSeedNode()
                {
                    Id = -1,
                    IncludeDescendants = true
                };

                dictionaryNodes.Add(customSeedNode);
            }
            else
            {

                var customSeedNode = new CustomSeedNode()
                {
                    IncludeDescendants = dictionaryNode.IncludeDescendants
                };


                if (Guid.TryParse(dictionaryNode.Id, out var parsedGuid))
                {
                    var dictionaryItem = await _dictionaryItemService.GetAsync(parsedGuid);
                    if (dictionaryItem == null) continue;

                    customSeedNode.Id = dictionaryItem.Id;
                }

                dictionaryNodes.Add(customSeedNode);
            }
        }
    }

    private void MapMediaNodes(U14CustomSeedModel customSeedModel, List<CustomSeedNode> mediaNodes)
    {
        foreach (var mediaNode in customSeedModel.MediaNodes)
        {
            if (mediaNode.Id == "Everything")
            {
                var customSeedNode = new CustomSeedNode()
                {
                    Id = -1,
                    IncludeDescendants = true
                };

                mediaNodes.Add(customSeedNode);
            }
            else
            {
                var customSeedNode = new CustomSeedNode()
                {
                    IncludeDescendants = mediaNode.IncludeDescendants
                };

                if (Guid.TryParse(mediaNode.Id, out var parsedGuid))
                {
                    var media = _mediaService.GetById(parsedGuid);
                    if (media == null) continue;
                    customSeedNode.Id = media.Id;
                }

                mediaNodes.Add(customSeedNode);
            }
        }
    }

    private void MapContentNodes(U14CustomSeedModel customSeedModel, List<CustomSeedNode> contentNodes)
    {
        foreach (var contentNode in customSeedModel.ContentNodes)
        {
            if (contentNode.Id == "Everything")
            {
                var customSeedNode = new CustomSeedNode()
                {
                    Id = -1,
                    IncludeDescendants = true
                };

                contentNodes.Add(customSeedNode);
            }
            else
            {
                var customSeedNode = new CustomSeedNode()
                {
                    IncludeDescendants = contentNode.IncludeDescendants
                };

                if (Guid.TryParse(contentNode.Id, out var parsedGuid))
                {
                    var content = _contentService.GetById(parsedGuid);
                    if (content == null) continue;

                    customSeedNode.Id = content.Id;
                }

                contentNodes.Add(customSeedNode);
            }
        }
    }
}
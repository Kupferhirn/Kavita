﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using API.Data;
using API.Entities;

namespace API.Helpers;

public static class TagHelper
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="allTags"></param>
    /// <param name="names"></param>
    /// <param name="action">Callback for every item. Will give said item back and a bool if item was added</param>
    public static void UpdateTag(ICollection<Tag> allTags, IEnumerable<string> names, Action<Tag, bool> action)
    {
        foreach (var name in names)
        {
            if (string.IsNullOrEmpty(name.Trim())) continue;

            var added = false;
            var normalizedName = Services.Tasks.Scanner.Parser.Parser.Normalize(name);

            var genre = allTags.FirstOrDefault(p =>
                p.NormalizedTitle.Equals(normalizedName));
            if (genre == null)
            {
                added = true;
                genre = DbFactory.Tag(name);
                allTags.Add(genre);
            }

            action(genre, added);
        }
    }

    public static void KeepOnlySameTagBetweenLists(ICollection<Tag> existingTags, ICollection<Tag> removeAllExcept, Action<Tag> action = null)
    {
        var existing = existingTags.ToList();
        foreach (var genre in existing)
        {
            var existingPerson = removeAllExcept.FirstOrDefault(g => genre.NormalizedTitle.Equals(g.NormalizedTitle));
            if (existingPerson != null) continue;
            existingTags.Remove(genre);
            action?.Invoke(genre);
        }

    }

    /// <summary>
    /// Adds the tag to the list if it's not already in there. This will ignore the ExternalTag.
    /// </summary>
    /// <param name="metadataTags"></param>
    /// <param name="tag"></param>
    public static void AddTagIfNotExists(ICollection<Tag> metadataTags, Tag tag)
    {
        var existingGenre = metadataTags.FirstOrDefault(p =>
            p.NormalizedTitle == Services.Tasks.Scanner.Parser.Parser.Normalize(tag.Title));
        if (existingGenre == null)
        {
            metadataTags.Add(tag);
        }
    }

    public static void AddTagIfNotExists(BlockingCollection<Tag> metadataTags, Tag tag)
    {
        var existingGenre = metadataTags.FirstOrDefault(p =>
            p.NormalizedTitle == Services.Tasks.Scanner.Parser.Parser.Normalize(tag.Title));
        if (existingGenre == null)
        {
            metadataTags.Add(tag);
        }
    }

    /// <summary>
    /// Remove tags on a list
    /// </summary>
    /// <remarks>Used to remove before we update/add new tags</remarks>
    /// <param name="existingTags">Existing tags on Entity</param>
    /// <param name="tags">Tags from metadata</param>
    /// <param name="isExternal">Remove external tags?</param>
    /// <param name="action">Callback which will be executed for each tag removed</param>
    public static void RemoveTags(ICollection<Tag> existingTags, IEnumerable<string> tags, Action<Tag> action = null)
    {
        var normalizedTags = tags.Select(Services.Tasks.Scanner.Parser.Parser.Normalize).ToList();
        foreach (var person in normalizedTags)
        {
            var existingTag = existingTags.FirstOrDefault(p => person.Equals(p.NormalizedTitle));
            if (existingTag == null) continue;

            existingTags.Remove(existingTag);
            action?.Invoke(existingTag);
        }

    }
}


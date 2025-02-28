﻿using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using API.Entities.Enums;
using API.Extensions;
using API.Parser;
using API.Services;
using API.Services.Tasks.Scanner.Parser;
using API.Tests.Helpers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace API.Tests.Extensions;

public class ParserInfoListExtensions
{
    private readonly IDefaultParser _defaultParser;
    public ParserInfoListExtensions()
    {
        _defaultParser =
            new DefaultParser(new DirectoryService(Substitute.For<ILogger<DirectoryService>>(),
                new MockFileSystem()));
    }

    [Theory]
    [InlineData(new[] {"1", "1", "3-5", "5", "8", "0", "0"}, new[] {"1", "3-5", "5", "8", "0"})]
    public void DistinctVolumesTest(string[] volumeNumbers, string[] expectedNumbers)
    {
        var infos = volumeNumbers.Select(n => new ParserInfo() {Volumes = n}).ToList();
        Assert.Equal(expectedNumbers, infos.DistinctVolumes());
    }

    [Theory]
    [InlineData(new[] {@"Cynthia The Mission - c000-006 (v06) [Desudesu&Brolen].zip"}, new[] {@"E:\Manga\Cynthia the Mission\Cynthia The Mission - c000-006 (v06) [Desudesu&Brolen].zip"}, true)]
    [InlineData(new[] {@"Cynthia The Mission - c000-006 (v06-07) [Desudesu&Brolen].zip"}, new[] {@"E:\Manga\Cynthia the Mission\Cynthia The Mission - c000-006 (v06) [Desudesu&Brolen].zip"}, true)]
    [InlineData(new[] {@"Cynthia The Mission v20 c12-20 [Desudesu&Brolen].zip"}, new[] {@"E:\Manga\Cynthia the Mission\Cynthia The Mission - c000-006 (v06) [Desudesu&Brolen].zip"}, false)]
    public void HasInfoTest(string[] inputInfos, string[] inputChapters, bool expectedHasInfo)
    {
        var infos = new List<ParserInfo>();
        foreach (var filename in inputInfos)
        {
            infos.Add(_defaultParser.Parse(
                filename,
                string.Empty));
        }

        var files = inputChapters.Select(s => EntityFactory.CreateMangaFile(s, MangaFormat.Archive, 199)).ToList();
        var chapter = EntityFactory.CreateChapter("0-6", false, files);

        Assert.Equal(expectedHasInfo, infos.HasInfo(chapter));
    }
}

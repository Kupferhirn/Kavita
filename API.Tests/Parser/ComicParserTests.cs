using System.IO.Abstractions.TestingHelpers;
using API.Parser;
using API.Services;
using API.Services.Tasks.Scanner.Parser;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace API.Tests.Parser;

public class ComicParserTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly DefaultParser _defaultParser;

    public ComicParserTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _defaultParser =
            new DefaultParser(new DirectoryService(Substitute.For<ILogger<DirectoryService>>(),
                new MockFileSystem()));
    }

    [Theory]
    [InlineData("04 - Asterix the Gladiator (1964) (Digital-Empire) (WebP by Doc MaKS)", "Asterix the Gladiator")]
    [InlineData("The First Asterix Frieze (WebP by Doc MaKS)", "The First Asterix Frieze")]
    [InlineData("Batman & Catwoman - Trail of the Gun 01", "Batman & Catwoman - Trail of the Gun")]
    [InlineData("Batman & Daredevil - King of New York", "Batman & Daredevil - King of New York")]
    [InlineData("Batman & Grendel (1996) 01 - Devil's Bones", "Batman & Grendel")]
    [InlineData("Batman & Robin the Teen Wonder #0", "Batman & Robin the Teen Wonder")]
    [InlineData("Batman & Wildcat (1 of 3)", "Batman & Wildcat")]
    [InlineData("Batman And Superman World's Finest #01", "Batman And Superman World's Finest")]
    [InlineData("Babe 01", "Babe")]
    [InlineData("Scott Pilgrim 01 - Scott Pilgrim's Precious Little Life (2004)", "Scott Pilgrim")]
    [InlineData("Teen Titans v1 001 (1966-02) (digital) (OkC.O.M.P.U.T.O.-Novus)", "Teen Titans")]
    [InlineData("Scott Pilgrim 02 - Scott Pilgrim vs. The World (2005)", "Scott Pilgrim")]
    [InlineData("Wolverine - Origins 003 (2006) (digital) (Minutemen-PhD)", "Wolverine - Origins")]
    [InlineData("Invincible Vol 01 Family matters (2005) (Digital).cbr", "Invincible")]
    [InlineData("Amazing Man Comics chapter 25", "Amazing Man Comics")]
    [InlineData("Amazing Man Comics issue #25", "Amazing Man Comics")]
    [InlineData("Teen Titans v1 038 (1972) (c2c).cbr", "Teen Titans")]
    [InlineData("Batman Beyond 02 (of 6) (1999)", "Batman Beyond")]
    [InlineData("Batman Beyond - Return of the Joker (2001)", "Batman Beyond - Return of the Joker")]
    [InlineData("Invincible 033.5 - Marvel Team-Up 14 (2006) (digital) (Minutemen-Slayer)", "Invincible")]
    [InlineData("Batman Wayne Family Adventures - Ep. 001 - Moving In", "Batman Wayne Family Adventures")]
    [InlineData("Saga 001 (2012) (Digital) (Empire-Zone).cbr", "Saga")]
    [InlineData("spawn-123", "spawn")]
    [InlineData("spawn-chapter-123", "spawn")]
    [InlineData("Spawn 062 (1997) (digital) (TLK-EMPIRE-HD).cbr", "Spawn")]
    [InlineData("Batman Beyond 04 (of 6) (1999)", "Batman Beyond")]
    [InlineData("Batman Beyond 001 (2012)", "Batman Beyond")]
    [InlineData("Batman Beyond 2.0 001 (2013)", "Batman Beyond 2.0")]
    [InlineData("Batman - Catwoman 001 (2021) (Webrip) (The Last Kryptonian-DCP)", "Batman - Catwoman")]
    [InlineData("Chew v1 - Taster´s Choise (2012) (Digital) (1920) (Kingpin-Empire)", "Chew")]
    [InlineData("Chew Script Book (2011) (digital-Empire) SP04", "Chew Script Book")]
    [InlineData("Batman - Detective Comics - Rebirth Deluxe Edition Book 02 (2018) (digital) (Son of Ultron-Empire)", "Batman - Detective Comics - Rebirth Deluxe Edition Book")]
    [InlineData("Cyberpunk 2077 - Your Voice #01", "Cyberpunk 2077 - Your Voice")]
    [InlineData("Cyberpunk 2077 #01", "Cyberpunk 2077")]
    [InlineData("Cyberpunk 2077 - Trauma Team #04.cbz", "Cyberpunk 2077 - Trauma Team")]
    [InlineData("Batgirl Vol.2000 #57 (December, 2004)", "Batgirl")]
    [InlineData("Batgirl V2000 #57", "Batgirl")]
    [InlineData("Fables 021 (2004) (Digital) (Nahga-Empire)", "Fables")]
    [InlineData("2000 AD 0366 [1984-04-28] (flopbie)", "2000 AD")]
    [InlineData("Daredevil - v6 - 10 - (2019)", "Daredevil")]
    [InlineData("Batman - The Man Who Laughs #1 (2005)", "Batman - The Man Who Laughs")]
    [InlineData("Demon 012 (Sep 1973) c2c", "Demon")]
    [InlineData("Dragon Age - Until We Sleep 01 (of 03)", "Dragon Age - Until We Sleep")]
    [InlineData("Green Lantern v2 017 - The Spy-Eye that doomed Green Lantern v2", "Green Lantern")]
    [InlineData("Green Lantern - Circle of Fire Special - Adam Strange (2000)", "Green Lantern - Circle of Fire - Adam Strange")]
    [InlineData("Identity Crisis Extra - Rags Morales Sketches (2005)", "Identity Crisis - Rags Morales Sketches")]
    [InlineData("Daredevil - t6 - 10 - (2019)", "Daredevil")]
    [InlineData("Batgirl T2000 #57", "Batgirl")]
    [InlineData("Teen Titans t1 001 (1966-02) (digital) (OkC.O.M.P.U.T.O.-Novus)", "Teen Titans")]
    [InlineData("Conquistador_-Tome_2", "Conquistador")]
    [InlineData("Max_l_explorateur-_Tome_0", "Max l explorateur")]
    [InlineData("Chevaliers d'Héliopolis T3 - Rubedo, l'oeuvre au rouge (Jodorowsky & Jérémy)", "Chevaliers d'Héliopolis")]
    [InlineData("Bd Fr-Aldebaran-Antares-t6", "Aldebaran-Antares")]
    [InlineData("Tintin - T22 Vol 714 pour Sydney", "Tintin")]
    [InlineData("Fables 2010 Vol. 1 Legends in Exile", "Fables 2010")]
    [InlineData("Kebab Том 1 Глава 1", "Kebab")]
    [InlineData("Манга Глава 1", "Манга")]
    public void ParseComicSeriesTest(string filename, string expected)
    {
        Assert.Equal(expected, API.Services.Tasks.Scanner.Parser.Parser.ParseComicSeries(filename));
    }

    [Theory]
    [InlineData("01 Spider-Man & Wolverine 01.cbr", "0")]
    [InlineData("04 - Asterix the Gladiator (1964) (Digital-Empire) (WebP by Doc MaKS)", "0")]
    [InlineData("The First Asterix Frieze (WebP by Doc MaKS)", "0")]
    [InlineData("Batman & Catwoman - Trail of the Gun 01", "0")]
    [InlineData("Batman & Daredevil - King of New York", "0")]
    [InlineData("Batman & Grendel (1996) 01 - Devil's Bones", "0")]
    [InlineData("Batman & Robin the Teen Wonder #0", "0")]
    [InlineData("Batman & Wildcat (1 of 3)", "0")]
    [InlineData("Batman And Superman World's Finest #01", "0")]
    [InlineData("Babe 01", "0")]
    [InlineData("Scott Pilgrim 01 - Scott Pilgrim's Precious Little Life (2004)", "0")]
    [InlineData("Teen Titans v1 001 (1966-02) (digital) (OkC.O.M.P.U.T.O.-Novus)", "1")]
    [InlineData("Scott Pilgrim 02 - Scott Pilgrim vs. The World (2005)", "0")]
    [InlineData("Superman v1 024 (09-10 1943)", "1")]
    [InlineData("Superman v1.5 024 (09-10 1943)", "1.5")]
    [InlineData("Amazing Man Comics chapter 25", "0")]
    [InlineData("Invincible 033.5 - Marvel Team-Up 14 (2006) (digital) (Minutemen-Slayer)", "0")]
    [InlineData("Cyberpunk 2077 - Trauma Team 04.cbz", "0")]
    [InlineData("spawn-123", "0")]
    [InlineData("spawn-chapter-123", "0")]
    [InlineData("Spawn 062 (1997) (digital) (TLK-EMPIRE-HD).cbr", "0")]
    [InlineData("Batman Beyond 04 (of 6) (1999)", "0")]
    [InlineData("Batman Beyond 001 (2012)", "0")]
    [InlineData("Batman Beyond 2.0 001 (2013)", "0")]
    [InlineData("Batman - Catwoman 001 (2021) (Webrip) (The Last Kryptonian-DCP)", "0")]
    [InlineData("Chew v1 - Taster´s Choise (2012) (Digital) (1920) (Kingpin-Empire)", "1")]
    [InlineData("Chew Script Book (2011) (digital-Empire) SP04", "0")]
    [InlineData("Batgirl Vol.2000 #57 (December, 2004)", "2000")]
    [InlineData("Batgirl V2000 #57", "2000")]
    [InlineData("Fables 021 (2004) (Digital) (Nahga-Empire).cbr", "0")]
    [InlineData("Cyberpunk 2077 - Trauma Team 04.cbz", "0")]
    [InlineData("2000 AD 0366 [1984-04-28] (flopbie)", "0")]
    [InlineData("Daredevil - v6 - 10 - (2019)", "6")]
    [InlineData("Daredevil - v6.5", "6.5")]
    // Tome Tests
    [InlineData("Daredevil - t6 - 10 - (2019)", "6")]
    [InlineData("Batgirl T2000 #57", "2000")]
    [InlineData("Teen Titans t1 001 (1966-02) (digital) (OkC.O.M.P.U.T.O.-Novus)", "1")]
    [InlineData("Conquistador_Tome_2", "2")]
    [InlineData("Max_l_explorateur-_Tome_0", "0")]
    [InlineData("Chevaliers d'Héliopolis T3 - Rubedo, l'oeuvre au rouge (Jodorowsky & Jérémy)", "3")]
    [InlineData("Adventure Time (2012)/Adventure Time #1 (2012)", "0")]
    [InlineData("Adventure Time TPB (2012)/Adventure Time v01 (2012).cbz", "1")]
    // Russian Tests
    [InlineData("Kebab Том 1 Глава 3", "1")]
    [InlineData("Манга Глава 2", "0")]
    public void ParseComicVolumeTest(string filename, string expected)
    {
        Assert.Equal(expected, API.Services.Tasks.Scanner.Parser.Parser.ParseComicVolume(filename));
    }

    [Theory]
    [InlineData("01 Spider-Man & Wolverine 01.cbr", "1")]
    [InlineData("04 - Asterix the Gladiator (1964) (Digital-Empire) (WebP by Doc MaKS)", "0")]
    [InlineData("The First Asterix Frieze (WebP by Doc MaKS)", "0")]
    [InlineData("Batman & Catwoman - Trail of the Gun 01", "1")]
    [InlineData("Batman & Daredevil - King of New York", "0")]
    [InlineData("Batman & Grendel (1996) 01 - Devil's Bones", "1")]
    [InlineData("Batman & Robin the Teen Wonder #0", "0")]
    [InlineData("Batman & Wildcat (1 of 3)", "1")]
    [InlineData("Batman & Wildcat (2 of 3)", "2")]
    [InlineData("Batman And Superman World's Finest #01", "1")]
    [InlineData("Babe 01", "1")]
    [InlineData("Scott Pilgrim 01 - Scott Pilgrim's Precious Little Life (2004)", "1")]
    [InlineData("Teen Titans v1 001 (1966-02) (digital) (OkC.O.M.P.U.T.O.-Novus)", "1")]
    [InlineData("Superman v1 024 (09-10 1943)", "24")]
    [InlineData("Invincible 070.5 - Invincible Returns 1 (2010) (digital) (Minutemen-InnerDemons).cbr", "70.5")]
    [InlineData("Amazing Man Comics chapter 25", "25")]
    [InlineData("Invincible 033.5 - Marvel Team-Up 14 (2006) (digital) (Minutemen-Slayer)", "33.5")]
    [InlineData("Batman Wayne Family Adventures - Ep. 014 - Moving In", "14")]
    [InlineData("Saga 001 (2012) (Digital) (Empire-Zone)", "1")]
    [InlineData("spawn-123", "123")]
    [InlineData("spawn-chapter-123", "123")]
    [InlineData("Spawn 062 (1997) (digital) (TLK-EMPIRE-HD).cbr", "62")]
    [InlineData("Batman Beyond 04 (of 6) (1999)", "4")]
    [InlineData("Invincible 052 (c2c) (2008) (Minutemen-TheCouple)", "52")]
    [InlineData("Y - The Last Man #001", "1")]
    [InlineData("Batman Beyond 001 (2012)", "1")]
    [InlineData("Batman Beyond 2.0 001 (2013)", "1")]
    [InlineData("Batman - Catwoman 001 (2021) (Webrip) (The Last Kryptonian-DCP)", "1")]
    [InlineData("Chew v1 - Taster´s Choise (2012) (Digital) (1920) (Kingpin-Empire)", "0")]
    [InlineData("Chew Script Book (2011) (digital-Empire) SP04", "0")]
    [InlineData("Batgirl Vol.2000 #57 (December, 2004)", "57")]
    [InlineData("Batgirl V2000 #57", "57")]
    [InlineData("Fables 021 (2004) (Digital) (Nahga-Empire).cbr", "21")]
    [InlineData("Cyberpunk 2077 - Trauma Team #04.cbz", "4")]
    [InlineData("2000 AD 0366 [1984-04-28] (flopbie)", "366")]
    [InlineData("Daredevil - v6 - 10 - (2019)", "10")]
    [InlineData("Batman Beyond 2016 - Chapter 001.cbz", "1")]
    [InlineData("Adventure Time (2012)/Adventure Time #1 (2012)", "1")]
    [InlineData("Adventure Time TPB (2012)/Adventure Time v01 (2012).cbz", "0")]
    [InlineData("Kebab Том 1 Глава 3", "3")]
    [InlineData("Манга Глава 2", "2")]
    [InlineData("Манга 2 Глава", "2")]
    [InlineData("Манга Том 1 2 Глава", "2")]
    public void ParseComicChapterTest(string filename, string expected)
    {
        Assert.Equal(expected, API.Services.Tasks.Scanner.Parser.Parser.ParseComicChapter(filename));
    }


    [Theory]
    [InlineData("Batman - Detective Comics - Rebirth Deluxe Edition Book 02 (2018) (digital) (Son of Ultron-Empire)", true)]
    [InlineData("Zombie Tramp vs. Vampblade TPB (2016) (Digital) (TheArchivist-Empire)", true)]
    [InlineData("Baldwin the Brave & Other Tales Special SP1.cbr", true)]
    [InlineData("Mouse Guard Specials - Spring 1153 - Fraggle Rock FCBD 2010", true)]
    [InlineData("Boule et Bill - THS -Bill à disparu", true)]
    [InlineData("Asterix - HS - Les 12 travaux d'Astérix", true)]
    [InlineData("Sillage Hors Série - Le Collectionneur - Concordance-DKFR", true)]
    [InlineData("laughs", false)]
    [InlineData("Annual Days of Summer", true)]
    [InlineData("Adventure Time 2013 Annual #001 (2013)", true)]
    [InlineData("Adventure Time 2013_Annual_#001 (2013)", true)]
    [InlineData("Adventure Time 2013_-_Annual #001 (2013)", true)]
    [InlineData("G.I. Joe - A Real American Hero Yearbook 004 Reprint (2021)", false)]
    [InlineData("Mazebook 001", false)]
    [InlineData("X-23 One Shot (2010)", true)]
    [InlineData("Casus Belli v1 Hors-Série 21 - Mousquetaires et Sorcellerie", true)]
    [InlineData("Batman Beyond Annual", true)]
    [InlineData("Batman Beyond Bonus", true)]
    [InlineData("Batman Beyond OneShot", true)]
    [InlineData("Batman Beyond Specials", true)]
    [InlineData("Batman Beyond Omnibus (1999)", true)]
    [InlineData("Batman Beyond Omnibus", true)]
    [InlineData("01 Annual Batman Beyond", true)]
    public void IsComicSpecialTest(string input, bool expected)
    {
        Assert.Equal(expected, API.Services.Tasks.Scanner.Parser.Parser.IsComicSpecial(input));
    }
}

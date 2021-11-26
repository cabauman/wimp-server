using Xunit;
using System;

namespace WIMP_Server.Tests.SearchMessageUtilities;

public class SearchMessageUtilitiesTests
{
    [Fact]
    public void ExtractNamesFromIntelStringDescendingLength_MultipleMatchingValues_ShouldReturnAll()
    {
        var intelInput = "Hecate* in my pants. Omen Drake Navy Issue Hecate Vexor GJ0-OJ";
        var possibleNames = new string[] { "Omen", "Hecate" };
        var expected = new string[] { "Hecate", "Hecate", "Omen" };

        var matchingNames = Searching.SearchMessageUtilities
            .ExtractNamesFromIntelStringDescendingLength(possibleNames, ref intelInput);

        Assert.Equal(expected, matchingNames);
    }

    [Fact]
    public void ExtractNamesFromIntelStringDescendingLength_NoMatchingValues_ShouldReturnNone()
    {
        var intelInput = "Hecate* in my pants. Omen Drake Navy Issue Hecate Vexor GJ0-OJ";
        var possibleNames = new string[] { "Loki", "Tengu" };

        var matchingNames = Searching.SearchMessageUtilities
            .ExtractNamesFromIntelStringDescendingLength(possibleNames, ref intelInput);

        Assert.Empty(matchingNames);
    }

    [Fact]
    public void ExtractNamesFromIntelStringDescendingLength_EmptyIntelInput_ShouldReturnNone()
    {
        var intelInput = "";
        var possibleNames = new string[] { "Loki", "Tengu" };

        var matchingNames = Searching.SearchMessageUtilities
            .ExtractNamesFromIntelStringDescendingLength(possibleNames, ref intelInput);

        Assert.Empty(matchingNames);
    }

    [Fact]
    public void ExtractNamesFromIntelStringDescendingLength_EmptyPossibleNamesInput_ShouldReturnNone()
    {
        var intelInput = "this is the intel";
        var possibleNames = Array.Empty<string>();

        var matchingNames = Searching.SearchMessageUtilities
            .ExtractNamesFromIntelStringDescendingLength(possibleNames, ref intelInput);

        Assert.Empty(matchingNames);
    }

    [Fact]
    public void ExtractNamesFromIntelStringDescendingLength_NullIntelInput_ShouldThrowArgumentNullException()
    {
        string intelInput = null;
        var possibleNames = new string[] { "Loki", "Tengu" };

        Assert.Throws<ArgumentNullException>(() => Searching.SearchMessageUtilities
            .ExtractNamesFromIntelStringDescendingLength(possibleNames, ref intelInput));
    }

    [Fact]
    public void ExtractNamesFromIntelStringDescendingLength_NullPossibleNamesInput_ShouldThrowArgumentNullException()
    {
        var intelInput = "this is the intel";
        string[] possibleNames = null;

        Assert.Throws<ArgumentNullException>(() => Searching.SearchMessageUtilities
            .ExtractNamesFromIntelStringDescendingLength(possibleNames, ref intelInput));
    }

    [Fact]
    public void ExtractNamesFromIntelStringDescendingLength_PartOfMatchingInput_ShouldRemoveMatchesFromInputString()
    {
        var intelInput = "Hecate* in my pants. Omen Drake Navy Issue Hecate Vexor GJ0-OJ";
        var possibleNames = new string[] { "Omen", "Hecate" };
        const string expectedInput = "* in my pants.  Drake Navy Issue  Vexor GJ0-OJ";

        Searching.SearchMessageUtilities
            .ExtractNamesFromIntelStringDescendingLength(possibleNames, ref intelInput);

        Assert.Equal(expectedInput, intelInput);
    }

    [Fact]
    public void ExtractNamesFromIntelStringDescendingLength_OverlappingNamesWithSpaces_ShouldReturnAll()
    {
        var intelInput = "Barack Obama Obama Barack";
        var possibleNames = new string[] { "Obama", "Barack Obama", "Barack" };
        var expected = new string[] { "Barack Obama", "Barack", "Obama" };

        var matchingNames = Searching.SearchMessageUtilities
            .ExtractNamesFromIntelStringDescendingLength(possibleNames, ref intelInput);

        Assert.Equal(expected, matchingNames);
    }
}

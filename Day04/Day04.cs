using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using Advent.Util;

Part1("Input.txt");
Part2("Input.txt");
void Part1(string filePath)
{
    var cards = Card.ParseAll(filePath);
    var score = cards
        .Select(x => x.GetMatchCount())
        .Where(x => x > 0)
        .Select(x => Math.Pow(2, x - 1))
        .Sum();
    Console.WriteLine(score);
}

void Part2(string filePath)
{
    var cards = Card.ParseAll(filePath);
    var copyCount = new int[cards.Count];
    Array.Fill(copyCount, 1);

    for (int i = 0; i < cards.Count; i++)
    {
        var card = cards[i];
        var matchCount = card.GetMatchCount();
        if (matchCount == 0)
            continue;

        var increment = copyCount[i];
        for (int j = 1; j <= matchCount && j + i < copyCount.Length; j++)
        {
            copyCount[i + j] += increment;
        }
    }
    Console.WriteLine(copyCount.Sum());
}

internal sealed class Card(HashSet<int> winningSet, HashSet<int> foundSet)
{
    internal int GetMatchCount()
    {
        var count = 0;
        foreach (var number in foundSet)
        {
            if (winningSet.Contains(number))
            {
                count++;
            }
        }

        return count;
    }

    public static Card Parse(ReadOnlySpan<char> line)
    {
        line = line.Slice(line.IndexOf(':') + 1); 
        Span<Range> ranges = stackalloc Range[2];

        var split = line.IndexOf('|');
        var winningSet = ParseNumbers(line.Slice(0, split));
        var foundSet = ParseNumbers(line.Slice(split + 1));
        return new Card(winningSet, foundSet);

        static HashSet<int> ParseNumbers(ReadOnlySpan<char> span)
        {
            var count = span.Count(' ') + 1;
            Span<Range> ranges = count > 20
                ? new Range[count]
                : stackalloc Range[count];
            int rangeCount = span.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries);
            var set = new HashSet<int>(capacity: count);
            for (int i = 0; i < rangeCount; i++)
            {
                var range = ranges[i];
                var number = int.Parse(span[range]);
                set.Add(number);
            }

            return set;
        }
    }

    public static List<Card> ParseAll(string filePath)
    {
        var list = new List<Card>();
        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            list.Add(Card.Parse(line));
        }

        return list;
    }
}
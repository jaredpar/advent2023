namespace Advent.Util;

public readonly struct GridSpan<T>
{
    public Grid<T> Grid { get; }
    public int Row { get; }
    public int Column { get; }
    public int Length { get; }
    public int End => Column + Length;
    public Memory<T> Memory => Grid.GetRowMemory(Row).Slice(Column, Length);
    public Span<T> Span => Grid.GetRowSpan(Row).Slice(Column, Length);

    public ref T this[int index] => ref Grid.GetValue(Row, Column + index);

    public GridSpan(Grid<T> grid, int row, int column, int length)
    {
        Grid = grid;
        Row = row;
        Column = column;
        Length = length;
    }

    public GridSpan<T> Slice(int start, int length) => new GridSpan<T>(Grid, Row, Column + start, length);

    public bool Contains(int row, int column) => row == Row && column >= Column && column < Column + Length;

    public bool IsAdjacent(int row, int column)
    {
        if (row == Row)
        {
            return column == Column - 1 || column == Column + Length;
        }

        if ((row + 1 == Row) || (row - 1 == Row))
        {
            return column >= (Column - 1) && column < (Column + Length + 1);
        }

        return false;
    }

    public override string ToString() => $"{nameof(GridSpan<T>)}: {Row}, {Column}, {Length}";
}

public sealed class Grid<T>
{
    private readonly T[] _items;
    private readonly int _columns;

    public int Rows => _items.Length / _columns;
    public int Columns => _columns;

    public Grid(int rows, int columns)
    {
        _items = new T[rows * columns];
        _columns = columns;
    }

    public IEnumerable<(int Row, int Column)> GetAdjacentIndexes(int row, int column)
    {
        if (row - 1 >= 0)
        {
            yield return (row - 1, column);
            if (column - 1 >= 0)
                yield return (row - 1, column - 1);
            if (column + 1 < Columns)
                yield return (row - 1, column + 1);
        }

        if (row + 1 < Rows)
        {
            yield return (row + 1, column);
            if (column - 1 >= 0)
                yield return (row + 1, column - 1);
            if (column + 1 < Columns)
                yield return (row + 1, column + 1);
        }

        if (column - 1 >= 0)
            yield return (row, column - 1);

        if (column + 1 < Columns)
            yield return (row, column + 1);
    }

    public ref T GetValue(int row, int column) => ref _items[(row * _columns) + column];

    public Memory<T> GetRowMemory(int row) => _items.AsMemory(row * _columns, _columns);

    public Span<T> GetRowSpan(int row) => _items.AsSpan(row * _columns, _columns);

    public GridSpan<T> GetGridSpan(int row) => new(this, row, 0, Columns);

    public IEnumerable<GridSpan<T>> GetGridSpans()
    {
        for (int r = 0; r < Rows; r++)
        {
            yield return new GridSpan<T>(this, r, 0, Columns);
        }
    }

    public IEnumerable<(T, int row, int column)> GetValues()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                yield return (GetValue(r, c), r, c);
            }
        }
    }

    public static Grid<char> Parse(string[] lines)
    {
        var grid = new Grid<char>(lines.Length, lines[0].Length);
        for (int r = 0; r < lines.Length; r++)
        {
            var line = lines[r];
            for (int c = 0; c < line.Length; c++)
            {
                grid.GetValue(r, c) = line[c];
            }
        }

        return grid;
    }
}
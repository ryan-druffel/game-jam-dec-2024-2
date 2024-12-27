using System.Collections.Generic;
using UnityEngine;

// grid of integers representing id numbers
// accompanying dictionary containing all elements with references to their numbers/grid spaces
public class JamGrid
{
    Dictionary<int, JamEntity> _entities;
    int[,] _grid;

    public void Setup(int width, int height)
    {
        _grid = new int[width, height];
    }

    public int GetCellValue(int col, int row)
    {
        return _grid[col, row];
    }

    public void SetCellValue(int col, int row, int value)
    {
        _grid[col, row] = value;
    }
}

public class JamEntity
{
    private static int _usedIDs = 0; // ensures all IDs are unique
    private JamGrid _grid;

    public readonly int ID;
    public int Column { get; private set; }
    public int Row { get; private set; }

    // constructor
    public JamEntity(int col, int row)
    {
        ID = _usedIDs++;
        Move(col, row);
    }

    // move this on the grid
    public void Move(int col, int row)
    {
        _grid.SetCellValue(col, row, ID);
        Column = col;
        Row = row;
    }

    public int GetColumn() { return Column; }
    public int GetRow() { return Row; }
}

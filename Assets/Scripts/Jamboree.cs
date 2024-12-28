using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// grid of integers representing id numbers
// accompanying dictionary containing all elements with references to their numbers/grid spaces
public class JamGridClass
{
    Dictionary<int, JamEntityClass> _entities;
    int[,] _grid;
    int _width, _height;

    public JamGridClass(int width, int height)
    {
        // set up grid
        _width = width;
        _height = height;
        _grid = new int[width, height];

        // setup dictionary
        _entities = new Dictionary<int, JamEntityClass>();
    }

    public void AddEntity(JamEntityClass entity)
    {
        _entities.Add(entity.ID, entity);
    }

    public int GetCellValue(int col, int row)
    {
        return _grid[col, row];
    }

    public void SetCellValue(int col, int row, int value)
    {
        _grid[col, row] = value;
    }

    public JamEntityClass[] GetAllEntities()
    {
        return _entities.Values.ToArray();
    }

    public JamEntityClass GetEntityFromID(int ID)
    {
        return _entities[ID];
    }

    /// <summary>
    /// Randomly populates the grid with new entities.
    /// </summary>
    public void PeterGriffin()
    {
        for (int r = 0; r < _height; r++)
        {
            for (int c = 0; c < _width; c++)
            {
                // add something maybe
                if (Random.value < 0.25f)
                {
                    AddEntity(new JamEntityClass(c, r));
                }
            }
        }
    }
}

public class JamEntityClass
{
    static int _usedIDs = 1; // ensures all IDs are unique
    SpriteRenderer _display;
    Transform _transform;

    public readonly int ID;
    public int Column { get; private set; }
    public int Row { get; private set; }

    // constructor (also creates an accompanying gameobject and sprite renderer)
    public JamEntityClass(int col, int row)
    {
        // assign new ID to self
        ID = _usedIDs++;

        // move into position
        Move(col, row);

        // create display object
        var obj = new GameObject("Entity " + ID);
        _transform = obj.transform;
        _display = obj.AddComponent<SpriteRenderer>();
    }

    // move this in gridspace
    public void Move(int col, int row)
    {
        Column = col;
        Row = row;
    }

    public int GetColumn() { return Column; }
    public int GetRow() { return Row; }

    public SpriteRenderer GetDisplay() { return _display; }
    public Transform GetTransform() { return _transform; }
}

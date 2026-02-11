using System;
using System.Collections.Generic;
using System.Linq;
using TuApp.Models;

namespace TuApp.Dxf;

public sealed class CanvasMapper
{
    public double Margin { get; }

    public CanvasMapper(double margin = 60)
    {
        Margin = margin;
    }

    public List<(string Id, double X, double Y)> Map(
        IEnumerable<ButtonMark> marks,
        double canvasWidth,
        double canvasHeight)
    {
        var list = marks.ToList();
        if (list.Count == 0) return new();

        double minX = list.Min(m => m.X);
        double maxX = list.Max(m => m.X);
        double minY = list.Min(m => m.Y);
        double maxY = list.Max(m => m.Y);

        double dx = Math.Max(1e-9, maxX - minX);
        double dy = Math.Max(1e-9, maxY - minY);

        double sx = (canvasWidth - 2 * Margin) / dx;
        double sy = (canvasHeight - 2 * Margin) / dy;
        double s = Math.Min(sx, sy);

        return list.Select(m =>
        {
            double x = Margin + (m.X - minX) * s;
            double y = Margin + (maxY - m.Y) * s; // invertir Y (DXF arriba, Canvas abajo)
            return (m.Id, Math.Round(x, 2), Math.Round(y, 2));
        }).ToList();
    }
}

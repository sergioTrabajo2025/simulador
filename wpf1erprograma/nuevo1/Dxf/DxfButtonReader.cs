using netDxf;
using netDxf.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using TuApp.Models;

namespace TuApp.Dxf;

public sealed class DxfButtonReader
{
    public string TargetLayer { get; }
    public string AttributeTag { get; }
    public string? BlockName { get; }

    public DxfButtonReader(string targetLayer = "BOTONES", string attributeTag = "ID1", string? blockName = "identificador")
    {
        TargetLayer = targetLayer;
        AttributeTag = attributeTag;
        BlockName = blockName;
    }

    public List<ButtonMark> Read(string dxfPath)
    {
        var dxf = DxfDocument.Load(dxfPath);
        if (dxf == null)
            throw new InvalidOperationException("No se pudo abrir el DXF.");

        var result = new List<ButtonMark>();

        foreach (Insert ins in dxf.Entities.Inserts)
        {
            // 1) Capa
            var layer = ins.Layer?.Name ?? "";
            if (!string.Equals(layer, TargetLayer, StringComparison.OrdinalIgnoreCase))
                continue;

            // 2) Nombre de bloque (opcional pero recomendado)
            if (!string.IsNullOrWhiteSpace(BlockName))
            {
                var bn = ins.Block?.Name ?? "";
                if (!string.Equals(bn, BlockName, StringComparison.OrdinalIgnoreCase))
                    continue;
            }

            // 3) Atributo (en tu caso: ID1)
            var idAttr = ins.Attributes?.FirstOrDefault(a =>
                string.Equals(a.Tag, AttributeTag, StringComparison.OrdinalIgnoreCase));

            var id = idAttr?.Value?.Trim();
            if (string.IsNullOrWhiteSpace(id))
                continue;

            result.Add(new ButtonMark(id, ins.Position.X, ins.Position.Y, layer));
        }

        return result;
    }
}

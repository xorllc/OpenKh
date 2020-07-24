﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenKh.Engine.Parsers;
using OpenKh.Kh2;
using System.Collections.Generic;
using System.Linq;

namespace OpenKh.Game.Models
{
    public static class MeshLoader
    {
        public static MeshGroup FromKH2(GraphicsDevice graphics, Mdlx model, ModelTexture texture)
        {
            if (model == null || texture == null)
                return null;

            var modelParsed = new MdlxParser(model);
            return modelParsed.MeshDescriptors != null ?
                LoadNew(graphics, modelParsed, texture) :
                LoadLegacy(graphics, modelParsed, texture);
        }

        private static MeshGroup LoadNew(GraphicsDevice graphics, MdlxParser model, ModelTexture texture)
        {
            return new MeshGroup
            {
                Segments = null,
                Parts = null,
                MeshDescriptors = model.MeshDescriptors?
                    .Select(x => new MeshDesc
                    {
                        Vertices = x.Vertices
                            .Select(v => new VertexPositionColorTexture(
                                new Vector3(v.X, v.Y, v.Z),
                                new Color((v.Color >> 16) & 0xff, (v.Color >> 8) & 0xff, v.Color & 0xff, (v.Color >> 24) & 0xff),
                                new Vector2(v.Tu, v.Tv)))
                            .ToArray(),
                        Indices = x.Indices,
                        TextureIndex = x.TextureIndex,
                        IsOpaque = x.IsOpaque
                    })
                    .ToList(),
                Textures = LoadTextures(graphics, texture).ToArray()
            };
        }

        private static MeshGroup LoadLegacy(GraphicsDevice graphics, MdlxParser model, ModelTexture texture)
        {
            MeshGroup.Segment[] segments = null;
            MeshGroup.Part[] parts = null;

            segments = model.Model.Segments.Select(segment => new MeshGroup.Segment
            {
                Vertices = segment.Vertices.Select(vertex => new VertexPositionColorTexture
                {
                    Position = new Vector3(vertex.X, vertex.Y, vertex.Z),
                    TextureCoordinate = new Vector2(vertex.U, vertex.V),
                    Color = new Color((vertex.Color >> 16) & 0xff, (vertex.Color >> 8) & 0xff, vertex.Color & 0xff, (vertex.Color >> 24) & 0xff)
                }).ToArray()
            }).ToArray();

            parts = model.Model.Parts.Select(part => new MeshGroup.Part
            {
                Indices = part.Indices,
                SegmentId = part.SegmentIndex,
                TextureId = part.TextureIndex,
                IsOpaque = part.IsOpaque,
            }).ToArray();

            return new MeshGroup
            {
                Segments = segments,
                Parts = parts,
                Textures = LoadTextures(graphics, texture).ToArray()
            };
        }

        public static IEnumerable<KingdomTexture> LoadTextures(
            GraphicsDevice graphics, ModelTexture texture) => texture?.Images?
                .Select(texture => new KingdomTexture(texture, graphics)).ToArray() ??
                new KingdomTexture[0];
    }
}

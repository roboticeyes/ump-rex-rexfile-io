/*
 * Copyright (c) 2019 Robotic Eyes GmbH software
 *
 * THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 * PARTICULAR PURPOSE.
 *
 */

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RoboticEyes.Rex.RexFileReader
{
    /**
    * Data block which allows to specify a 3D mesh
    */
    public class RexDataMesh : RexDataBlock
    {
        private const byte SizeLod = 2;
        private const byte SizeMaxLod = 2;
        private const byte SizeNrOfVtxCoords = 4;
        private const byte SizeNrOfNorCoords = 4;
        private const byte SizeNrOfTexCoords = 4;
        private const byte SizeNrOfVtxColors = 4;
        private const byte SizeNrTriangles = 4;
        private const byte SizeStartVtxCoords = 4;
        private const byte SizeStartNorCoords = 4;
        private const byte SizeStartTexCoords = 4;
        private const byte SizeStartVtxColors = 4;
        private const byte SizeStartTriangles = 4;
        private const byte SizeMaterialId = 8;
        private const byte SizeNameSize = 2;
        private const byte SizeName = 74;

        public ushort lod;
        public ushort maxLod;
        public List<Vector3> vertexCoordinates;     //!< 3D coordinates of all vertices unit meters [#vtx:3]
        public List<Vector3> normalVectors;
        public List<Vector2> textureCoordinates;    //!< 2D texture coordinates between 0 and 1 [#tex:2]
        public List<Color> vertexColors;            //!< color for each vertex
        public int[] vertexTriangles;               //!< vetex indices that define triangles [#triangles:3]
        public ulong materialId;                   //!< materialId which refers to a MeshMaterialStandard block
        public string name;                         //!< name of the mesh

        #region WRITE
        public RexDataMesh (string name, ushort lod, ushort maxLod,
                            List<Vector3> vertexCoordinates,
                            List<Vector3> normalVectors,
                            List<Vector2> textureCoordinates,
                            List<Color> vertexColors,
                            int[] vertexTriangles,
                            ulong materialId)
        {
            type = RexDataBlockType.Mesh;
            this.name = name;
            this.lod = lod;
            this.maxLod = maxLod;
            this.vertexCoordinates = vertexCoordinates;
            this.normalVectors = normalVectors;
            this.textureCoordinates = textureCoordinates;
            this.vertexColors = vertexColors;
            this.vertexTriangles = vertexTriangles;
            this.materialId = materialId;
        }

        protected override byte[] GetBlockBytes ()
        {
            List<byte> data = new List<byte> ();

            data.AddRange (BitConverter.GetBytes (lod) );
            data.AddRange (BitConverter.GetBytes (maxLod) );

            data.AddRange (BitConverter.GetBytes (vertexCoordinates.Count) );
            data.AddRange (BitConverter.GetBytes (normalVectors.Count) );
            data.AddRange (BitConverter.GetBytes (textureCoordinates.Count) );
            data.AddRange (BitConverter.GetBytes (vertexColors.Count) );
            data.AddRange (BitConverter.GetBytes (vertexTriangles.Length / 3) );

            // calculate block positions in resulting block
            byte[] nameBytes = Encoding.ASCII.GetBytes (name);
            ushort nameLength = (ushort) nameBytes.Length;

            int offset = DataBlockHeaderSize + (2 * sizeof (ushort) ) + (5 * sizeof (int) ) + (5 * sizeof (int) ) + sizeof (ulong) + sizeof (ushort) + SizeNameSize + SizeName;

            data.AddRange (BitConverter.GetBytes (offset) );
            offset += vertexCoordinates.Count * 3 * sizeof (float);

            data.AddRange (BitConverter.GetBytes (offset) );
            offset += normalVectors.Count * 3 * sizeof (float);

            data.AddRange (BitConverter.GetBytes (offset) );
            offset += textureCoordinates.Count * 2 * sizeof (float);

            data.AddRange (BitConverter.GetBytes (offset) );
            offset += vertexColors.Count * 3 * sizeof (float);

            data.AddRange (BitConverter.GetBytes (offset) );
            offset += vertexTriangles.Length * sizeof (uint);

            // metadata
            data.AddRange (BitConverter.GetBytes (materialId) );
            data.AddRange (BitConverter.GetBytes (nameLength) );
            data.AddRange (nameBytes);

            for (int i = nameLength; i < SizeName; i++)
            {
                data.Add (0);
            }

            // add mesh data
            data.AddRange (Utils.BytesFromVector3List (vertexCoordinates) );
            data.AddRange (Utils.BytesFromVector3List (normalVectors) );
            data.AddRange (Utils.BytesFromVector2List (textureCoordinates) );
            data.AddRange (Utils.BytesFromColorList (vertexColors) );
            foreach (var index in Utils.FlipTriangles (vertexTriangles) )
            {
                data.AddRange (BitConverter.GetBytes (index) );
            }

            return data.ToArray ();
        }
        #endregion

        #region READ
        public RexDataMesh (byte[] buffer, int offset) : base (buffer, ref offset)
        {
            lod = BitConverter.ToUInt16 (buffer, offset);
            offset += SizeLod;

            maxLod = BitConverter.ToUInt16 (buffer, offset);
            offset += SizeMaxLod;


            UInt32 nrVertices = BitConverter.ToUInt32 (buffer, offset);
            offset += SizeNrOfVtxCoords;

            UInt32 nrNormals = BitConverter.ToUInt32 (buffer, offset);
            offset += SizeNrOfNorCoords;

            UInt32 nrTextureCoords = BitConverter.ToUInt32 (buffer, offset);
            offset += SizeNrOfTexCoords;

            UInt32 nrVertexColors = BitConverter.ToUInt32 (buffer, offset);
            offset += SizeNrOfVtxColors;

            UInt32 nrTriangles = BitConverter.ToUInt32 (buffer, offset);
            offset += SizeNrTriangles;

            //startVtxCoords
            offset += SizeStartVtxCoords;

            //startNorCoords
            offset += SizeStartNorCoords;

            //startTexCoords
            offset += SizeStartTexCoords;

            //startVtxColors
            offset += SizeStartVtxColors;

            //startTriangles
            offset += SizeStartTriangles;

            materialId = BitConverter.ToUInt64 (buffer, offset);
            offset += SizeMaterialId;

            UInt16 sz = BitConverter.ToUInt16 (buffer, offset);
            offset += SizeNameSize;

            name = Encoding.ASCII.GetString (buffer, offset, sz);
            offset += SizeName;

            offset += FillVertexCoords (buffer, offset, (int) nrVertices);

            offset += FillVertexNormals (buffer, offset, (int) nrNormals);

            offset += FillTextureCoords (buffer, offset, (int) nrTextureCoords);

            offset += FillVertexColors (buffer, offset, (int) nrVertexColors);

            offset += FillVertexTriangles (buffer, offset, (int) nrTriangles);
        }

        private int FillVertexCoords (byte[] buffer, int offset, int nrVertices)
        {
            int sizeOfVerticesBlock = nrVertices * 3 * sizeof (float);
            vertexCoordinates = Utils.Vector3ListFromArray (buffer, offset, sizeOfVerticesBlock);

            return sizeOfVerticesBlock;
        }

        private int FillVertexNormals (byte[] buffer, int offset, int nrNormals)
        {
            int sizeOfNormalsBlock = nrNormals * 3 * sizeof (float);
            normalVectors = Utils.Vector3ListFromArray (buffer, offset, sizeOfNormalsBlock);

            return sizeOfNormalsBlock;
        }

        private int FillTextureCoords (byte[] buffer, int offset, int nrTextureCoords)
        {
            int sizeOfTextureCoordsBlock = nrTextureCoords * 2 * sizeof (float);
            textureCoordinates = Utils.Vector2ListFromArray (buffer, offset, sizeOfTextureCoordsBlock);

            return sizeOfTextureCoordsBlock;
        }

        private int FillVertexColors (byte[] buffer, int offset, int nrColors)
        {
            int sizeOfVertexColorsBlock = nrColors * 3 * sizeof (float);
            vertexColors = Utils.ColorListFromArray (buffer, offset, sizeOfVertexColorsBlock);

            return sizeOfVertexColorsBlock;
        }

        private int FillVertexTriangles (byte[] buffer, int offset, int nrTriangles)
        {
            int sizeOfTrianglessBlock = nrTriangles * 3 * sizeof (UInt32);

            vertexTriangles = new int[nrTriangles * 3];
            Buffer.BlockCopy (buffer, offset, vertexTriangles, 0, sizeOfTrianglessBlock);

            return sizeOfTrianglessBlock;
        }
        #endregion
    }
}

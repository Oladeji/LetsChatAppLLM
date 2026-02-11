using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using System;
using System.Collections.Generic;
using System.Numerics.Tensors;
using System.Text;

namespace VectorSearch
{
    internal class Movie
    {
        const int dimensions= 384;//Embedding Dimensions: Supports flexible dimension from 768 to 256 through Matryoshka

        [VectorStoreKey]
        public int key { get; set; }
        [VectorStoreData]
        public string Title { get; set; } = string.Empty;
        [VectorStoreData]
        public string Description { get; set; }

      //  [VectorStoreData]
        //public List<Movie> Movies { get; set; } = new List<Movie>();

        [VectorStoreVector(    Dimensions: dimensions, DistanceFunction =DistanceFunction.CosineSimilarity)]
        public ReadOnlyMemory<float> Vector { get; set; }
    }
}

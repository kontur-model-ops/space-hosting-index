using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using Vostok.Logging.Abstractions;

namespace SpaceHosting.Index.Benchmarks
{
    public class IndexStoreBuilder
    {
        private readonly ILog log;

        public IndexStoreBuilder(ILog log)
        {
            this.log = log;
        }

        public IIndexStore<int, object, TVector> BuildIndexStore<TVector>(
            string indexAlgorithm,
            IList<TVector> vectors,
            int indexBatchSize = 1000,
            object[]? metadata = null)
            where TVector : IVector
        {
            var indexDataPoints = vectors
                .Select(
                    (v, i) => new IndexDataPointOrTombstone<int, object, TVector>(
                        new IndexDataPoint<int, object, TVector>(
                            Id: i,
                            Vector: v,
                            Data: metadata?[i]))
                )
                .ToArray();

            var indexStore = new IndexStoreFactory<int, object>(log).Create<TVector>(
                indexAlgorithm,
                vectorDimension: vectors.First().Dimension,
                withDataStorage: metadata != null,
                idComparer: EqualityComparer<int>.Default);

            foreach (var batch in indexDataPoints.Batch(indexBatchSize, b => b.ToArray()))
                indexStore.UpdateIndex(batch);

            return indexStore;
        }
    }
}

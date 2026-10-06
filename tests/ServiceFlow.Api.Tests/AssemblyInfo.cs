// Integration tests share one real database.
// Running them in parallel makes two API instances migrate at the same time.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
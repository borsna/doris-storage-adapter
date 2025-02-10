﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DorisStorageAdapter.Services.Implementation.BagIt;

internal interface IBagItElement<T> where T : IBagItElement<T>
{
    static abstract string FileName { get; }

    bool HasValues();

    static abstract Task<T> Parse(Stream stream, CancellationToken cancellationToken);

    byte[] Serialize();
}

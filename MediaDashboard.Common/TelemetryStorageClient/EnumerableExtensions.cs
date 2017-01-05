//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Net;
using Microsoft.WindowsAzure.Storage;

namespace MediaDashboard.Common.TelemetryStorageClient
{
    /// <summary>
    /// Extension methods for the CloudTable class.
    /// </summary>
    internal static class EnumerableExtensions
    {
        /// <summary>
        /// An enumerator for a collection which skips table not found exceptions.
        /// </summary>
        /// <typeparam name="T">The record type.</typeparam>
        /// <param name="collection">The collection to enumerate.</param>
        /// <returns>An enumerable collection.</returns>
        public static IEnumerable<T> SkipTableNotFoundErrors<T>(this IEnumerable<T> collection)
        {
            var e = collection.GetEnumerator();
            while (true)
            {
                try
                {
                    if (!e.MoveNext())
                    {
                        yield break;
                    }
                }
                catch (StorageException se)
                {
                    if (se.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                    {
                        yield break;
                    }
                    else
                    {
                        throw;
                    }
                }

                yield return e.Current;
            }
        }
    }
}
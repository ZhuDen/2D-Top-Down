using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameLibrary.Tools
{
    public static class NetId
    {
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1);
        private static Task<string> uniqueIdTask;

        public static Task<string> Generate()
        {
            return GenerateUniqueIdAsync();
        }

        private static async Task<string> GenerateUniqueIdAsync()
        {
            await semaphore.WaitAsync();
            try
            {
                if (uniqueIdTask == null || uniqueIdTask.IsCompleted)
                {
                    uniqueIdTask = Task.Run(() =>
                    {
                        Guid guid = Guid.NewGuid();
                        return guid.ToString();
                    });
                }

                return await uniqueIdTask;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}

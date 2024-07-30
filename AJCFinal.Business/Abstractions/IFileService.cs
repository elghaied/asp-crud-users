using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJCFinal.Business.Abstractions
{
    public interface IFileService
    {
        Task<byte[]> GetFromAzureAsync(string fileName);

        Task SendToAzureAsync(byte[] data, string fileName, string contentType);

        Task DeleteFromAzureAsync(string fileName);
    }
}

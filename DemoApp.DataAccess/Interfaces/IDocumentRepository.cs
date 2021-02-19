using DemoApp.Core.Entities;
using System;
using System.Threading.Tasks;

namespace DemoApp.Core.Interfaces
{
    public interface IDocumentRepository
    {
        Task<Document> GetDocumentById(int docId);
        Task<int?> UploadDocument(Document dto);
    }
}
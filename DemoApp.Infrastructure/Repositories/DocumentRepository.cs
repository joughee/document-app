using DemoApp.Core.Entities;
using DemoApp.Core.Interfaces;
using DemoApp.Infrastructure.Configuration;
using DemoApp.Infrastructure.DataAccess;
using DemoApp.Infrastructure.DTOs;
//using DemoApp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DemoApp.Infrastructure.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private IDataAccessManager _dataAccess;

        public DocumentRepository(IDataAccessManager dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public async Task<Document> GetDocumentById(int id)
        {
            return await _dataAccess.DownloadDocument(id);
        }
        public async Task<int?> UploadDocument(Document dto)
        {
            var document = new Document
            {
                Name = dto.Name,
                Description = dto.Description,
                Categories = dto.Categories,
                InsertUserId = 21,
                InsertDateTime = DateTime.UtcNow
            };

            return await _dataAccess.UploadDocument(document);
        }
    }
}

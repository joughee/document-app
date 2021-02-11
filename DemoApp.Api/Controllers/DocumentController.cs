using DemoApp.Core.Entities;
using DemoApp.Core.Interfaces;
using DemoApp.Infrastructure.DTOs;
using DemoApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoApp.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private IDocumentRepository _documentRepository;
        public DocumentController(IDocumentRepository docRepo)
        {
            _documentRepository = docRepo;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var doc = await _documentRepository.GetDocumentById(id);

            return Ok(doc);
        }

        [HttpPost]
        public async Task<IActionResult> Post(DocumentDTO document)
        {
            var doc = new Document
            {
                Name = document.Name,
                Description = document.Description,
                Categories = document.Categories
            };
            var docId = await _documentRepository.UploadDocument(doc);

            return Ok(docId);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SignalR.Domain.Models;
using SignalR.Domain.Repositories;

namespace SignalR.Api.Controllers;

[Route("[controller]")]
public class DocumentController(IDocumentRepository documentRepository) : ControllerBase
{
    [HttpGet("open")]
    public ActionResult<IEnumerable<Document>> GetAllOpenItems()
    {
        return Ok(documentRepository.GetAllOpenItems());
    }

    [HttpPut("updateOwner/{id:int}/{ownerName}")]
    public IActionResult UpdateOwner(int id, string ownerName)
    {
        documentRepository.UpdateDocumentOwner(id, ownerName);
        
        return Ok();
    }

    [HttpPut("updateOwner/{id:int}/release")]
    public IActionResult UpdateRelease(int id)
    {
        documentRepository.UpdateDocumentOwner(id, null);
        
        return Ok();
    }

    [HttpPut("complete/{id:int}")]
    public IActionResult MarkAsCompleted(int id)
    {
        documentRepository.MarkAsCompleted(id);

        return Ok();
    }
}
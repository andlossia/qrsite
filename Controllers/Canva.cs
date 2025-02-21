using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CanvaDesignsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllDesigns() => Ok("All Canva Designs");

    [HttpPost]
    public IActionResult CreateDesign([FromBody] CanvaDesign design) => Ok("Design created");

    [HttpGet("{id}")]
    public IActionResult GetDesignById(long id) => Ok($"Design {id}");

    [HttpPut("{id}")]
    public IActionResult UpdateDesign(long id, [FromBody] CanvaDesign design) => Ok($"Design {id} updated");

    [HttpDelete("{id}")]
    public IActionResult DeleteDesign(long id) => Ok($"Design {id} deleted");

    [HttpPost("bulk")]
    public IActionResult BulkCreateDesigns([FromBody] List<CanvaDesign> designs) => Ok("Bulk designs created");

    [HttpDelete("bulk")]
    public IActionResult BulkDeleteDesigns([FromBody] List<long> ids) => Ok("Bulk designs deleted");
}

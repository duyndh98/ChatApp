using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApplication1.Entities;
using WebApplication1.Helpers;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ResourcesController : ControllerBase
    {
        private IResourceService _resourceService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public ResourcesController(
           IResourceService resourceService,
           IMapper mapper,
           IOptions<AppSettings> appSettings)
        {
            _resourceService = resourceService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        // GET: api/Resources
        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public IActionResult GetResources()
        {
            try
            {
                var resources = _resourceService.GetAll();
                var model = _mapper.Map<IList<ResourceBasicViewModel>>(resources);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { resource = ex.Message });
            }
        }

        // GET: api/Resources/5
        [HttpGet("{id}")]
        public IActionResult GetResource(int id)
        {
            try
            {
                var resource = _resourceService.GetById(id);
                var model = _mapper.Map<ResourceFullViewModel>(resource);

                model.Data = Utils.DownloadData(model.Code);

                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { resource = ex.Message });
            }
        }

        // POST: api/Resources
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public IActionResult PostResource([FromBody]ResourceCreationModel model)
        {
            try
            {
                if (model.Data == null || model.Data.Length == 0)
                    throw new Exception("Resource data is required");

                // Map model to entity
                var resource = _mapper.Map<Resource>(model);

                // Create code randomly
                resource.Code = Guid.NewGuid().ToString("B");

                // Create
                var createdResource = _resourceService.Create(resource);

                // Upload data
                Utils.UploadData(model.Data, resource.Code);

                return Ok(createdResource);
            }
            catch (Exception ex)
            {
                return BadRequest(new { resource = ex.Message });
            }
        }

        // DELETE: api/Resources/5
        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{id}")]
        public IActionResult DeleteResource(int id)
        {
            try
            {
                var resource = _resourceService.GetById(id);
                
                _resourceService.Delete(id);
                Utils.DeleteData(resource.Code);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { resource = ex.Message });
            }
        }
    }
}
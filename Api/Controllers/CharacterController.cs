using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Api.Models;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        /*
        private readonly IConfiguration _configuration;

        public CharacterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        */
        private readonly CharacterContext _context;

        public CharacterController(CharacterContext context)
        {
            _context = context;
        }

        //GET all api/character
        [AllowAnonymous]
        [HttpGet]
        public List<Character> GetAll()
        {
            var characters = from c in _context.Character select c;
            characters = characters.OrderBy(c => c.Origin).ThenBy(c => c.Name);
            return characters.ToList();
        }

        //GET api/character/id
        [AllowAnonymous]
        [HttpGet("{id}", Name = "GetCharacter")]
        public IActionResult Get(int? id)
        {
            var character = _context.Character.Find(id);
            if (character == null)
            {
                return NotFound();
            }
            return Ok(character);
        }   

        //POST create new api/character
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] Character character)
        {
            if (character == null)
            {
                return BadRequest();
            }

            _context.Character.Add(character);
            _context.SaveChanges();

            return CreatedAtRoute("GetCharacter", new { id = character.Id }, character);
        }

        //PUT update api/character/id
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Update(int? id, [FromBody] Character character)
        {
            if (character == null || character.Id != id)
            {
                return BadRequest();
            }

            var todo = _context.Character.Find(id);
            if (todo == null)
            {
                return NotFound();
            }
            todo.Id = character.Id;
            todo.Name = character.Name;
            todo.Age = character.Name;
            todo.Gender = character.Gender;
            todo.Race = character.Race;
            todo.Job = character.Job;
            todo.Height = character.Height;
            todo.Weight = character.Weight;
            todo.Origin = character.Origin;
            todo.Description = character.Description;
            todo.Picture = character.Picture;

            _context.Character.Update(todo);
            _context.SaveChanges();
            return NoContent();
        }

        //DELETE api/character by Id
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int? id)
        {
            var todo = _context.Character.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            _context.Character.Remove(todo);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
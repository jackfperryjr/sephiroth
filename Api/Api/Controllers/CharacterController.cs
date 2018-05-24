using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Api.Models;
using Api.Services;

namespace Api.Controllers
{
    public class CharacterController : ApiController
    {
        private CharacterRepository characterRepository;

        public CharacterController()
        {
            this.characterRepository = new CharacterRepository();
        }

        public Character[] Get()
        {
            return characterRepository.GetAllCharacters();
        }
    }
}
using System;  
using Microsoft.AspNetCore.Http;

namespace Api.Models  
{  
    public class Character
    {  
        public int Id { get; set; }  
        public string Name { get; set; }  
        public string Age { get; set; } 
        public string Gender { get; set; }
        public string Race { get; set; }  
        public string Job { get; set; } 
        public string Height { get; set; }     
        public string Weight { get; set; } 
        public string Origin { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
    }  
}
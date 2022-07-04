using System.Collections;
using System.Collections.Generic;
using Castle.DynamicProxy.Generators.Emitters;

namespace MusicHub.Data.Models
{
    public class Producer
    {
        public Producer()
        {
            Albums = new HashSet<Album>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Pseudonym  { get; set; }
        public string PhoneNumber { get; set; }

        public ICollection<Album> Albums { get; set; }
    }
}
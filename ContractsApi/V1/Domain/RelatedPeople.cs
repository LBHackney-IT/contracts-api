using System;

namespace ContractsApi.V1.Domain
{
    public class RelatedPeople
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool? IsResponsible { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
    }
}

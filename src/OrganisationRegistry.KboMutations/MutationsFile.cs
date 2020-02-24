namespace OrganisationRegistry.KboMutations
{
    using System.Collections.Generic;

    public class MutationsFile
    {
        public string FullName { get; set; }
        public IEnumerable<MutationsLine> KboMutations { get; set; }
        public string Name { get; set; }
    }
}

namespace OrganisationRegistry.KboMutations.Ftps
{
    public readonly struct FtpsListItem
    {
        public string Name { get; }
        public string FullName { get; }
        public string Directory { get; }
        public int Size { get; }

        public FtpsListItem(string name, string fullName, string directory, string size)
        {
            Name = name;
            FullName = fullName;
            Directory = directory;
            Size = int.Parse(size);
        }
    }
}

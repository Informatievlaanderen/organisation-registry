namespace OrganisationRegistry.KboMutations.Ftps
{
    using System.Collections.Generic;
    using System.IO;

    public interface IFtpsClient
    {
        IEnumerable<FtpsListItem> GetListing(string sourceDirectory);
        bool Download(Stream stream, string fullName);
        void MoveFile(string sourceFileFullName, string destinationDirectory);
    }
}

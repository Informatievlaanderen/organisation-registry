namespace OrganisationRegistry.KboMutations.Ftps
{
    using System.Collections.Generic;
    using System.IO;

    public interface IFtpsClient
    {
        string GetListing(string sourceDirectory);
        bool Download(Stream stream, string sourceFilePath);
        void MoveFile(string baseUri, string sourceFilePath, string destinationFilePath);
    }
}

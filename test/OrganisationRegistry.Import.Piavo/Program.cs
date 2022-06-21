namespace OrganisationRegistry.Import.Piavo
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Text;
    using Microsoft.Rest;
    using Models;

    public class Program
    {
        private static List<Key> _keys;
        private static List<ContactType> _contactTypes;
        private static List<Building> _buildings;
        private static List<Location> _locations;
        private static List<Person> _people;
        private static List<Function> _functions;
        private static List<FormalFramework> _formalFramework;
        private static List<OrganisationClassificationType> _organisationClassificationTypes;
        private static List<OrganisationClassification> _organisationClassifications;
        private static List<Organisation> _organisations;
        private static List<LabelType> _labels;
        private static List<CapacityType> _capacities;

        private static ContactType _email;
        private static ContactType _mobile;
        private static ContactType _phone;

        public static void Main(string[] args)
        {
        }

        public static void Import(string endpoint, string jwt)
        {
            var baseAddress = new Uri(endpoint);

            var client = new OrganisationRegistryAPI(baseAddress);
            client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            try
            {
                BuildDatabase();
                ImportKeys(client);
                ImportLabels(client);
                ImportFunctions(client);
                ImportFormalFrameworks(client);
                ImportContactTypes(client);
                ImportCapacityTypes(client);
                ImportBuildings(client);
                ImportLocations(client);
                ImportOrganisationClassificationTypes(client);
                ImportOrganisationClassifications(client);

                ImportPeople(client);
                ImportOrganisations(client);
                ImportOrganisationParents(client);
            }
            catch (HttpOperationException httpEx)
            {
                Console.WriteLine(httpEx.ToString());
                Console.WriteLine(httpEx.Response.ReasonPhrase);
                Console.WriteLine(httpEx.Response.StatusCode);
                Console.WriteLine(httpEx.Response.Content);
            }
        }

        private static void ImportOrganisations(IOrganisationRegistryAPI client)
        {
            var sortedOrgs = Sort(_organisations, x => x.ParentOrganisation != null ? new List<Organisation> { x.ParentOrganisation } : new List<Organisation>(), true).ToList();
            var total = sortedOrgs.Count;
            var padLength = total.ToString().Length;

            Console.WriteLine($"[{0.ToString().PadLeft(padLength, '0')}/{total}] Importing Organisations...");
            for (var i = 0; i < sortedOrgs.Count; i++)
            {
                var organisation = sortedOrgs[i];

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] '{organisation.NewId}'...");
                client.OrganisationsPost(new CreateOrganisationRequest
                {
                    Id = organisation.NewId,
                    Name = organisation.Name,
                    OvoNumber = organisation.OvoNumber,
                    ShortName = organisation.ShortName,
                    Description = organisation.Description,
                    ValidFrom = string.IsNullOrWhiteSpace(organisation.StartDate) ? new DateTime?() : DateTime.ParseExact(organisation.StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    ValidTo = string.IsNullOrWhiteSpace(organisation.EndDate) ? new DateTime?() : DateTime.ParseExact(organisation.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                }).CheckBadRequest();

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] Keys...");
                foreach (var key in organisation.OrganisationKeys)
                {
                    Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] Key [{key.Key.Name}] = [{key.KeyValue}]...");
                    client.OrganisationsByOrganisationIdKeysPost(organisation.NewId, new AddOrganisationKeyRequest
                    {
                        OrganisationKeyId = key.NewId,
                        KeyTypeId = key.Key.NewId,
                        KeyValue = key.KeyValue,
                        ValidFrom = string.IsNullOrWhiteSpace(key.StartDate) ? new DateTime?() : DateTime.ParseExact(key.StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        ValidTo = string.IsNullOrWhiteSpace(key.EndDate) ? new DateTime?() : DateTime.ParseExact(key.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    }).CheckBadRequest();
                }

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] Labels...");
                foreach (var label in organisation.OrganisationLabels)
                {
                    Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] Label [{label.Type.Name}] = [{label.Label}]...");
                    client.OrganisationsByOrganisationIdLabelsPost(organisation.NewId, new AddOrganisationLabelRequest
                    {
                        OrganisationLabelId = label.NewId,
                        LabelTypeId = label.Type.NewId,
                        LabelValue = label.Label,
                        ValidFrom = string.IsNullOrWhiteSpace(label.StartDate) ? new DateTime?() : DateTime.ParseExact(label.StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        ValidTo = string.IsNullOrWhiteSpace(label.EndDate) ? new DateTime?() : DateTime.ParseExact(label.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    }).CheckBadRequest();
                }

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] Classifications...");
                foreach (var classification in organisation.OrganisationClassifications)
                {
                    Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] Classification [{classification.OrganisationClassification.OrganisationClassificationType.Name}] = [{classification.OrganisationClassification.Name}]...");
                    client.OrganisationsByOrganisationIdClassificationsPost(organisation.NewId, new AddOrganisationOrganisationClassificationRequest
                    {
                        OrganisationOrganisationClassificationId = classification.NewId,
                        OrganisationClassificationTypeId = classification.OrganisationClassification.OrganisationClassificationType.NewId,
                        OrganisationClassificationId = classification.OrganisationClassification.NewId,
                        ValidFrom = string.IsNullOrWhiteSpace(classification.StartDate) ? new DateTime?() : DateTime.ParseExact(classification.StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        ValidTo = string.IsNullOrWhiteSpace(classification.EndDate) ? new DateTime?() : DateTime.ParseExact(classification.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    }).CheckBadRequest();
                }

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] Contacts...");
                foreach (var contact in organisation.OrganisationContacts)
                {
                    Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] Contact [{contact.ContactType.Name.UpperCaseFirstLetter()}] = [{contact.Contact}]...");
                    client.OrganisationsByOrganisationIdContactsPost(organisation.NewId, new AddOrganisationContactRequest
                    {
                        OrganisationContactId = contact.NewId,
                        ContactTypeId = contact.ContactType.NewId,
                        ContactValue = contact.Contact,
                    }).CheckBadRequest();
                }

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] Capacities...");
                foreach (var capacity in organisation.OrganisationCapacities)
                {
                    var contacts = BuildCapacityContacts(capacity);
                    Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] Capacity [{capacity.CapacityType.Name.UpperCaseFirstLetter()}] = [{capacity.Person.FirstName} {capacity.Person.Name}] with {contacts.Count} contacts...");
                    client.OrganisationsByOrganisationIdCapacitiesPost(organisation.NewId, new AddOrganisationCapacityRequest
                    {
                        OrganisationCapacityId = capacity.NewId,
                        CapacityId = capacity.CapacityType.NewId,
                        PersonId = capacity.Person.NewId,
                        Contacts = contacts,
                        ValidFrom = string.IsNullOrWhiteSpace(capacity.StartDate) ? new DateTime?() : DateTime.ParseExact(capacity.StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        ValidTo = string.IsNullOrWhiteSpace(capacity.EndDate) ? new DateTime?() : DateTime.ParseExact(capacity.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    }).CheckBadRequest();
                }

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] Buildings...");
                foreach (var building in organisation.OrganisationBuildings)
                {
                    Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] Building [{building.Building.Name}]...");
                    client.OrganisationsByOrganisationIdBuildingsPost(organisation.NewId, new AddOrganisationBuildingRequest
                    {
                        OrganisationBuildingId = building.NewId,
                        BuildingId = building.Building.NewId,
                        IsMainBuilding = building.IsMainBuilding == "on",
                        ValidFrom = string.IsNullOrWhiteSpace(building.StartDate) ? new DateTime?() : DateTime.ParseExact(building.StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        ValidTo = string.IsNullOrWhiteSpace(building.EndDate) ? new DateTime?() : DateTime.ParseExact(building.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    }).CheckBadRequest();
                }

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] Locations...");
                foreach (var location in organisation.OrganisationLocations)
                {
                    Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] Location [{location.Location.Street} {location.Location.Number}, {location.Location.PostalCode} {location.Location.PostalCode} {location.Location.Country}]...");
                    client.OrganisationsByOrganisationIdLocationsPost(organisation.NewId, new AddOrganisationLocationRequest
                    {
                        OrganisationLocationId = location.NewId,
                        LocationId = location.Location.NewId,
                        IsMainLocation = location.IsMainLocation == "1",
                        ValidFrom = string.IsNullOrWhiteSpace(location.StartDate) ? new DateTime?() : DateTime.ParseExact(location.StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        ValidTo = string.IsNullOrWhiteSpace(location.EndDate) ? new DateTime?() : DateTime.ParseExact(location.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    }).CheckBadRequest();
                }

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] Functions...");
                foreach (var function in organisation.OrganisationFunctions)
                {
                    Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] Function [{function.Function.Name.UpperCaseFirstLetter()}] = [{function.Person.FirstName} {function.Person.Name}]...");
                    client.OrganisationsByOrganisationIdFunctionsPost(organisation.NewId, new AddOrganisationFunctionRequest
                    {
                        OrganisationFunctionId = function.NewId,
                        FunctionId = function.Function.NewId,
                        PersonId = function.Person.NewId,
                        ValidFrom = string.IsNullOrWhiteSpace(function.StartDate) ? new DateTime?() : DateTime.ParseExact(function.StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        ValidTo = string.IsNullOrWhiteSpace(function.EndDate) ? new DateTime?() : DateTime.ParseExact(function.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    }).CheckBadRequest();
                }

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] Formal Frameworks...");
                foreach (var formalFramework in organisation.OrganisationFormalFrameworks)
                {
                    if (formalFramework.TargetOrganisationId < 0)
                        continue;

                    Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] Formal Framework [{formalFramework.FormalFramework.Name}] = [{formalFramework.Organisation.Name}]...");
                    client.OrganisationsByOrganisationIdFormalframeworksPost(organisation.NewId, new AddOrganisationFormalFrameworkRequest
                    {
                        OrganisationFormalFrameworkId = formalFramework.NewId,
                        FormalFrameworkId = formalFramework.FormalFramework.NewId,
                        ParentOrganisationId = formalFramework.Organisation.NewId,
                        ValidFrom = string.IsNullOrWhiteSpace(formalFramework.StartDate) ? new DateTime?() : DateTime.ParseExact(formalFramework.StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        ValidTo = string.IsNullOrWhiteSpace(formalFramework.EndDate) ? new DateTime?() : DateTime.ParseExact(formalFramework.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    }).CheckBadRequest();
                }
            }
            Console.WriteLine();
        }

        private static IDictionary<string, string> BuildCapacityContacts(OrganisationCapacity capacity)
        {
            var contacts = new Dictionary<string, string>();

            if (_email != null && !string.IsNullOrWhiteSpace(capacity.Email))
                contacts.Add(_email.NewId.ToString(), capacity.Email);

            if (_mobile != null && !string.IsNullOrWhiteSpace(capacity.Mobile))
                contacts.Add(_mobile.NewId.ToString(), capacity.Mobile);

            if (_phone != null && !string.IsNullOrWhiteSpace(capacity.Phone))
                contacts.Add(_phone.NewId.ToString(), capacity.Phone);

            return contacts;
        }

        private static void ImportOrganisationParents(IOrganisationRegistryAPI client)
        {
            var organisationsWithParents = _organisations.Where(x => x.ParentOrganisation != null).ToList();
            var total = organisationsWithParents.Count;
            var padLength = total.ToString().Length;

            Console.WriteLine($"[{0.ToString().PadLeft(padLength, '0')}/{total}] Importing Organisation Parents...");
            for (var i = 0; i < organisationsWithParents.Count; i++)
            {
                var organisation = organisationsWithParents[i];

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisation.Name}] Parents...");
                client.OrganisationsByOrganisationIdParentsPost(organisation.NewId, new AddOrganisationParentRequest
                {
                    OrganisationOrganisationParentId = Guid.NewGuid(),
                    ParentOrganisationId = organisation.ParentOrganisation.NewId,
                    ValidFrom = string.IsNullOrWhiteSpace(organisation.ParentOrganisationStartDate) ? new DateTime?() : DateTime.ParseExact(organisation.ParentOrganisationStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    ValidTo = string.IsNullOrWhiteSpace(organisation.ParentOrganisationEndDate) ? new DateTime?() : DateTime.ParseExact(organisation.ParentOrganisationEndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                }).CheckBadRequest();
            }

            Console.WriteLine();
        }

        private static void ImportOrganisationClassifications(IOrganisationRegistryAPI client)
        {
            var total = _organisationClassifications.Count;
            var padLength = total.ToString().Length;

            Console.WriteLine($"[{0.ToString().PadLeft(padLength, '0')}/{total}] Importing OrganisationClassifications...");
            for (var i = 0; i < _organisationClassifications.Count; i++)
            {
                var organisationClassification = _organisationClassifications[i];

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisationClassification.Name}]...");
                client.OrganisationclassificationsPost(new CreateOrganisationClassificationRequest
                {
                    Id = organisationClassification.NewId,
                    Name = organisationClassification.Name,
                    Order = organisationClassification.Order,
                    Active = organisationClassification.Active.ToLowerInvariant().Trim() == "on",
                    OrganisationClassificationTypeId = organisationClassification.OrganisationClassificationType.NewId,
                }).CheckBadRequest();
            }
            Console.WriteLine();
        }

        private static void ImportOrganisationClassificationTypes(IOrganisationRegistryAPI client)
        {
            var total = _organisationClassificationTypes.Count;
            var padLength = total.ToString().Length;

            Console.WriteLine($"[{0.ToString().PadLeft(padLength, '0')}/{total}] Importing OrganisationClassificationTypes...");
            for (var i = 0; i < _organisationClassificationTypes.Count; i++)
            {
                var organisationClassificationType = _organisationClassificationTypes[i];

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{organisationClassificationType.Name}]...");
                client.OrganisationclassificationtypesPost(new CreateOrganisationClassificationTypeRequest
                {
                    Id = organisationClassificationType.NewId,
                    Name = organisationClassificationType.Name,
                }).CheckBadRequest();
            }
            Console.WriteLine();
        }

        private static void ImportFunctions(IOrganisationRegistryAPI client)
        {
            var total = _functions.Count;
            var padLength = total.ToString().Length;

            Console.WriteLine($"[{0.ToString().PadLeft(padLength, '0')}/{total}] Importing Functions...");
            for (var i = 0; i < _functions.Count; i++)
            {
                var function = _functions[i];

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{function.Name.UpperCaseFirstLetter()}]...");
                client.FunctiontypesPost(new CreateFunctionTypeRequest()
                {
                    Id = function.NewId,
                    Name = function.Name.UpperCaseFirstLetter(),
                }).CheckBadRequest();
            }
            Console.WriteLine();
        }

        private static void ImportPeople(IOrganisationRegistryAPI client)
        {
            var total = _people.Count;
            var padLength = total.ToString().Length;

            Console.WriteLine($"[{0.ToString().PadLeft(padLength, '0')}/{total}] Importing People...");
            for (var i = 0; i < _people.Count; i++)
            {
                var person = _people[i];

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{person.Name}]...");

                var sex = string.Empty;
                if (person.Sex == "V") sex = "Female";
                if (person.Sex == "M") sex = "Male";

                client.PeoplePost(new CreatePersonRequest
                {
                    Id = person.NewId,
                    FirstName = person.FirstName,
                    Name = person.Name,
                    Sex = sex,
                    DateOfBirth =
                        string.IsNullOrWhiteSpace(person.BirthDate)
                            ? new DateTime?()
                            : DateTime.ParseExact(person.BirthDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                }).CheckBadRequest();
            }
            Console.WriteLine();
        }

        private static void ImportLocations(IOrganisationRegistryAPI client)
        {
            var total = _locations.Count;
            var padLength = total.ToString().Length;

            Console.WriteLine($"[{0.ToString().PadLeft(padLength, '0')}/{total}] Importing Locations...");
            for (var i = 0; i < _locations.Count; i++)
            {
                var location = _locations[i];

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{location.Street} {location.Number}, {location.PostalCode} {location.City} {location.Country}]...");
                client.LocationsPost(new CreateLocationRequest
                {
                    Id = location.NewId,
                    Street = $"{location.Street} {location.Number}",
                    ZipCode = location.PostalCode,
                    City = location.City,
                    Country = location.Country,
                }).CheckBadRequest();
            }
            Console.WriteLine();
        }

        private static void ImportBuildings(IOrganisationRegistryAPI client)
        {
            var total = _buildings.Count;
            var padLength = total.ToString().Length;

            Console.WriteLine($"[{0.ToString().PadLeft(padLength, '0')}/{total}] Importing Buildings...");
            for (var i = 0; i < _buildings.Count; i++)
            {
                var building = _buildings[i];

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{building.Name}]...");
                client.BuildingsPost(new CreateBuildingRequest
                {
                    Id = building.NewId,
                    Name = building.Name,
                    VimId = building.Id,
                }).CheckBadRequest();
            }
            Console.WriteLine();
        }

        private static void ImportKeys(IOrganisationRegistryAPI client)
        {
            var total = _keys.Count;
            var padLength = total.ToString().Length;

            Console.WriteLine($"[{0.ToString().PadLeft(padLength, '0')}/{total}] Importing Keys...");
            for (var i = 0; i < _keys.Count; i++)
            {
                var key = _keys[i];

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{key.Name}]...");
                client.KeytypesPost(new CreateKeyTypeRequest
                {
                    Id = key.NewId,
                    Name = key.Name,
                }).CheckBadRequest();
            }
            Console.WriteLine();
        }

        private static void ImportLabels(IOrganisationRegistryAPI client)
        {
            var total = _labels.Count;
            var padLength = total.ToString().Length;

            Console.WriteLine($"[{0.ToString().PadLeft(padLength, '0')}/{total}] Importing Labels...");
            for (var i = 0; i < _labels.Count; i++)
            {
                var label = _labels[i];

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{label.Name}]...");
                client.LabeltypesPost(new CreateLabelTypeRequest
                {
                    Id = label.NewId,
                    Name = label.Name,
                }).CheckBadRequest();
            }
            Console.WriteLine();
        }

        private static void ImportFormalFrameworks(IOrganisationRegistryAPI client)
        {
            var total = _formalFramework.Count;
            var padLength = total.ToString().Length;

            Console.WriteLine($"[{0.ToString().PadLeft(padLength, '0')}/{total}] Importing FormalFrameworks...");

            var formalFrameworkCategoryId = Guid.NewGuid();
            client.FormalframeworkcategoriesPost(new CreateFormalFrameworkCategoryRequest
            {
                Id = formalFrameworkCategoryId,
                Name = "Organisatie",
            }).CheckBadRequest();

            for (var i = 0; i < _formalFramework.Count; i++)
            {
                var formalFramework = _formalFramework[i];

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{formalFramework.Name}]...");
                client.FormalframeworksPost(new CreateFormalFrameworkRequest
                {
                    Id = formalFramework.NewId,
                    Code = formalFramework.Code,
                    Name = formalFramework.Name,
                    FormalFrameworkCategoryId = formalFrameworkCategoryId,
                }).CheckBadRequest();
            }
            Console.WriteLine();
        }

        private static void ImportContactTypes(IOrganisationRegistryAPI client)
        {
            var total = _contactTypes.Count;
            var padLength = total.ToString().Length;

            Console.WriteLine($"[{0.ToString().PadLeft(padLength, '0')}/{total}] Importing Contact Types...");
            for (var i = 0; i < _contactTypes.Count; i++)
            {
                var contactType = _contactTypes[i];

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{contactType.Name.UpperCaseFirstLetter()}]...");
                client.ContacttypesPost(new CreateContactTypeRequest
                {
                    Id = contactType.NewId,
                    Name = contactType.Name.UpperCaseFirstLetter(),
                }).CheckBadRequest();
            }
            Console.WriteLine();
        }

        private static void ImportCapacityTypes(IOrganisationRegistryAPI client)
        {
            var total = _capacities.Count;
            var padLength = total.ToString().Length;

            Console.WriteLine($"[{0.ToString().PadLeft(padLength, '0')}/{total}] Importing Capacity Types...");
            for (var i = 0; i < _capacities.Count; i++)
            {
                var capacity = _capacities[i];

                Console.WriteLine($"[{(i + 1).ToString().PadLeft(padLength, '0')}/{total}] Importing [{capacity.Name.UpperCaseFirstLetter()}]...");
                client.CapacitiesPost(new CreateCapacityRequest
                {
                    Id = capacity.NewId,
                    Name = capacity.Name.UpperCaseFirstLetter(),
                }).CheckBadRequest();
            }
            Console.WriteLine();
        }

        private static void BuildDatabase()
        {
            using (var f = File.OpenRead(Path.Combine("ImportFiles", "Keys.txt")))
                _keys = ReadCsv<Key>(f, true, "|");

            using (var f = File.OpenRead(Path.Combine("ImportFiles", "ContactpointType.txt")))
                _contactTypes = ReadCsv<ContactType>(f, true, "|");

            _email = _contactTypes.FirstOrDefault(x => x.Name == "email");
            _phone = _contactTypes.FirstOrDefault(x => x.Name == "telefoon");
            _mobile = _contactTypes.FirstOrDefault(x => x.Name == "mobilephone");

            using (var f = File.OpenRead(Path.Combine("ImportFiles", "Buildings.txt")))
                _buildings = ReadCsv<Building>(f, true, "|");

            using (var f = File.OpenRead(Path.Combine("ImportFiles", "location.txt")))
                _locations = ReadCsv<Location>(f, true, "|");

            using (var f = File.OpenRead(Path.Combine("ImportFiles", "function.txt")))
                _functions = ReadCsv<Function>(f, true, "|");

            using (var f = File.OpenRead(Path.Combine("ImportFiles", "formalFramework.txt")))
                _formalFramework = ReadCsv<FormalFramework>(f, true, "|");

            using (var f = File.OpenRead(Path.Combine("ImportFiles", "person.txt")))
                _people = ReadCsv<Person>(f, true, "|");

            var people = _people.ToDictionary(x => x.Id, x => x);

            using (var f = File.OpenRead(Path.Combine("ImportFiles", "classificationtypes.txt")))
                _organisationClassificationTypes = ReadCsv<OrganisationClassificationType>(f, true, "|");

            using (var f = File.OpenRead(Path.Combine("ImportFiles", "classifications.txt")))
                _organisationClassifications = ReadCsv<OrganisationClassification>(f, true, "|");

            foreach (var organisationClassification in _organisationClassifications)
                organisationClassification.OrganisationClassificationType =
                    _organisationClassificationTypes.FirstOrDefault(x =>
                        x.Id == organisationClassification.OrganisationClassificationTypeId);

            using (var f = File.OpenRead(Path.Combine("ImportFiles", "Organisation.txt")))
                _organisations = ReadCsv<Organisation>(f, true, "|");

            var organisations = _organisations.ToDictionary(x => x.Id, x => x);

            List<OrganisationLabel> organisationLabels;
            using (var f = File.OpenRead(Path.Combine("ImportFiles", "altLabels.txt")))
                organisationLabels = ReadCsv<OrganisationLabel>(f, true, "|");

            _labels = organisationLabels.Select(x => x.LabelType).Distinct().Select(x => new LabelType { Name = x })
                .ToList();
            var labels = _labels.ToDictionary(x => x.Name, x => x);
            foreach (var organisationLabel in organisationLabels)
                organisationLabel.Type = labels[organisationLabel.LabelType];

            List<OrganisationFunction> organisationFunctions;
            using (var f = File.OpenRead(Path.Combine("ImportFiles", "personfunction.txt")))
                organisationFunctions = ReadCsv<OrganisationFunction>(f, true, "|");

            var functions = _functions.ToDictionary(x => x.Id, x => x);
            foreach (var organisationFunction in organisationFunctions)
            {
                organisationFunction.Function = functions[organisationFunction.FunctionId];
                organisationFunction.Person = people[organisationFunction.PersonId];
            }

            List<OrganisationCapacity> organisationCapacities;
            using (var f = File.OpenRead(Path.Combine("ImportFiles", "capacities.txt")))
                organisationCapacities = ReadCsv<OrganisationCapacity>(f, true, "|");

            _capacities = organisationCapacities.Select(x => x.Type).Distinct().Select(x => new CapacityType { Name = x })
                .ToList();
            var capacities = _capacities.ToDictionary(x => x.Name, x => x);
            foreach (var organisationCapacity in organisationCapacities)
            {
                organisationCapacity.CapacityType = capacities[organisationCapacity.Type];
                organisationCapacity.Person = people[organisationCapacity.PersonId];
            }

            List<OrganisationFormalFramework> organisationFormalFrameworks;
            using (var f = File.OpenRead(Path.Combine("ImportFiles", "relatedOrganization.txt")))
                organisationFormalFrameworks = ReadCsv<OrganisationFormalFramework>(f, true, "|");

            var formalFrameworks = _formalFramework.ToDictionary(x => x.Id, x => x);
            foreach (var organisationFormalFramework in organisationFormalFrameworks)
            {
                organisationFormalFramework.FormalFramework =
                    formalFrameworks[organisationFormalFramework.FormalFrameworkId];

                //if (organisationFormalFramework.TargetOrganisationId > 0)
                organisationFormalFramework.Organisation =
                    organisations[organisationFormalFramework.TargetOrganisationId];
            }

            List<OrganisationOrganisationClassification> organisationOrganisationClassifications;
            using (var f = File.OpenRead(Path.Combine("ImportFiles", "ORGclassifications.txt")))
                organisationOrganisationClassifications = ReadCsv<OrganisationOrganisationClassification>(f, true, "|");

            var organisationClassifications = _organisationClassifications.ToDictionary(x => x.Id, x => x);
            foreach (var organisationOrganisationClassification in organisationOrganisationClassifications)
                organisationOrganisationClassification.OrganisationClassification =
                    organisationClassifications[organisationOrganisationClassification.ClassificationId];

            List<OrganisationBuilding> organisationBuildings;
            using (var f = File.OpenRead(Path.Combine("ImportFiles", "organizationBuilding.txt")
        ))

                organisationBuildings = ReadCsv<OrganisationBuilding>(f, true, "|");

            var buildings = _buildings.ToDictionary(x => x.Id, x => x);
            foreach (var organisationBuilding in organisationBuildings)
                organisationBuilding.Building = buildings[organisationBuilding.BuildingId];

            List<OrganisationKey> organisationKeys;
            using (var f = File.OpenRead(Path.Combine("ImportFiles", "orgKeys.txt")))
                organisationKeys = ReadCsv<OrganisationKey>(f, true, "|");//.Distinct(new OrganisationKeyComparer()).ToList();

            var keys = _keys.ToDictionary(x => x.Id, x => x);
            foreach (var organisationKey in organisationKeys)
                organisationKey.Key = keys[organisationKey.KeyTypeId];

            List<OrganisationLocation> organisationLocations;
            using (var f = File.OpenRead(Path.Combine("ImportFiles", "OrgLocation.txt")))
                organisationLocations = ReadCsv<OrganisationLocation>(f, true, "|");

            var locations = _locations.ToDictionary(x => x.Id, x => x);
            foreach (var organisationLocation in organisationLocations)
                organisationLocation.Location = locations[organisationLocation.LocationId];

            List<OrganisationContact> organisationContacts;
            using (var f = File.OpenRead(Path.Combine("ImportFiles", "Contactpoint.txt")))
                organisationContacts = ReadCsv<OrganisationContact>(f, true, "|").OrderBy(x => x.Id).ToList();

            var contactTypes = _contactTypes.ToDictionary(x => x.Id, x => x);
            foreach (var organisationContact in organisationContacts)
                organisationContact.ContactType = contactTypes[organisationContact.Type];

            foreach (var organisation in _organisations)
            {
                if (organisation.ParentOrganisationId.HasValue && organisation.ParentOrganisationId.Value > 0 && organisations.ContainsKey(organisation.ParentOrganisationId.Value))
                    organisation.ParentOrganisation = organisations[organisation.ParentOrganisationId.Value];
            }

            var orgsByOvo = _organisations.ToDictionary(x => x.OvoNumber, x => x);
            foreach (var organisationKey in organisationKeys)
                orgsByOvo[organisationKey.OvoNumber].OrganisationKeys.Add(organisationKey);

            foreach (var organisationLabel in organisationLabels)
                orgsByOvo[organisationLabel.OvoNumber].OrganisationLabels.Add(organisationLabel);

            foreach (var organisationBuilding in organisationBuildings)
                organisations[organisationBuilding.OrganisationId].OrganisationBuildings.Add(organisationBuilding);

            foreach (var organisationLocation in organisationLocations)
                orgsByOvo[organisationLocation.OvoNumber].OrganisationLocations.Add(organisationLocation);

            foreach (var organisationOrganisationClassification in organisationOrganisationClassifications)
                orgsByOvo[organisationOrganisationClassification.OvoNumber].OrganisationClassifications.Add(organisationOrganisationClassification);

            foreach (var organisationContact in organisationContacts)
                organisations[organisationContact.OrganisationId].OrganisationContacts.Add(organisationContact);

            foreach (var organisationFunction in organisationFunctions)
                organisations[organisationFunction.OrganisationId].OrganisationFunctions.Add(organisationFunction);

            foreach (var organisationCapacity in organisationCapacities)
                organisations[organisationCapacity.OrganisationId].OrganisationCapacities.Add(organisationCapacity);

            foreach (var organisationFormalFramework in organisationFormalFrameworks)
                organisations[organisationFormalFramework.SourceOrganisationId].OrganisationFormalFrameworks.Add(organisationFormalFramework);
        }

        private static List<T> ReadCsv<T>(Stream stream, bool useSingleLineHeaderInCsv, string csvDelimiter)
        {
            var items = new List<T>();

            var reader = new StreamReader(stream, Encoding.UTF8, true);

            var skipFirstLine = useSingleLineHeaderInCsv;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(csvDelimiter.ToCharArray());
                if (skipFirstLine)
                {
                    skipFirstLine = false;
                }
                else
                {
                    var item = Activator.CreateInstance(typeof(T));
                    var properties = item.GetType().GetProperties();
                    for (var i = 0; i < values.Length; i++)
                    {
                        var value = values[i];

                        value = value == "--" ? string.Empty : value;
                        value = value == "NULL" ? string.Empty : value;
                        value = value == "null" ? string.Empty : value;
                        value = value == "9999-12-99" ? string.Empty : value;
                        value = value == "9999-12-31" ? string.Empty : value;
                        //value = value == "1970-01-01" ? string.Empty : value;

                        properties[i].SetValue(item, StringToType(value, properties[i].PropertyType), null);
                    }

                    items.Add((T)item);
                }
            }

            return items;
        }

        private static object StringToType(string value, Type propertyType)
        {
            var underlyingType = Nullable.GetUnderlyingType(propertyType);
            if (underlyingType != null && string.IsNullOrEmpty(value))
            {
                // an underlying nullable type, so the type is nullable, apply logic for null or empty test
                return null;
            }

            return Convert.ChangeType(value, underlyingType ?? propertyType, CultureInfo.InvariantCulture);
        }

        private static IEnumerable<T> Sort<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> dependencies, bool throwOnCycle = false)
        {
            var sorted = new List<T>();
            var visited = new HashSet<T>();

            foreach (var item in source)
                Visit(item, visited, sorted, dependencies, throwOnCycle);

            return sorted;
        }

        private static void Visit<T>(T item, HashSet<T> visited, List<T> sorted, Func<T, IEnumerable<T>> dependencies, bool throwOnCycle)
        {
            if (!visited.Contains(item))
            {
                visited.Add(item);

                foreach (var dep in dependencies(item))
                    Visit(dep, visited, sorted, dependencies, throwOnCycle);

                sorted.Add(item);
            }
            else
            {
                if (throwOnCycle && !sorted.Contains(item))
                    throw new Exception("Cyclic dependency found");
            }
        }
    }

    public class Key
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Guid NewId { get; set; }
        public Key() { NewId = Guid.NewGuid(); }
    }

    public class Building
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Guid NewId { get; set; }
        public Building() { NewId = Guid.NewGuid(); }
    }

    public class Location
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string IsoCountryCode { get; set; }

        public Guid NewId { get; set; }
        public Location() { NewId = Guid.NewGuid(); }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string Sex { get; set; }
        public string BirthDate { get; set; }
        public string SaluationId { get; set; }
        public string Salutation { get; set; }

        public Guid NewId { get; set; }
        public Person() { NewId = Guid.NewGuid(); }
    }

    public class OrganisationClassificationType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Guid NewId { get; set; }
        public OrganisationClassificationType() { NewId = Guid.NewGuid(); }
    }

    public class OrganisationClassification
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrganisationClassificationTypeId { get; set; }
        public string OrganisationClassificationTypeName { get; set; }
        public string Active { get; set; }
        public int Order { get; set; }
        public int Something { get; set; }

        public OrganisationClassificationType OrganisationClassificationType { get; set; }

        public Guid NewId { get; set; }
        public OrganisationClassification() { NewId = Guid.NewGuid(); }
    }

    public class LabelType
    {
        public string Name { get; set; }

        public Guid NewId { get; set; }
        public LabelType() { NewId = Guid.NewGuid(); }
    }

    public class FormalFramework
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public Guid NewId { get; set; }
        public FormalFramework() { NewId = Guid.NewGuid(); }
    }

    public class Function
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Guid NewId { get; set; }
        public Function() { NewId = Guid.NewGuid(); }
    }

    public class ContactType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Guid NewId { get; set; }
        public ContactType() { NewId = Guid.NewGuid(); }
    }

    public class CapacityType
    {
        public string Name { get; set; }

        public Guid NewId { get; set; }
        public CapacityType() { NewId = Guid.NewGuid(); }
    }

    public class OrganisationLabel
    {
        public int Id { get; set; }
        public string OvoNumber { get; set; }
        public string LabelType { get; set; }
        public string Label { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public LabelType Type { get; set; }

        public Guid NewId { get; set; }
        public OrganisationLabel() { NewId = Guid.NewGuid(); }
    }

    public class OrganisationContact
    {
        public int Id { get; set; }
        public int OrganisationId { get; set; }
        public string Contact { get; set; }
        public int Type { get; set; }

        public ContactType ContactType { get; set; }

        public Guid NewId { get; set; }
        public OrganisationContact() { NewId = Guid.NewGuid(); }
    }

    public class OrganisationFunction
    {
        public int PersonId { get; set; }
        public int FunctionId { get; set; }
        public int OrganisationId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public Function Function { get; set; }
        public Person Person { get; set; }

        public Guid NewId { get; set; }
        public OrganisationFunction() { NewId = Guid.NewGuid(); }
    }

    public class OrganisationCapacity
    {
        public int PersonId { get; set; }
        public int OrganisationId { get; set; }
        public string Type { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }

        public CapacityType CapacityType { get; set; }
        public Person Person { get; set; }

        public Guid NewId { get; set; }
        public OrganisationCapacity() { NewId = Guid.NewGuid(); }
    }

    public class Organisation
    {
        public int Id { get; set; }
        public string OvoNumber { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string OrgStatus { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string LastUpdatedDate { get; set; }
        public string Logo { get; set; }
        public int? ParentOrganisationId { get; set; }
        public string ParentOrganisationStartDate { get; set; }
        public string ParentOrganisationEndDate { get; set; }

        public Organisation ParentOrganisation { get; set; }
        public List<OrganisationKey> OrganisationKeys { get; set; }
        public List<OrganisationLabel> OrganisationLabels { get; set; }
        public List<OrganisationOrganisationClassification> OrganisationClassifications { get; set; }
        public List<OrganisationContact> OrganisationContacts { get; set; }
        public List<OrganisationCapacity> OrganisationCapacities { get; set; }
        public List<OrganisationBuilding> OrganisationBuildings { get; set; }
        public List<OrganisationLocation> OrganisationLocations { get; set; }
        public List<OrganisationFunction> OrganisationFunctions { get; set; }
        public List<OrganisationFormalFramework> OrganisationFormalFrameworks { get; set; }

        public Guid NewId { get; set; }

        public Organisation()
        {
            NewId = Guid.NewGuid();
            OrganisationKeys = new List<OrganisationKey>();
            OrganisationLabels = new List<OrganisationLabel>();
            OrganisationClassifications = new List<OrganisationOrganisationClassification>();
            OrganisationContacts = new List<OrganisationContact>();
            OrganisationCapacities = new List<OrganisationCapacity>();
            OrganisationBuildings = new List<OrganisationBuilding>();
            OrganisationLocations = new List<OrganisationLocation>();
            OrganisationFunctions = new List<OrganisationFunction>();
            OrganisationFormalFrameworks = new List<OrganisationFormalFramework>();
        }
    }

    public class OrganisationFormalFramework
    {
        public string RelationType { get; set; }
        public int SourceOrganisationId { get; set; }
        public int TargetOrganisationId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int FormalFrameworkId { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }

        public FormalFramework FormalFramework { get; set; }
        public Organisation Organisation { get; set; }

        public Guid NewId { get; set; }
        public OrganisationFormalFramework() { NewId = Guid.NewGuid(); }
    }

    public class OrganisationOrganisationClassification
    {
        public int Id { get; set; }
        public string OvoNumber { get; set; }
        public int ClassificationId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public OrganisationClassification OrganisationClassification { get; set; }

        public Guid NewId { get; set; }
        public OrganisationOrganisationClassification() { NewId = Guid.NewGuid(); }
    }

    public class OrganisationBuilding
    {
        public int OrganisationId { get; set; }
        public int BuildingId { get; set; }
        public string IsMainBuilding { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public Building Building { get; set; }

        public Guid NewId { get; set; }
        public OrganisationBuilding() { NewId = Guid.NewGuid(); }
    }

    public class OrganisationLocation
    {
        public string OvoNumber { get; set; }
        public int LocationId { get; set; }
        public string IsMainLocation { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public Location Location { get; set; }

        public Guid NewId { get; set; }
        public OrganisationLocation() { NewId = Guid.NewGuid(); }
    }

    public class OrganisationKey
    {
        public string OvoNumber { get; set; }
        public int KeyTypeId { get; set; }
        public string KeyValue { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public Key Key { get; set; }

        public Guid NewId { get; set; }
        public OrganisationKey() { NewId = Guid.NewGuid(); }
    }

    public static class StringHelper
    {
        public static string UpperCaseFirstLetter(this string str)
        {
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }
    }

    public static class ResultHelper
    {
        public static void CheckBadRequest(this object response)
        {
            if (response is BadRequestResult)
                throw new Exception($"Bad request, import data is crap!");
        }
    }
}

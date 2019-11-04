namespace OrganisationRegistry.Body
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class BodyMandates : IEnumerable<BodyMandate>
    {
        private readonly List<BodyMandate> _bodyMandates;

        public BodyMandates()
        {
            _bodyMandates = new List<BodyMandate>();
        }

        public BodyMandates(IEnumerable<BodyMandate> bodyMandates)
        {
            _bodyMandates = bodyMandates.ToList();
        }

        public bool HasABodyMandateWithOverlappingValidity(Period validity)
        {
            return _bodyMandates.Any(x => x.Validity.OverlapsWith(validity));
        }

        public bool HasAnotherBodyMandateWithOverlappingValidity(Period validity, BodyMandateId bodyMandateId)
        {
            return _bodyMandates
                .Where(x => x.BodyMandateId != bodyMandateId)
                .Any(x => x.Validity.OverlapsWith(validity));
        }

        public void Add(BodyMandate bodyMandate)
        {
            _bodyMandates.Add(bodyMandate);
        }

        public void RemoveById(BodyMandateId bodyMandateId)
        {
            Remove(_bodyMandates.Single(mandate => mandate.BodyMandateId == bodyMandateId));
        }

        public void Remove(BodyMandate bodyMandate)
        {
            _bodyMandates.Remove(bodyMandate);
        }

        public BodyMandates ForPeriod(Period validity)
        {
            var bodyMandates = _bodyMandates.Where(mandate => mandate.Validity.OverlapsWith(validity));
            return new BodyMandates(bodyMandates); // TODO: why not make this collection immutable as a whole?
        }

        public IReadOnlyCollection<BodyMandate> AsReadOnly() => _bodyMandates.AsReadOnly();

        public IEnumerator<BodyMandate> GetEnumerator() => _bodyMandates.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

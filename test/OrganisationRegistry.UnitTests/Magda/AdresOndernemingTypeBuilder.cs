namespace OrganisationRegistry.UnitTests.Magda;

using System.Linq;
using global::Magda.GeefOnderneming;

public class AdresOndernemingTypeBuilder
{
    private string? _taalcode;
    private string? _straat;
    private string? _huisnummer;
    private string? _gemeente;
    private string? _land;
    private string? _typeCode;

    private static readonly AdresOndernemingType DefaultAdres = new AdresOndernemingType
    {
        Type = new TypeAdresOndernemingType
        {
            Code = new CodeTypeAdresOndernemingType
            {
                Value = "DEFAULT_TYPE_CODE"
            }
        },
        Gemeente = new GemeenteOptioneel2_0Type
        {
            Naam = "DefaultGemeente"
        },
        Land = new Land2_0Type
        {
            Naam = "België"
        },
        Straat = new StraatRR2_0Type
        {
            Naam = "DefaultStraat"
        },
        Huisnummer = "1",
        Descripties = new[]
        {
            new DescriptieType
            {
                Taalcode = "nl"
            }
        }
    };

    public AdresOndernemingTypeBuilder WithTaalcode(string taalcode)
    {
        _taalcode = taalcode;
        return this;
    }

    public AdresOndernemingTypeBuilder WithStraat(string straat)
    {
        _straat = straat;
        return this;
    }

    public AdresOndernemingTypeBuilder WithHuisnummer(string huisnummer)
    {
        _huisnummer = huisnummer;
        return this;
    }

    public AdresOndernemingTypeBuilder WithGemeente(string gemeenteNaam)
    {
        _gemeente = gemeenteNaam;
        return this;
    }

    public AdresOndernemingTypeBuilder WithLand(string landNaam)
    {
        _land = landNaam;
        return this;
    }

    public AdresOndernemingTypeBuilder WithTypeCode(string typeCode)
    {
        _typeCode = typeCode;
        return this;
    }

    public AdresOndernemingType Build()
    {
        var adres = Clone(DefaultAdres);

        if (_typeCode != null)
            adres.Type.Code.Value = _typeCode;

        if (_gemeente != null)
            adres.Gemeente.Naam = _gemeente;

        if (_land != null)
            adres.Land.Naam = _land;

        if (_straat != null)
            adres.Straat.Naam = _straat;

        if (_huisnummer != null)
            adres.Huisnummer = _huisnummer;

        if (_taalcode != null && adres.Descripties != null && adres.Descripties.Length > 0)
            adres.Descripties[0].Taalcode = _taalcode;

        return adres;
    }

    private static AdresOndernemingType Clone(AdresOndernemingType original)
    {
        return new AdresOndernemingType
        {
            Type = new TypeAdresOndernemingType
            {
                Code = new CodeTypeAdresOndernemingType
                {
                    Value = original.Type?.Code?.Value
                }
            },
            Gemeente = new GemeenteOptioneel2_0Type
            {
                Naam = original.Gemeente?.Naam
            },
            Land = new Land2_0Type
            {
                Naam = original.Land?.Naam
            },
            Straat = new StraatRR2_0Type
            {
                Naam = original.Straat?.Naam
            },
            Huisnummer = original.Huisnummer,
            Descripties = original.Descripties?
                .Select(d => new DescriptieType
                {
                    Taalcode = d.Taalcode
                })
                .ToArray()
        };
    }

    public AdresOndernemingType BuildWithDefaults()
    {
        return Build();
    }
}

using System;
using System.Xml.Serialization;

namespace EcbGateway.Models;

[XmlRoot(ElementName = "Envelope", Namespace = "http://www.gesmes.org/xml/2002-08-01")]
public class EcbRatesResponse
{
    [XmlElement(ElementName = "subject", Namespace = "http://www.gesmes.org/xml/2002-08-01")]
    public string? Subject { get; set; }

    [XmlElement(ElementName = "Sender", Namespace = "http://www.gesmes.org/xml/2002-08-01")]
    public Sender? Sender { get; set; }

    [XmlElement(ElementName = "Cube", Namespace = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref")]
    public CubeDateContainer? Cube { get; set; }
}

public class Sender
{
    [XmlElement(ElementName = "name", Namespace = "http://www.gesmes.org/xml/2002-08-01")]
    public string? Name { get; set; }
}

public class CubeDateContainer
{
    [XmlElement(ElementName = "Cube", Namespace = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref")]
    public CubeDate? CubeDate { get; set; }
}

public class CubeDate
{
    [XmlAttribute(AttributeName = "time")]
    public DateTime Date { get; set; }

    [XmlElement(ElementName = "Cube")]
    public CurrencyRate[]? Rates { get; set; }
}

public class CurrencyRate
{
    [XmlAttribute(AttributeName = "currency")]
    public string? Currency { get; set; }

    [XmlAttribute(AttributeName = "rate")]
    public decimal Rate { get; set; }
}

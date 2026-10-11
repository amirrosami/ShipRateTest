using System.Xml.Linq;

namespace ShipRate.Common.ExternalServices.Accouting
{
    public class AccDocumentXmlBuilder
    {
        public static string Build(AccDocument doc)
        {
            var art = new XElement("_Art");

            AddAttr(art, "FA_Date", doc.PersianDate);
            AddAttr(art, "LA_Date", doc.GregorianDate);
            AddAttr(art, "Desc", doc.Description);
            AddAttr(art, "ODesc", doc.LatinDescription);
            AddAttr(art, "DeptCode", doc.DepartmentCode);
            AddAttr(art, "Kind", (int)doc.Kind);
            AddAttr(art, "State", (int)doc.State);
            AddAttr(art, "RefN", doc.ReferenceNumber);
            AddAttr(art, "ImpId", doc.ImportId);
            AddAttr(art, "Note", doc.Note);

            foreach (var t in doc.AccDocumentDtls)
                art.Add(BuildTrans(t));

            return art.ToString(SaveOptions.DisableFormatting);
        }

        private static XElement BuildTrans(AccDocumentDetail t)
        {
            var x = new XElement("_Trans");

            AddAttr(x, "AccCode", t.AccountCode);
            AddAttr(x, "Desc", t.Description);
            AddAttr(x, "ODesc", t.LatinDescription);

            AddAttr(x, "Det1Code", t.Detail1);
            AddAttr(x, "Det2Code", t.Detail2);
            AddAttr(x, "Det3Code", t.Detail3);
            AddAttr(x, "Det4Code", t.Detail4);
            AddAttr(x, "Det5Code", t.Detail5);

            AddAttr(x, "Deb", t.Debit);
            AddAttr(x, "Cre", t.Credit);

            if (t.CurrencyAmount > 0)
            {
                AddAttr(x, "CurrCode", t.CurrencyCode);
                AddAttr(x, "CurrAmnt", t.CurrencyAmount);
            }

            if (t.UnitAmount > 0)
            {
                AddAttr(x, "UnitCode", t.UnitCode);
                AddAttr(x, "UnitAmnt", t.UnitAmount);
            }

            AddAttr(x, "FA_DueD", t.DuePersianDate);
            AddAttr(x, "LA_DueD", t.DueGregorianDate);
            AddAttr(x, "DueN", t.DueNumber);

            AddAttr(x, "FA_RefD", t.RefPersianDate);
            AddAttr(x, "LA_RefD", t.RefGregorianDate);
            AddAttr(x, "RefN", t.RefNumber);

            AddAttr(x, "FA_OtherD", t.OtherPersianDate);
            AddAttr(x, "LA_OtherD", t.OtherGregorianDate);
            AddAttr(x, "OtherN", t.OtherNumber);

            return x;
        }

        private static void AddAttr(XElement el, string name, object? value)
        {
            if (value == null) return;

            if (value is DateOnly d)
                el.SetAttributeValue(name, d.ToString("yyyy/MM/dd"));
            else
                el.SetAttributeValue(name, value);
        }
    }
}


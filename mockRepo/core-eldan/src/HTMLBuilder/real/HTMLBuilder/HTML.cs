using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Eldan.TypeExtensions;

namespace Eldan.HTMLBuilder
{
    public class HTMLTag : Tag
    {
        public HTMLTag(string name) => Name = name;
    }

    public class META : Tag
    {
        public META() => Name = nameof(META);
    }

    public class STYLE : Tag
    {
        public STYLE(object value) : this() => Value = value;
        public STYLE() => Name = nameof(STYLE);
    }

    public class HTML : Tag
    {
        public HTML(BODY body) : this()
        {
            Value = new List<Tag>
            {
                body
            };
        }

        public HTML(HEAD head, BODY body) : this()
        {
            Value = new List<Tag>
            {
                head,
                body
            };
        }

        public HTML() => Name = nameof(HTML);
    }

    public class HEAD : Tag
    {
        public HEAD(params Tag[] tags) : this() => Value = tags;

        public HEAD() => Name = nameof(HEAD);
    }

    public class BODY : Tag
    {
        public BODY(params Tag[] tags) : this() => Value = tags;

        public BODY() => Name = nameof(BODY);
    }

    public class BR : Tag
    {
        public BR() => Name = nameof(BR);
    }

    public class P : Tag
    {
        public P(object value) : this() => Value = value;
        public P() => Name = nameof(P);
    }

    public class H1 : Tag
    {
        public H1(object value) : this() => Value = value;
        public H1() => Name = nameof(H1);
    }

    public class H2 : Tag
    {
        public H2(object value) : this() => Value = value;
        public H2() => Name = nameof(H2);
    }

    public class H3 : Tag
    {
        public H3(object value) : this() => Value = value;
        public H3() => Name = nameof(H3);
    }

    public class H4 : Tag
    {
        public H4(object value) : this() => Value = value;
        public H4() => Name = nameof(H4);
    }

    public class H5 : Tag
    {
        public H5(object value) : this() => Value = value;
        public H5() => Name = nameof(H5);
    }

    public class H6 : Tag
    {
        public H6(object value) : this() => Value = value;
        public H6() => Name = nameof(H6);
    }

    public class DIV : Tag
    {
        public DIV(params Tag[] tags) : this() => Value = tags;
        public DIV(object value) : this() => Value = value;
        public DIV() => Name = nameof(DIV);
    }

    public class A : Tag
    {
        public A(object value) : this() => Value = value;
        public A(object value, string href) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(href), href));
            Value = value;
        }
        public A() => Name = nameof(A);
    }

    public class ABBR : Tag
    {
        public ABBR(object value) : this() => Value = value;
        public ABBR(object value, string title) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(title), title));
            Value = value;
        }
        public ABBR() => Name = nameof(A);
    }

    public class ADDRESS : Tag
    {
        public ADDRESS(object value) : this() => Value = value;
        public ADDRESS() => Name = nameof(ADDRESS);
    }

    public class MAP : Tag
    {
        public MAP(object value) : this() => Value = value;
        public MAP(params AREA[] AREAs) : this() => Value = AREAs;
        public MAP() => Name = nameof(MAP);
    }

    public class AREA : Tag
    {
        public AREA(object value) : this() => Value = value;
        public AREA(string shape, string coords, string alt, string href) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(shape), shape),
                                        new Attribute(nameof(coords), coords),
                                        new Attribute(nameof(alt), alt),
                                        new Attribute(nameof(href), href));
        }
        public AREA() => Name = nameof(AREA);
    }

    public class ARTICLE : Tag
    {
        public ARTICLE(object value) : this() => Value = value;
        public ARTICLE() => Name = nameof(ARTICLE);
    }
    public class ASIDE : Tag
    {
        public ASIDE(object value) : this() => Value = value;
        public ASIDE() => Name = nameof(ASIDE);
    }

    public class AODIO : Tag
    {
        public AODIO(object value) : this() => Value = value;
        public AODIO(params SOURCE[] SOURCEs) : this() => Value = SOURCEs;
        public AODIO() => Name = nameof(AODIO);
    }

    public class SOURCE : Tag
    {
        public SOURCE(string src, string type) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(src), src),
                                        new Attribute(nameof(type), type));
        }
        public SOURCE() => Name = nameof(SOURCE);
    }

    public class B : Tag
    {
        public B(object value) : this() => Value = value;
        public B() => Name = nameof(B);
    }

    public class BASE : Tag
    {
        public BASE(string href, string target) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(href), href),
                                        new Attribute(nameof(target), target));
        }
        public BASE() => Name = nameof(BASE);
    }

    public class BDI : Tag
    {
        public BDI(object value) : this() => Value = value;
        public BDI() => Name = nameof(BDI);
    }

    public class BDO : Tag
    {
        public BDO(object value) : this() => Value = value;
        public BDO() => Name = nameof(BDO);
    }

    public class BLOCKQUOTE : Tag
    {
        public BLOCKQUOTE(object value) : this() => Value = value;
        public BLOCKQUOTE() => Name = nameof(BLOCKQUOTE);
    }

    public class BUTTON : Tag
    {
        public BUTTON(object value) : this() => Value = value;
        public BUTTON() => Name = nameof(BUTTON);
    }

    public class CANVAS : Tag
    {
        public CANVAS(object value) : this() => Value = value;
        public CANVAS() => Name = nameof(CANVAS);
    }

    public class CAPTION : Tag
    {
        public CAPTION(object value) : this() => Value = value;
        public CAPTION() => Name = nameof(CAPTION);
    }

    public class CITY : Tag
    {
        public CITY(object value) : this() => Value = value;
        public CITY() => Name = nameof(CITY);
    }

    public class CODE : Tag
    {
        public CODE(object value) : this() => Value = value;
        public CODE() => Name = nameof(CODE);
    }

    public class COL : Tag
    {
        public COL(object value) : this() => Value = value;
        public COL() => Name = nameof(COL);
    }

    public class COLGROUP : Tag
    {
        public COLGROUP(object value) : this() => Value = value;
        public COLGROUP(params COL[] COLs) : this() => Value = COLs;
        public COLGROUP() => Name = nameof(COLGROUP);
    }

    public class DATA : Tag
    {
        public DATA(object value) : this() => Value = value;
        public DATA() => Name = nameof(DATA);
    }

    public class OPTION : Tag
    {
        public OPTION(string value) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(value), value));
        }
        public OPTION() => Name = nameof(OPTION);
    }

    public class DATALIST : Tag
    {
        public DATALIST(params OPTION[] OPTIONs) : this() => Value = OPTIONs;
        public DATALIST() => Name = nameof(DATALIST);
    }

    public abstract class Ditem : Tag
    {
        public Ditem() : base()
        { }
    }

    public class DD : Ditem
    {
        public DD(object value) : this() => Value = value;
        public DD() => Name = nameof(DD);
    }

    public class DT : Ditem
    {
        public DT(object value) : this() => Value = value;
        public DT() => Name = nameof(DT);
    }

    public class DL : Tag
    {
        public DL(params Ditem[] ditems) : this() =>  Value = ditems;   
        public DL() => Name = nameof(DL);
    }


    public class DEL : Tag
    {
        public DEL(object value) : this() => Value = value;
        public DEL() => Name = nameof(DEL);
    }

    public class DETAILS : Tag
    {
        public DETAILS(object value) : this() => Value = value;
        public DETAILS() => Name = nameof(DETAILS);
    }

    public class DFN : Tag
    {
        public DFN(object value) : this() => Value = value;
        public DFN() => Name = nameof(DFN);
    }

    public class DIALOG : Tag
    {
        public DIALOG(object value) : this() => Value = value;
        public DIALOG() => Name = nameof(DIALOG);
    }

    public class EM : Tag
    {
        public EM(object value) : this() => Value = value;
        public EM() => Name = nameof(EM);
    }

    public class EMBED : Tag
    {
        public EMBED(string type, string scr, string width, string height) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(type), type),
                                        new Attribute(nameof(scr), scr),
                                        new Attribute(nameof(width), width),
                                        new Attribute(nameof(height), height));
        }
        public EMBED() => Name = nameof(EMBED);
    }

    public class FIELDSET : Tag
    {
        public FIELDSET(params Tag[] tags) : this() => Value = tags;
        public FIELDSET(object value) : this() => Value = value;
        public FIELDSET() => Name = nameof(FIELDSET);
    }

    public class FIGCAPTION : Tag
    {
        public FIGCAPTION(object value) : this() => Value = value;
        public FIGCAPTION() => Name = nameof(FIGCAPTION);
    }

    public class FIGURE : Tag
    {
        public FIGURE(object value) : this() => Value = value;
        public FIGURE() => Name = nameof(FIGURE);
    }

    public class FOOTER : Tag
    {
        public FOOTER(object value) : this() => Value = value;
        public FOOTER() => Name = nameof(FOOTER);
    }

    public class FORM : Tag
    {
        public FORM(object value) : this() => Value = value;

        public FORM(string action, string method, object value) : this(action, method)
        {
            Value = value;
        }

        public FORM(string action, string method) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(action), action),
                                        new Attribute(nameof(method), method));
        }
        public FORM() => Name = nameof(FORM);
    }

    public class HEADER : Tag
    {
        public HEADER(object value) : this() => Value = value;
        public HEADER() => Name = nameof(HEADER);
    }

    public class HR : Tag
    {
        public HR(object value) : this() => Value = value;
        public HR() => Name = nameof(HR);
    }

    public class I : Tag
    {
        public I(object value) : this() => Value = value;
        public I() => Name = nameof(I);
    }

    public class IFRAME : Tag
    {
        public IFRAME(string src, string title) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(src), src),
                                        new Attribute(nameof(title), title));
        }
        public IFRAME() => Name = nameof(IFRAME);
    }

    public class IMG : Tag
    {
        public IMG(string scr, string alt, string width, string height) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(scr), scr), 
                                        new Attribute(nameof(alt), alt),
                                        new Attribute(nameof(width), width),
                                        new Attribute(nameof(height), height));
        }
        public IMG() => Name = nameof(IMG);
    }

    public class INPUT : Tag
    {
        public INPUT(string type, string id, string name) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(type), type),
                                        new Attribute(nameof(id), id),
                                        new Attribute(nameof(name), name));
        }
        public INPUT(string type, string value) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(type), type),
                                        new Attribute(nameof(value), value));
        }

        public INPUT() => Name = nameof(INPUT);
    }

    public class INS : Tag
    {
        public INS(object value) : this() => Value = value;
        public INS() => Name = nameof(INS);
    }

    public class KBD : Tag
    {
        public KBD(object value) : this() => Value = value;
        public KBD() => Name = nameof(KBD);
    }

    public class LABEL : Tag
    {
        public LABEL(string @for, string value) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(@for), @for),
                                        new Attribute(nameof(value), value));
        }

        public LABEL() => Name = nameof(LABEL);
    }

    public class LEGEND : Tag
    {
        public LEGEND(object value) : this() => Value = value;
        public LEGEND() => Name = nameof(LEGEND);
    }

    public class LI : Tag
    {
        public LI(object value) : this() => Value = value;
        public LI() => Name = nameof(LI);
    }

    public class OL : Tag
    {
        public OL(LI[] LIs) : this() => Value = LIs;
        public OL() => Name = nameof(OL);
    }

    public class UL : Tag
    {
        public UL(LI[] LIs) : this() => Value = LIs;
        public UL() => Name = nameof(UL);
    }

    public class LINK : Tag
    {
        public LINK(string rel, string href) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(rel), rel),
                                        new Attribute(nameof(href), href));
        }
        public LINK() => Name = nameof(LINK);
    }

    public class MAIN : Tag
    {
        public MAIN(object value) : this() => Value = value;
        public MAIN() => Name = nameof(MAIN);
    }

    public class MARK : Tag
    {
        public MARK(object value) : this() => Value = value;
        public MARK() => Name = nameof(MARK);
    }

    public class METER : Tag
    {
        public METER(object value) : this() => Value = value;
        public METER() => Name = nameof(METER);
    }

    public class NAV : Tag
    {
        public NAV(A[] As) : this() => Value = As;
        public NAV() => Name = nameof(NAV);
    }

    public class NOSCRIPT : Tag
    {
        public NOSCRIPT(object value) : this() => Value = value;
        public NOSCRIPT() => Name = nameof(NOSCRIPT);
    }

    public class OPTGROUP : Tag
    {
        public OPTGROUP(params OPTION[] OPTIONs) : this() => Value = OPTIONs;
        public OPTGROUP() => Name = nameof(OPTGROUP);
    }

    public class OUTPUT : Tag
    {
        public OUTPUT(string value, string @for) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(@for), @for),
                                        new Attribute(nameof(value), value));
        }

        public OUTPUT() => Name = nameof(OUTPUT);
    }

    public class PARAM : Tag
    {
        public PARAM(string name, string value) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(name), name), 
                                        new Attribute(nameof(value), value));
        }

        public PARAM() => Name = nameof(PARAM);
    }

    public class PICTURE : Tag
    {
        public PICTURE(object value) : this() => Value = value;
        public PICTURE() => Name = nameof(PICTURE);
    }

    public class PRE : Tag
    {
        public PRE(object value) : this() => Value = value;
        public PRE() => Name = nameof(PRE);
    }

    public class PROGRESS : Tag
    {
        public PROGRESS(string id, string value, string max) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(id), id),
                                        new Attribute(nameof(value), value),
                                        new Attribute(nameof(max), max));
        }
        public PROGRESS() => Name = nameof(PROGRESS);
    }

    public class Q : Tag
    {
        public Q(object value) : this() => Value = value;
        public Q() => Name = nameof(Q);
    }

    public class RP : Tag
    {
        public RP(object value) : this() => Value = value;
        public RP() => Name = nameof(RP);
    }

    public class RT : Tag
    {
        public RT(object value) : this() => Value = value;
        public RT() => Name = nameof(RT);
    }

    public class RUBY : Tag
    {
        public RUBY(object value) : this() => Value = value;
        public RUBY() => Name = nameof(RUBY);
    }

    public class S : Tag
    {
        public S(object value) : this() => Value = value;
        public S() => Name = nameof(S);
    }

    public class SPAM : Tag
    {
        public SPAM(object value) : this() => Value = value;
        public SPAM() => Name = nameof(SPAM);
    }

    public class SCRIPT : Tag
    {
        public SCRIPT(object value) : this() => Value = value;
        public SCRIPT() => Name = nameof(SCRIPT);
    }

    public class SELECTION : Tag
    {
        public SELECTION(object value) : this() => Value = value;
        public SELECTION() => Name = nameof(SELECTION);
    }

    public class SELECT : Tag
    {
        public SELECT(params OPTION[] OPTIONs) : this() => Value = OPTIONs;
        public SELECT() => Name = nameof(SELECT);
    }

    public class SMALL : Tag
    {
        public SMALL(object value) : this() => Value = value;
        public SMALL() => Name = nameof(SMALL);
    }

    public class STRONG : Tag
    {
        public STRONG(object value) : this() => Value = value;
        public STRONG() => Name = nameof(STRONG);
    }

    public class SPAN : Tag
    {
        public SPAN(object value) : this() => Value = value;
        public SPAN() => Name = nameof(SPAN);
    }

    public class SUB : Tag
    {
        public SUB(object value) : this() => Value = value;
        public SUB() => Name = nameof(SUB);
    }

    public class SUMMARY : Tag
    {
        public SUMMARY(object value) : this() => Value = value;
        public SUMMARY() => Name = nameof(SUMMARY);
    }

    public class SUP : Tag
    {
        public SUP(object value) : this() => Value = value;
        public SUP() => Name = nameof(SUP);
    }

    public class SVG : Tag
    {
        public SVG(string width, string height) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(width), width),
                                        new Attribute(nameof(height), height));
        }
        public SVG() => Name = nameof(SVG);
    }

    public class TEMPLATE : Tag
    {
        public TEMPLATE(object value) : this() => Value = value;
        public TEMPLATE() => Name = nameof(TEMPLATE);
    }

    public class TEXTAREA : Tag
    {
        public TEXTAREA(string id, string name, string rows, string cols, object value) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(id), id),
                                        new Attribute(nameof(name), name),
                                        new Attribute(nameof(rows), rows),
                                        new Attribute(nameof(cols), cols));
            Value = value;
        }
        public TEXTAREA() => Name = nameof(TEXTAREA);
    }

    public class TIME : Tag
    {
        public TIME(object value) : this() => Value = value;
        public TIME() => Name = nameof(TIME);
    }

    public class TITLE : Tag
    {
        public TITLE(object value) : this() => Value = value;
        public TITLE() => Name = nameof(TITLE);
    }

    public class TRACK : Tag
    {
        public TRACK(string src, string kind, string srclang, string lable) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(src), src),
                                        new Attribute(nameof(kind), kind),
                                        new Attribute(nameof(srclang), srclang),
                                        new Attribute(nameof(lable), lable));
        }
        public TRACK() => Name = nameof(TRACK);
    }

    public class U : Tag
    {
        public U(object value) : this() => Value = value;
        public U() => Name = nameof(U);
    }

    public class VAR : Tag
    {
        public VAR(object value) : this() => Value = value;
        public VAR() => Name = nameof(VAR);
    }

    public class VIDEO : Tag
    {
        public VIDEO(string width, string height, string controls) : this()
        {
            Attributes = new Attributes(new Attribute(nameof(width), width),
                                        new Attribute(nameof(height), height),
                                        new Attribute(nameof(controls), controls));
        }
        public VIDEO() => Name = nameof(VIDEO);
    }

    public class WBR : Tag
    {
        public WBR(object value) : this() => Value = value;
        public WBR() => Name = nameof(WBR);
    }

    public class TABLE : Rows
    {
        public TABLE(THEAD thead, TBODY tbody, TFOOT tfoot) : this()
        {
            Value = new List<Tag>
            {
                thead,
                tbody,
                tfoot
            };
        }

        public TABLE(THEAD thead, TBODY tbody) : this()
        {
            Value = new List<Tag>
            {
                thead,
                tbody
            };
        }

        public TABLE(params TR[] TRs) : this() => Value = TRs;

        public TABLE() => Name = nameof(TABLE);
    }

    public class THEAD : Tag
    {
        public THEAD(params TR[] TRs) : this() => Value = TRs;

        public THEAD() => Name = nameof(THEAD);
    }

    public class TBODY : Rows
    {
        public TBODY(params TR[] TRs) : this() => Value = TRs;

        public TBODY() => Name = nameof(TBODY);
    }

    public class TFOOT : Rows
    {
        public TFOOT(params TR[] TRs) : this() => Value = TRs;

        public TFOOT() => Name = nameof(TFOOT);
    }

    public abstract class Rows : Tag
    {
        public Rows()
        {
            ZebraBlackStyle = new STYLE("background-color: #f2f2f2");
            ZebraWhiteStyle = new STYLE("background-color: white");
        }

        public bool IsZebra { get; set; }
        public STYLE ZebraBlackStyle { get; set; }
        public STYLE ZebraWhiteStyle { get; set; }

        protected override XDocument ToDocument()
        {
            if (IsZebra)
            {
                Value = GetZebraTRs(Value);
                if (Value is TBODY tbody)
                    tbody.Value = GetZebraTRs(tbody.Value);
            }

            return base.ToDocument();
        }

        private object GetZebraTRs(object value)
        {
            if (value is IEnumerable<TR> trs)
            {
                return trs.Select((tr, i) => AddTRStyle(tr, i));
            }

            return value;
        }

        private TR AddTRStyle(TR tr, int index)
        {
            if (tr == null)
                return null;

            if (tr.Value is TH || tr.Value is IEnumerable<TH>)
                return tr;

            if (tr.Value is IEnumerable<Cell> enumerable)
            {
                if (enumerable.All(x => x is TH))
                    return tr;
            }

            if (tr.Attributes == null)
                tr.Attributes = new Attributes();
            tr.Attributes.RemoveAll(x => x.Name.ToLower() == "style");
            tr.Attributes.Add(new Attribute("style", (index % 2 == 0) ? ZebraWhiteStyle.Value.ToString() : ZebraBlackStyle.Value.ToString()));  
            return tr;
        }
    }

    public class TR : Tag
    {
        public TR(params object[] tdValues) : this()
        {
            Value = tdValues.Select(x => new TD(x));
        }

        public TR(params Cell[] cells) : this() 
        {
            Value = cells;
        }

        public TR()
        {
            Name = nameof(TR);
        }
    }

    public class TH : Cell
    {
        public TH(object value) : this() => Value = value;
        public TH() => Name = nameof(TH);
    }

    public class TD : Cell
    {
        public TD(object value) : this() => Value = value;

        public TD() => Name = nameof(TD);
    }

    public abstract class Cell : Tag
    {
        public Cell() : base()
        { }
    }

    public abstract class Tag
    {
        private const string COMMNET_ATT_NAME = "_HBAddCommentAttName";

        public Tag(string name, object value) : this(name) => Value = value;

        public Tag(string name) : this() => Name = name;

        public Tag()
        { }

        protected string Name { get; set; }

        public object Value { get; set; }

        public Attributes Attributes { get; set; }

        public string Comment { get; set; }

        public override string ToString()
        {
            XDocument doc = ToDocument();
            var commentNodes = doc.Descendants().Where(x => x.GetAttributeValue(COMMNET_ATT_NAME) != null);

            foreach (var commentNode in commentNodes)
            {
                XComment comm = new XComment(commentNode.GetAttributeValue(COMMNET_ATT_NAME));
                commentNode.AddBeforeSelf(comm);
                commentNode.Attribute(COMMNET_ATT_NAME).Remove();
            }

            return doc.ToString();
        }

        virtual protected XDocument ToDocument()
        {
            if (Comment != null)
            {
                if (Attributes == null)
                    Attributes = new Attributes();

                Attributes.Add(new Attribute(COMMNET_ATT_NAME, Comment));
            }

            XDocument doc = new XDocument(
                new XElement(Name.ToLower(), Attributes?.GetXAttributes(), GetXValue()));

            return doc;
        }

        private object GetXValue()
        {
            if (Value == null)
                return null;

            if (Value is Tag tag)
                return tag.ToDocument().Root;

            if (Value is IEnumerable<Tag> tags)
                return tags.Select(x => x?.ToDocument().Root);

            if (Value is IEnumerable<object> objs)
                return objs.Select((x) => { if (x == null) return null;
                                            if (x is Tag tag1) return tag1.ToDocument().Root;
                                            return x; });
            return Value.ToString();
        }

        public static Values operator +(Tag tag1, Tag tag2)
        {
            return new Values { tag1, tag2 };
        }

        public static Values operator +(Tag tag, string value)
        {
            return new Values { tag, value };
        }

        public static Values operator +(string value, Tag tag)
        {
            return new Values { value, tag };
        }

        public static Values operator +(Tag tag, Values values)
        {
            values.Insert(0, tag);
            return values;
        }

        public static Values operator +(Values values, Tag tag)
        {
            values.Add(tag);
            return values;
        }
    }

    public class Values : List<object>
    {
        public static Values operator +(Values values, string value)
        {
            values.Add(value);
            return values;
        }

        public static Values operator +(string value, Values values)
        {
            values.Insert(0, value);
            return values;
        }
    }

    public class Attributes : List<Attribute>
    {
        public Attributes() : this(new List<Attribute>().ToArray())
        { }

        public Attributes(params Attribute[] attributes) : base(attributes)
        { }

        internal List<XAttribute> GetXAttributes()
        {
            if (this.Distinct().Count() != this.Select(x => x.Name).Distinct().Count())
                throw new ArgumentException(string.Format("{0}.{1} - Attributes: '{2}' have duplicate names",
                                            nameof(Attribute), nameof(GetXAttributes), this.ToJSON()));

            return this.Select(x => new XAttribute(x.Name, x.Value)).ToList();
        }
    }

    public class Attribute
    {
        public Attribute()
        { }

        public Attribute(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(string.Format("{0}.{1} - Attribute name can't be null", nameof(Attribute), "constractor"));
            Name = name;
            Value = value.ToNullLessString();
        }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
